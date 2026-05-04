<template>
	<div class="resource-dialog-container">
		<el-dialog
			v-model="visible"
			width="480px"
			destroy-on-close
			draggable
			:close-on-click-modal="false"
			@closed="onClosed"
		>
			<template #header>
				<div style="color: #fff">
					<el-icon size="16" style="margin-right: 4px; display: inline; vertical-align: middle">
						<ele-Edit v-if="isEdit" />
						<ele-Plus v-else />
					</el-icon>
					<span>{{ isEdit ? '编辑资源' : '新增资源' }}</span>
				</div>
			</template>

			<el-form ref="formRef" :model="form" :rules="rules" label-width="90px" size="default">
				<el-form-item label="资源名称" prop="name">
					<el-input v-model="form.name" :disabled="isEdit" placeholder="如 Admin、Identity" clearable />
				</el-form-item>
				<el-form-item label="显示名称" prop="displayName">
					<el-input v-model="form.displayName" placeholder="如 管理后台、身份认证" clearable />
				</el-form-item>
				<el-form-item label="默认语言" prop="defaultCultureName">
					<el-input v-model="form.defaultCultureName" placeholder="如 zh-Hans" clearable />
				</el-form-item>
			</el-form>

			<template #footer>
				<el-button icon="ele-CircleClose" @click="visible = false">取 消</el-button>
				<el-button type="primary" icon="ele-Select" :loading="saving" @click="onSubmit">确 定</el-button>
			</template>
		</el-dialog>
	</div>
</template>

<script setup lang="ts">
import { ref, reactive } from 'vue';
import { ElMessage } from 'element-plus';
import type { FormInstance, FormRules } from 'element-plus';
import { useLocalizationManagementApi } from '/@/api/apis';
import type { LocalizationResourceDto } from '/@/api/models/localization-management';

const emit = defineEmits<{ (e: 'refresh'): void }>();

const localizationApi = useLocalizationManagementApi();

const visible = ref(false);
const saving = ref(false);
const isEdit = ref(false);
const editId = ref('');
const formRef = ref<FormInstance>();

const form = reactive({
	name: '',
	displayName: '',
	defaultCultureName: '',
});

const rules: FormRules = {
	name: [{ required: true, message: '请输入资源名称', trigger: 'blur' }],
};

function open() {
	isEdit.value = false;
	editId.value = '';
	form.name = '';
	form.displayName = '';
	form.defaultCultureName = '';
	visible.value = true;
}

function openEdit(row: LocalizationResourceDto) {
	isEdit.value = true;
	editId.value = row.id;
	form.name = row.name;
	form.displayName = row.displayName ?? '';
	form.defaultCultureName = row.defaultCultureName ?? '';
	visible.value = true;
}

async function onSubmit() {
	if (!(await formRef.value?.validate().catch(() => false))) return;
	saving.value = true;
	try {
		const input = {
			name: form.name,
			displayName: form.displayName || null,
			defaultCultureName: form.defaultCultureName || null,
		};
		if (isEdit.value) {
			await localizationApi.updateResource(editId.value, input);
			ElMessage.success('更新成功');
		} else {
			await localizationApi.createResource(input);
			ElMessage.success('新增成功');
		}
		visible.value = false;
		emit('refresh');
	} finally {
		saving.value = false;
	}
}

function onClosed() {
	formRef.value?.resetFields();
}

defineExpose({ open, openEdit });
</script>
