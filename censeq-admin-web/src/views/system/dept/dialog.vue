<template>
	<div class="system-dept-dialog-container">
		<el-dialog v-model="state.dialog.isShowDialog" width="560px" destroy-on-close draggable :close-on-click-modal="false" @closed="onClosed">
			<template #header>
				<div style="color: #fff">
					<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"><ele-OfficeBuilding /></el-icon>
					<span>{{ state.dialog.title }}</span>
				</div>
			</template>
			<el-form ref="formRef" :model="state.ruleForm" :rules="formRules" label-width="80px" size="default">
				<el-form-item label="上级机构">
					<el-select v-model="state.ruleForm.parentId" placeholder="请选择上级机构" clearable filterable class="w100" :disabled="state.dialog.type === 'edit'">
						<el-option v-for="opt in state.parentOptions" :key="opt.id" :label="parentLabel(opt)" :value="opt.id!" />
					</el-select>
				</el-form-item>
				<el-form-item label="机构名称" prop="displayName">
					<el-input v-model="state.ruleForm.displayName" maxlength="128" show-word-limit clearable placeholder="机构名称" />
				</el-form-item>
				<el-form-item label="机构编码">
					<el-input v-model="state.ruleForm.code" readonly placeholder="保存后自动生成" :prefix-icon="Lock" style="color: #999" />
				</el-form-item>
				<el-form-item label="状　　态">
					<el-radio-group v-model="state.ruleForm.status">
						<el-radio :value="1">
							<el-icon style="color: var(--el-color-success); vertical-align: middle"><ele-CircleCheck /></el-icon>
							启用
						</el-radio>
						<el-radio :value="0">
							<el-icon style="color: var(--el-color-info); vertical-align: middle"><ele-CircleClose /></el-icon>
							禁用
						</el-radio>
					</el-radio-group>
				</el-form-item>
				<el-form-item label="备　　注">
					<el-input v-model="state.ruleForm.remark" type="textarea" :rows="3" maxlength="512" show-word-limit placeholder="请输入备注内容" />
				</el-form-item>
				<el-alert
					v-if="state.dialog.type === 'edit'"
					type="info"
					show-icon
					:closable="false"
					style="margin-bottom: 10px"
					title="上级机构由编码链路维护，编辑时不支持变更父级。"
				/>
			</el-form>
			<template #footer>
				<el-button size="default" icon="ele-CircleClose" @click="onCancel">取 消</el-button>
				<el-button type="primary" size="default" icon="ele-CircleCheck" :loading="state.submitting" @click="onSubmit">{{ state.dialog.submitTxt }}</el-button>
			</template>
		</el-dialog>
	</div>
</template>

<script setup lang="ts" name="systemDeptDialog">
import { reactive, ref, nextTick } from 'vue';
import type { FormInstance, FormRules } from 'element-plus';
import { ElMessage } from 'element-plus';
import { Lock } from '@element-plus/icons-vue';
import { useIdentityApi } from '/@/api/apis';
import type { OrganizationUnitDto } from '/@/api/models/identity';

const emit = defineEmits(['refresh']);

const formRef = ref<FormInstance>();

const state = reactive({
	ruleForm: {
		id: '' as string,
		displayName: '',
		parentId: null as string | null,
		code: '',
		status: 1,
		remark: '',
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
		{ required: true, message: '请输入机构名称', trigger: 'blur' },
		{ min: 1, max: 128, message: '长度 1~128 字符', trigger: 'blur' },
	],
};

function parentLabel(ou: OrganizationUnitDto): string {
	const c = ou.code ? ` [${ou.code}]` : '';
	return `${ou.displayName ?? ''}${c}`;
}

const openDialog = async (type: string, row: OrganizationUnitDto | null, parentId: string | null) => {
	state.dialog.type = type as 'add' | 'edit';
	state.ruleForm = { id: '', displayName: '', parentId: parentId ?? null, code: '', status: 1, remark: '' };
	state.dialog.isShowDialog = true;
	if (type === 'edit' && row?.id) {
		state.dialog.title = '编辑机构';
		state.dialog.submitTxt = '保 存';
		state.ruleForm.id = row.id;
		state.ruleForm.displayName = row.displayName ?? '';
		state.ruleForm.parentId = row.parentId ?? null;
		state.ruleForm.code = row.code ?? '';
		state.ruleForm.status = row.status ?? 1;
		state.ruleForm.remark = row.remark ?? '';
	} else {
		state.dialog.title = parentId ? '新增子机构' : '新增机构';
		state.dialog.submitTxt = '确 定';
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
					status: state.ruleForm.status,
					remark: state.ruleForm.remark || undefined,
				});
				ElMessage.success('新增成功');
			} else if (state.dialog.type === 'edit' && state.ruleForm.id) {
				await api.updateOrganizationUnit(state.ruleForm.id, {
					displayName: state.ruleForm.displayName.trim(),
					status: state.ruleForm.status,
					remark: state.ruleForm.remark || undefined,
				});
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
