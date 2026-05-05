<template>
	<div class="system-audit-log-container layout-padding">
		<el-card shadow="hover" :body-style="{ paddingBottom: '0' }">
			<el-form ref="queryFormRef" :model="state.queryParam" :inline="true">
				<el-form-item label="时间范围">
					<el-date-picker
						v-model="state.searchDateRange"
						type="datetimerange"
						range-separator="至"
						start-placeholder="开始时间"
						end-placeholder="结束时间"
						value-format="YYYY-MM-DD HH:mm:ss"
						style="width: 360px"
					/>
				</el-form-item>
				<el-form-item label="用户名">
					<el-input v-model="state.queryParam.userName" placeholder="用户名" clearable style="width: 140px" @keyup.enter="onQuery" />
				</el-form-item>
				<el-form-item label="URL">
					<el-input v-model="state.queryParam.url" placeholder="URL" clearable style="width: 200px" @keyup.enter="onQuery" />
				</el-form-item>
				<el-form-item label="HTTP方法">
					<el-select v-model="state.queryParam.httpMethod" placeholder="全部" clearable style="width: 110px">
						<el-option label="GET" value="GET" />
						<el-option label="POST" value="POST" />
						<el-option label="PUT" value="PUT" />
						<el-option label="DELETE" value="DELETE" />
					</el-select>
				</el-form-item>
				<el-form-item label="状态">
					<el-select v-model="state.queryParam.hasException" placeholder="全部" clearable style="width: 100px">
						<el-option label="正常" :value="false" />
						<el-option label="异常" :value="true" />
					</el-select>
				</el-form-item>
				<el-form-item>
					<el-button-group>
						<el-button type="primary" icon="ele-Search" @click="onQuery">查询</el-button>
						<el-button icon="ele-Refresh" @click="onReset">重置</el-button>
					</el-button-group>
				</el-form-item>
			</el-form>
		</el-card>

		<el-card class="full-table" shadow="hover" style="margin-top: 5px">
			<el-table :data="state.tableData.data" v-loading="state.tableData.loading" style="width: 100%" border stripe>
				<el-table-column type="index" label="序号" width="60" align="center" fixed />
				<el-table-column label="执行时间" width="160" show-overflow-tooltip>
					<template #default="{ row }">
						{{ formatDateTime(row.executionTime) }}
					</template>
				</el-table-column>
				<el-table-column prop="userName" label="用户名" width="120" show-overflow-tooltip>
					<template #default="{ row }">
						<span style="font-weight: 600; color: var(--el-color-primary)">{{ row.userName || '—' }}</span>
					</template>
				</el-table-column>
				<el-table-column prop="httpMethod" label="HTTP方法" width="100" align="center">
					<template #default="{ row }">
						<el-tag size="small" :type="getHttpMethodType(row.httpMethod)" effect="light">
							{{ row.httpMethod }}
						</el-tag>
					</template>
				</el-table-column>
				<el-table-column prop="url" label="URL" min-width="220" show-overflow-tooltip />
				<el-table-column prop="clientIpAddress" label="IP地址" width="130" show-overflow-tooltip />
				<el-table-column prop="executionDuration" label="执行时长" width="100" align="center">
					<template #default="{ row }">
						<el-tag size="small" :type="row.executionDuration > 1000 ? 'warning' : 'success'" effect="light">
							{{ row.executionDuration }}ms
						</el-tag>
					</template>
				</el-table-column>
				<el-table-column label="状态" width="80" align="center">
					<template #default="{ row }">
						<el-tag size="small" :type="row.hasException ? 'danger' : 'success'" effect="light">
							{{ row.hasException ? '异常' : '正常' }}
						</el-tag>
					</template>
				</el-table-column>
				<el-table-column label="操作" width="140" fixed="right" align="center">
					<template #default="{ row }">
						<el-button icon="ele-View" size="small" text type="primary" @click="onViewDetail(row)">详情</el-button>
						<el-popconfirm title="确定删除该操作日志吗？" @confirm="onDelete(row)">
							<template #reference>
								<el-button icon="ele-Delete" size="small" text type="danger">删除</el-button>
							</template>
						</el-popconfirm>
					</template>
				</el-table-column>
			</el-table>
			<el-pagination
				@size-change="onHandleSizeChange"
				@current-change="onHandleCurrentChange"
				class="pagination"
				:pager-count="5"
				:page-sizes="[10, 20, 50, 100]"
				v-model:current-page="state.tableData.param.pageIndex"
				background
				size="small"
				v-model:page-size="state.tableData.param.pageSize"
				layout="total, sizes, prev, pager, next, jumper"
				:total="state.tableData.total"
			/>
		</el-card>

		<AuditLogDetailDialog ref="detailDialogRef" />
	</div>
</template>

<script setup lang="ts" name="systemAuditLog">
import { defineAsyncComponent, reactive, onMounted, ref } from 'vue';
import { ElMessage } from 'element-plus';
import { useAuditLogApi } from '/@/api/apis';
import type { AuditLogDto } from '/@/api/models/audit-logging';

const AuditLogDetailDialog = defineAsyncComponent(() => import('./detail.vue'));

const detailDialogRef = ref();

const state = reactive({
	searchDateRange: [] as string[],
	queryParam: {
		userName: '',
		url: '',
		httpMethod: '',
		hasException: undefined as boolean | undefined,
	},
	tableData: {
		data: [] as AuditLogDto[],
		total: 0,
		loading: false,
		param: {
			pageIndex: 1,
			pageSize: 20,
		},
	},
});

const formatDateTime = (dateStr: string) => {
	if (!dateStr) return '—';
	return dateStr.replace('T', ' ').substring(0, 19);
};

const getHttpMethodType = (method?: string): '' | 'success' | 'warning' | 'danger' | 'primary' | 'info' => {
	switch (method) {
		case 'GET': return 'success';
		case 'POST': return 'primary';
		case 'PUT': return 'warning';
		case 'DELETE': return 'danger';
		default: return 'info';
	}
};

const getTableData = async () => {
	state.tableData.loading = true;
	try {
		const { getAuditLogPage } = useAuditLogApi();
		const data = await getAuditLogPage({
			skipCount: (state.tableData.param.pageIndex - 1) * state.tableData.param.pageSize,
			maxResultCount: state.tableData.param.pageSize,
			startTime: state.searchDateRange?.[0],
			endTime: state.searchDateRange?.[1],
			userName: state.queryParam.userName || undefined,
			url: state.queryParam.url || undefined,
			httpMethod: state.queryParam.httpMethod || undefined,
			hasException: state.queryParam.hasException,
		});
		state.tableData.data = data.items ?? [];
		state.tableData.total = data.totalCount ?? 0;
	} finally {
		state.tableData.loading = false;
	}
};

const onQuery = () => {
	state.tableData.param.pageIndex = 1;
	getTableData();
};

const onReset = () => {
	state.searchDateRange = [];
	state.queryParam = { userName: '', url: '', httpMethod: '', hasException: undefined };
	onQuery();
};

const onViewDetail = (row: AuditLogDto) => {
	detailDialogRef.value.openDialog(row);
};

const onDelete = async (row: AuditLogDto) => {
	try {
		const { deleteAuditLog } = useAuditLogApi();
		await deleteAuditLog(row.id);
		ElMessage.success('删除成功');
		await getTableData();
	} catch {
		// 删除失败
	}
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
.system-audit-log-container {
	:deep(.full-table) {
		.el-card__body {
			padding: 0;
		}
		.el-table {
			border-radius: 0;
		}
		.pagination {
			padding: 15px;
			margin-top: 0;
			justify-content: flex-end;
		}
	}
}
</style>
