<template>
	<div class="role-edit-dialog-container">
		<el-dialog :title="state.dialog.title" v-model="state.dialog.isShowDialog" width="500px" destroy-on-close>
			<el-form ref="formRef" :model="state.ruleForm" :rules="rules" size="default" label-width="90px">
				<el-form-item label="角色名称" prop="name">
					<el-input v-model="state.ruleForm.name" placeholder="请输入角色名称" clearable></el-input>
				</el-form-item>
				<el-form-item label="是否默认">
					<el-radio-group v-model="state.ruleForm.isDefault">
						<el-radio :label="true">是</el-radio>
						<el-radio :label="false">否</el-radio>
					</el-radio-group>
					<div class="form-tip">默认角色会自动分配给新用户</div>
				</el-form-item>
				<el-form-item label="是否公共">
					<el-radio-group v-model="state.ruleForm.isPublic">
						<el-radio :label="true">是</el-radio>
						<el-radio :label="false">否</el-radio>
					</el-radio-group>
					<div class="form-tip">公共角色可以被其他用户查看</div>
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
import { IdentityRoleDto } from '/@/api/models/identity';
import { useIdentityApi } from '/@/api/apis';

// 定义子组件向父组件传值/事件
const emit = defineEmits(['refresh']);

const formRef = ref();

const state = reactive({
	ruleForm: {
		isDefault: false,
		isPublic: false,
	} as IdentityRoleDto,
	dialog: {
		isShowDialog: false,
		type: '' as 'add' | 'edit' | '',
		title: '',
		submitTxt: '',
	},
	loading: false,
});

const rules: FormRules = {
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
		state.dialog.title = '修改角色';
		state.dialog.submitTxt = '修 改';
	} else {
		state.ruleForm = { isDefault: false, isPublic: false, name: '' } as IdentityRoleDto;
		state.dialog.title = '新增角色';
		state.dialog.submitTxt = '新 增';
	}
	state.dialog.isShowDialog = true;
};

// 关闭弹窗
const closeDialog = () => {
	state.dialog.isShowDialog = false;
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
					name: state.ruleForm.name,
					isDefault: state.ruleForm.isDefault,
					isPublic: state.ruleForm.isPublic,
					concurrencyStamp: state.ruleForm.concurrencyStamp,
				});
				ElMessage.success('修改成功');
			} else {
				await createRole({
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
	.form-tip {
		font-size: 12px;
		color: #909399;
		margin-top: 5px;
	}
}
</style>
