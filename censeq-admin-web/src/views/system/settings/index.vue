<template>
	<div class="system-configs-container layout-padding">
		<!-- 筛选栏 -->
		<el-card shadow="hover">
			<div class="filter-bar">
				<label class="filter-bar__label">关键字</label>
				<el-input
					v-model="state.filter"
					placeholder="配置名称 / 配置编码"
					clearable
					style="width: 240px"
					@keyup.enter="loadData"
					@clear="loadData"
				/>
				<el-button-group>
					<el-button type="primary" icon="ele-Search" @click="() => { page.current = 1; loadData(); }">查询</el-button>
					<el-button icon="ele-Refresh" @click="onReset">重置</el-button>
				</el-button-group>
				<el-button type="primary" icon="ele-Plus" @click="onAdd">新增配置</el-button>
			</div>
		</el-card>

		<!-- 数据表格 -->
		<el-card class="full-table settings-table-card" shadow="hover" style="margin-top: 5px">
			<el-table :data="state.tableData" v-loading="state.loading" style="width: 100%" border highlight-current-row>
				<el-table-column type="index" label="序号" width="60" align="center" header-align="center" :index="(i: number) => (page.current - 1) * page.size + i + 1" />
				<el-table-column prop="displayName" label="配置名称" min-width="180" header-align="center" show-overflow-tooltip>
					<template #default="{ row }">
						<span class="setting-name-cell">{{ row.displayName }}</span>
					</template>
				</el-table-column>
				<el-table-column prop="name" label="配置编码" min-width="200" header-align="center" align="center" show-overflow-tooltip>
					<template #default="{ row }">
						<el-tag size="small" type="info" effect="plain">{{ row.name }}</el-tag>
					</template>
				</el-table-column>
				<el-table-column prop="currentValue" label="全局值" min-width="180" header-align="center" align="center" show-overflow-tooltip>
					<template #default="{ row }">
						<el-tag v-if="row.currentValue !== null && row.currentValue !== undefined" size="small" effect="plain" class="setting-value-tag">
							{{ row.currentValue }}
						</el-tag>
						<el-text v-else type="info" size="small">（使用默认值）</el-text>
					</template>
				</el-table-column>
				<el-table-column prop="defaultValue" label="默认值" min-width="140" header-align="center" align="center" show-overflow-tooltip>
					<template #default="{ row }">
						<span v-if="row.defaultValue">{{ row.defaultValue }}</span>
						<el-text v-else type="info" size="small">—</el-text>
					</template>
				</el-table-column>
				<el-table-column prop="description" label="描述" min-width="180" header-align="center" show-overflow-tooltip>
					<template #default="{ row }">
						<span v-if="row.description">{{ row.description }}</span>
						<el-text v-else type="info" size="small">—</el-text>
					</template>
				</el-table-column>
				<el-table-column label="内置" width="80" align="center" header-align="center">
					<template #default="{ row }">
						<el-tag v-if="row.isSystem" size="small" type="warning">是</el-tag>
						<el-tag v-else size="small" type="success">否</el-tag>
					</template>
				</el-table-column>
				<el-table-column label="操作" width="230" align="center" header-align="center" fixed="right">
					<template #default="{ row }">
						<div class="table-actions">
							<el-button text type="primary" size="small" icon="ele-Edit" @click="onEdit(row)">编辑</el-button>
							<el-button text type="primary" size="small" icon="ele-Setting" @click="openValueDrawer(row)">配置值</el-button>
							<el-button text type="danger" size="small" icon="ele-Delete" :disabled="row.isSystem" @click="onDelete(row)">删除</el-button>
						</div>
					</template>
				</el-table-column>
			</el-table>
			<!-- 分页 -->
			<div class="table-pagination">
				<el-pagination
					v-model:current-page="page.current"
					v-model:page-size="page.size"
					:page-sizes="[10, 20, 50, 100]"
					:total="state.total"
					layout="total, sizes, prev, pager, next, jumper"
					background
					@current-change="onPageChange"
					@size-change="() => { page.current = 1; onPageChange(); }"
				/>
			</div>
		</el-card>

		<!-- 新增 / 编辑 弹窗 -->
		<el-dialog v-model="dialog.visible" class="settings-dialog" width="600px" draggable :close-on-click-modal="false" destroy-on-close>
			<template #header>
				<div class="settings-dialog__title">
					<el-icon size="16"><ele-Edit /></el-icon>
					<span>{{ dialog.isEdit ? '编辑配置' : '新增配置' }}</span>
				</div>
			</template>
			<el-form ref="formRef" :model="dialog.form" :rules="formRules" label-width="auto" class="settings-dialog-form">
				<el-row :gutter="24">
					<el-col :xs="24" :sm="12" class="mb20">
						<el-form-item label="配置编码" prop="name">
							<el-input v-model="dialog.form.name" placeholder="唯一编码，例如 App.Theme" clearable :disabled="dialog.isEdit" />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" class="mb20">
						<el-form-item label="配置名称" prop="displayName">
							<el-input v-model="dialog.form.displayName" placeholder="可读名称" clearable />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" class="mb20">
						<el-form-item label="全局值">
							<el-input v-model="dialog.form.currentValue" placeholder="Global 级别值，留空则不写入" clearable />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" class="mb20">
						<el-form-item label="默认值">
							<el-input v-model="dialog.form.defaultValue" placeholder="内置默认值（可选）" clearable />
						</el-form-item>
					</el-col>
					<el-col :xs="24" class="mb20">
						<el-form-item label="描述">
							<el-input v-model="dialog.form.description" type="textarea" :rows="3" placeholder="备注或说明" clearable />
						</el-form-item>
					</el-col>
					<el-col :xs="24" class="mb20">
						<el-form-item label="客户端可见">
							<el-switch v-model="dialog.form.isVisibleToClients" />
						</el-form-item>
					</el-col>
				</el-row>
			</el-form>
			<template #footer>
				<span class="dialog-footer">
					<el-button @click="dialog.visible = false">取消</el-button>
					<el-button type="primary" :loading="dialog.saving" @click="onSubmit">确定</el-button>
				</span>
			</template>
		</el-dialog>

		<!-- 配置值管理 Drawer -->
		<el-drawer
			v-model="valueDrawer.visible"
			:title="`配置值管理 — ${valueDrawer.displayName}`"
			size="680px"
			destroy-on-close
		>
			<div v-loading="valueDrawer.loading" class="setting-value-drawer">
				<div class="setting-value-hero">
					<div>
						<div class="setting-value-hero__title">{{ valueDrawer.displayName }}</div>
						<div class="setting-value-hero__code">{{ valueDrawer.name }}</div>
					</div>
					<el-alert
						title="优先级：用户 > 租户 > 全局 > 默认值"
						type="info"
						:closable="false"
					/>
				</div>

				<el-card shadow="never" class="setting-value-editor">
					<template #header>
						<div class="setting-value-editor__header">
							<span>{{ valueDrawer.editingKey ? '编辑配置值' : '新增配置值' }}</span>
							<el-button v-if="valueDrawer.editingKey" link @click="resetValueForm">取消编辑</el-button>
						</div>
					</template>
					<el-form label-width="88px" class="setting-value-form">
						<el-form-item label="类型">
							<el-radio-group v-model="valueDrawer.form.providerName" @change="onScopeChange">
								<el-radio-button v-for="option in scopeOptions" :key="option.value" :label="option.value">{{ option.label }}</el-radio-button>
							</el-radio-group>
						</el-form-item>
						<el-form-item v-if="valueDrawer.form.providerName === 'T'" label="租户">
							<el-select v-model="valueDrawer.form.providerKey" filterable placeholder="选择租户" style="width: 100%">
								<el-option v-for="tenant in valueDrawer.tenants" :key="tenant.id" :label="tenant.name ?? tenant.id" :value="tenant.id" />
							</el-select>
						</el-form-item>
						<el-form-item v-if="valueDrawer.form.providerName === 'U'" label="用户">
							<el-select v-model="valueDrawer.form.providerKey" filterable placeholder="选择用户" style="width: 100%">
								<el-option v-for="user in valueDrawer.users" :key="user.id" :label="getUserName(user)" :value="user.id" />
							</el-select>
						</el-form-item>
						<el-form-item label="配置值">
							<el-input v-model="valueDrawer.form.value" placeholder="输入该作用域下的配置值" clearable />
						</el-form-item>
						<el-form-item>
							<el-button type="primary" :loading="valueDrawer.saving" @click="saveValueEntry">{{ valueDrawer.editingKey ? '保存修改' : '新增配置值' }}</el-button>
							<el-button v-if="valueDrawer.form.value" @click="valueDrawer.form.value = ''">清空输入</el-button>
						</el-form-item>
					</el-form>
				</el-card>

				<el-card shadow="never" class="setting-value-list">
					<template #header>
						<div class="setting-value-editor__header">
							<span>已配置的作用域值</span>
							<el-tag type="info" effect="plain">{{ valueDrawer.entries.length }} 条</el-tag>
						</div>
					</template>
					<el-empty v-if="!valueDrawer.entries.length" description="还没有覆盖值，将回退到默认值" />
					<el-table v-else :data="valueDrawer.entries" class="setting-scope-table" border size="small" style="width: 100%">
						<el-table-column label="类型" width="96" align="center">
							<template #default="{ row }">
								<el-tag :type="row.scopeTag" effect="light">{{ row.scopeLabel }}</el-tag>
							</template>
						</el-table-column>
						<el-table-column label="目标" min-width="180" show-overflow-tooltip>
							<template #default="{ row }">
								{{ getTargetLabel(row.providerName, row.providerKey) }}
							</template>
						</el-table-column>
						<el-table-column label="配置值" min-width="180" show-overflow-tooltip>
							<template #default="{ row }">
								<span v-if="row.value">{{ row.value }}</span>
								<el-text v-else type="info" size="small">—</el-text>
							</template>
						</el-table-column>
						<el-table-column label="操作" width="120" align="center">
							<template #default="{ row }">
								<div class="table-actions table-actions--compact">
									<el-button text type="primary" size="small" @click="editValueEntry(row)">编辑</el-button>
									<el-button text type="danger" size="small" @click="deleteValueEntry(row)">删除</el-button>
								</div>
							</template>
						</el-table-column>
					</el-table>
				</el-card>
			</div>
		</el-drawer>
	</div>
</template>

<script setup lang="ts" name="systemSettings">
import { reactive, ref, onMounted } from 'vue';
import type { FormInstance, FormRules } from 'element-plus';
import { ElMessage, ElMessageBox } from 'element-plus';
import { useIdentityUserApi, useSettingManagementApi } from '/@/api/apis';
import { useTenantApi } from '/@/api/apis/tenant-management/tenant.service';
import { SETTING_PROVIDER_NAMES, type SettingDefinitionDto, type SettingProviderName } from '/@/api/models/setting-management';
import type { IdentityUserDto } from '/@/api/models/identity';
import type { TenantDto } from '/@/api/models/tenant';

const api = useSettingManagementApi();
const identityUserApi = useIdentityUserApi();
const tenantApi = useTenantApi();
const formRef = ref<FormInstance>();

type ScopeType = SettingProviderName;

interface SettingValueEntry {
	providerName: ScopeType;
	providerKey: string | null;
	value: string | null;
	scopeLabel: string;
	scopeTag: 'primary' | 'success' | 'warning';
}

const scopeOptions = [
	{ label: '全局', value: SETTING_PROVIDER_NAMES.global },
	{ label: '租户', value: SETTING_PROVIDER_NAMES.tenant },
	{ label: '用户', value: SETTING_PROVIDER_NAMES.user },
];

const scopeLabelMap: Record<ScopeType, string> = {
	[SETTING_PROVIDER_NAMES.global]: '全局',
	[SETTING_PROVIDER_NAMES.tenant]: '租户',
	[SETTING_PROVIDER_NAMES.user]: '用户',
};

const scopeTagType: Record<ScopeType, 'primary' | 'success' | 'warning'> = {
	[SETTING_PROVIDER_NAMES.global]: 'primary',
	[SETTING_PROVIDER_NAMES.tenant]: 'success',
	[SETTING_PROVIDER_NAMES.user]: 'warning',
};

const state = reactive({
	loading: false,
	filter: '',
	tableData: [] as SettingDefinitionDto[],
	total: 0,
});

const page = reactive({
	current: 1,
	size: 10,
});

const dialog = reactive({
	visible: false,
	isEdit: false,
	saving: false,
	editId: '',
	form: {
		name: '',
		displayName: '',
		currentValue: '',
		defaultValue: '',
		description: '',
		isVisibleToClients: false,
	},
});

const formRules: FormRules = {
	name: [{ required: true, message: '请输入配置编码', trigger: 'blur' }],
	displayName: [{ required: true, message: '请输入配置名称', trigger: 'blur' }],
};

async function loadData() {
	state.loading = true;
	try {
		const res = await api.getSettingDefinitions({
			filter: state.filter || undefined,
			skipCount: (page.current - 1) * page.size,
			maxResultCount: page.size,
		});
		state.tableData = res.items ?? [];
		state.total = res.totalCount ?? 0;
	} finally {
		state.loading = false;
	}
}

function onPageChange() {
	loadData();
}

function onReset() {
	state.filter = '';
	page.current = 1;
	loadData();
}

function onAdd() {
	dialog.isEdit = false;
	dialog.editId = '';
	dialog.form = { name: '', displayName: '', currentValue: '', defaultValue: '', description: '', isVisibleToClients: false };
	dialog.visible = true;
}

function onEdit(row: SettingDefinitionDto) {
	dialog.isEdit = true;
	dialog.editId = row.id;
	dialog.form = {
		name: row.name,
		displayName: row.displayName,
		currentValue: row.currentValue ?? '',
		defaultValue: row.defaultValue ?? '',
		description: row.description ?? '',
		isVisibleToClients: row.isVisibleToClients,
	};
	dialog.visible = true;
}

async function onSubmit() {
	const valid = await formRef.value?.validate().catch(() => false);
	if (!valid) return;
	dialog.saving = true;
	try {
		if (dialog.isEdit) {
			await api.updateSettingDefinition(dialog.editId, {
				displayName: dialog.form.displayName,
				description: dialog.form.description || null,
				defaultValue: dialog.form.defaultValue || null,
				currentValue: dialog.form.currentValue || null,
				isVisibleToClients: dialog.form.isVisibleToClients,
			});
			ElMessage.success('更新成功');
		} else {
			await api.createSettingDefinition({
				name: dialog.form.name,
				displayName: dialog.form.displayName,
				description: dialog.form.description || null,
				defaultValue: dialog.form.defaultValue || null,
				currentValue: dialog.form.currentValue || null,
				isVisibleToClients: dialog.form.isVisibleToClients,
			});
			ElMessage.success('新增成功');
		}
		dialog.visible = false;
		await loadData();
	} finally {
		dialog.saving = false;
	}
}

async function onDelete(row: SettingDefinitionDto) {
	await ElMessageBox.confirm(`确定删除配置「${row.displayName}」？`, '提示', {
		type: 'warning',
		confirmButtonText: '删除',
		confirmButtonClass: 'el-button--danger',
	});
	await api.deleteSettingDefinition(row.id);
	ElMessage.success('删除成功');
	await loadData();
}

// ── 配置值管理 ─────────────────────────────────────────────────────────────────

const valueDrawer = reactive({
	visible: false,
	loading: false,
	saving: false,
	name: '',
	displayName: '',
	editingKey: '' as string,
	entries: [] as SettingValueEntry[],
	tenants: [] as TenantDto[],
	users: [] as IdentityUserDto[],
	form: {
		providerName: SETTING_PROVIDER_NAMES.global as ScopeType,
		providerKey: '' as string | null,
		value: '',
	},
});

function getTenantName(tenantId: string): string {
	const t = valueDrawer.tenants.find((x) => x.id === tenantId);
	return t?.name ?? tenantId;
}

function getUserName(user: IdentityUserDto): string {
	const fullName = [user.name, user.surname].filter(Boolean).join(' ');
	return fullName || user.userName || user.email || user.id || '';
}

function getUserLabel(userId: string): string {
	const user = valueDrawer.users.find((x) => x.id === userId);
	return user ? getUserName(user) : userId;
}

function getTargetLabel(providerName: ScopeType, providerKey: string | null): string {
	if (providerName === SETTING_PROVIDER_NAMES.global) {
		return '全站默认';
	}

	if (!providerKey) {
		return '未指定';
	}

	return providerName === SETTING_PROVIDER_NAMES.tenant ? getTenantName(providerKey) : getUserLabel(providerKey);
}

function toEntryKey(providerName: ScopeType, providerKey: string | null): string {
	return `${providerName}:${providerKey ?? ''}`;
}

function resetValueForm() {
	valueDrawer.editingKey = '';
	valueDrawer.form.providerName = SETTING_PROVIDER_NAMES.global;
	valueDrawer.form.providerKey = null;
	valueDrawer.form.value = '';
}

function onScopeChange() {
	valueDrawer.form.providerKey = valueDrawer.form.providerName === SETTING_PROVIDER_NAMES.global ? null : '';
}

function applyValueData(valueData: { globalValue?: string | null; tenantValues: Array<{ tenantId: string; value?: string | null }>; userValues: Array<{ userId: string; value?: string | null }> }) {
	const entries: SettingValueEntry[] = [];

	if (valueData.globalValue !== null && valueData.globalValue !== undefined) {
		entries.push({ providerName: SETTING_PROVIDER_NAMES.global, providerKey: null, value: valueData.globalValue, scopeLabel: scopeLabelMap[SETTING_PROVIDER_NAMES.global], scopeTag: scopeTagType[SETTING_PROVIDER_NAMES.global] });
	}

	entries.push(
		...(valueData.tenantValues ?? []).map((item) => ({
			providerName: SETTING_PROVIDER_NAMES.tenant as ScopeType,
			providerKey: item.tenantId,
			value: item.value ?? null,
			scopeLabel: scopeLabelMap[SETTING_PROVIDER_NAMES.tenant],
			scopeTag: scopeTagType[SETTING_PROVIDER_NAMES.tenant],
		}))
	);

	entries.push(
		...(valueData.userValues ?? []).map((item) => ({
			providerName: SETTING_PROVIDER_NAMES.user as ScopeType,
			providerKey: item.userId,
			value: item.value ?? null,
			scopeLabel: scopeLabelMap[SETTING_PROVIDER_NAMES.user],
			scopeTag: scopeTagType[SETTING_PROVIDER_NAMES.user],
		}))
	);

	valueDrawer.entries = entries;
}

async function refreshValueEntries() {
	const valueData = await api.getSettingValue(valueDrawer.name);
	applyValueData(valueData);
}

async function openValueDrawer(row: SettingDefinitionDto) {
	valueDrawer.name = row.name;
	valueDrawer.displayName = row.displayName;
	valueDrawer.visible = true;
	valueDrawer.loading = true;
	resetValueForm();
	try {
		const [valueData, tenantPage, userPage] = await Promise.all([
			api.getSettingValue(row.name),
			tenantApi.getTenantPage({ maxResultCount: 1000 }),
			identityUserApi.getIdentityUserPage({ maxResultCount: 1000 }),
		]);
		applyValueData(valueData);
		valueDrawer.tenants = tenantPage.items ?? [];
		valueDrawer.users = userPage.items ?? [];
	} finally {
		valueDrawer.loading = false;
	}
}

function editValueEntry(row: SettingValueEntry) {
	valueDrawer.editingKey = toEntryKey(row.providerName, row.providerKey);
	valueDrawer.form.providerName = row.providerName;
	valueDrawer.form.providerKey = row.providerKey;
	valueDrawer.form.value = row.value ?? '';
}

async function saveValueEntry() {
	if (valueDrawer.form.providerName !== SETTING_PROVIDER_NAMES.global && !valueDrawer.form.providerKey) {
		ElMessage.warning(valueDrawer.form.providerName === SETTING_PROVIDER_NAMES.tenant ? '请选择租户' : '请选择用户');
		return;
	}

	valueDrawer.saving = true;
	try {
		await api.setSettingValue({
			name: valueDrawer.name,
			providerName: valueDrawer.form.providerName,
			providerKey: valueDrawer.form.providerName === SETTING_PROVIDER_NAMES.global ? null : valueDrawer.form.providerKey,
			value: valueDrawer.form.value || null,
		});
		await refreshValueEntries();
		resetValueForm();
		ElMessage.success('配置值已保存');
	} finally {
		valueDrawer.saving = false;
	}

}

async function deleteValueEntry(row: SettingValueEntry) {
	await ElMessageBox.confirm(`确定删除${scopeLabelMap[row.providerName]}作用域下的配置值？`, '提示', { type: 'warning' });
	await api.deleteSettingValue(valueDrawer.name, row.providerName, row.providerKey);
	valueDrawer.entries = valueDrawer.entries.filter((item) => toEntryKey(item.providerName, item.providerKey) !== toEntryKey(row.providerName, row.providerKey));
	if (valueDrawer.editingKey === toEntryKey(row.providerName, row.providerKey)) {
		resetValueForm();
	}
	ElMessage.success('已删除');
}

onMounted(() => {
	loadData();
});
</script>

<style scoped>
.filter-bar {
	display: flex;
	align-items: center;
	gap: 10px;
	flex-wrap: wrap;
	padding-bottom: 4px;
}

.filter-bar__label {
	font-size: 14px;
	color: #606266;
	white-space: nowrap;
}

.table-pagination {
	display: flex;
	justify-content: flex-end;
	padding: 16px 0 4px;
}

.settings-table-card :deep(.el-table__header-wrapper th.el-table__cell) {
	background: #f5f7fa;
	color: #606266;
}

.settings-dialog__title {
	display: inline-flex;
	align-items: center;
	gap: 6px;
	font-weight: 600;
}

.settings-dialog-form :deep(.el-form-item) {
	margin-bottom: 0;
}

.settings-dialog :deep(.el-dialog__body) {
	padding-top: 18px;
	padding-bottom: 8px;
}

.settings-dialog :deep(.el-dialog__footer) {
	padding-top: 0;
}

.settings-table-card :deep(.el-table td.el-table__cell) {
	padding-top: 10px;
	padding-bottom: 10px;
}

.setting-name-cell {
	font-weight: 500;
	color: #1f2937;
}

.setting-value-tag {
	max-width: 100%;
	overflow: hidden;
	text-overflow: ellipsis;
	vertical-align: middle;
}

.table-actions {
	display: flex;
	align-items: center;
	justify-content: center;
	gap: 4px;
	white-space: nowrap;
	flex-wrap: nowrap;
}

.table-actions :deep(.el-button) {
	margin-left: 0;
	padding-left: 4px;
	padding-right: 4px;
}

.table-actions--compact {
	gap: 2px;
}

.setting-value-drawer {
	display: grid;
	gap: 16px;
	padding: 4px;
}

.setting-value-hero {
	display: grid;
	gap: 12px;
	padding: 16px;
	border-radius: 6px;
	background: #f8fafc;
	border: 1px solid #ebeef5;
}

.setting-value-hero__title {
	font-size: 18px;
	font-weight: 600;
	color: #1f2a37;
}

.setting-value-hero__code {
	margin-top: 6px;
	font-size: 13px;
	color: #5b6472;
}

.setting-value-editor,
.setting-value-list {
	border-radius: 4px;
	border: 1px solid #ebeef5;
}

.setting-value-editor__header {
	display: flex;
	align-items: center;
	justify-content: space-between;
	gap: 12px;
	font-weight: 600;
}

.setting-value-form :deep(.el-form-item) {
	margin-bottom: 18px;
}

.setting-scope-table :deep(.el-table td.el-table__cell) {
	padding-top: 8px;
	padding-bottom: 8px;
}

</style>
