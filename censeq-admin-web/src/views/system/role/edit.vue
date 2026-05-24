<template>
	<div class="role-edit-dialog-container">
		<el-dialog v-model="state.dialog.isShowDialog" width="560px" destroy-on-close draggable :close-on-click-modal="false">
			<template #header>
				<div style="color: #fff">
					<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"><ele-Edit /></el-icon>
					<span>{{ state.dialog.title }}</span>
				</div>
			</template>
			<div class="dialog-intro">维护角色名称、状态及其基础属性。默认角色会自动分配给新用户，公共角色可供其他用户查看和选择。</div>
			<el-form ref="formRef" :model="state.ruleForm" :rules="rules" size="default" label-width="96px" class="role-form">
				<el-form-item prop="code">
					<template #label>
						<span class="form-label-with-help">
							角色编码
							<el-tooltip content="角色编码唯一；编码一旦设置，后续不允许修改。" placement="top">
								<el-icon class="label-help-icon"><ele-QuestionFilled /></el-icon>
							</el-tooltip>
						</span>
					</template>
					<el-input
						v-model="state.ruleForm.code"
						placeholder="请输入角色编码"
						clearable
						maxlength="64"
						show-word-limit
						:disabled="state.dialog.type === 'edit' && !!state.originalCode"
					/>
				</el-form-item>
				<el-form-item prop="name">
					<template #label>
						<span class="form-label-with-help">
							角色名称
							<el-tooltip v-if="state.dialog.type === 'edit' && state.ruleForm.isStatic" content="静态角色名称不允许修改。" placement="top">
								<el-icon class="label-help-icon"><ele-QuestionFilled /></el-icon>
							</el-tooltip>
						</span>
					</template>
					<el-input
						v-model="state.ruleForm.name"
						placeholder="请输入角色名称"
						clearable
						maxlength="50"
						show-word-limit
						:disabled="state.dialog.type === 'edit' && state.ruleForm.isStatic"
					/>
				</el-form-item>
				<el-row :gutter="12">
					<el-col :span="12">
						<el-form-item>
							<template #label>
								<span class="form-label-with-help">
									默认角色
									<el-tooltip content="默认角色会自动分配给新用户" placement="top">
										<el-icon class="label-help-icon"><ele-QuestionFilled /></el-icon>
									</el-tooltip>
								</span>
							</template>
							<el-switch v-model="state.ruleForm.isDefault" inline-prompt active-text="是" inactive-text="否" />
						</el-form-item>
					</el-col>
					<el-col :span="12">
						<el-form-item>
							<template #label>
								<span class="form-label-with-help">
									公共角色
									<el-tooltip content="公共角色可以被其他用户查看" placement="top">
										<el-icon class="label-help-icon"><ele-QuestionFilled /></el-icon>
									</el-tooltip>
								</span>
							</template>
							<el-switch v-model="state.ruleForm.isPublic" inline-prompt active-text="是" inactive-text="否" />
						</el-form-item>
					</el-col>
				</el-row>
				<el-form-item label="状态">
					<el-radio-group v-model="state.ruleForm.status">
						<el-radio :value="CommonStatus.Enabled">启用</el-radio>
						<el-radio :value="CommonStatus.Disabled">禁用</el-radio>
					</el-radio-group>
				</el-form-item>
				<el-form-item label="备注">
					<el-input
						v-model="state.ruleForm.remark"
						type="textarea"
						:rows="3"
						maxlength="512"
						show-word-limit
						placeholder="请输入备注内容"
					/>
				</el-form-item>
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
import { CommonStatus, IdentityRoleDto } from '/@/api/models/identity';
import { useIdentityApi } from '/@/api/apis';

const emit = defineEmits(['refresh']);

const formRef = ref();

const state = reactive({
	ruleForm: {
		code: '',
		isDefault: false,
		status: CommonStatus.Enabled,
		isPublic: false,
		remark: '',
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
	code: [
		{ required: true, message: '请输入角色编码', trigger: 'blur' },
		{ max: 64, message: '长度不能超过 64 个字符', trigger: 'blur' },
	],
	name: [
		{ required: true, message: '请输入角色名称', trigger: 'blur' },
		{ min: 2, max: 50, message: '长度在 2 到 50 个字符', trigger: 'blur' },
	],
};

const openDialog = (row?: IdentityRoleDto) => {
	state.dialog.type = row ? 'edit' : 'add';
	if (row) {
		state.ruleForm = { ...row };
		state.originalCode = row.code || '';
		state.dialog.title = '修改角色';
		state.dialog.submitTxt = '修 改';
	} else {
		state.ruleForm = { isDefault: false, status: CommonStatus.Enabled, isPublic: false, name: '', code: '', remark: '' } as IdentityRoleDto;
		state.originalCode = '';
		state.dialog.title = '新增角色';
		state.dialog.submitTxt = '新 增';
	}
	state.dialog.isShowDialog = true;
};

const closeDialog = () => {
	state.dialog.isShowDialog = false;
	state.originalCode = '';
	formRef.value?.resetFields();
};

const onCancel = () => {
	closeDialog();
};

const onSubmit = () => {
	formRef.value.validate(async (valid: boolean) => {
		if (!valid) return;
		state.loading = true;
		try {
			const { createRole, updateRole } = useIdentityApi();
			if (state.dialog.type === 'edit' && state.ruleForm.id) {
				await updateRole(state.ruleForm.id, {
					code: state.ruleForm.code.trim(),
					name: state.ruleForm.name,
					isDefault: state.ruleForm.isDefault,
					status: state.ruleForm.status,
					isPublic: state.ruleForm.isPublic,
					remark: state.ruleForm.remark || undefined,
					concurrencyStamp: state.ruleForm.concurrencyStamp,
				});
				ElMessage.success('修改成功');
			} else {
				await createRole({
					code: state.ruleForm.code.trim(),
					name: state.ruleForm.name,
					isDefault: state.ruleForm.isDefault,
					status: state.ruleForm.status,
					isPublic: state.ruleForm.isPublic,
					remark: state.ruleForm.remark || undefined,
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

	.form-label-with-help {
		display: inline-flex;
		align-items: center;
		justify-content: flex-end;
		gap: 6px;
		white-space: nowrap;
	}

	.label-help-icon {
		font-size: 14px;
		color: var(--el-color-info);
		cursor: help;
	}
}
</style>
