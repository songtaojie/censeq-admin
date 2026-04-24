<template>
	<div class="system-tenant-dialog-container">
		<el-dialog :title="state.dialog.title" v-model="state.dialog.isShowDialog" width="680px" destroy-on-close draggable :close-on-click-modal="false" @closed="onClosed">
			<template #header>
				<div style="color: #fff">
					<el-icon size="16" style="margin-right: 4px; display: inline; vertical-align: middle">
						<ele-Edit v-if="state.dialog.type === 'edit'" />
						<ele-Plus v-else />
					</el-icon>
					<span>{{ state.dialog.title }}</span>
				</div>
			</template>
			<el-form ref="formRef" :model="state.ruleForm" :rules="formRules" size="default" label-width="110px">
				<el-tabs v-model="state.activeTab">
					<el-tab-pane label="基本信息" name="basic" style="overflow-y: auto; overflow-x: hidden; padding-right: 4px">
						<el-row :gutter="24">
							<el-col :xs="24" :sm="12" class="mb20">
								<el-form-item label="租户名称" prop="name">
									<el-input v-model="state.ruleForm.name" placeholder="请输入租户名称" clearable maxlength="64" show-word-limit />
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="12" class="mb20">
								<el-form-item label="租户编码" prop="code">
									<el-input
										v-model="state.ruleForm.code"
										placeholder="企业登录账号，须唯一"
										:disabled="state.dialog.type === 'edit'"
										clearable
										maxlength="64"
										show-word-limit
									/>
								</el-form-item>
							</el-col>
							<template v-if="state.dialog.type === 'add'">
								<el-col :xs="24" :sm="12" class="mb20">
									<el-form-item label="管理员邮箱" prop="adminEmailAddress">
										<el-input v-model="state.ruleForm.adminEmailAddress" type="email" placeholder="租户初始管理员邮箱" clearable maxlength="256" />
									</el-form-item>
								</el-col>
								<el-col :xs="24" :sm="12" class="mb20">
									<el-form-item label="管理员密码" prop="adminPassword">
										<el-input v-model="state.ruleForm.adminPassword" type="password" placeholder="初始管理员密码" clearable show-password maxlength="128" />
									</el-form-item>
								</el-col>
							</template>
						</el-row>
					</el-tab-pane>
					<el-tab-pane v-if="state.dialog.type === 'edit'" label="数据库连接" name="connection">
						<div v-if="state.connectionStringUi.forbidden" class="mb15">
							<el-alert type="info" show-icon :closable="false" title="当前账号无「管理连接串」权限，仅可修改租户名称与编码。" />
						</div>
						<template v-else>
							<el-row :gutter="24">
								<el-col :span="24" class="mb20">
									<el-form-item label="连接字符串" label-width="90px">
										<el-input v-model="state.ruleForm.defaultConnectionString" type="textarea" :rows="5" placeholder="留空则使用系统默认连接串" />
									</el-form-item>
								</el-col>
								<el-col v-if="state.connectionStringUi.hasExisting" :span="24" class="mb10">
									<el-form-item label-width="90px">
										<el-button type="danger" plain size="small" icon="ele-Delete" @click="onClearConnectionString">清除已保存的连接串</el-button>
									</el-form-item>
								</el-col>
							</el-row>
						</template>
					</el-tab-pane>
				</el-tabs>
			</el-form>
			<template #footer>
				<span class="dialog-footer">
					<el-button size="default" icon="ele-CircleClose" @click="onCancel">取 消</el-button>
					<el-button type="primary" size="default" icon="ele-Select" :loading="state.submitting" @click="onSubmit">{{ state.dialog.submitTxt }}</el-button>
				</span>
			</template>
		</el-dialog>
	</div>
</template>

<script setup lang="ts" name="systemTenantDialog">
import { reactive, ref, nextTick, computed } from 'vue';
import type { FormInstance, FormRules } from 'element-plus';
import { ElMessage, ElMessageBox } from 'element-plus';
import { useTenantApi } from '/@/api/apis';
import type { TenantDto } from '/@/api/models/tenant';

const emit = defineEmits(['refresh']);

const formRef = ref<FormInstance>();

const emptyForm = () => ({
	name: '',
	code: '',
	adminEmailAddress: '',
	adminPassword: '',
	defaultConnectionString: '',
	concurrencyStamp: '',
	id: '' as string,
});

const state = reactive({
	ruleForm: emptyForm(),
	dialog: {
		isShowDialog: false,
		type: '' as 'add' | 'edit' | '',
		title: '',
		submitTxt: '',
	},
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
	};
	if (state.dialog.type === 'add') {
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
	state.activeTab = 'basic';
	state.connectionStringUi.visible = false;
	state.connectionStringUi.forbidden = false;
	state.connectionStringUi.hasExisting = false;
	state.connectionStringUi.initialSnapshot = '';
};

const openDialog = async (type: string, row?: TenantDto) => {
	resetState();
	state.dialog.type = type as 'add' | 'edit';
	if (type === 'edit' && row) {
		state.ruleForm.name = row.name ?? '';
		state.ruleForm.code = row.code ?? '';
		state.ruleForm.id = row.id!;
		state.ruleForm.concurrencyStamp = row.concurrencyStamp ?? '';
		state.dialog.title = '修改租户';
		state.dialog.submitTxt = '保 存';
		const { getDefaultConnectionString } = useTenantApi();
		try {
			const cs = await getDefaultConnectionString(row.id!);
			state.ruleForm.defaultConnectionString = cs ?? '';
			state.connectionStringUi.initialSnapshot = state.ruleForm.defaultConnectionString;
			state.connectionStringUi.hasExisting = Boolean(cs);
			state.connectionStringUi.visible = true;
			state.connectionStringUi.forbidden = false;
		} catch {
			state.connectionStringUi.visible = false;
			state.connectionStringUi.forbidden = true;
		}
	} else {
		state.dialog.title = '新增租户';
		state.dialog.submitTxt = '新 增';
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

const onClearConnectionString = () => {
	const id = state.ruleForm.id;
	if (!id) return;
	ElMessageBox.confirm('确定清除该租户的默认数据库连接串？', '提示', {
		type: 'warning',
		confirmButtonText: '清除',
		cancelButtonText: '取消',
	})
		.then(async () => {
			const { deleteDefaultConnectionString } = useTenantApi();
			await deleteDefaultConnectionString(id);
			state.ruleForm.defaultConnectionString = '';
			state.connectionStringUi.initialSnapshot = '';
			state.connectionStringUi.hasExisting = false;
			ElMessage.success('已清除');
		})
		.catch(() => {});
};

const onSubmit = async () => {
	if (!formRef.value) return;
	await formRef.value.validate(async (valid) => {
		if (!valid) return;
		state.submitting = true;
		const { createTenant, updateTenant, updateDefaultConnectionString } = useTenantApi();
		try {
			if (state.dialog.type === 'add') {
				await createTenant({
					name: state.ruleForm.name.trim(),
					code: state.ruleForm.code.trim(),
					adminEmailAddress: state.ruleForm.adminEmailAddress.trim(),
					adminPassword: state.ruleForm.adminPassword,
				});
				ElMessage.success('创建成功');
			} else if (state.dialog.type === 'edit') {
				await updateTenant(state.ruleForm.id, {
					name: state.ruleForm.name.trim(),
					code: state.ruleForm.code.trim(),
					concurrencyStamp: state.ruleForm.concurrencyStamp,
				});
				if (state.connectionStringUi.visible) {
					const nextCs = state.ruleForm.defaultConnectionString;
					if (nextCs !== state.connectionStringUi.initialSnapshot) {
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
