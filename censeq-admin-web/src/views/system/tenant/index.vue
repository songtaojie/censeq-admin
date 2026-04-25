<template>
	<div class="system-tenant-container layout-padding">
		<el-card shadow="hover" :body-style="{ paddingBottom: '0' }">
			<el-form :model="state.tableData.param" ref="queryFormRef" :inline="true">
				<el-form-item label="租户名称">
					<el-input v-model="state.tableData.param.search" placeholder="租户名称或编码" clearable @keyup.enter="onQuery" />
				</el-form-item>
				<el-form-item>
					<el-button-group>
						<el-button type="primary" icon="ele-Search" @click="onQuery">查询</el-button>
						<el-button icon="ele-Refresh" @click="onReset">重置</el-button>
					</el-button-group>
				</el-form-item>
				<el-form-item>
					<el-button type="primary" icon="ele-Plus" @click="onOpenAddTenant('add')">新增</el-button>
				</el-form-item>
			</el-form>
		</el-card>

		<el-card class="full-table" shadow="hover" style="margin-top: 5px">
			<el-table :data="state.tableData.data" v-loading="state.tableData.loading" style="width: 100%" border stripe highlight-current-row>
				<el-table-column type="index" label="序号" width="60" align="center" fixed />
				<el-table-column prop="name" label="租户名称" min-width="140" show-overflow-tooltip>
					<template #default="scope">
						<el-text type="primary" tag="b">{{ scope.row.name }}</el-text>
					</template>
				</el-table-column>
				<el-table-column prop="code" label="租户编码" min-width="120" align="center" show-overflow-tooltip>
					<template #default="scope">
						<el-tag v-if="scope.row.code" size="small" effect="plain">{{ scope.row.code }}</el-tag>
						<el-text v-else type="info" size="small">—</el-text>
					</template>
				</el-table-column>
				<el-table-column label="连接字符串" min-width="120" align="center" show-overflow-tooltip>
					<template #default="scope">
						<el-tag
							:type="scope.row._hasConnection ? 'success' : 'info'"
							size="small"
							effect="light"
						>{{ scope.row._hasConnection ? '已配置' : '默认' }}</el-tag>
					</template>
				</el-table-column>
				<el-table-column prop="id" label="租户 ID" min-width="290" show-overflow-tooltip>
					<template #default="scope">
						<el-text size="small" style="font-family: monospace">{{ scope.row.id }}</el-text>
					</template>
				</el-table-column>
				<el-table-column prop="isActive" label="状态" width="90" align="center">
					<template #default="scope">
						<el-switch
							v-model="scope.row.isActive"
							@change="onToggleStatus(scope.row)"
							inline-prompt
							active-text="启用"
							inactive-text="禁用"
						/>
					</template>
				</el-table-column>
				<el-table-column label="操作" width="240" fixed="right" align="center">
					<template #default="scope">
						<el-button icon="ele-Edit" size="small" text type="primary" @click="onOpenEditTenant('edit', scope.row)">编辑</el-button>
						<el-button icon="ele-Key" size="small" text type="warning" @click="onResetPassword(scope.row)">重置密码</el-button>
						<el-button icon="ele-Delete" size="small" text type="danger" @click="onRowDel(scope.row)">删除</el-button>
					</template>
				</el-table-column>
			</el-table>
			<el-pagination
				@size-change="onHandleSizeChange"
				@current-change="onHandleCurrentChange"
				class="mt15"
				:pager-count="5"
				:page-sizes="[10, 20, 50]"
				v-model:current-page="state.tableData.param.pageIndex"
				background
				size="small"
				v-model:page-size="state.tableData.param.pageSize"
				layout="total, sizes, prev, pager, next, jumper"
				:total="state.tableData.total"
			/>
		</el-card>

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
const queryFormRef = ref();

const state = reactive({
	tableData: {
		data: [] as (TenantDto & { _hasConnection?: boolean })[],
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
		const { getTenantPage, getDefaultConnectionString } = useTenantApi();
		const data = await getTenantPage({
			filter: state.tableData.param.search || undefined,
			skipCount: (state.tableData.param.pageIndex - 1) * state.tableData.param.pageSize,
			maxResultCount: state.tableData.param.pageSize,
		});
		const items = data.items ?? [];
		// 异步并发检查各租户是否已配置连接字符串
		const rows = await Promise.all(
			items.map(async (item) => {
				try {
					const cs = await getDefaultConnectionString(item.id!);
					return { ...item, _hasConnection: Boolean(cs) };
				} catch {
					return { ...item, _hasConnection: false };
				}
			}),
		);
		state.tableData.data = rows;
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

const onReset = () => {
	state.tableData.param.search = '';
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

const onToggleStatus = async (row: TenantDto & { _hasConnection?: boolean }) => {
	try {
		const { updateTenant } = useTenantApi();
		await updateTenant(row.id!, {
			name: row.name!,
			code: row.code,
			isActive: row.isActive,
			concurrencyStamp: row.concurrencyStamp,
		});
		ElMessage.success(row.isActive ? '已启用' : '已禁用');
		await getTableData();
	} catch {
		// 回滚 UI 状态
		row.isActive = !row.isActive;
		ElMessage.error('操作失败');
	}
};

const onResetPassword = (row: TenantDto) => {
	ElMessageBox.prompt(`请输入租户「${row.name}」新的 admin 密码`, '重置管理员密码', {
		confirmButtonText: '确认',
		cancelButtonText: '取消',
		inputType: 'password',
		inputValidator: (val) => (val && val.length >= 6 ? true : '密码长度至少 6 位'),
	})
		.then(async ({ value }) => {
			const { resetAdminPassword } = useTenantApi();
			await resetAdminPassword(row.id!, value);
			ElMessage.success('密码已重置');
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
	.full-table {
		:deep(.el-card__body) {
			padding-bottom: 0;
		}
		.el-table {
			flex: 1;
		}
	}
}
</style>
