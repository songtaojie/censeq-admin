<template>
	<div class="culture-dialog-container">
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
					<span>{{ isEdit ? '编辑语言' : '新增语言' }}</span>
				</div>
			</template>

			<el-form ref="formRef" :model="form" :rules="rules" label-width="100px" size="default">
				<el-form-item label="语言代码" prop="cultureName">
					<el-input v-model="form.cultureName" :disabled="isEdit" placeholder="如 zh-Hans、en" clearable />
				</el-form-item>
				<el-form-item label="显示名称" prop="displayName">
					<el-input v-model="form.displayName" placeholder="如 简体中文、English" clearable />
				</el-form-item>
				<el-form-item label="UI 语言代码" prop="uiCultureName">
					<el-input v-model="form.uiCultureName" placeholder="留空则与语言代码相同" clearable />
				</el-form-item>
				<el-form-item label="状态" prop="isEnabled">
					<el-switch v-model="form.isEnabled" active-text="启用" inactive-text="禁用" />
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
import type { LocalizationCultureDto } from '/@/api/models/localization-management';

const emit = defineEmits<{ (e: 'refresh'): void }>();

const localizationApi = useLocalizationManagementApi();

const visible = ref(false);
const saving = ref(false);
const isEdit = ref(false);
const editId = ref('');
const formRef = ref<FormInstance>();

const form = reactive({
	cultureName: '',
	displayName: '',
	uiCultureName: '',
	isEnabled: true,
});

const rules: FormRules = {
	cultureName: [{ required: true, message: '请输入语言代码', trigger: 'blur' }],
	displayName: [{ required: true, message: '请输入显示名称', trigger: 'blur' }],
};

function open() {
	isEdit.value = false;
	editId.value = '';
	form.cultureName = '';
	form.displayName = '';
	form.uiCultureName = '';
	form.isEnabled = true;
	visible.value = true;
}

function openEdit(row: LocalizationCultureDto) {
	isEdit.value = true;
	editId.value = row.id;
	form.cultureName = row.cultureName;
	form.displayName = row.displayName;
	form.uiCultureName = row.uiCultureName ?? '';
	form.isEnabled = row.isEnabled;
	visible.value = true;
}

async function onSubmit() {
	if (!(await formRef.value?.validate().catch(() => false))) return;
	saving.value = true;
	try {
		const input = {
			cultureName: form.cultureName,
			displayName: form.displayName,
			uiCultureName: form.uiCultureName || null,
			isEnabled: form.isEnabled,
		};
		if (isEdit.value) {
			await localizationApi.updateCulture(editId.value, input);
			ElMessage.success('更新成功');
		} else {
			await localizationApi.createCulture(input);
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
