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
				<!-- 基本信息 -->
				<el-tab-pane label="基本信息" name="basic">
					<el-form ref="formRef" :model="state.ruleForm" :rules="formRules" label-width="100px" size="default" class="user-form-grid">
						<el-row :gutter="20">
							<el-col :span="12">
								<el-form-item label="用户名" prop="userName">
									<el-input v-model="state.ruleForm.userName" placeholder="登录账号" :disabled="state.dialog.type === 'edit'" clearable />
								</el-form-item>
							</el-col>
							<el-col :span="12">
								<el-form-item
									:label="state.dialog.type === 'add' ? '密码' : '新密码'"
									prop="password"
									:required="state.dialog.type === 'add'"
								>
									<el-input v-model="state.ruleForm.password" type="password" show-password clearable :placeholder="state.dialog.type === 'edit' ? '留空则不修改' : '请输入密码'" />
								</el-form-item>
							</el-col>
						</el-row>
						<el-row :gutter="20">
							<el-col :span="12">
								<el-form-item label="姓" prop="surname">
									<el-input v-model="state.ruleForm.surname" placeholder="Surname" clearable />
								</el-form-item>
							</el-col>
							<el-col :span="12">
								<el-form-item label="名" prop="name">
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
								<el-form-item label="手机号码">
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
					</el-form>
				</el-tab-pane>

				<!-- 角色授权 -->
				<el-tab-pane label="角色授权" name="roles">
					<el-form label-width="80px" size="default" style="margin-top: 8px">
						<el-form-item label="所属角色">
							<el-select v-model="state.roleNames" multiple filterable collapse-tags collapse-tags-tooltip placeholder="请选择角色" class="w100">
								<el-option v-for="r in state.roleOptions" :key="r.id" :label="r.name ?? ''" :value="r.name ?? ''" />
							</el-select>
						</el-form-item>
						<div class="role-hint">
							<el-icon><ele-InfoFilled /></el-icon>
							<span>用户将继承所选角色的所有权限</span>
						</div>
					</el-form>
				</el-tab-pane>

				<!-- 组织机构 -->
				<el-tab-pane label="组织机构" name="orgs">
					<el-form label-width="80px" size="default" style="margin-top: 8px">
						<el-form-item label="所属机构">
							<el-select
								v-model="state.organizationUnitIds"
								multiple
								filterable
								collapse-tags
								collapse-tags-tooltip
								placeholder="可选多个组织单元"
								class="w100"
							>
								<el-option v-for="ou in state.ouOptions" :key="ou.id" :label="ouLabel(ou)" :value="ou.id!" />
							</el-select>
						</el-form-item>
						<div class="role-hint">
							<el-icon><ele-InfoFilled /></el-icon>
							<span>可将用户归属到一个或多个组织机构</span>
						</div>
					</el-form>
				</el-tab-pane>
			</el-tabs>

			<template #footer>
				<el-button icon="ele-CircleClose" @click="onCancel">取 消</el-button>
				<el-button type="primary" icon="ele-Select" :loading="state.submitting" @click="onSubmit">{{ state.dialog.submitTxt }}</el-button>
			</template>
		</el-dialog>
	</div>
</template>

<script setup lang="ts" name="systemUserDialog">
import { reactive, ref, nextTick, computed } from 'vue';
import type { FormInstance, FormRules } from 'element-plus';
import { ElMessage } from 'element-plus';
import { useIdentityApi } from '/@/api/apis';
import type { IdentityRoleDto, IdentityUserDto, OrganizationUnitDto } from '/@/api/models/identity';

const emit = defineEmits(['refresh']);

const formRef = ref<FormInstance>();
const activeTab = ref('basic');

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
});

const state = reactive({
	ruleForm: emptyForm(),
	roleNames: [] as string[],
	organizationUnitIds: [] as string[],
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
	};
	if (state.dialog.type === 'add') {
		rules.password = [
			{ required: true, message: '请输入初始密码', trigger: 'blur' },
			{ min: 6, message: '至少 6 位', trigger: 'blur' },
		];
	} else {
		rules.password = [{ validator: validateOptionalPassword, trigger: 'blur' }];
	}
	return rules;
});

function validateOptionalPassword(_: unknown, val: string, cb: (e?: Error) => void) {
	if (val && val.length > 0 && val.length < 6) {
		cb(new Error('若填写新密码须至少 6 位'));
	} else {
		cb();
	}
}

function ouLabel(ou: OrganizationUnitDto): string {
	const code = ou.code ? ` [${ou.code}]` : '';
	return `${ou.displayName ?? ''}${code}`;
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
	state.organizationUnitIds = [];
	activeTab.value = 'basic';
	state.dialog.isShowDialog = true;
	state.dialog.title = type === 'edit' ? '修改用户' : '新增用户';
	state.dialog.submitTxt = type === 'edit' ? '保 存' : '新 增';
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
		state.ruleForm.password = '';
		const api = useIdentityApi();
		const [roleRes, ouRes] = await Promise.all([api.getUserRoles(row.id), api.getUserOrganizationUnits(row.id)]);
		state.roleNames = (roleRes.items ?? []).map((r: IdentityRoleDto) => r.name).filter(Boolean) as string[];
		state.organizationUnitIds = (ouRes.items ?? []).map((o) => o.id!).filter(Boolean);
	}
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

const onSubmit = async () => {
	if (!formRef.value) return;
	await formRef.value.validate(async (valid) => {
		if (!valid) return;
		state.submitting = true;
		const api = useIdentityApi();
		try {
			const phone = state.ruleForm.phoneNumber?.trim() || undefined;
			if (state.dialog.type === 'add') {
				const created = await api.createUser({
					userName: state.ruleForm.userName.trim(),
					password: state.ruleForm.password,
					name: state.ruleForm.name?.trim() || undefined,
					surname: state.ruleForm.surname?.trim() || undefined,
					email: state.ruleForm.email.trim(),
					phoneNumber: phone,
					isActive: state.ruleForm.isActive,
					lockoutEnabled: state.ruleForm.lockoutEnabled,
					roleNames: [...state.roleNames],
				});
				if (created.id && state.organizationUnitIds.length) {
					await api.updateUserOrganizationUnits(created.id, { organizationUnitIds: [...state.organizationUnitIds] });
				}
				ElMessage.success('创建成功');
			} else if (state.dialog.type === 'edit' && state.ruleForm.userId) {
				const id = state.ruleForm.userId;
				const pwd = state.ruleForm.password?.trim();
				await api.updateUser(id, {
					userName: state.ruleForm.userName.trim(),
					name: state.ruleForm.name?.trim() || undefined,
					surname: state.ruleForm.surname?.trim() || undefined,
					email: state.ruleForm.email.trim(),
					phoneNumber: phone,
					isActive: state.ruleForm.isActive,
					lockoutEnabled: state.ruleForm.lockoutEnabled,
					roleNames: [...state.roleNames],
					concurrencyStamp: state.ruleForm.concurrencyStamp,
					...(pwd ? { password: pwd } : {}),
				});
				await api.updateUserOrganizationUnits(id, { organizationUnitIds: [...state.organizationUnitIds] });
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
</style>

