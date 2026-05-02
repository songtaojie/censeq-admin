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
			<el-alert type="info" :closable="false" class="mb15" title="这里维护系统允许使用的角色声明类型，角色声明页会从这里读取可选项。" />
			<el-table :data="state.tableData.data" v-loading="state.loading" border stripe style="width: 100%">
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

		<!-- 新增/编辑对话框 -->
		<el-dialog
			v-model="state.dialogVisible"
			width="560px"
			destroy-on-close
			draggable
			:close-on-click-modal="false"
		>
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
					<el-select v-model="state.form.valueType" placeholder="请选择值类型" style="width: 100%">
						<el-option label="字符串" value="String" />
						<el-option label="整数" value="Int" />
						<el-option label="布尔" value="Boolean" />
						<el-option label="日期时间" value="DateTime" />
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
			</el-form>
			<template #footer>
				<el-button icon="ele-CircleClose" @click="state.dialogVisible = false">取 消</el-button>
				<el-button type="primary" icon="ele-Select" :loading="state.submitLoading" @click="onSubmit">
					{{ state.isEdit ? '保 存' : '新 增' }}
				</el-button>
			</template>
		</el-dialog>
	</div>
</template>

<script setup lang="ts" name="claimTypeManage">
import { reactive, ref, onMounted } from 'vue';
import { ElMessage, ElMessageBox, type FormInstance, type FormRules } from 'element-plus';
import { useIdentityClaimTypeApi } from '/@/api/apis';
import type { IdentityClaimTypeDto, IdentityClaimTypeCreateDto, IdentityClaimTypeUpdateDto } from '/@/api/models/identity';

const { getList, get, create, update, delete: deleteClaimType } = useIdentityClaimTypeApi();

const formRef = ref<FormInstance>();
const queryFormRef = ref();

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
	form: {
		id: '',
		name: '',
		required: false,
		isStatic: false,
		regex: '',
		regexDescription: '',
		description: '',
		valueType: 'String' as IdentityClaimTypeDto['valueType'],
	},
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
	state.form = {
		id: '',
		name: '',
		required: false,
		isStatic: false,
		regex: '',
		regexDescription: '',
		description: '',
		valueType: 'String',
	};
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

const onSubmit = async () => {
	if (!formRef.value) return;
	await formRef.value.validate(async (valid) => {
		if (!valid) return;
		state.submitLoading = true;
		try {
			if (state.isEdit) {
				const payload: IdentityClaimTypeUpdateDto = {
					name: state.form.name,
					required: state.form.required,
					regex: state.form.regex || undefined,
					regexDescription: state.form.regexDescription || undefined,
					description: state.form.description || undefined,
					valueType: state.form.valueType,
				};
				await update(state.form.id, payload);
				ElMessage.success('更新成功');
			} else {
				const payload: IdentityClaimTypeCreateDto = {
					name: state.form.name,
					required: state.form.required,
					isStatic: state.form.isStatic,
					regex: state.form.regex || undefined,
					regexDescription: state.form.regexDescription || undefined,
					description: state.form.description || undefined,
					valueType: state.form.valueType,
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
}

.pagination {
	margin-top: 14px;
	justify-content: flex-end;
}
</style>