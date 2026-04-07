<template>
	<div class="openiddict-scope-container">
		<el-card shadow="hover">
			<div class="system-user-search mb15">
				<el-form :model="state.searchForm" size="default" inline>
					<el-form-item label="关键词">
						<el-input v-model="state.searchForm.filter" placeholder="请输入名称或显示名称" clearable />
					</el-form-item>
					<el-form-item>
						<el-button size="default" type="primary" @click="handleSearch">
							<el-icon><ele-Search /></el-icon>查询
						</el-button>
						<el-button size="default" @click="resetSearch">
							<el-icon><ele-RefreshLeft /></el-icon>重置
						</el-button>
						<el-button type="success" size="default" @click="handleCreate">
							<el-icon><ele-Plus /></el-icon>新建作用域
						</el-button>
					</el-form-item>
				</el-form>
			</div>

			<el-table :data="state.dataList" v-loading="state.loading" border>
				<el-table-column type="index" label="序号" width="60" align="center" />
				<el-table-column prop="name" label="名称" min-width="120" show-overflow-tooltip />
				<el-table-column prop="displayName" label="显示名称" min-width="150" show-overflow-tooltip>
					<template #default="{ row }">
						{{ row.displayName || '-' }}
					</template>
				</el-table-column>
				<el-table-column prop="description" label="描述" min-width="200" show-overflow-tooltip>
					<template #default="{ row }">
						{{ row.description || '-' }}
					</template>
				</el-table-column>
				<el-table-column prop="resources" label="资源" min-width="150" show-overflow-tooltip>
					<template #default="{ row }">
						{{ formatArray(row.resources) }}
					</template>
				</el-table-column>
				<el-table-column prop="creationTime" label="创建时间" width="160" align="center">
					<template #default="{ row }">
						{{ formatDateTime(row.creationTime) }}
					</template>
				</el-table-column>
				<el-table-column label="操作" width="150" align="center" fixed="right">
					<template #default="{ row }">
						<el-button type="primary" link size="small" @click="handleEdit(row)">编辑</el-button>
						<el-button type="danger" link size="small" @click="handleDelete(row)">删除</el-button>
					</template>
				</el-table-column>
			</el-table>

			<el-pagination
				v-model:current-page="state.searchForm.skipCount"
				v-model:page-size="state.searchForm.maxResultCount"
				:pager-count="5"
				:page-sizes="[10, 20, 30, 50, 100]"
				:total="state.total"
				layout="total, sizes, prev, pager, next, jumper"
				background
				@size-change="handlePageSizeChange"
				@current-change="getList"
			></el-pagination>
		</el-card>

		<!-- 编辑弹窗 -->
		<el-dialog
			v-model="state.dialogVisible"
			:title="state.isEdit ? '编辑作用域' : '新建作用域'"
			width="500px"
			destroy-on-close
		>
			<el-form
				ref="formRef"
				:model="state.form"
				:rules="formRules"
				label-width="100px"
				label-position="right"
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
					<el-tag
						v-for="(tag, index) in state.form.resources"
						:key="index"
						closable
						@close="removeResource(index)"
						class="mr5 mb5"
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
					<el-button v-else size="small" @click="showInput">+ 添加资源</el-button>
				</el-form-item>
			</el-form>
			<template #footer>
				<div class="dialog-footer">
					<el-button @click="state.dialogVisible = false">取消</el-button>
					<el-button type="primary" @click="handleSubmit" :loading="state.submitLoading">确定</el-button>
				</div>
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
.openiddict-scope-container {
	.mr5 {
		margin-right: 5px;
	}

	.mb5 {
		margin-bottom: 5px;
	}
}
</style>