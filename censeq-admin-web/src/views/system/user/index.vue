<template>
	<div class="system-user-container layout-padding">
		<div class="system-user-padding layout-padding-auto layout-padding-view">
			<div class="system-user-search mb15">
				<el-input v-model="state.tableData.param.search" size="default" placeholder="用户名 / 邮箱 / 姓名" style="max-width: 220px" clearable @keyup.enter="onQuery" />
				<el-button size="default" type="primary" class="ml10" @click="onQuery">
					<el-icon>
						<ele-Search />
					</el-icon>
					查询
				</el-button>
				<el-button size="default" type="success" class="ml10" @click="onOpenAdd('add')">
					<el-icon>
						<ele-FolderAdd />
					</el-icon>
					新增用户
				</el-button>
			</div>
			<el-table :data="state.tableData.data" v-loading="state.tableData.loading" style="width: 100%">
				<el-table-column type="index" label="序号" width="58" />
				<el-table-column prop="userName" label="用户名" min-width="120" show-overflow-tooltip />
				<el-table-column label="姓名" min-width="120" show-overflow-tooltip>
					<template #default="{ row }">
						{{ [row.name, row.surname].filter(Boolean).join(' ') || '—' }}
					</template>
				</el-table-column>
				<el-table-column prop="email" label="邮箱" min-width="180" show-overflow-tooltip />
				<el-table-column prop="phoneNumber" label="手机" width="120" show-overflow-tooltip />
				<el-table-column prop="isActive" label="启用" width="80">
					<template #default="{ row }">
						<el-tag :type="row.isActive ? 'success' : 'info'" size="small">{{ row.isActive ? '是' : '否' }}</el-tag>
					</template>
				</el-table-column>
				<el-table-column prop="lockoutEnabled" label="锁定策略" width="90">
					<template #default="{ row }">
						<el-tag :type="row.lockoutEnabled ? 'warning' : 'info'" size="small">{{ row.lockoutEnabled ? '开' : '关' }}</el-tag>
					</template>
				</el-table-column>
				<el-table-column prop="creationTime" label="创建时间" min-width="160" show-overflow-tooltip />
				<el-table-column label="操作" width="140" fixed="right">
					<template #default="{ row }">
						<el-button size="small" text type="primary" @click="onOpenEdit('edit', row)">修改</el-button>
						<el-button size="small" text type="primary" @click="onRowDel(row)">删除</el-button>
					</template>
				</el-table-column>
			</el-table>
			<el-pagination
				@size-change="onHandleSizeChange"
				@current-change="onHandleCurrentChange"
				class="mt15"
				:pager-count="5"
				:page-sizes="[10, 20, 30]"
				v-model:current-page="state.tableData.param.pageIndex"
				background
				v-model:page-size="state.tableData.param.pageSize"
				layout="total, sizes, prev, pager, next, jumper"
				:total="state.tableData.total"
			/>
		</div>
		<UserDialog ref="userDialogRef" @refresh="getTableData()" />
	</div>
</template>

<script setup lang="ts" name="systemUser">
import { defineAsyncComponent, reactive, onMounted, ref } from 'vue';
import { ElMessageBox, ElMessage } from 'element-plus';
import { useIdentityApi } from '/@/api/apis';
import type { IdentityUserDto } from '/@/api/models/identity';

const UserDialog = defineAsyncComponent(() => import('/@/views/system/user/dialog.vue'));

const userDialogRef = ref();
const state = reactive({
	tableData: {
		data: [] as IdentityUserDto[],
		total: 0,
		loading: false,
		param: {
			search: '',
			pageIndex: 1,
			pageSize: 10,
		},
	},
});

const getTableData = async () => {
	state.tableData.loading = true;
	try {
		const { getUserPage } = useIdentityApi();
		const data = await getUserPage({
			filter: state.tableData.param.search || undefined,
			skipCount: (state.tableData.param.pageIndex - 1) * state.tableData.param.pageSize,
			maxResultCount: state.tableData.param.pageSize,
		});
		state.tableData.data = data.items ?? [];
		state.tableData.total = data.totalCount ?? 0;
	} catch {
		state.tableData.data = [];
		state.tableData.total = 0;
	} finally {
		state.tableData.loading = false;
	}
};

const onQuery = () => {
	state.tableData.param.pageIndex = 1;
	getTableData();
};

const onResetQuery = () => {
	state.tableData.param.search = '';
	state.tableData.param.pageIndex = 1;
	getTableData();
};

const onOpenAdd = (type: string) => {
	userDialogRef.value.openDialog(type);
};

const onOpenEdit = (type: string, row: IdentityUserDto) => {
	userDialogRef.value.openDialog(type, row);
};

const onRowDel = (row: IdentityUserDto) => {
	ElMessageBox.confirm(`此操作将永久删除用户「${row.userName}」，是否继续?`, '提示', {
		confirmButtonText: '确认',
		cancelButtonText: '取消',
		type: 'warning',
	})
		.then(async () => {
			const { deleteUser } = useIdentityApi();
			await deleteUser(row.id!);
			ElMessage.success('删除成功');
			await getTableData();
		})
		.catch(() => {});
};

const onHandleSizeChange = (val: number) => {
	state.tableData.param.pageSize = val;
	getTableData();
};

const onHandleCurrentChange = (val: number) => {
	state.tableData.param.pageIndex = val;
	getTableData();
};

onMounted(() => {
	getTableData();
});
</script>

<style scoped lang="scss">
.system-user-container {
	.system-user-padding {
		display: flex;
		flex-direction: column;
		gap: 0;
	}
}
</style>
