<template>
	<el-dialog :title="title" v-model="state.visible" width="480px" destroy-on-close @close="onCancel">
		<el-form ref="formRef" :model="state.form" :rules="rules" label-width="100px">
			<el-form-item label="显示名称" prop="displayName">
				<el-input
					v-model="state.form.displayName"
					:maxlength="256"
					show-word-limit
					placeholder="请输入显示名称"
					clearable
				/>
			</el-form-item>
		</el-form>
		<template #footer>
			<el-button @click="onCancel">取 消</el-button>
			<el-button type="primary" @click="onConfirm" :loading="state.loading">确 定</el-button>
		</template>
	</el-dialog>
</template>

<script setup lang="ts" name="EditDisplayNameDialog">
import { reactive, ref, watch } from 'vue';
import type { FormInstance, FormRules } from 'element-plus';

const props = defineProps<{
	visible: boolean;
	title: string;
	currentValue: string;
}>();

const emit = defineEmits<{
	(e: 'confirm', displayName: string): void;
	(e: 'cancel'): void;
	(e: 'update:visible', value: boolean): void;
}>();

const formRef = ref<FormInstance>();

const state = reactive({
	visible: false,
	loading: false,
	form: {
		displayName: '',
	},
});

const rules: FormRules = {
	displayName: [
		{ required: true, message: '显示名称不能为空', trigger: 'blur' },
		{ min: 1, max: 256, message: '显示名称长度为 1-256 个字符', trigger: 'blur' },
		{
			validator: (_rule, value, callback) => {
				if (value && value.trim() === '') {
					callback(new Error('显示名称不能为纯空白字符'));
				} else {
					callback();
				}
			},
			trigger: 'blur',
		},
	],
};

watch(
	() => props.visible,
	(val) => {
		state.visible = val;
		if (val) {
			state.form.displayName = props.currentValue;
		}
	}
);

watch(
	() => state.visible,
	(val) => {
		if (!val) emit('update:visible', false);
	}
);

const onConfirm = async () => {
	if (!formRef.value) return;
	await formRef.value.validate((valid) => {
		if (valid) {
			emit('confirm', state.form.displayName.trim());
		}
	});
};

const onCancel = () => {
	state.visible = false;
	emit('cancel');
};
</script>
