<template>
	<div class="system-feature-container layout-padding">
		<div class="system-feature-padding layout-padding-auto layout-padding-view">
			<el-alert type="info" show-icon :closable="false" class="mb15">
				特性用于按「宿主 / 租户」等功能范围**覆盖**模块在代码里定义的默认值（见各模块的 <code>FeatureDefinitionProvider</code>）。此处页面只维护**取值**，不新增特性定义。
			</el-alert>

			<div class="system-feature-toolbar mb15">
				<el-radio-group v-model="state.scope" size="default" @change="onScopeChange">
					<el-radio-button label="host">宿主（全局默认）</el-radio-button>
					<el-radio-button label="tenant">指定租户</el-radio-button>
				</el-radio-group>
				<el-select
					v-if="state.scope === 'tenant'"
					v-model="state.tenantId"
					class="ml10 tenant-select"
					filterable
					clearable
					placeholder="选择租户"
					@change="loadFeatures"
				>
					<el-option v-for="t in state.tenantOptions" :key="t.id" :label="`${t.name ?? t.id}`" :value="t.id!" />
				</el-select>
				<el-button type="primary" class="ml10" :loading="state.loading" :disabled="loadDisabled" @click="loadFeatures">刷新</el-button>
				<el-button type="success" :loading="state.saving" :disabled="saveDisabled" @click="onSave">保存</el-button>
				<el-button type="warning" plain :disabled="resetDisabled" @click="onResetOverrides">清除本范围覆盖（恢复默认）</el-button>
			</div>

			<div v-loading="state.loading">
				<template v-if="state.groups.length">
					<el-collapse v-model="state.activeGroups">
						<el-collapse-item v-for="g in state.groups" :key="g.name" :name="g.name">
							<template #title>
								<span class="collapse-title">{{ g.displayName || g.name }}</span>
								<span class="text-secondary text-xs ml10">{{ g.name }}</span>
							</template>
							<div v-for="f in sortedFeatures(g.features)" :key="f.name" class="feature-row" :style="{ paddingLeft: `${12 + (f.depth ?? 0) * 16}px` }">
								<div class="feature-label">
									<div class="name">{{ f.displayName || f.name }}</div>
									<div v-if="f.description" class="desc text-secondary">{{ f.description }}</div>
									<div class="meta text-secondary">名称：{{ f.name }}</div>
								</div>
								<div class="feature-control">
									<el-switch
										v-if="isToggleFeature(f)"
										v-model="state.values[f.name!]"
										active-value="true"
										inactive-value="false"
										inline-prompt
										active-text="开"
										inactive-text="关"
									/>
									<el-input v-else v-model="state.values[f.name!]" clearable placeholder="取值" />
								</div>
							</div>
						</el-collapse-item>
					</el-collapse>
				</template>
				<el-empty v-else-if="!state.loading" description="当前范围无特性或暂无数据" />
			</div>
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
	loading: false,
	saving: false,
});

const loadDisabled = computed(() => state.scope === 'tenant' && !state.tenantId);
const saveDisabled = computed(() => state.loading || loadDisabled.value || !state.groups.length);
const resetDisabled = computed(() => state.loading || loadDisabled.value || !state.groups.length);

function sortedFeatures(list?: FeatureDto[]) {
	return [...(list ?? [])].sort((a, b) => (a.depth ?? 0) - (b.depth ?? 0));
}

function isToggleFeature(f: FeatureDto): boolean {
	const vt = f.valueType;
	if (!vt || typeof vt !== 'object') {
		return f.value === 'true' || f.value === 'false';
	}
	const name = String((vt as Record<string, unknown>).name ?? (vt as Record<string, unknown>).Name ?? '');
	return name.includes('Toggle');
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
		padding: 15px;
	}
}
.system-feature-toolbar {
	display: flex;
	flex-wrap: wrap;
	align-items: center;
}
.tenant-select {
	min-width: 220px;
}
.collapse-title {
	font-weight: 600;
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
		.name {
			font-weight: 500;
		}
		.desc {
			font-size: 12px;
			margin-top: 4px;
		}
		.meta {
			font-size: 12px;
			margin-top: 4px;
		}
	}
	.feature-control {
		width: 220px;
		flex-shrink: 0;
	}
}
.text-secondary {
	color: var(--el-text-color-secondary);
}
.text-xs {
	font-size: 12px;
}
</style>
