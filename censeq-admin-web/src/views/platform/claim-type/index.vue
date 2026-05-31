<template>
	<div class="claim-type-container layout-padding">
		<el-card shadow="hover" :body-style="{ paddingBottom: '0' }">
			<el-form ref="queryFormRef" :model="state.tableData.param" :inline="true">
				<el-form-item label="声明类型名称">
					<el-input v-model="state.tableData.param.search" placeholder="请输入声明类型名称" clearable style="width: 220px" @keyup.enter="onQuery" />
				</el-form-item>
				<el-form-item>
					<el-button-group>
						<el-button type="primary" icon="ele-Search" @click="onQuery">查询</el-button>
						<el-button icon="ele-Refresh" @click="onReset">重置</el-button>
					</el-button-group>
				</el-form-item>
				<el-form-item>
					<el-button type="primary" icon="ele-Plus" @click="onOpenAdd">新增</el-button>
				</el-form-item>
			</el-form>
		</el-card>

		<el-card shadow="hover" style="margin-top: 5px">
			<el-alert type="info" :closable="false" class="mb15" title="这里维护系统允许使用的声明类型。值类型为下拉选项时，角色声明会自动读取这里配置的选项。" />
			<el-table :data="state.tableData.data" v-loading="state.tableData.loading" border stripe style="width: 100%">
				<el-table-column type="index" label="序号" width="60" align="center" />
				<el-table-column prop="name" label="名称" min-width="160" show-overflow-tooltip>
					<template #default="{ row }">
						<span style="font-weight: 600; color: var(--el-color-primary)">{{ row.name }}</span>
					</template>
				</el-table-column>
				<el-table-column prop="valueType" label="值类型" width="120" align="center">
					<template #default="{ row }">
						<el-tag size="small" type="info" effect="light">{{ formatValueType(row.valueType) }}</el-tag>
					</template>
				</el-table-column>
				<el-table-column label="选项" min-width="220" show-overflow-tooltip>
					<template #default="{ row }">
						<span v-if="row.options?.length">{{ row.options.filter((x: any) => x.isEnabled).map((x: any) => `${x.label}(${x.value})`).join('、') }}</span>
						<span v-else>—</span>
					</template>
				</el-table-column>
				<el-table-column prop="required" label="必填" width="80" align="center">
					<template #default="{ row }">
						<el-tag size="small" :type="row.required ? 'success' : 'info'" effect="light">{{ row.required ? '是' : '否' }}</el-tag>
					</template>
				</el-table-column>
				<el-table-column prop="isStatic" label="静态" width="80" align="center">
					<template #default="{ row }">
						<el-tag size="small" :type="row.isStatic ? 'warning' : 'info'" effect="light">{{ row.isStatic ? '是' : '否' }}</el-tag>
					</template>
				</el-table-column>
				<el-table-column prop="regex" label="正则" min-width="160" show-overflow-tooltip>
					<template #default="{ row }">
						<span style="font-family: monospace; font-size: 12px">{{ row.regex || '—' }}</span>
					</template>
				</el-table-column>
				<el-table-column prop="description" label="描述" min-width="180" show-overflow-tooltip>
					<template #default="{ row }">{{ row.description || '—' }}</template>
				</el-table-column>
				<el-table-column label="操作" width="140" align="center" fixed="right">
					<template #default="{ row }">
						<el-button icon="ele-Edit" size="small" text type="primary" :disabled="row.isStatic" @click="onOpenEdit(row)">编辑</el-button>
						<el-button icon="ele-Delete" size="small" text type="danger" :disabled="row.isStatic" @click="onDelete(row)">删除</el-button>
					</template>
				</el-table-column>
			</el-table>

			<el-pagination
				v-model:current-page="state.tableData.param.pageIndex"
				v-model:page-size="state.tableData.param.pageSize"
				:pager-count="5"
				:page-sizes="[10, 20, 50]"
				:total="state.tableData.total"
				layout="total, sizes, prev, pager, next, jumper"
				background
				size="small"
				class="pagination"
				@size-change="onPageSizeChange"
				@current-change="getTableData"
			/>
		</el-card>

		<el-dialog v-model="state.dialogVisible" width="680px" destroy-on-close draggable :close-on-click-modal="false">
			<template #header>
				<div style="color: #fff">
					<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle">
						<ele-Edit v-if="state.isEdit" />
						<ele-Plus v-else />
					</el-icon>
					<span>{{ state.isEdit ? '编辑声明类型' : '新增声明类型' }}</span>
				</div>
			</template>
			<el-form ref="formRef" :model="state.form" :rules="formRules" label-width="110px" size="default">
				<el-form-item label="名称" prop="name">
					<el-input v-model="state.form.name" placeholder="请输入声明类型名称" :disabled="state.isEdit && state.form.isStatic" clearable />
				</el-form-item>
				<el-form-item label="值类型" prop="valueType">
					<el-select v-model="state.form.valueType" placeholder="请选择值类型" style="width: 100%" @change="onValueTypeChange">
						<el-option label="字符串" value="String" />
						<el-option label="整数" value="Int" />
						<el-option label="布尔" value="Boolean" />
						<el-option label="日期时间" value="DateTime" />
						<el-option label="下拉选项" value="Option" />
					</el-select>
				</el-form-item>
				<el-form-item label="必填">
					<el-switch v-model="state.form.required" inline-prompt active-text="是" inactive-text="否" />
				</el-form-item>
				<el-form-item label="静态" v-if="!state.isEdit">
					<el-switch v-model="state.form.isStatic" inline-prompt active-text="是" inactive-text="否" />
				</el-form-item>
				<el-form-item label="正则表达式">
					<el-input v-model="state.form.regex" placeholder="请输入正则表达式" clearable />
				</el-form-item>
				<el-form-item label="正则说明">
					<el-input v-model="state.form.regexDescription" placeholder="请输入正则说明" clearable />
				</el-form-item>
				<el-form-item label="描述">
					<el-input v-model="state.form.description" type="textarea" :rows="3" placeholder="请输入描述" />
				</el-form-item>

				<el-form-item v-if="state.form.valueType === 'Option'" label="下拉选项" required>
					<div class="option-editor">
						<div v-for="(option, index) in state.form.options" :key="index" class="option-row">
							<el-input v-model="option.label" placeholder="显示名称" />
							<el-input v-model="option.value" placeholder="选项值" />
							<el-input-number v-model="option.sort" :min="0" :controls="false" placeholder="排序" />
							<el-switch v-model="option.isEnabled" inline-prompt active-text="启用" inactive-text="禁用" />
							<el-button icon="ele-Delete" text type="danger" @click="removeOption(index)" />
						</div>
						<el-button icon="ele-Plus" @click="addOption">添加选项</el-button>
					</div>
				</el-form-item>
			</el-form>
			<template #footer>
				<el-button icon="ele-CircleClose" @click="state.dialogVisible = false">取消</el-button>
				<el-button type="primary" icon="ele-Select" :loading="state.submitLoading" @click="onSubmit">
					{{ state.isEdit ? '保存' : '新增' }}
				</el-button>
			</template>
		</el-dialog>
	</div>
</template>

<script setup lang="ts" name="claimTypeManage">
import { reactive, ref, onMounted } from 'vue';
import { ElMessage, ElMessageBox, type FormInstance, type FormRules } from 'element-plus';
import { useIdentityClaimTypeApi } from '/@/api/apis';
import type {
	IdentityClaimTypeDto,
	IdentityClaimTypeCreateDto,
	IdentityClaimTypeOptionCreateOrUpdateDto,
	IdentityClaimTypeUpdateDto,
} from '/@/api/models/identity';

const { getList, get, create, update, delete: deleteClaimType } = useIdentityClaimTypeApi();

const formRef = ref<FormInstance>();
const queryFormRef = ref();

const emptyForm = () => ({
	id: '',
	name: '',
	required: false,
	isStatic: false,
	regex: '',
	regexDescription: '',
	description: '',
	valueType: 'String' as IdentityClaimTypeDto['valueType'],
	options: [] as IdentityClaimTypeOptionCreateOrUpdateDto[],
});

const state = reactive({
	submitLoading: false,
	dialogVisible: false,
	isEdit: false,
	tableData: {
		data: [] as IdentityClaimTypeDto[],
		loading: false,
		total: 0,
		param: {
			search: '',
			pageIndex: 1,
			pageSize: 10,
		},
	},
	form: emptyForm(),
});

const formRules = reactive<FormRules>({
	name: [{ required: true, message: '请输入声明类型名称', trigger: 'blur' }],
	valueType: [{ required: true, message: '请选择值类型', trigger: 'change' }],
});

const getTableData = async () => {
	state.tableData.loading = true;
	try {
		const res = await getList({
			filter: state.tableData.param.search || undefined,
			sorting: 'Name',
			skipCount: (state.tableData.param.pageIndex - 1) * state.tableData.param.pageSize,
			maxResultCount: state.tableData.param.pageSize,
		});
		state.tableData.data = res.items ?? [];
		state.tableData.total = res.totalCount ?? 0;
	} finally {
		state.tableData.loading = false;
	}
};

const onQuery = () => {
	state.tableData.param.pageIndex = 1;
	getTableData();
};

const onReset = () => {
	state.tableData.param.search = '';
	state.tableData.param.pageIndex = 1;
	getTableData();
};

const onPageSizeChange = () => {
	state.tableData.param.pageIndex = 1;
	getTableData();
};

const onOpenAdd = () => {
	state.isEdit = false;
	state.form = emptyForm();
	state.dialogVisible = true;
};

const onOpenEdit = async (row: IdentityClaimTypeDto) => {
	state.isEdit = true;
	const detail = await get(row.id);
	state.form = {
		id: detail.id,
		name: detail.name,
		required: detail.required,
		isStatic: detail.isStatic,
		regex: detail.regex || '',
		regexDescription: detail.regexDescription || '',
		description: detail.description || '',
		valueType: detail.valueType,
		options: (detail.options || []).map((item) => ({
			label: item.label,
			value: item.value,
			sort: item.sort,
			isEnabled: item.isEnabled,
		})),
	};
	state.dialogVisible = true;
};

const onDelete = async (row: IdentityClaimTypeDto) => {
	try {
		await ElMessageBox.confirm(`确定删除声明类型 "${row.name}" 吗？`, '提示', {
			confirmButtonText: '确定',
			cancelButtonText: '取消',
			type: 'warning',
		});
		await deleteClaimType(row.id);
		ElMessage.success('删除成功');
		await getTableData();
	} catch (error: any) {
		if (error !== 'cancel') {
			ElMessage.error('删除失败');
		}
	}
};

const onValueTypeChange = () => {
	if (state.form.valueType === 'Option' && !state.form.options.length) {
		addOption();
	}
};

const addOption = () => {
	state.form.options.push({
		label: '',
		value: '',
		sort: state.form.options.length + 1,
		isEnabled: true,
	});
};

const removeOption = (index: number) => {
	state.form.options.splice(index, 1);
};

const buildPayload = () => ({
	name: state.form.name,
	required: state.form.required,
	regex: state.form.regex || undefined,
	regexDescription: state.form.regexDescription || undefined,
	description: state.form.description || undefined,
	valueType: state.form.valueType,
	options: state.form.valueType === 'Option' ? state.form.options : [],
});

const validateOptions = () => {
	if (state.form.valueType !== 'Option') return true;
	const options = state.form.options.filter((item) => item.label.trim() && item.value.trim());
	if (!options.length) {
		ElMessage.warning('请至少维护一个下拉选项');
		return false;
	}
	if (new Set(options.map((item) => item.value)).size !== options.length) {
		ElMessage.warning('下拉选项值不能重复');
		return false;
	}
	state.form.options = options;
	return true;
};

const onSubmit = async () => {
	if (!formRef.value) return;
	await formRef.value.validate(async (valid) => {
		if (!valid || !validateOptions()) return;
		state.submitLoading = true;
		try {
			if (state.isEdit) {
				const payload: IdentityClaimTypeUpdateDto = buildPayload();
				await update(state.form.id, payload);
				ElMessage.success('更新成功');
			} else {
				const payload: IdentityClaimTypeCreateDto = {
					...buildPayload(),
					isStatic: state.form.isStatic,
				};
				await create(payload);
				ElMessage.success('创建成功');
			}
			state.dialogVisible = false;
			await getTableData();
		} catch (error) {
			ElMessage.error('保存失败');
		} finally {
			state.submitLoading = false;
		}
	});
};

const formatValueType = (valueType: string) => {
	const displayMap: Record<string, string> = {
		String: '字符串',
		Int: '整数',
		Boolean: '布尔',
		DateTime: '日期时间',
		Option: '下拉选项',
	};
	return displayMap[valueType] || valueType;
};

onMounted(() => {
	getTableData();
});
</script>

<style scoped lang="scss">
.claim-type-container {
	display: flex;
	flex-direction: column;
	gap: 0;

	.option-editor {
		display: flex;
		flex: 1;
		flex-direction: column;
		gap: 10px;
	}

	.option-row {
		display: grid;
		grid-template-columns: minmax(120px, 1fr) minmax(120px, 1fr) 80px 82px 36px;
		gap: 8px;
		align-items: center;
		width: 100%;
	}
}
</style>
