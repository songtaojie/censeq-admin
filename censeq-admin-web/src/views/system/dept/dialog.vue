<template>
	<div class="system-dept-dialog-container">
		<el-dialog :title="state.dialog.title" v-model="state.dialog.isShowDialog" width="520px" destroy-on-close @closed="onClosed">
			<el-form ref="formRef" :model="state.ruleForm" :rules="formRules" label-width="100px" size="default">
				<el-form-item label="显示名称" prop="displayName">
					<el-input v-model="state.ruleForm.displayName" maxlength="128" show-word-limit clearable />
				</el-form-item>
				<el-form-item label="上级部门">
					<el-select v-model="state.ruleForm.parentId" placeholder="空表示根节点" clearable filterable class="w100" :disabled="state.dialog.type === 'edit'">
						<el-option v-for="opt in state.parentOptions" :key="opt.id" :label="parentLabel(opt)" :value="opt.id!" />
					</el-select>
				</el-form-item>
				<el-alert
					v-if="state.dialog.type === 'edit'"
					type="info"
					show-icon
					:closable="false"
					class="mb10"
					title="上级部门在 ABP 中由编码链路维护，此处不提供变更父级；可删除后重建或使用后续「移动」接口扩展。"
				/>
			</el-form>
			<template #footer>
				<el-button size="default" @click="onCancel">取 消</el-button>
				<el-button type="primary" size="default" :loading="state.submitting" @click="onSubmit">{{ state.dialog.submitTxt }}</el-button>
			</template>
		</el-dialog>
	</div>
</template>

<script setup lang="ts" name="systemDeptDialog">
import { reactive, ref, nextTick } from 'vue';
import type { FormInstance, FormRules } from 'element-plus';
import { ElMessage } from 'element-plus';
import { useIdentityApi } from '/@/api/apis';
import type { OrganizationUnitDto } from '/@/api/models/identity';

const emit = defineEmits(['refresh']);

const formRef = ref<FormInstance>();

const state = reactive({
	ruleForm: {
		id: '' as string,
		displayName: '',
		parentId: null as string | null,
	},
	parentOptions: [] as OrganizationUnitDto[],
	dialog: {
		isShowDialog: false,
		type: '' as 'add' | 'edit' | '',
		title: '',
		submitTxt: '',
	},
	submitting: false,
});

const formRules: FormRules = {
	displayName: [
		{ required: true, message: '请输入显示名称', trigger: 'blur' },
		{ min: 1, max: 128, message: '长度 1~128', trigger: 'blur' },
	],
};

function parentLabel(ou: OrganizationUnitDto): string {
	const c = ou.code ? ` [${ou.code}]` : '';
	return `${ou.displayName ?? ''}${c}`;
}

const openDialog = async (type: string, row: OrganizationUnitDto | null, parentId: string | null) => {
	state.dialog.type = type as 'add' | 'edit';
	state.ruleForm = { id: '', displayName: '', parentId: parentId ?? null };
	state.dialog.isShowDialog = true;
	if (type === 'edit' && row?.id) {
		state.dialog.title = '编辑组织机构';
		state.dialog.submitTxt = '保 存';
		state.ruleForm.id = row.id;
		state.ruleForm.displayName = row.displayName ?? '';
		state.ruleForm.parentId = row.parentId ?? null;
	} else {
		state.dialog.title = parentId ? '新增子级' : '新增根组织机构';
		state.dialog.submitTxt = '新 增';
	}
	const { getOrganizationUnitAllList } = useIdentityApi();
	const res = await getOrganizationUnitAllList();
	const all = res.items ?? [];
	if (type === 'edit' && state.ruleForm.id) {
		state.parentOptions = all.filter((x) => x.id !== state.ruleForm.id);
	} else {
		state.parentOptions = all;
	}
	await nextTick();
	formRef.value?.clearValidate();
};

const onClosed = () => {
	state.dialog.type = '';
};

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
			if (state.dialog.type === 'add') {
				await api.createOrganizationUnit({
					displayName: state.ruleForm.displayName.trim(),
					parentId: state.ruleForm.parentId || undefined,
				});
				ElMessage.success('已创建');
			} else if (state.dialog.type === 'edit' && state.ruleForm.id) {
				await api.updateOrganizationUnit(state.ruleForm.id, {
					displayName: state.ruleForm.displayName.trim(),
				});
				ElMessage.success('已保存');
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
