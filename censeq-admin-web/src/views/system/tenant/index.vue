<template>
	<div class="system-tenant-container layout-padding">
		<div class="system-tenant-padding layout-padding-auto layout-padding-view">
			<div class="system-tenant-search mb15">
				<el-input v-model="state.tableData.param.search" size="default" placeholder="租户名称或编码过滤" style="max-width: 200px" clearable @keyup.enter="onQuery" />
				<el-button size="default" type="primary" class="ml10" @click="onQuery">
					<el-icon>
						<ele-Search />
					</el-icon>
					查询
				</el-button>
				<el-button size="default" type="success" class="ml10" @click="onOpenAddTenant('add')">
					<el-icon>
						<ele-FolderAdd />
					</el-icon>
					新增租户
				</el-button>
			</div>
			<el-table :data="state.tableData.data" v-loading="state.tableData.loading" style="width: 100%">
				<el-table-column type="index" label="序号" width="60" />
				<el-table-column prop="name" label="租户名称" min-width="160" show-overflow-tooltip />
				<el-table-column prop="code" label="租户编码" min-width="120" show-overflow-tooltip />
				<el-table-column prop="id" label="租户 Id" min-width="280" show-overflow-tooltip />
				<el-table-column label="操作" width="140" fixed="right">
					<template #default="scope">
						<el-button size="small" text type="primary" @click="onOpenEditTenant('edit', scope.row)">修改</el-button>
						<el-button size="small" text type="primary" @click="onRowDel(scope.row)">删除</el-button>
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
		<TenantDialog ref="tenantDialogRef" @refresh="getTableData()" />
	</div>
</template>

<script setup lang="ts" name="systemTenant">
import { defineAsyncComponent, reactive, onMounted, ref } from 'vue';
import { ElMessageBox, ElMessage } from 'element-plus';
import { useTenantApi } from '/@/api/apis';
import type { TenantDto } from '/@/api/models/tenant';

const TenantDialog = defineAsyncComponent(() => import('/@/views/system/tenant/dialog.vue'));

const tenantDialogRef = ref();

const state = reactive({
	tableData: {
		data: [] as TenantDto[],
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
		const { getTenantPage } = useTenantApi();
		const data = await getTenantPage({
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

const onOpenAddTenant = (type: string) => {
	tenantDialogRef.value.openDialog(type);
};

const onOpenEditTenant = (type: string, row: TenantDto) => {
	tenantDialogRef.value.openDialog(type, row);
};

const onRowDel = (row: TenantDto) => {
	const name = row.name ?? row.id;
	ElMessageBox.confirm(`此操作将永久删除租户「${name}」，是否继续?`, '提示', {
		confirmButtonText: '确认',
		cancelButtonText: '取消',
		type: 'warning',
	})
		.then(async () => {
			const { deleteTenant } = useTenantApi();
			await deleteTenant(row.id!);
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
.system-tenant-container {
	.system-tenant-padding {
		.el-table {
			flex: 1;
		}
	}
}
</style>
