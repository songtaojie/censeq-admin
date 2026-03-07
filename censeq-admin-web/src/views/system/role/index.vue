<template>
	<div class="system-role-container layout-padding">
		<div class="system-role-padding layout-padding-auto layout-padding-view">
			<div class="system-user-search mb15">
				<el-input v-model="state.tableData.param.search" size="default" placeholder="请输入角色名称" style="max-width: 180px"> </el-input>
				<el-button size="default" type="primary" class="ml10" @click="onQuery">
					<el-icon>
						<ele-Search />
					</el-icon>
					查询
				</el-button>
				<el-button size="default" type="success" class="ml10" @click="onOpenAddRole('add')">
					<el-icon>
						<ele-FolderAdd />
					</el-icon>
					新增角色
				</el-button>
			</div>
			<el-table :data="state.tableData.data" v-loading="state.tableData.loading" style="width: 100%">
				<el-table-column type="index" label="序号" width="60" />
				<el-table-column prop="name" label="角色名称" show-overflow-tooltip></el-table-column>
				<!-- <el-table-column prop="sort" label="排序" show-overflow-tooltip></el-table-column> -->
				 <el-table-column prop="isDefault" label="是否默认" show-overflow-tooltip>
					<template #default="scope">
						<el-switch v-model="scope.row.isDefault" disabled size="default" />
					</template>
				</el-table-column>
				<el-table-column prop="isPublic" label="是否公共" show-overflow-tooltip>
					<template #default="scope">
						<el-switch v-model="scope.row.isPublic" disabled size="default" />
					</template>
				</el-table-column>
				<el-table-column prop="isStatic" label="是否静态" show-overflow-tooltip>
					<template #default="scope">
						<el-switch v-model="scope.row.isStatic" disabled  size="default" />
					</template>
				</el-table-column>
				<!-- <el-table-column prop="describe" label="角色描述" show-overflow-tooltip></el-table-column> -->
				<!-- <el-table-column prop="createTime" label="创建时间" show-overflow-tooltip></el-table-column> -->
				<el-table-column label="操作" width="100">
					<template #default="scope">
						<el-button :disabled="scope.row.isDefault" size="small" text type="primary" @click="onOpenEditRole('edit', scope.row)"
							>修改</el-button
						>
						<el-button :disabled="scope.row.isDefault" size="small" text type="primary" @click="onRowDel(scope.row)">删除</el-button>
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
			>
			</el-pagination>
		</div>
		<RoleDialog ref="roleDialogRef" @refresh="getTableData()" />
	</div>
</template>

<script setup lang="ts" name="systemRole">
import { defineAsyncComponent, reactive, onMounted, ref } from 'vue';
import { ElMessageBox, ElMessage } from 'element-plus';
import { useIdentityApi } from '/@/api/apis';
import { IdentityRoleDto } from '/@/api/models/identity';
// 引入组件
const RoleDialog = defineAsyncComponent(() => import('/@/views/system/role/dialog.vue'));

// 定义变量内容
const roleDialogRef = ref();
const state = reactive({
	tableData: {
		data: [] as Array<IdentityRoleDto>,
		total: 0,
		loading: false,
		param: {
			search: '',
			pageIndex: 1,
			pageSize: 10,
		},
	},
});
// 初始化表格数据
const getTableData = async () => {
	state.tableData.loading = true;
	const { getRolePage } = useIdentityApi();
    var data = await getRolePage({
		skipCount:(state.tableData.param.pageIndex - 1) * state.tableData.param.pageSize,
		maxResultCount: state.tableData.param.pageSize,
		filter:state.tableData.param.search
	});
	state.tableData.data = data.items ?? [];
	state.tableData.total = data.totalCount  ?? 0;
	setTimeout(() => {
		state.tableData.loading = false;
	}, 500);
};
// 打开新增角色弹窗
const onOpenAddRole = (type: string) => {
	roleDialogRef.value.openDialog(type);
};
// 打开修改角色弹窗
const onOpenEditRole = (type: string, row: Object) => {
	roleDialogRef.value.openDialog(type, row);
};
// 删除角色
const onRowDel = (row: RowRoleType) => {
	ElMessageBox.confirm(`此操作将永久删除角色名称：“${row.roleName}”，是否继续?`, '提示', {
		confirmButtonText: '确认',
		cancelButtonText: '取消',
		type: 'warning',
	})
		.then(() => {
			getTableData();
			ElMessage.success('删除成功');
		})
		.catch(() => {});
};
const onQuery = () => {
	getTableData();
};
// 分页改变
const onHandleSizeChange = (val: number) => {
	state.tableData.param.pageSize = val;
	getTableData();
};
// 分页改变
const onHandleCurrentChange = (val: number) => {
	state.tableData.param.pageIndex = val;
	getTableData();
};
// 页面加载时
onMounted(() => {
	getTableData();
});
</script>

<style scoped lang="scss">
.system-role-container {
	.system-role-padding {
		padding: 15px;
		.el-table {
			flex: 1;
		}
	}
}
</style>
