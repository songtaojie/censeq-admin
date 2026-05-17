<template>
	<div class="system-tenant-dialog-container">
		<el-dialog :title="state.dialog.title" v-model="state.dialog.isShowDialog" width="760px" destroy-on-close
			draggable :close-on-click-modal="false" @closed="onClosed">
			<template #header>
				<div style="color: #fff">
					<el-icon size="16" style="margin-right: 4px; display: inline; vertical-align: middle">
						<ele-Edit v-if="state.dialog.type === 'edit'" />
						<ele-Plus v-else />
					</el-icon>
					<span>{{ state.dialog.title }}</span>
				</div>
			</template>
			<el-form ref="formRef" :model="state.ruleForm" :rules="formRules" size="default" label-width="110px"
				autocomplete="off">
				<el-tabs v-model="state.activeTab">
					<el-tab-pane label="基本信息" name="basic"
						style="overflow-y: auto; overflow-x: hidden; padding-right: 4px">
						<el-row :gutter="24">
							<el-col :xs="24" :sm="12" class="mb20">
								<el-form-item label="租户名称" prop="name">
									<el-input v-model="state.ruleForm.name" placeholder="请输入租户名称" clearable
										maxlength="64" show-word-limit />
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="12" class="mb20">
								<el-form-item label="租户编码" prop="code">
									<el-input v-model="state.ruleForm.code" placeholder="企业登录账号，须唯一"
										:disabled="state.dialog.type === 'edit'" clearable maxlength="64"
										show-word-limit />
								</el-form-item>
							</el-col>
							<template v-if="state.dialog.type === 'add'">
								<el-col :xs="24" :sm="12" class="mb20">
									<el-form-item label="租管账号" prop="adminUserName">
										<el-input v-model="state.ruleForm.adminUserName" name="tenant-admin-username"
											autocomplete="off" placeholder="租户管理员登录账号" clearable maxlength="256" />
									</el-form-item>
								</el-col>
								<el-col :xs="24" :sm="12" class="mb20">
									<el-form-item label="租管名称" prop="adminName">
										<el-input v-model="state.ruleForm.adminName" name="tenant-admin-name"
											autocomplete="off" placeholder="租户管理员显示名称" clearable maxlength="64" />
									</el-form-item>
								</el-col>
								<el-col :xs="24" :sm="12" class="mb20">
									<el-form-item label="管理员邮箱" prop="adminEmailAddress">
										<el-input v-model="state.ruleForm.adminEmailAddress" type="email"
											name="tenant-admin-email" autocomplete="off" placeholder="租户初始管理员邮箱"
											clearable maxlength="256" />
									</el-form-item>
								</el-col>
								<el-col :xs="24" :sm="12" class="mb20">
									<el-form-item label="管理员密码" prop="adminPassword">
										<el-input v-model="state.ruleForm.adminPassword" type="password"
											name="tenant-admin-password" autocomplete="new-password"
											placeholder="初始管理员密码" clearable show-password maxlength="128" />
									</el-form-item>
								</el-col>
							</template>
							<el-col v-if="state.dialog.type === 'edit'" :xs="24" :sm="12" class="mb20">
								<el-form-item label="状态">
									<el-switch v-model="state.ruleForm.isActive" inline-prompt active-text="启用"
										inactive-text="禁用" />
								</el-form-item>
							</el-col>
						</el-row>
					</el-tab-pane>
					<el-tab-pane label="扩展信息" name="profile">
						<div class="tenant-profile-form">
							<el-row :gutter="24">
								<el-col :xs="24" :sm="12" class="mb20">
									<el-form-item prop="domain">
										<template #label>
											<span class="form-label-with-help">
												域名
												<el-tooltip content="用于租户访问跳转和租户解析，例如 tenant.example.com。"
													placement="top">
													<el-icon class="label-help-icon"><ele-QuestionFilled /></el-icon>
												</el-tooltip>
											</span>
										</template>
										<el-input v-model="state.ruleForm.domain" placeholder="tenant.example.com"
											clearable maxlength="256" show-word-limit />
									</el-form-item>
								</el-col>
								<el-col :xs="24" :sm="12" class="mb20">
									<el-form-item prop="maxUserCount">
										<template #label>
											<span class="form-label-with-help">
												用户上限
												<el-tooltip content="当前租户允许创建的最大用户数量，0 表示不限制。" placement="top">
													<el-icon class="label-help-icon"><ele-QuestionFilled /></el-icon>
												</el-tooltip>
											</span>
										</template>
										<el-input-number v-model="state.ruleForm.maxUserCount" :min="0" :precision="0"
											controls-position="right" style="width: 100%" />
									</el-form-item>
								</el-col>
								<el-col :xs="24" :sm="12" class="mb20">
									<el-form-item prop="icon">
										<template #label>
											<span class="form-label-with-help">
												图标 URL
												<el-tooltip content="租户品牌图标图片地址，建议使用 HTTPS 图片链接。" placement="top">
													<el-icon class="label-help-icon"><ele-QuestionFilled /></el-icon>
												</el-tooltip>
											</span>
										</template>
										<el-input v-model="state.ruleForm.icon"
											placeholder="https://example.com/logo.png" clearable maxlength="512"
											show-word-limit />
									</el-form-item>
								</el-col>
								<el-col :xs="24" :sm="12" class="mb20">
									<el-form-item label="版权信息" prop="copyright">
										<el-input v-model="state.ruleForm.copyright" placeholder="Copyright © 2026 企业名称"
											clearable maxlength="256" show-word-limit />
									</el-form-item>
								</el-col>
								<el-col :xs="24" :sm="12" class="mb20">
									<el-form-item label="ICP备案号" prop="icpNo">
										<el-input v-model="state.ruleForm.icpNo" placeholder="京ICP备xxxxxxxx号" clearable
											maxlength="64" show-word-limit />
									</el-form-item>
								</el-col>
								<el-col :xs="24" :sm="12" class="mb20">
									<el-form-item prop="icpAddress">
										<template #label>
											<span class="form-label-with-help">
												ICP地址
												<el-tooltip content="备案链接地址，通常为 https://beian.miit.gov.cn。"
													placement="top">
													<el-icon class="label-help-icon"><ele-QuestionFilled /></el-icon>
												</el-tooltip>
											</span>
										</template>
										<el-input v-model="state.ruleForm.icpAddress"
											placeholder="https://beian.miit.gov.cn" clearable maxlength="512"
											show-word-limit />
									</el-form-item>
								</el-col>
								<el-col :span="24" class="mb20">
									<el-form-item label="备注" prop="remark">
										<el-input v-model="state.ruleForm.remark" type="textarea" :rows="3"
											placeholder="租户内部备注" maxlength="1024" show-word-limit />
									</el-form-item>
								</el-col>
							</el-row>
						</div>
					</el-tab-pane>
					<el-tab-pane label="数据隔离" name="connection">
						<div class="tenant-isolation-panel">
							<el-alert
								type="info"
								show-icon
								:closable="false"
								title="共享库模式使用系统默认连接串并依赖 TenantId 隔离；独立库模式会为当前租户保存默认数据库连接串。"
							/>
							<el-alert
								v-if="state.connectionStringUi.forbidden"
								type="warning"
								show-icon
								:closable="false"
								title="当前账号无「管理连接串」权限，仅可查看当前隔离方式，不能修改连接串配置。"
							/>
							<el-row :gutter="24">
								<el-col :span="24" class="mb20">
									<el-form-item label="隔离方式" label-width="90px">
										<el-radio-group v-model="state.isolationMode" :disabled="state.connectionStringUi.forbidden" @change="onIsolationModeChange">
											<el-radio label="shared">共享库（TenantId 隔离）</el-radio>
											<el-radio label="dedicated">独立库（连接串隔离）</el-radio>
										</el-radio-group>
									</el-form-item>
								</el-col>
								<el-col :span="24" class="mb20">
									<div v-if="state.isolationMode === 'shared'" class="isolation-tip-card shared">
										<strong>当前将使用共享库模式</strong>
										<p>租户数据会写入系统默认数据库，并通过 TenantId 进行逻辑隔离。此模式下无需配置租户专属连接串。</p>
									</div>
									<div v-else class="isolation-tip-card dedicated">
										<strong>当前将使用独立库模式</strong>
										<p>需要为该租户保存默认数据库连接串。保存后，该租户的数据库迁移与运行时解析都会走租户独立库。</p>
									</div>
								</el-col>
								<el-col v-if="state.isolationMode === 'dedicated' && !state.connectionStringUi.forbidden" :span="24" class="mb20">
									<el-form-item label="连接字符串" label-width="90px" prop="defaultConnectionString">
										<el-input v-model="state.ruleForm.defaultConnectionString" type="textarea"
											:rows="5" placeholder="请输入租户独立库连接串" />
									</el-form-item>
								</el-col>
							</el-row>
						</div>
					</el-tab-pane>
				</el-tabs>
			</el-form>
			<template #footer>
				<span class="dialog-footer">
					<el-button icon="ele-CircleClose" @click="onCancel">取 消</el-button>
					<el-button type="primary" icon="ele-Select" :loading="state.submitting" @click="onSubmit">{{
						state.dialog.submitTxt }}</el-button>
				</span>
			</template>
		</el-dialog>
	</div>
</template>

<script setup lang="ts" name="systemTenantDialog">
import { reactive, ref, nextTick, computed } from 'vue';
import type { FormInstance, FormRules } from 'element-plus';
import { ElMessage } from 'element-plus';
import { useTenantApi } from '/@/api/apis';
import type { TenantDto } from '/@/api/models/tenant';

const emit = defineEmits(['refresh']);

const formRef = ref<FormInstance>();

type IsolationMode = 'shared' | 'dedicated';
type TenantDialogRow = TenantDto & { _hasConnection?: boolean };

const emptyForm = () => ({
	name: '',
	code: '',
	isActive: true,
	adminEmailAddress: '',
	adminUserName: '',
	adminName: '',
	adminPassword: '',
	defaultConnectionString: '',
	concurrencyStamp: '',
	id: '' as string,
	domain: '',
	icon: '',
	copyright: '',
	icpNo: '',
	icpAddress: '',
	remark: '',
	maxUserCount: 0,
});

const state = reactive({
	ruleForm: emptyForm(),
	dialog: {
		isShowDialog: false,
		type: '' as 'add' | 'edit' | '',
		title: '',
		submitTxt: '',
	},
	isolationMode: 'shared' as IsolationMode,
	activeTab: 'basic',
	submitting: false,
	connectionStringUi: {
		visible: false,
		forbidden: false,
		hasExisting: false,
		/** 打开弹窗时拉取的原始连接串，用于判断是否调用更新接口 */
		initialSnapshot: '' as string,
	},
});

/** 新增时才校验管理员字段，避免编辑模式下隐藏项触发必填校验 */
const formRules = computed<FormRules>(() => {
	const rules: FormRules = {
		name: [
			{ required: true, message: '请输入租户名称', trigger: 'blur' },
			{ min: 1, max: 64, message: '长度 1~64 个字符', trigger: 'blur' },
		],
		code: [
			{ required: true, message: '请输入租户编码', trigger: 'blur' },
			{ min: 1, max: 64, message: '长度 1~64 个字符', trigger: 'blur' },
		],
		domain: [{ max: 256, message: '最大 256 个字符', trigger: 'blur' }],
		icon: [{ max: 512, message: '最大 512 个字符', trigger: 'blur' }],
		copyright: [{ max: 256, message: '最大 256 个字符', trigger: 'blur' }],
		icpNo: [{ max: 64, message: '最大 64 个字符', trigger: 'blur' }],
		defaultConnectionString:
			state.isolationMode === 'dedicated' && !state.connectionStringUi.forbidden
				? [{ required: true, message: '独立库模式下请输入连接字符串', trigger: 'blur' }]
				: [],
		icpAddress: [
			{ type: 'url', message: '请输入正确的 URL', trigger: 'blur' },
			{ max: 512, message: '最大 512 个字符', trigger: 'blur' },
		],
		remark: [{ max: 1024, message: '最大 1024 个字符', trigger: 'blur' }],
		maxUserCount: [{ type: 'number', min: 0, message: '用户上限不能小于 0', trigger: 'change' }],
	};
	if (state.dialog.type === 'add') {
		rules.adminUserName = [
			{ required: true, message: '请输入租管账号', trigger: 'blur' },
			{ min: 1, max: 256, message: '长度 1~256 个字符', trigger: 'blur' },
		];
		rules.adminName = [
			{ required: true, message: '请输入租管名称', trigger: 'blur' },
			{ min: 1, max: 64, message: '长度 1~64 个字符', trigger: 'blur' },
		];
		rules.adminEmailAddress = [
			{ required: true, message: '请输入管理员邮箱', trigger: 'blur' },
			{ type: 'email', message: '邮箱格式不正确', trigger: 'blur' },
		];
		rules.adminPassword = [
			{ required: true, message: '请输入管理员密码', trigger: 'blur' },
			{ min: 6, max: 128, message: '建议长度 6~128 个字符', trigger: 'blur' },
		];
	}
	return rules;
});

const resetState = () => {
	state.ruleForm = emptyForm();
	state.isolationMode = 'shared';
	state.activeTab = 'basic';
	state.connectionStringUi.visible = false;
	state.connectionStringUi.forbidden = false;
	state.connectionStringUi.hasExisting = false;
	state.connectionStringUi.initialSnapshot = '';
};

const onIsolationModeChange = () => {
	formRef.value?.clearValidate(['defaultConnectionString']);
};

const openDialog = async (type: string, row?: TenantDialogRow) => {
	resetState();
	state.dialog.type = type as 'add' | 'edit';
	if (type === 'edit' && row) {
		state.ruleForm.name = row.name ?? '';
		state.ruleForm.code = row.code ?? '';
		state.ruleForm.isActive = row.isActive ?? true;
		state.ruleForm.domain = row.domain ?? '';
		state.ruleForm.icon = row.icon ?? '';
		state.ruleForm.copyright = row.copyright ?? '';
		state.ruleForm.icpNo = row.icpNo ?? '';
		state.ruleForm.icpAddress = row.icpAddress ?? '';
		state.ruleForm.remark = row.remark ?? '';
		state.ruleForm.maxUserCount = row.maxUserCount ?? 0;
		state.ruleForm.id = row.id!;
		state.ruleForm.concurrencyStamp = row.concurrencyStamp ?? '';
		state.dialog.title = '修改租户';
		state.dialog.submitTxt = '保 存';
		const { getDefaultConnectionString } = useTenantApi();
		try {
			const cs = await getDefaultConnectionString(row.id!);
			state.ruleForm.defaultConnectionString = (cs ?? '').trim();
			state.connectionStringUi.initialSnapshot = state.ruleForm.defaultConnectionString;
			state.connectionStringUi.hasExisting = Boolean(cs);
			state.connectionStringUi.visible = true;
			state.connectionStringUi.forbidden = false;
			state.isolationMode = cs ? 'dedicated' : 'shared';
		} catch {
			state.connectionStringUi.visible = false;
			state.connectionStringUi.forbidden = true;
			state.isolationMode = row._hasConnection ? 'dedicated' : 'shared';
		}
	} else {
		state.dialog.title = '新增租户';
		state.dialog.submitTxt = '新 增';
		state.connectionStringUi.visible = true;
		state.connectionStringUi.forbidden = false;
		state.isolationMode = 'shared';
	}
	state.dialog.isShowDialog = true;
	await nextTick();
	formRef.value?.clearValidate();
};

const closeDialog = () => {
	state.dialog.isShowDialog = false;
};

const onCancel = () => {
	closeDialog();
};

const onClosed = () => {
	resetState();
	state.dialog.type = '';
};

const normalizeOptional = (value: string) => value.trim() || null;

const buildProfilePayload = () => ({
	domain: normalizeOptional(state.ruleForm.domain),
	icon: normalizeOptional(state.ruleForm.icon),
	copyright: normalizeOptional(state.ruleForm.copyright),
	icpNo: normalizeOptional(state.ruleForm.icpNo),
	icpAddress: normalizeOptional(state.ruleForm.icpAddress),
	remark: normalizeOptional(state.ruleForm.remark),
	maxUserCount: state.ruleForm.maxUserCount ?? 0,
});

const onSubmit = async () => {
	if (!formRef.value) return;
	await formRef.value.validate(async (valid) => {
		if (!valid) return;
		state.submitting = true;
		const { createTenant, updateTenant, updateDefaultConnectionString, deleteDefaultConnectionString } = useTenantApi();
		try {
			if (state.dialog.type === 'add') {
				const created = await createTenant({
					name: state.ruleForm.name.trim(),
					code: state.ruleForm.code.trim(),
					adminUserName: state.ruleForm.adminUserName.trim(),
					adminName: state.ruleForm.adminName.trim(),
					adminEmailAddress: state.ruleForm.adminEmailAddress.trim(),
					adminPassword: state.ruleForm.adminPassword,
					...buildProfilePayload(),
				});
				if (state.isolationMode === 'dedicated' && !state.connectionStringUi.forbidden) {
					await updateDefaultConnectionString(created.id, state.ruleForm.defaultConnectionString.trim());
				}
				ElMessage.success('创建成功');
			} else if (state.dialog.type === 'edit') {
				await updateTenant(state.ruleForm.id, {
					name: state.ruleForm.name.trim(),
					code: state.ruleForm.code.trim(),
					isActive: state.ruleForm.isActive,
					concurrencyStamp: state.ruleForm.concurrencyStamp,
					...buildProfilePayload(),
				});
				if (!state.connectionStringUi.forbidden) {
					const nextCs = state.ruleForm.defaultConnectionString.trim();
					const hadConnection = Boolean(state.connectionStringUi.initialSnapshot);
					if (state.isolationMode === 'shared') {
						if (hadConnection) {
							await deleteDefaultConnectionString(state.ruleForm.id);
						}
					} else if (nextCs !== state.connectionStringUi.initialSnapshot) {
						await updateDefaultConnectionString(state.ruleForm.id, nextCs);
					}
				}
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
.system-tenant-dialog-container {
	.tenant-isolation-panel {
		display: grid;
		gap: 14px;
		padding: 4px 4px 0 0;
	}

	.isolation-tip-card {
		padding: 14px 16px;
		border-radius: 10px;
		border: 1px solid var(--el-border-color-light);
		background: var(--el-fill-color-extra-light);
		display: grid;
		gap: 6px;

		strong {
			font-size: 14px;
			color: var(--el-text-color-primary);
		}

		p {
			margin: 0;
			font-size: 13px;
			line-height: 1.7;
			color: var(--el-text-color-regular);
		}
	}

	.isolation-tip-card.shared {
		background: var(--el-fill-color-extra-light);
	}

	.isolation-tip-card.dedicated {
		background: var(--el-color-warning-light-9);
		border-color: var(--el-color-warning-light-5);
	}

	.tenant-profile-form {
		max-height: 460px;
		overflow-y: auto;
		overflow-x: hidden;
		padding: 14px 10px 0 0;
	}

	.form-label-with-help {
		display: inline-flex;
		align-items: center;
		justify-content: flex-end;
		gap: 4px;
		width: 100%;
		height: 32px;
		line-height: 32px;
	}

	.label-help-icon {
		display: inline-flex;
		align-items: center;
		justify-content: center;
		color: var(--el-text-color-secondary);
		cursor: help;
		font-size: 14px;
		height: 32px;
		line-height: 32px;
		transition: color 0.2s ease;

		&:hover {
			color: var(--el-color-primary);
		}
	}

	:deep(.tenant-profile-form .el-form-item) {
		margin-bottom: 0;
	}

	:deep(.tenant-profile-form .el-textarea__inner) {
		resize: vertical;
	}
}
</style>
