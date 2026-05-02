<template>
	<div class="system-feature-container layout-padding">
		<div class="system-feature-padding layout-padding-auto">
			<el-card shadow="hover" :body-style="{ paddingBottom: '0' }" class="feature-query-card">
				<el-form :inline="true">
					<el-form-item label="范围">
						<el-radio-group v-model="state.scope" size="default" @change="onScopeChange">
							<el-radio-button label="host">宿主默认</el-radio-button>
							<el-radio-button label="tenant">租户覆盖</el-radio-button>
						</el-radio-group>
					</el-form-item>
					<el-form-item v-if="state.scope === 'tenant'" label="租户">
						<el-select
							v-model="state.tenantId"
							class="tenant-select"
							filterable
							clearable
							placeholder="选择租户"
							@change="loadFeatures"
						>
							<el-option v-for="t in state.tenantOptions" :key="t.id" :label="`${t.name ?? t.id}`" :value="t.id!" />
						</el-select>
					</el-form-item>
					<el-form-item label="关键字">
						<el-input v-model="state.keyword" class="feature-search" clearable placeholder="名称 / 描述 / 默认值" />
					</el-form-item>
					<el-form-item>
						<el-checkbox v-model="state.onlyOverridden">仅显示已覆盖</el-checkbox>
					</el-form-item>
					<el-form-item>
						<el-button-group>
							<el-button type="primary" :loading="state.loading" :disabled="loadDisabled" @click="loadFeatures"> 刷新 </el-button>
							<el-button type="success" :loading="state.saving" :disabled="saveDisabled" @click="onSave"> 保存 </el-button>
							<el-button type="warning" plain :disabled="resetDisabled" @click="onResetOverrides"> 清除覆盖 </el-button>
						</el-button-group>
					</el-form-item>
				</el-form>
			</el-card>

			<el-card class="full-table feature-table-card" shadow="hover" style="margin-top: 5px">
				<div v-loading="state.loading" class="feature-table-body">
				<template v-if="filteredGroups.length">
					<el-collapse v-model="state.activeGroups">
						<el-collapse-item v-for="g in filteredGroups" :key="g.name" :name="g.name">
							<template #title>
								<div class="group-title">
									<span class="collapse-title">{{ g.displayName || g.name }}</span>
									<span class="group-meta">{{ g.name }}</span>
									<span class="group-count">{{ sortedFeatures(g.features).length }} 项</span>
								</div>
							</template>
							<div v-for="f in sortedFeatures(g.features)" :key="f.name" class="feature-row" :style="{ paddingLeft: `${12 + (f.depth ?? 0) * 16}px` }">
								<div class="feature-label">
									<div class="name-line">
										<div class="name">{{ f.displayName || f.name }}</div>
										<el-tag size="small" :type="hasStoredValue(f) ? 'success' : 'info'">
											{{ hasStoredValue(f) ? '已覆盖' : '默认值' }}
										</el-tag>
									</div>
									<div v-if="f.description" class="desc text-secondary">{{ f.description }}</div>
									<div class="meta-row">
										<span class="meta">名称：{{ f.name }}</span>
										<span v-if="f.parentName" class="meta">父级：{{ f.parentName }}</span>
										<span v-if="f.defaultValue" class="meta">默认值：{{ f.defaultValue }}</span>
										<span class="meta">当前范围：{{ getProviderLabel(f) }}</span>
									</div>
									<div class="tag-row">
										<el-tag v-if="!f.isAvailableToHost" size="small" type="warning" effect="plain">宿主不可用</el-tag>
										<el-tag v-if="!f.isVisibleToClients" size="small" type="danger" effect="plain">客户端不可见</el-tag>
										<el-tag v-for="provider in f.allowedProviders ?? []" :key="`${f.name}-${provider}`" size="small" effect="plain">{{ provider }}</el-tag>
										<el-tag v-if="!f.allowedProviders?.length" size="small" effect="plain">未限制 provider</el-tag>
									</div>
								</div>
								<div class="feature-control">
									<div class="control-hint">{{ getValueTypeLabel(f) }}</div>
									<el-switch
										v-if="isToggleFeature(f)"
										v-model="state.values[f.name!]"
										active-value="true"
										inactive-value="false"
										inline-prompt
										active-text="开"
										inactive-text="关"
									/>
									<el-select
										v-else-if="getValueTypeOptions(f).length"
										v-model="state.values[f.name!]"
										clearable
										filterable
										placeholder="请选择"
									>
										<el-option v-for="option in getValueTypeOptions(f)" :key="`${f.name}-${option.value}`" :label="option.label" :value="option.value" />
									</el-select>
									<el-input v-else v-model="state.values[f.name!]" clearable :placeholder="getValuePlaceholder(f)" />
									<div class="control-note">
										<span>保存后写入当前范围的覆盖值</span>
									</div>
								</div>
							</div>
						</el-collapse-item>
					</el-collapse>
				</template>
				<el-empty v-else-if="!state.loading" description="当前范围无特性或暂无匹配结果" />
				</div>
			</el-card>
		</div>
	</div>
</template>

<script setup lang="ts" name="systemFeature">
import { computed, reactive, onMounted } from 'vue';
import { ElMessageBox, ElMessage } from 'element-plus';
import { useFeatureManagementApi } from '/@/api/apis';
import { TENANT_FEATURE_PROVIDER } from '/@/api/models/feature-management';
import type { FeatureDto, FeatureGroupDto } from '/@/api/models/feature-management';
import { useTenantApi } from '/@/api/apis';
import type { TenantDto } from '/@/api/models/tenant';

const state = reactive({
	scope: 'host' as 'host' | 'tenant',
	tenantId: '' as string,
	tenantOptions: [] as TenantDto[],
	groups: [] as FeatureGroupDto[],
	values: {} as Record<string, string>,
	activeGroups: [] as string[],
	keyword: '',
	onlyOverridden: false,
	loading: false,
	saving: false,
});

const loadDisabled = computed(() => state.scope === 'tenant' && !state.tenantId);
const saveDisabled = computed(() => state.loading || loadDisabled.value || !state.groups.length);
const resetDisabled = computed(() => state.loading || loadDisabled.value || !state.groups.length);

const filteredGroups = computed(() => {
	const keyword = state.keyword.trim().toLowerCase();

	return state.groups
		.map((group) => {
			const features = sortedFeatures(group.features).filter((feature) => {
				if (state.onlyOverridden && !hasStoredValue(feature)) {
					return false;
				}

				if (!keyword) {
					return true;
				}

				const haystack = [
					feature.name,
					feature.displayName,
					feature.description,
					feature.defaultValue,
					feature.parentName,
					feature.allowedProviders?.join(','),
					getValueTypeLabel(feature),
				]
					.filter(Boolean)
					.join(' ')
					.toLowerCase();

				return haystack.includes(keyword);
			});

			return {
				...group,
				features,
			};
		})
		.filter((group) => (group.features?.length ?? 0) > 0);
});

function sortedFeatures(list?: FeatureDto[]) {
	return [...(list ?? [])].sort((a, b) => (a.depth ?? 0) - (b.depth ?? 0));
}

function getValueTypeName(f: FeatureDto): string {
	const vt = f.valueType;
	if (!vt || typeof vt !== 'object') {
		return '';
	}

	const record = vt as Record<string, unknown>;
	return String(record.name ?? record.Name ?? record.type ?? record.Type ?? '');
}

function isToggleFeature(f: FeatureDto): boolean {
	const valueTypeName = getValueTypeName(f);
	return valueTypeName.includes('Toggle') || f.value === 'true' || f.value === 'false';
}

function getValueTypeLabel(f: FeatureDto): string {
	const valueTypeName = getValueTypeName(f);
	if (!valueTypeName) {
		return '普通文本';
	}

	if (valueTypeName.includes('Toggle')) {
		return '布尔开关';
	}

	if (valueTypeName.includes('Selection')) {
		return '枚举选择';
	}

	return valueTypeName;
}

function getValueTypeOptions(f: FeatureDto): Array<{ label: string; value: string }> {
	const vt = f.valueType;
	if (!vt || typeof vt !== 'object') {
		return [];
	}

	const record = vt as Record<string, unknown>;
	const rawOptions = record.items ?? record.values ?? record.options;
	if (!Array.isArray(rawOptions)) {
		return [];
	}

	return rawOptions
		.map((option) => {
			if (typeof option === 'string' || typeof option === 'number' || typeof option === 'boolean') {
				return { label: String(option), value: String(option) };
			}

			if (option && typeof option === 'object') {
				const optionRecord = option as Record<string, unknown>;
				const value = optionRecord.value ?? optionRecord.key ?? optionRecord.name ?? optionRecord.id;
				if (value === undefined || value === null || value === '') {
					return null;
				}
				const label = optionRecord.label ?? optionRecord.displayName ?? optionRecord.text ?? value;
				return { label: String(label), value: String(value) };
			}

			return null;
		})
		.filter((option): option is { label: string; value: string } => option !== null);
}

function getValuePlaceholder(f: FeatureDto): string {
	return f.defaultValue ? `默认值：${f.defaultValue}` : '请输入取值';
}

function hasStoredValue(f: FeatureDto): boolean {
	return Boolean(f.provider?.name);
}

function getProviderLabel(f: FeatureDto): string {
	if (!hasStoredValue(f)) {
		return f.defaultValue ? '默认值' : '未覆盖';
	}

	const providerName = f.provider?.name ?? '';
	const providerKey = f.provider?.key ?? '';
	if (!providerName && !providerKey) {
		return '默认值';
	}

	return providerKey ? `${providerName} / ${providerKey}` : providerName;
}

async function loadTenantOptions() {
	const { getTenantPage } = useTenantApi();
	const res = await getTenantPage({ skipCount: 0, maxResultCount: 200 });
	state.tenantOptions = res.items ?? [];
}

function applyResult(groups: FeatureGroupDto[]) {
	state.groups = groups;
	const vals: Record<string, string> = {};
	for (const g of groups) {
		for (const f of g.features ?? []) {
			if (f.name) vals[f.name] = f.value ?? '';
		}
	}
	state.values = vals;
	state.activeGroups = groups.map((g) => g.name!).filter(Boolean);
}

async function loadFeatures() {
	if (loadDisabled.value) {
		state.groups = [];
		state.values = {};
		state.activeGroups = [];
		return;
	}
	state.loading = true;
	try {
		const { getFeatures } = useFeatureManagementApi();
		const providerKey = state.scope === 'host' ? '' : state.tenantId;
		const res = await getFeatures(TENANT_FEATURE_PROVIDER, providerKey);
		applyResult(res.groups ?? []);
	} catch {
		state.groups = [];
		state.values = {};
		state.activeGroups = [];
		ElMessage.error('加载特性失败');
	} finally {
		state.loading = false;
	}
}

function onScopeChange() {
	if (state.scope === 'host') state.tenantId = '';
	loadFeatures();
}

async function onSave() {
	const flat: { name: string; value: string }[] = [];
	for (const g of state.groups) {
		for (const f of g.features ?? []) {
			if (!f.name) continue;
			flat.push({ name: f.name, value: state.values[f.name] ?? '' });
		}
	}
	state.saving = true;
	try {
		const { updateFeatures } = useFeatureManagementApi();
		const providerKey = state.scope === 'host' ? '' : state.tenantId;
		await updateFeatures(TENANT_FEATURE_PROVIDER, providerKey, { features: flat });
		ElMessage.success('已保存');
		await loadFeatures();
	} finally {
		state.saving = false;
	}
}

async function onResetOverrides() {
	await ElMessageBox.confirm(
		state.scope === 'host'
			? '将清除宿主侧对租户特性（Tenant Feature）的覆盖，恢复为代码与内置默认。是否继续？'
			: '将清除该租户的特性覆盖，改为继承宿主/默认。是否继续？',
		'确认',
		{ type: 'warning' }
	);
	const { deleteFeatureOverrides } = useFeatureManagementApi();
	const providerKey = state.scope === 'host' ? '' : state.tenantId;
	await deleteFeatureOverrides(TENANT_FEATURE_PROVIDER, providerKey);
	ElMessage.success('已清除覆盖');
	await loadFeatures();
}

onMounted(async () => {
	await loadTenantOptions();
	await loadFeatures();
});
</script>

<style scoped lang="scss">
.system-feature-container {
	.system-feature-padding {
		display: flex;
		flex-direction: column;
		gap: 5px;
		min-height: 100%;
	}
}

.feature-query-card {
	:deep(.el-card__body) {
		padding-bottom: 0;
	}
}

.feature-table-card {
	flex: 1;
	display: flex;
	flex-direction: column;

	:deep(.el-card__body) {
		flex: 1;
		display: flex;
		flex-direction: column;
	}

	.feature-table-body {
		flex: 1;
		display: flex;
		flex-direction: column;
	}
}

.tenant-select {
	min-width: 220px;
}
.feature-search {
	min-width: 260px;
	width: 260px;
}
.group-title {
	display: flex;
	align-items: center;
	gap: 10px;
	flex-wrap: wrap;
	width: 100%;
}
.collapse-title {
	font-weight: 600;
}
.group-meta,
.group-count {
	font-size: 12px;
	color: var(--el-text-color-secondary);
}
.feature-row {
	display: flex;
	align-items: flex-start;
	gap: 16px;
	padding: 10px 0;
	border-bottom: 1px solid var(--el-border-color-lighter);
	.feature-label {
		flex: 1;
		min-width: 0;
		.name-line {
			display: flex;
			align-items: center;
			gap: 10px;
			flex-wrap: wrap;
		}
		.name {
			font-weight: 500;
			font-size: 15px;
		}
		.desc {
			font-size: 12px;
			margin-top: 4px;
		}
		.meta-row,
		.tag-row {
			display: flex;
			flex-wrap: wrap;
			gap: 8px;
			margin-top: 8px;
		}
		.meta {
			font-size: 12px;
			color: var(--el-text-color-secondary);
		}
	}
	.feature-control {
		width: 280px;
		flex-shrink: 0;
		.control-hint {
			margin-bottom: 8px;
			font-size: 12px;
			color: var(--el-text-color-secondary);
		}
		.control-note {
			margin-top: 8px;
			font-size: 12px;
			color: var(--el-text-color-secondary);
		}
	}
}
.text-secondary {
	color: var(--el-text-color-secondary);
}

@media (max-width: 960px) {
	.feature-search,
	.feature-control {
		width: 100%;
		min-width: 0;
	}
	.feature-row {
		flex-direction: column;
	}
}
</style>
