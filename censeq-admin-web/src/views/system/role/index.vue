<template>
	<div class="system-role-container layout-padding">
		<div class="system-role-padding layout-padding-auto">
			<!-- 搜索栏 -->
			<el-card shadow="never" class="search-card">
				<el-form :model="state.tableData.param" :inline="true" class="search-form">
					<el-form-item label="角色名称/编码">
						<el-input
							v-model="state.tableData.param.search"
							placeholder="输入角色名称或编码搜索"
							clearable
							class="search-input"
							@keyup.enter="onQuery"
						>
							<template #prefix><el-icon><ele-Search /></el-icon></template>
						</el-input>
					</el-form-item>
					<el-form-item>
						<el-button type="primary" icon="ele-Search" @click="onQuery">查询</el-button>
						<el-button icon="ele-Refresh" @click="onResetQuery">重置</el-button>
					</el-form-item>
					<el-form-item class="add-btn-item">
						<el-button type="primary" icon="ele-Plus" @click="onOpenAddRole">新增角色</el-button>
					</el-form-item>
				</el-form>
			</el-card>

			<!-- 数据表格 -->
			<el-card shadow="hover" style="margin-top: 5px" class="full-table table-card">
				<el-table
					:data="state.tableData.data"
					v-loading="state.tableData.loading"
					style="width: 100%"
					border
					stripe
					highlight-current-row
					row-key="id"
				>
					<el-table-column type="index" label="#" width="52" align="center" />

					<el-table-column label="角色名称" min-width="180" show-overflow-tooltip>
						<template #default="scope">
							<div class="role-name-cell">
								<el-icon v-if="scope.row.isStatic" class="static-icon" title="静态角色"><ele-Lock /></el-icon>
								<span class="role-name">{{ scope.row.name }}</span>
							</div>
						</template>
					</el-table-column>

					<el-table-column prop="code" label="角色编码" width="160" show-overflow-tooltip>
						<template #default="scope">
							<el-text type="info" size="small">{{ scope.row.code || '—' }}</el-text>
						</template>
					</el-table-column>

					<el-table-column label="属性" width="180" align="center">
						<template #default="scope">
							<div class="badge-group">
								<el-tag v-if="scope.row.isStatic" size="small" type="warning" effect="light">静态</el-tag>
								<el-tag v-if="scope.row.isDefault" size="small" type="success" effect="light">默认</el-tag>
								<el-tag v-if="scope.row.isPublic" size="small" type="primary" effect="light">公共</el-tag>
								<el-text v-if="!scope.row.isStatic && !scope.row.isDefault && !scope.row.isPublic" type="info" size="small">—</el-text>
							</div>
						</template>
					</el-table-column>

					<el-table-column label="操作" width="290" align="center" fixed="right">
						<template #default="scope">
							<el-button size="small" text type="primary" @click="onOpenEditRole(scope.row)">
								<el-icon><ele-Edit /></el-icon>编辑
							</el-button>
							<el-divider direction="vertical" />
							<el-button size="small" text type="primary" @click="onOpenPermission(scope.row)">
								<el-icon><ele-Menu /></el-icon>授权菜单
							</el-button>
							<el-divider direction="vertical" />
							<el-button size="small" text type="primary" @click="onOpenClaims(scope.row)">
								<el-icon><ele-DocumentChecked /></el-icon>声明
							</el-button>
							<el-divider direction="vertical" />
							<el-button
								size="small"
								text
								type="danger"
								:disabled="scope.row.isDefault || scope.row.isStatic"
								@click="onConfirmDel(scope.row)"
							>
								<el-icon><ele-Delete /></el-icon>删除
							</el-button>
						</template>
					</el-table-column>
				</el-table>

				<el-pagination
					@size-change="onHandleSizeChange"
					@current-change="onHandleCurrentChange"
					class="pagination"
					:pager-count="5"
					:page-sizes="[10, 20, 30]"
					v-model:current-page="state.tableData.param.pageIndex"
					background
					v-model:page-size="state.tableData.param.pageSize"
					layout="total, sizes, prev, pager, next, jumper"
					:total="state.tableData.total"
				/>
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
import { ElMessage, ElMessageBox } from 'element-plus';
import { useIdentityApi } from '/@/api/apis';
import { IdentityRoleDto } from '/@/api/models/identity';

const RoleEditDialog = defineAsyncComponent(() => import('./edit.vue'));
const RolePermissionDialog = defineAsyncComponent(() => import('./permission.vue'));
const RoleClaimDialog = defineAsyncComponent(() => import('./claim.vue'));

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

const onOpenAddRole = () => roleEditRef.value.openDialog();
const onOpenEditRole = (row: IdentityRoleDto) => roleEditRef.value.openDialog(row);
const onOpenPermission = (row: IdentityRoleDto) => rolePermissionRef.value.openDialog(row);
const onOpenClaims = (row: IdentityRoleDto) => roleClaimRef.value.openDialog(row);

const onConfirmDel = (row: IdentityRoleDto) => {
	ElMessageBox.confirm(`确定删除角色：【${row.name}】?`, '提示', {
		confirmButtonText: '确定',
		cancelButtonText: '取消',
		type: 'warning',
	})
		.then(async () => {
			const { deleteRole } = useIdentityApi();
			await deleteRole(row.id!);
			ElMessage.success('删除成功');
			await getTableData();
		})
		.catch(() => {});
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
		gap: 12px;
		min-height: 100%;
	}
}

.search-card {
	:deep(.el-card__body) {
		padding-bottom: 0;
	}

	.search-form {
		display: flex;
		flex-wrap: wrap;
		align-items: center;
	}

	.search-input {
		width: 280px;
	}

	.add-btn-item {
		margin-left: auto;
	}
}

.table-card {
	flex: 1;
	display: flex;
	flex-direction: column;

	:deep(.el-card__body) {
		flex: 1;
		display: flex;
		flex-direction: column;
		padding: 16px;
	}

	:deep(.el-table) {
		flex: 1;
	}
}

.role-name-cell {
	display: flex;
	align-items: center;
	gap: 6px;

	.static-icon {
		color: var(--el-color-warning);
		font-size: 14px;
		flex-shrink: 0;
	}

	.role-name {
		font-weight: 500;
	}
}

.badge-group {
	display: flex;
	flex-wrap: wrap;
	gap: 4px;
	justify-content: center;
}

.pagination {
	margin-top: 14px;
	justify-content: flex-end;
}
</style>
