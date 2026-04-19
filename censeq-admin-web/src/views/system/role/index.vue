<template>
	<div class="system-role-container layout-padding">
		<div class="system-role-padding layout-padding-auto">
			<el-card shadow="hover" :body-style="{ paddingBottom: '0' }" class="role-query-card">
				<el-form :model="state.tableData.param" :inline="true">
					<el-form-item label="角色名称/编码">
						<el-input v-model="state.tableData.param.search" placeholder="角色名称或编码" clearable class="role-search" @keyup.enter="onQuery" />
					</el-form-item>
					<el-form-item>
						<el-button-group>
							<el-button type="primary" icon="ele-Search" @click="onQuery"> 查询 </el-button>
							<el-button icon="ele-Refresh" @click="onResetQuery"> 重置 </el-button>
						</el-button-group>
					</el-form-item>
					<el-form-item>
						<el-button type="primary" icon="ele-Plus" @click="onOpenAddRole"> 新增 </el-button>
					</el-form-item>
				</el-form>
			</el-card>

			<el-card class="full-table role-table-card" shadow="hover" style="margin-top: 5px">
				<el-table :data="state.tableData.data" v-loading="state.tableData.loading" style="width: 100%" stripe border highlight-current-row>
				<el-table-column type="index" label="序号" width="60" />
				<el-table-column prop="code" label="角色编码" width="160" show-overflow-tooltip>
					<template #default="scope">
						{{ scope.row.code || '-' }}
					</template>
				</el-table-column>
				<el-table-column prop="name" label="角色名称" min-width="220" show-overflow-tooltip>
					<template #default="scope">
						<div class="role-name-cell">
							<div class="role-name">{{ scope.row.name }}</div>
							<div class="role-id">ID: {{ scope.row.id }}</div>
						</div>
					</template>
				</el-table-column>
				<el-table-column prop="isDefault" label="是否默认" width="100" align="center">
					<template #default="scope">
						<el-tag size="small" :type="scope.row.isDefault ? 'success' : 'info'">
							{{ scope.row.isDefault ? '是' : '否' }}
						</el-tag>
					</template>
				</el-table-column>
				<el-table-column prop="isPublic" label="是否公共" width="100" align="center">
					<template #default="scope">
						<el-tag size="small" :type="scope.row.isPublic ? 'success' : 'info'">
							{{ scope.row.isPublic ? '是' : '否' }}
						</el-tag>
					</template>
				</el-table-column>
				<el-table-column prop="isStatic" label="是否静态" width="100" align="center">
					<template #default="scope">
						<el-tag size="small" :type="scope.row.isStatic ? 'warning' : 'info'">
							{{ scope.row.isStatic ? '是' : '否' }}
						</el-tag>
					</template>
				</el-table-column>
				<el-table-column label="操作" width="380" align="center" fixed="right">
					<template #default="scope">
						<el-button size="small" text type="primary" @click="onOpenEditRole(scope.row)">
							<el-icon><ele-Edit /></el-icon>
							编辑
						</el-button>
						<el-button size="small" text type="primary" @click="onOpenPermission(scope.row)">
							<el-icon><ele-Menu /></el-icon>
							授权菜单
						</el-button>
						<el-button size="small" text type="primary" @click="onOpenClaims(scope.row)">
							<el-icon><ele-DocumentChecked /></el-icon>
							角色声明
						</el-button>
						<el-popconfirm title="确定删除该角色吗？" @confirm="onRowDel(scope.row)">
							<template #reference>
								<el-button size="small" text type="danger" :disabled="scope.row.isDefault || scope.row.isStatic">
									<el-icon><ele-Delete /></el-icon>
									删除
								</el-button>
							</template>
						</el-popconfirm>
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
			</el-card>
		</div>

		<!-- 新增/编辑角色对话框 -->
		<RoleEditDialog ref="roleEditRef" @refresh="getTableData()" />
		<!-- 授权菜单对话框 -->
		<RolePermissionDialog ref="rolePermissionRef" />
		<!-- 角色声明对话框 -->
		<RoleClaimDialog ref="roleClaimRef" />
	</div>
</template>

<script setup lang="ts" name="systemRole">
import { defineAsyncComponent, reactive, onMounted, ref } from 'vue';
import { ElMessage } from 'element-plus';
import { useIdentityApi } from '/@/api/apis';
import { IdentityRoleDto } from '/@/api/models/identity';

// 引入组件
const RoleEditDialog = defineAsyncComponent(() => import('./edit.vue'));
const RolePermissionDialog = defineAsyncComponent(() => import('./permission.vue'));
const RoleClaimDialog = defineAsyncComponent(() => import('./claim.vue'));

// 定义变量内容
const roleEditRef = ref();
const rolePermissionRef = ref();
const roleClaimRef = ref();

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
	try {
		const { getRolePage } = useIdentityApi();
		const data = await getRolePage({
			skipCount: (state.tableData.param.pageIndex - 1) * state.tableData.param.pageSize,
			maxResultCount: state.tableData.param.pageSize,
			filter: state.tableData.param.search || undefined,
		});
		state.tableData.data = data.items ?? [];
		state.tableData.total = data.totalCount ?? 0;
	} finally {
		state.tableData.loading = false;
	}
};

// 打开新增角色弹窗
const onOpenAddRole = () => {
	roleEditRef.value.openDialog();
};

// 打开编辑角色弹窗
const onOpenEditRole = (row: IdentityRoleDto) => {
	roleEditRef.value.openDialog(row);
};

// 打开授权菜单弹窗
const onOpenPermission = (row: IdentityRoleDto) => {
	rolePermissionRef.value.openDialog(row);
};

// 打开角色声明弹窗
const onOpenClaims = (row: IdentityRoleDto) => {
	roleClaimRef.value.openDialog(row);
};

// 删除角色
const onRowDel = async (row: IdentityRoleDto) => {
	try {
		const { deleteRole } = useIdentityApi();
		await deleteRole(row.id!);
		ElMessage.success('删除成功');
		await getTableData();
	} catch (error) {
		// 删除失败
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
		display: flex;
		flex-direction: column;
		gap: 5px;
		min-height: 100%;
	}
}

.role-query-card {
	:deep(.el-card__body) {
		padding-bottom: 0;
	}
}

.role-table-card {
	flex: 1;
	display: flex;
	flex-direction: column;

	:deep(.el-card__body) {
		flex: 1;
		display: flex;
		flex-direction: column;
	}

	:deep(.el-table) {
		flex: 1;
	}
}

.role-search {
	min-width: 260px;
	width: 260px;
}
.role-name-cell {
	display: flex;
	flex-direction: column;
	gap: 4px;

	.role-name {
		font-weight: 600;
	}

	.role-id {
		font-size: 12px;
		color: var(--el-text-color-secondary);
	}
}

@media (max-width: 960px) {
	.role-search {
		width: 100%;
		min-width: 0;
		flex: 1;
	}
}
</style>
