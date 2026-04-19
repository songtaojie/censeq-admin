<template>
	<div class="role-edit-dialog-container">
		<el-dialog :title="state.dialog.title" v-model="state.dialog.isShowDialog" width="560px" destroy-on-close>
			<div class="dialog-intro">维护角色名称及其基础属性。默认角色会自动分配给新用户，公共角色可供其他用户查看和选择。</div>
			<el-form ref="formRef" :model="state.ruleForm" :rules="rules" size="default" label-width="96px" class="role-form">
				<el-form-item label="角色编码" prop="code">
					<el-input
						v-model="state.ruleForm.code"
						placeholder="请输入角色编码"
						clearable
						maxlength="64"
						show-word-limit
						:disabled="state.dialog.type === 'edit' && !!state.originalCode"
					/>
					<div class="form-tip">角色编码唯一；编码一旦设置，后续不允许修改。</div>
				</el-form-item>
				<el-form-item label="角色名称" prop="name">
					<el-input v-model="state.ruleForm.name" placeholder="请输入角色名称" clearable maxlength="50" show-word-limit />
				</el-form-item>
				<el-row :gutter="12">
					<el-col :span="12">
						<el-form-item label="默认角色">
							<el-switch v-model="state.ruleForm.isDefault" inline-prompt active-text="是" inactive-text="否" />
							<div class="form-tip">默认角色会自动分配给新用户</div>
						</el-form-item>
					</el-col>
					<el-col :span="12">
						<el-form-item label="公共角色">
							<el-switch v-model="state.ruleForm.isPublic" inline-prompt active-text="是" inactive-text="否" />
							<div class="form-tip">公共角色可以被其他用户查看</div>
						</el-form-item>
					</el-col>
				</el-row>
			</el-form>
			<template #footer>
				<span class="dialog-footer">
					<el-button @click="onCancel" size="default">取 消</el-button>
					<el-button type="primary" @click="onSubmit" size="default" :loading="state.loading">
						{{ state.dialog.submitTxt }}
					</el-button>
				</span>
			</template>
		</el-dialog>
	</div>
</template>

<script setup lang="ts" name="roleEditDialog">
import { reactive, ref } from 'vue';
import { ElMessage, FormRules } from 'element-plus';
import { IdentityRoleDto } from '/@/api/models/identity';
import { useIdentityApi } from '/@/api/apis';

// 定义子组件向父组件传值/事件
const emit = defineEmits(['refresh']);

const formRef = ref();

const state = reactive({
	ruleForm: {
		code: '',
		isDefault: false,
		isPublic: false,
	} as IdentityRoleDto,
	originalCode: '',
	dialog: {
		isShowDialog: false,
		type: '' as 'add' | 'edit' | '',
		title: '',
		submitTxt: '',
	},
	loading: false,
});

const rules: FormRules = {
	code: [{ max: 64, message: '长度不能超过 64 个字符', trigger: 'blur' }],
	name: [
		{ required: true, message: '请输入角色名称', trigger: 'blur' },
		{ min: 2, max: 50, message: '长度在 2 到 50 个字符', trigger: 'blur' },
	],
};

// 打开弹窗
const openDialog = (row?: IdentityRoleDto) => {
	state.dialog.type = row ? 'edit' : 'add';
	if (row) {
		state.ruleForm = { ...row };
		state.originalCode = row.code || '';
		state.dialog.title = '修改角色';
		state.dialog.submitTxt = '修 改';
	} else {
		state.ruleForm = { isDefault: false, isPublic: false, name: '', code: '' } as IdentityRoleDto;
		state.originalCode = '';
		state.dialog.title = '新增角色';
		state.dialog.submitTxt = '新 增';
	}
	state.dialog.isShowDialog = true;
};

// 关闭弹窗
const closeDialog = () => {
	state.dialog.isShowDialog = false;
	state.originalCode = '';
	formRef.value?.resetFields();
};

// 取消
const onCancel = () => {
	closeDialog();
};

// 提交
const onSubmit = () => {
	formRef.value.validate(async (valid: boolean) => {
		if (!valid) return;
		state.loading = true;
		try {
			const { createRole, updateRole } = useIdentityApi();
			if (state.dialog.type === 'edit' && state.ruleForm.id) {
				await updateRole(state.ruleForm.id, {
					code: state.ruleForm.code || undefined,
					name: state.ruleForm.name,
					isDefault: state.ruleForm.isDefault,
					isPublic: state.ruleForm.isPublic,
					concurrencyStamp: state.ruleForm.concurrencyStamp,
				});
				ElMessage.success('修改成功');
			} else {
				await createRole({
					code: state.ruleForm.code || undefined,
					name: state.ruleForm.name,
					isDefault: state.ruleForm.isDefault,
					isPublic: state.ruleForm.isPublic,
				});
				ElMessage.success('新增成功');
			}
			closeDialog();
			emit('refresh');
		} finally {
			state.loading = false;
		}
	});
};

// 暴露变量
defineExpose({
	openDialog,
});
</script>

<style scoped lang="scss">
.role-edit-dialog-container {
	.dialog-intro {
		margin-bottom: 16px;
		padding: 12px 14px;
		border-radius: 12px;
		background: var(--el-fill-color-light);
		color: var(--el-text-color-secondary);
		line-height: 1.7;
	}

	.role-form {
		:deep(.el-form-item__content) {
			flex-wrap: wrap;
		}
	}

	.form-tip {
		font-size: 12px;
		color: #909399;
		margin-top: 6px;
		line-height: 1.5;
	}
}
</style>
