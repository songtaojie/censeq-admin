<template>
	<div class="localization-edit-dialog-container">
		<el-dialog
			v-model="visible"
			width="520px"
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
					<span>{{ isEdit ? '编辑翻译' : '新增翻译' }}</span>
				</div>
			</template>

			<el-form ref="formRef" :model="form" :rules="rules" label-width="90px" size="default">
				<el-form-item label="资源" prop="resourceName">
					<el-input v-model="form.resourceName" :disabled="isEdit" placeholder="如 Admin" clearable />
				</el-form-item>
				<el-form-item label="语言" prop="cultureName">
					<el-input v-model="form.cultureName" :disabled="isEdit" placeholder="如 zh-Hans" clearable />
				</el-form-item>
				<el-form-item label="Key" prop="key">
					<el-input v-model="form.key" :disabled="isEdit" placeholder="翻译键" clearable />
				</el-form-item>
				<el-form-item label="翻译值" prop="value">
					<el-input
						v-model="form.value"
						type="textarea"
						:rows="4"
						placeholder="留空则回退到 JSON 文件默认值"
						clearable
					/>
				</el-form-item>
				<div v-if="isEdit" class="key-hint">
					<el-icon><ele-InfoFilled /></el-icon>
					<span>资源、语言和 Key 不可修改，仅可更新翻译值</span>
				</div>
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
import type { LocalizationTextDto } from '/@/api/models/localization-management';

const emit = defineEmits<{ (e: 'refresh'): void }>();

const localizationApi = useLocalizationManagementApi();

const visible = ref(false);
const saving = ref(false);
const isEdit = ref(false);
const editId = ref('');
const formRef = ref<FormInstance>();

const form = reactive({
	resourceName: '',
	cultureName: '',
	key: '',
	value: '',
});

const rules: FormRules = {
	resourceName: [{ required: true, message: '请输入资源名称', trigger: 'blur' }],
	cultureName: [{ required: true, message: '请输入语言', trigger: 'blur' }],
	key: [{ required: true, message: '请输入翻译键', trigger: 'blur' }],
};

function open(resourceName: string, cultureName: string) {
	isEdit.value = false;
	editId.value = '';
	form.resourceName = resourceName;
	form.cultureName = cultureName;
	form.key = '';
	form.value = '';
	visible.value = true;
}

function openEdit(row: LocalizationTextDto) {
	isEdit.value = true;
	editId.value = row.id;
	form.resourceName = row.resourceName;
	form.cultureName = row.cultureName;
	form.key = row.key;
	form.value = row.value ?? '';
	visible.value = true;
}

async function onSubmit() {
	if (!(await formRef.value?.validate().catch(() => false))) return;
	saving.value = true;
	try {
		if (isEdit.value) {
			await localizationApi.updateText(editId.value, { value: form.value || null });
			ElMessage.success('更新成功');
		} else {
			await localizationApi.createText({
				resourceName: form.resourceName,
				cultureName: form.cultureName,
				key: form.key,
				value: form.value || null,
			});
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

<style scoped lang="scss">
.key-hint {
	display: flex;
	align-items: center;
	gap: 6px;
	color: var(--el-text-color-secondary);
	font-size: 12px;
	margin-top: -8px;
	padding-left: 90px;

	.el-icon {
		color: var(--el-color-info);
	}
}
</style>
