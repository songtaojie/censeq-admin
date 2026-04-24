<template>
	<div class="scope-container layout-padding">
		<el-card shadow="hover" :body-style="{ paddingBottom: '0' }">
			<el-form ref="queryFormRef" :model="state.searchForm" :inline="true">
				<el-form-item label="关键词">
					<el-input v-model="state.searchForm.filter" placeholder="请输入名称或显示名称" clearable style="width: 220px" />
				</el-form-item>
				<el-form-item>
					<el-button-group>
						<el-button type="primary" @click="handleSearch">
							<el-icon><ele-Search /></el-icon>查询
						</el-button>
						<el-button @click="resetSearch">
							<el-icon><ele-RefreshLeft /></el-icon>重置
						</el-button>
					</el-button-group>
				</el-form-item>
				<el-form-item>
					<el-button type="primary" @click="handleCreate">
						<el-icon><ele-Plus /></el-icon>新建作用域
					</el-button>
				</el-form-item>
			</el-form>
		</el-card>

		<el-card shadow="hover" style="margin-top: 8px">
			<el-table :data="state.dataList" v-loading="state.loading" border stripe>
				<el-table-column type="index" label="序号" width="60" align="center" />
				<el-table-column prop="name" label="名称" min-width="140" show-overflow-tooltip>
					<template #default="{ row }">
						<span class="scope-name-text">{{ row.name }}</span>
					</template>
				</el-table-column>
				<el-table-column prop="displayName" label="显示名称" min-width="140" show-overflow-tooltip>
					<template #default="{ row }">
						{{ row.displayName || '-' }}
					</template>
				</el-table-column>
				<el-table-column prop="description" label="描述" min-width="200" show-overflow-tooltip>
					<template #default="{ row }">
						{{ row.description || '-' }}
					</template>
				</el-table-column>
				<el-table-column prop="resources" label="资源" min-width="160" show-overflow-tooltip>
					<template #default="{ row }">
						<template v-if="row.resources && row.resources.length">
							<el-tag
								v-for="r in row.resources"
								:key="r"
								size="small"
								type="info"
								effect="light"
								style="margin-right: 4px"
							>{{ r }}</el-tag>
						</template>
						<span v-else>-</span>
					</template>
				</el-table-column>
				<el-table-column prop="creationTime" label="创建时间" width="160" align="center">
					<template #default="{ row }">
						{{ formatDateTime(row.creationTime) }}
					</template>
				</el-table-column>
				<el-table-column label="操作" width="150" align="center" fixed="right">
					<template #default="{ row }">
						<el-button icon="ele-Edit" size="small" text type="primary" @click="handleEdit(row)">编辑</el-button>
						<el-button icon="ele-Delete" size="small" text type="danger" @click="handleDelete(row)">删除</el-button>
					</template>
				</el-table-column>
			</el-table>
			<el-pagination
				class="pagination"
				v-model:current-page="state.searchForm.skipCount"
				v-model:page-size="state.searchForm.maxResultCount"
				:pager-count="5"
				:page-sizes="[10, 20, 50, 100]"
				:total="state.total"
				layout="total, sizes, prev, pager, next, jumper"
				background
				size="small"
				@size-change="handlePageSizeChange"
				@current-change="getList"
			/>
		</el-card>

		<!-- 编辑弹窗 -->
		<el-dialog
			v-model="state.dialogVisible"
			width="500px"
			destroy-on-close
			draggable
			:close-on-click-modal="false"
		>
			<template #header>
				<div class="dialog-header">
					<el-icon v-if="state.isEdit" color="#409EFF" size="18"><ele-Edit /></el-icon>
					<el-icon v-else color="#67C23A" size="18"><ele-Plus /></el-icon>
					<span>{{ state.isEdit ? '编辑作用域' : '新建作用域' }}</span>
				</div>
			</template>
			<el-form
				ref="formRef"
				:model="state.form"
				:rules="formRules"
				label-width="100px"
				label-position="right"
				class="dialog-form"
			>
				<el-form-item label="名称" prop="name">
					<el-input v-model="state.form.name" :disabled="state.isEdit" placeholder="请输入作用域名称" />
				</el-form-item>
				<el-form-item label="显示名称" prop="displayName">
					<el-input v-model="state.form.displayName" placeholder="请输入显示名称" />
				</el-form-item>
				<el-form-item label="描述" prop="description">
					<el-input v-model="state.form.description" type="textarea" :rows="3" placeholder="请输入描述" />
				</el-form-item>
				<el-form-item label="资源" prop="resources">
					<div class="tag-input-area">
						<el-tag
							v-for="(tag, index) in state.form.resources"
							:key="index"
							closable
							type="info"
							effect="light"
							@close="removeResource(index)"
						>
							{{ tag }}
						</el-tag>
						<el-input
							v-if="state.inputVisible"
							ref="inputRef"
							v-model="state.inputValue"
							size="small"
							@keyup.enter="handleInputConfirm"
							@blur="handleInputConfirm"
							style="width: 150px"
						/>
						<el-button v-else size="small" plain @click="showInput">
							<el-icon><ele-Plus /></el-icon>添加资源
						</el-button>
					</div>
				</el-form-item>
			</el-form>
			<template #footer>
				<el-button icon="ele-CircleClose" @click="state.dialogVisible = false">取消</el-button>
				<el-button type="primary" icon="ele-Select" @click="handleSubmit" :loading="state.submitLoading">保存</el-button>
			</template>
		</el-dialog>
	</div>
</template>

<script setup lang="ts" name="openiddictScope">
import { reactive, ref, nextTick } from 'vue';
import { ElMessage, ElMessageBox } from 'element-plus';
import type { FormInstance, FormRules } from 'element-plus';
import { useOpenIddictScopeApi, type OpenIddictScopeDto } from '/@/api/apis/openiddict';
const { getList: getScopeList, create, update, delete: deleteScope } = useOpenIddictScopeApi();

const formRef = ref<FormInstance>();
const queryFormRef = ref();
const inputRef = ref<HTMLInputElement>();

const state = reactive({
	loading: false,
	submitLoading: false,
	total: 0,
	dialogVisible: false,
	isEdit: false,
	dataList: [] as OpenIddictScopeDto[],
	searchForm: {
		filter: '',
		skipCount: 1,
		maxResultCount: 20,
	},
	form: {
		id: '',
		name: '',
		displayName: '',
		description: '',
		resources: [] as string[],
	},
	inputVisible: false,
	inputValue: '',
});

const formRules = reactive<FormRules>({
	name: [{ required: true, message: '请输入作用域名称', trigger: 'blur' }],
});

// 获取列表
const getList = async () => {
	state.loading = true;
	try {
		const res = await getScopeList({
			...state.searchForm,
			skipCount: (state.searchForm.skipCount - 1) * state.searchForm.maxResultCount,
		});
		state.dataList = res.items || [];
		state.total = res.totalCount || 0;
	} finally {
		state.loading = false;
	}
};

// 搜索
const handleSearch = () => {
	state.searchForm.skipCount = 1;
	getList();
};

// 重置搜索
const resetSearch = () => {
	state.searchForm.filter = '';
	state.searchForm.skipCount = 1;
	getList();
};

const handlePageSizeChange = () => {
	state.searchForm.skipCount = 1;
	getList();
};

// 格式化数组
const formatArray = (arr?: string[]) => {
	if (!arr || arr.length === 0) return '-';
	return arr.join(', ');
};

// 格式化日期
const formatDateTime = (dateStr?: string) => {
	if (!dateStr) return '-';
	const date = new Date(dateStr);
	return date.toLocaleString('zh-CN');
};

// 新建
const handleCreate = () => {
	state.isEdit = false;
	state.form = {
		id: '',
		name: '',
		displayName: '',
		description: '',
		resources: [],
	};
	state.dialogVisible = true;
};

// 编辑
const handleEdit = (row: OpenIddictScopeDto) => {
	state.isEdit = true;
	state.form = {
		id: row.id,
		name: row.name,
		displayName: row.displayName || '',
		description: row.description || '',
		resources: [...row.resources],
	};
	state.dialogVisible = true;
};

// 删除
const handleDelete = async (row: OpenIddictScopeDto) => {
	try {
		await ElMessageBox.confirm(`确定要删除作用域 "${row.name}" 吗？`, '提示', {
			confirmButtonText: '确定',
			cancelButtonText: '取消',
			type: 'warning',
		});
		await deleteScope(row.id);
		ElMessage.success('删除成功');
		getList();
	} catch (error: any) {
		if (error !== 'cancel') {
			console.error('删除失败', error);
			ElMessage.error('删除失败');
		}
	}
};

// 提交
const handleSubmit = async () => {
	if (!formRef.value) return;
	await formRef.value.validate(async (valid) => {
		if (!valid) return;
		state.submitLoading = true;
		try {
			if (state.isEdit) {
				await update(state.form.id, {
					displayName: state.form.displayName,
					description: state.form.description,
					resources: state.form.resources,
				});
				ElMessage.success('更新成功');
			} else {
				await create({
					name: state.form.name,
					displayName: state.form.displayName,
					description: state.form.description,
					resources: state.form.resources,
				});
				ElMessage.success('创建成功');
			}
			state.dialogVisible = false;
			getList();
		} catch (error) {
			console.error('提交失败', error);
			ElMessage.error('提交失败');
		} finally {
			state.submitLoading = false;
		}
	});
};

// Tag输入相关方法
const removeResource = (index: number) => {
	state.form.resources.splice(index, 1);
};

const showInput = () => {
	state.inputVisible = true;
	state.inputValue = '';
	nextTick(() => {
		inputRef.value?.focus();
	});
};

const handleInputConfirm = () => {
	const value = state.inputValue?.trim();
	if (value && !state.form.resources.includes(value)) {
		state.form.resources.push(value);
	}
	state.inputVisible = false;
	state.inputValue = '';
};

// 初始化
getList();
</script>

<style scoped lang="scss">
.scope-container {
	display: flex;
	flex-direction: column;
}

.pagination {
	margin-top: 14px;
	justify-content: flex-end;
}

.scope-name-text {
	color: var(--el-color-primary);
	font-weight: 500;
}

.dialog-header {
	display: flex;
	align-items: center;
	gap: 8px;
	font-size: 16px;
	font-weight: 600;
}

.dialog-form {
	:deep(.el-form-item) {
		margin-bottom: 20px !important;
	}
}

.tag-input-area {
	display: flex;
	flex-wrap: wrap;
	gap: 6px;
	align-items: center;
}
</style>