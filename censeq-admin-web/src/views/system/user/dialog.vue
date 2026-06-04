<template>
	<div class="system-user-dialog-container">
		<el-dialog v-model="state.dialog.isShowDialog" width="760px" destroy-on-close draggable :close-on-click-modal="false" @closed="onClosed">
			<template #header>
				<div style="color: #fff">
					<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle">
						<ele-Edit v-if="state.dialog.type === 'edit'" />
						<ele-Plus v-else />
					</el-icon>
					<span>{{ state.dialog.title }}</span>
				</div>
			</template>

			<el-tabs v-model="activeTab" class="user-dialog-tabs">
				<el-tab-pane label="基本信息" name="basic">
					<el-form ref="formRef" :model="state.ruleForm" :rules="formRules" label-width="100px" size="default" class="user-form-grid">
						<el-row :gutter="20">
							<el-col :span="12">
								<el-form-item label="用户名" prop="userName">
									<el-input v-model="state.ruleForm.userName" placeholder="登录账号" :disabled="state.dialog.type === 'edit'" clearable />
								</el-form-item>
							</el-col>
							<el-col v-if="state.dialog.type === 'add'" :span="12">
								<el-form-item label="密码" prop="password" required>
									<el-input v-model="state.ruleForm.password" type="password" show-password clearable placeholder="请输入密码" />
								</el-form-item>
							</el-col>
						</el-row>
						<el-row :gutter="20">
							<el-col :span="12">
								<el-form-item label="姓氏" prop="surname">
									<el-input v-model="state.ruleForm.surname" placeholder="Surname" clearable />
								</el-form-item>
							</el-col>
							<el-col :span="12">
								<el-form-item label="名字" prop="name">
									<el-input v-model="state.ruleForm.name" placeholder="Name" clearable />
								</el-form-item>
							</el-col>
						</el-row>
						<el-row :gutter="20">
							<el-col :span="12">
								<el-form-item label="邮箱" prop="email">
									<el-input v-model="state.ruleForm.email" type="email" clearable />
								</el-form-item>
							</el-col>
							<el-col :span="12">
								<el-form-item label="手机号">
									<el-input v-model="state.ruleForm.phoneNumber" clearable />
								</el-form-item>
							</el-col>
						</el-row>
						<el-row :gutter="20">
							<el-col :span="12">
								<el-form-item label="启用状态">
									<el-switch v-model="state.ruleForm.isActive" inline-prompt active-text="启用" inactive-text="禁用" />
								</el-form-item>
							</el-col>
							<el-col :span="12">
								<el-form-item label="失败锁定">
									<el-switch v-model="state.ruleForm.lockoutEnabled" inline-prompt active-text="开启" inactive-text="关闭" />
								</el-form-item>
							</el-col>
						</el-row>
						<el-form-item prop="organizationUnitIds" required>
							<template #label>
								<span class="form-label-with-help">
									所属机构
									<el-tooltip content="用户至少需要归属到一个组织机构，多选时默认第一个为主组织" placement="top">
										<el-icon class="form-label-help"><ele-QuestionFilled /></el-icon>
									</el-tooltip>
								</span>
							</template>
							<el-tree-select
								v-model="state.ruleForm.organizationUnitIds"
								:data="organizationUnitTreeOptions"
								:props="organizationUnitTreeProps"
								node-key="id"
								multiple
								show-checkbox
								check-strictly
								filterable
								collapse-tags
								collapse-tags-tooltip
								default-expand-all
								:render-after-expand="false"
								placeholder="可选多个组织单元"
								class="w100"
								@change="onOrganizationUnitsChange"
							/>
						</el-form-item>
						<el-form-item v-if="state.ruleForm.organizationUnitIds.length > 1" label="主组织机构">
							<el-select v-model="state.primaryOrganizationUnitId" filterable placeholder="请选择主组织机构" class="w100">
								<el-option v-for="ou in selectedOrganizationUnits" :key="ou.id" :label="ouLabel(ou)" :value="ou.id!" />
							</el-select>
						</el-form-item>
					</el-form>
				</el-tab-pane>

				<el-tab-pane label="角色授权" name="roles">
					<div class="role-transfer">
						<div class="role-transfer-panel">
							<div class="role-transfer-header">
								<el-checkbox
									:model-value="isAllAvailableChecked"
									:indeterminate="isAvailableIndeterminate"
									:disabled="!availableRoles.length"
									@change="toggleAvailableChecked"
								>
									未授权
								</el-checkbox>
								<span>{{ state.roleTransfer.availableCheckedRoleNames.length }}/{{ availableRoles.length }}</span>
							</div>
							<div class="role-transfer-search">
								<el-input v-model="state.roleTransfer.availableKeyword" placeholder="搜索" :prefix-icon="Search" clearable />
							</div>
							<el-checkbox-group v-model="state.roleTransfer.availableCheckedRoleNames" class="role-transfer-list">
								<el-checkbox v-for="role in availableRoles" :key="role.name" :label="role.name">
									{{ roleDisplayName(role) }}
								</el-checkbox>
							</el-checkbox-group>
						</div>

						<div class="role-transfer-actions">
							<el-button
								type="primary"
								icon="ele-DArrowRight"
								:disabled="!availableRoles.length"
								@click="authorizeRoles(availableRoles.map((role) => role.name))"
							/>
							<el-button
								type="primary"
								icon="ele-ArrowRight"
								:disabled="!state.roleTransfer.availableCheckedRoleNames.length"
								@click="authorizeRoles(state.roleTransfer.availableCheckedRoleNames)"
							/>
							<el-button
								type="primary"
								icon="ele-ArrowLeft"
								:disabled="!state.roleTransfer.selectedCheckedRoleNames.length"
								@click="revokeRoles(state.roleTransfer.selectedCheckedRoleNames)"
							/>
							<el-button
								type="primary"
								icon="ele-DArrowLeft"
								:disabled="!selectedRoles.length"
								@click="revokeRoles(selectedRoles.map((role) => role.name))"
							/>
						</div>

						<div class="role-transfer-panel">
							<div class="role-transfer-header">
								<el-checkbox
									:model-value="isAllSelectedChecked"
									:indeterminate="isSelectedIndeterminate"
									:disabled="!selectedRoles.length"
									@change="toggleSelectedChecked"
								>
									已授权
								</el-checkbox>
								<span>{{ state.roleTransfer.selectedCheckedRoleNames.length }}/{{ selectedRoles.length }}</span>
							</div>
							<div class="role-transfer-search">
								<el-input v-model="state.roleTransfer.selectedKeyword" placeholder="搜索" :prefix-icon="Search" clearable />
							</div>
							<el-checkbox-group v-model="state.roleTransfer.selectedCheckedRoleNames" class="role-transfer-list">
								<el-checkbox v-for="role in selectedRoles" :key="role.name" :label="role.name">
									{{ roleDisplayName(role) }}
								</el-checkbox>
							</el-checkbox-group>
						</div>
					</div>
				</el-tab-pane>
			</el-tabs>

			<template #footer>
				<el-button icon="ele-CircleClose" @click="onCancel">取消</el-button>
				<el-button type="primary" icon="ele-Select" :loading="state.submitting" @click="onSubmit">{{ state.dialog.submitTxt }}</el-button>
			</template>
		</el-dialog>
	</div>
</template>

<script setup lang="ts" name="systemUserDialog">
import { reactive, ref, nextTick, computed, watch } from 'vue';
import type { FormInstance, FormRules } from 'element-plus';
import { ElMessage } from 'element-plus';
import { Search } from '@element-plus/icons-vue';
import { useIdentityApi } from '/@/api/apis';
import type { IdentityRoleDto, IdentityUserDto, OrganizationUnitDto } from '/@/api/models/identity';

const emit = defineEmits(['refresh']);

const formRef = ref<FormInstance>();
const activeTab = ref('basic');

type OrganizationUnitTreeOption = OrganizationUnitDto & {
	value: string;
	label: string;
	children?: OrganizationUnitTreeOption[];
};

interface RuleForm {
	userId: string;
	userName: string;
	password: string;
	name: string;
	surname: string;
	email: string;
	phoneNumber: string;
	isActive: boolean;
	lockoutEnabled: boolean;
	concurrencyStamp: string;
	organizationUnitIds: string[];
}

const emptyForm = (): RuleForm => ({
	userId: '',
	userName: '',
	password: '',
	name: '',
	surname: '',
	email: '',
	phoneNumber: '',
	isActive: true,
	lockoutEnabled: true,
	concurrencyStamp: '',
	organizationUnitIds: [],
});

const state = reactive({
	ruleForm: emptyForm(),
	roleNames: [] as string[],
	roleTransfer: {
		availableKeyword: '',
		selectedKeyword: '',
		availableCheckedRoleNames: [] as string[],
		selectedCheckedRoleNames: [] as string[],
	},
	primaryOrganizationUnitId: null as string | null,
	roleOptions: [] as IdentityRoleDto[],
	ouOptions: [] as OrganizationUnitDto[],
	dialog: {
		isShowDialog: false,
		type: '' as 'add' | 'edit' | '',
		title: '',
		submitTxt: '',
	},
	submitting: false,
});

const formRules = computed<FormRules>(() => {
	const rules: FormRules = {
		userName: [{ required: true, message: '请输入用户名', trigger: 'blur' }],
		email: [
			{ required: true, message: '请输入邮箱', trigger: 'blur' },
			{ type: 'email', message: '邮箱格式不正确', trigger: 'blur' },
		],
		organizationUnitIds: [
			{
				validator: (_rule, _value, callback) => {
					if (state.ruleForm.organizationUnitIds.length) {
						callback();
					} else {
						callback(new Error('请选择所属机构'));
					}
				},
				trigger: 'change',
			},
		],
	};
	if (state.dialog.type === 'add') {
		rules.password = [
			{ required: true, message: '请输入初始密码', trigger: 'blur' },
			{ min: 6, message: '至少 6 位', trigger: 'blur' },
		];
	}
	return rules;
});

function ouLabel(ou: OrganizationUnitDto): string {
	return ou.displayName ?? '';
}

const organizationUnitTreeProps = {
	label: 'label',
	value: 'value',
	children: 'children',
};

function buildOrganizationUnitTree(items: OrganizationUnitDto[]): OrganizationUnitTreeOption[] {
	const byId = new Map<string, OrganizationUnitTreeOption>();
	items.forEach((item) => {
		if (!item.id) return;
		byId.set(item.id, { ...item, value: item.id, label: ouLabel(item), children: [] });
	});

	const roots: OrganizationUnitTreeOption[] = [];
	byId.forEach((node) => {
		const parentId = node.parentId;
		if (parentId && byId.has(parentId)) {
			byId.get(parentId)!.children!.push(node);
		} else {
			roots.push(node);
		}
	});

	const sortTree = (nodes: OrganizationUnitTreeOption[]) => {
		nodes.sort((a, b) => (a.code ?? '').localeCompare(b.code ?? '', undefined, { numeric: true }));
		nodes.forEach((node) => {
			if (node.children?.length) {
				sortTree(node.children);
			}
		});
	};
	sortTree(roots);
	return roots;
}

const organizationUnitTreeOptions = computed(() => buildOrganizationUnitTree(state.ouOptions));

const selectedOrganizationUnits = computed(() => state.ouOptions.filter((ou) => !!ou.id && state.ruleForm.organizationUnitIds.includes(ou.id)));

type RoleTransferItem = IdentityRoleDto & { name: string };

function roleDisplayName(role: RoleTransferItem): string {
	return role.code ? `${role.name} [${role.code}]` : role.name;
}

function roleMatchesKeyword(role: RoleTransferItem, keyword: string): boolean {
	const value = keyword.trim().toLowerCase();
	if (!value) return true;
	return [role.name, role.code, role.remark].some((text) => (text ?? '').toLowerCase().includes(value));
}

const normalizedRoleOptions = computed<RoleTransferItem[]>(() => state.roleOptions.filter((role): role is RoleTransferItem => !!role.name));
const selectedRoleNameSet = computed(() => new Set(state.roleNames));
const availableRoles = computed(() =>
	normalizedRoleOptions.value.filter((role) => !selectedRoleNameSet.value.has(role.name) && roleMatchesKeyword(role, state.roleTransfer.availableKeyword))
);
const selectedRoles = computed(() =>
	normalizedRoleOptions.value.filter((role) => selectedRoleNameSet.value.has(role.name) && roleMatchesKeyword(role, state.roleTransfer.selectedKeyword))
);

const isAllAvailableChecked = computed(() => availableRoles.value.length > 0 && state.roleTransfer.availableCheckedRoleNames.length === availableRoles.value.length);
const isAvailableIndeterminate = computed(
	() => state.roleTransfer.availableCheckedRoleNames.length > 0 && state.roleTransfer.availableCheckedRoleNames.length < availableRoles.value.length
);
const isAllSelectedChecked = computed(() => selectedRoles.value.length > 0 && state.roleTransfer.selectedCheckedRoleNames.length === selectedRoles.value.length);
const isSelectedIndeterminate = computed(
	() => state.roleTransfer.selectedCheckedRoleNames.length > 0 && state.roleTransfer.selectedCheckedRoleNames.length < selectedRoles.value.length
);

function toggleAvailableChecked(checked: boolean) {
	state.roleTransfer.availableCheckedRoleNames = checked ? availableRoles.value.map((role) => role.name) : [];
}

function toggleSelectedChecked(checked: boolean) {
	state.roleTransfer.selectedCheckedRoleNames = checked ? selectedRoles.value.map((role) => role.name) : [];
}

function authorizeRoles(roleNames: string[]) {
	const next = new Set(state.roleNames);
	roleNames.forEach((name) => next.add(name));
	state.roleNames = normalizedRoleOptions.value.filter((role) => next.has(role.name)).map((role) => role.name);
	state.roleTransfer.availableCheckedRoleNames = [];
}

function revokeRoles(roleNames: string[]) {
	const revokeSet = new Set(roleNames);
	state.roleNames = state.roleNames.filter((name) => !revokeSet.has(name));
	state.roleTransfer.selectedCheckedRoleNames = [];
}

watch([availableRoles, selectedRoles], () => {
	const availableNameSet = new Set(availableRoles.value.map((role) => role.name));
	const selectedNameSet = new Set(selectedRoles.value.map((role) => role.name));
	state.roleTransfer.availableCheckedRoleNames = state.roleTransfer.availableCheckedRoleNames.filter((name) => availableNameSet.has(name));
	state.roleTransfer.selectedCheckedRoleNames = state.roleTransfer.selectedCheckedRoleNames.filter((name) => selectedNameSet.has(name));
});

function syncPrimaryOrganizationUnit() {
	const ids = state.ruleForm.organizationUnitIds;
	if (!ids.length) {
		state.primaryOrganizationUnitId = null;
		return;
	}
	if (!state.primaryOrganizationUnitId || !ids.includes(state.primaryOrganizationUnitId)) {
		state.primaryOrganizationUnitId = ids[0];
	}
}

watch(() => [...state.ruleForm.organizationUnitIds], syncPrimaryOrganizationUnit);

async function onOrganizationUnitsChange() {
	syncPrimaryOrganizationUnit();
	await nextTick();
	formRef.value?.validateField('organizationUnitIds');
}

const loadRolesAndOus = async () => {
	const api = useIdentityApi();
	const [rolesRes, ouRes] = await Promise.all([api.getAllRoles(), api.getOrganizationUnitAllList()]);
	state.roleOptions = rolesRes.items ?? [];
	state.ouOptions = ouRes.items ?? [];
};

const openDialog = async (type: string, row?: IdentityUserDto) => {
	state.dialog.type = type as 'add' | 'edit';
	state.ruleForm = emptyForm();
	state.roleNames = [];
	state.roleTransfer.availableKeyword = '';
	state.roleTransfer.selectedKeyword = '';
	state.roleTransfer.availableCheckedRoleNames = [];
	state.roleTransfer.selectedCheckedRoleNames = [];
	state.primaryOrganizationUnitId = null;
	activeTab.value = 'basic';
	state.dialog.isShowDialog = true;
	state.dialog.title = type === 'edit' ? '修改用户' : '新增用户';
	state.dialog.submitTxt = type === 'edit' ? '保存' : '新增';
	await loadRolesAndOus();
	if (type === 'edit' && row?.id) {
		state.ruleForm.userId = row.id;
		state.ruleForm.userName = row.userName ?? '';
		state.ruleForm.name = row.name ?? '';
		state.ruleForm.surname = row.surname ?? '';
		state.ruleForm.email = row.email ?? '';
		state.ruleForm.phoneNumber = row.phoneNumber ?? '';
		state.ruleForm.isActive = row.isActive ?? true;
		state.ruleForm.lockoutEnabled = row.lockoutEnabled ?? true;
		state.ruleForm.concurrencyStamp = row.concurrencyStamp ?? '';
		const api = useIdentityApi();
		const [roleRes, ouRes] = await Promise.all([api.getUserRoles(row.id), api.getUserOrganizationUnits(row.id)]);
		state.roleNames = (roleRes.items ?? []).map((r: IdentityRoleDto) => r.name).filter(Boolean) as string[];
		state.ruleForm.organizationUnitIds = (ouRes.items ?? []).map((o) => o.id!).filter(Boolean);
		state.primaryOrganizationUnitId = (ouRes.items ?? []).find((o) => o.isPrimary)?.id ?? state.ruleForm.organizationUnitIds[0] ?? null;
	}
	syncPrimaryOrganizationUnit();
	await nextTick();
	formRef.value?.clearValidate();
};

function onClosed() {
	state.dialog.type = '';
}

const closeDialog = () => {
	state.dialog.isShowDialog = false;
};

const onCancel = () => closeDialog();

const buildUserPayload = () => ({
	userName: state.ruleForm.userName.trim(),
	name: state.ruleForm.name?.trim() || undefined,
	surname: state.ruleForm.surname?.trim() || undefined,
	email: state.ruleForm.email.trim(),
	phoneNumber: state.ruleForm.phoneNumber?.trim() || undefined,
	isActive: state.ruleForm.isActive,
	lockoutEnabled: state.ruleForm.lockoutEnabled,
	roleNames: [...state.roleNames],
});

const buildOrganizationUnitPayload = () => ({
	organizationUnitIds: [...state.ruleForm.organizationUnitIds],
	primaryOrganizationUnitId: state.primaryOrganizationUnitId,
});

const onSubmit = async () => {
	if (!formRef.value) return;
	await formRef.value.validate(async (valid) => {
		if (!valid) return;
		state.submitting = true;
		const api = useIdentityApi();
		try {
			if (state.dialog.type === 'add') {
				const created = await api.createUser({
					...buildUserPayload(),
					password: state.ruleForm.password,
				});
				if (created.id && state.ruleForm.organizationUnitIds.length) {
					await api.updateUserOrganizationUnits(created.id, buildOrganizationUnitPayload());
				}
				ElMessage.success('创建成功');
			} else if (state.dialog.type === 'edit' && state.ruleForm.userId) {
				await api.updateUser(state.ruleForm.userId, {
					...buildUserPayload(),
					concurrencyStamp: state.ruleForm.concurrencyStamp,
				});
				await api.updateUserOrganizationUnits(state.ruleForm.userId, buildOrganizationUnitPayload());
				ElMessage.success('保存成功');
			}
			closeDialog();
			emit('refresh');
		} finally {
			state.submitting = false;
		}
	});
};

defineExpose({ openDialog });
</script>

<style scoped lang="scss">
.user-dialog-tabs {
	:deep(.el-tabs__header) {
		margin-bottom: 16px;
	}
}

.user-form-grid {
	:deep(.el-form-item) {
		margin-bottom: 20px !important;
	}
}

.role-hint {
	display: flex;
	align-items: center;
	gap: 6px;
	color: var(--el-text-color-secondary);
	font-size: 12px;
	margin-top: -4px;
	padding-left: 80px;

	.el-icon {
		color: var(--el-color-info);
	}
}

.basic-org-hint {
	margin-top: -12px;
	padding-left: 100px;
}

.form-label-with-help {
	display: inline-flex;
	align-items: center;
	gap: 4px;
}

.form-label-help {
	color: var(--el-text-color-secondary);
	cursor: help;
	font-size: 14px;
	vertical-align: middle;
}

.role-transfer {
	display: grid;
	grid-template-columns: minmax(0, 1fr) 56px minmax(0, 1fr);
	gap: 16px;
	align-items: center;
	min-height: 330px;
	margin-top: 8px;
}

.role-transfer-panel {
	height: 330px;
	border: 1px solid var(--el-border-color);
	border-radius: 4px;
	background: var(--el-bg-color);
	display: flex;
	flex-direction: column;
	overflow: hidden;
}

.role-transfer-header {
	height: 36px;
	padding: 0 10px;
	border-bottom: 1px solid var(--el-border-color-lighter);
	display: flex;
	align-items: center;
	justify-content: space-between;
	color: var(--el-text-color-primary);
	font-size: 13px;
	background: var(--el-fill-color-extra-light);
}

.role-transfer-search {
	padding: 8px;
	border-bottom: 1px solid var(--el-border-color-lighter);

	:deep(.el-input__wrapper) {
		border-radius: 4px;
	}
}

.role-transfer-list {
	flex: 1;
	padding: 8px 10px;
	overflow-y: auto;

	:deep(.el-checkbox) {
		width: 100%;
		height: 28px;
		margin-right: 0;
		display: flex;
		align-items: center;
	}

	:deep(.el-checkbox__label) {
		min-width: 0;
		overflow: hidden;
		text-overflow: ellipsis;
		white-space: nowrap;
	}
}

.role-transfer-actions {
	display: flex;
	flex-direction: column;
	align-items: center;
	gap: 10px;

	:deep(.el-button) {
		width: 34px;
		height: 28px;
		margin-left: 0;
		padding: 0;
	}
}
</style>
