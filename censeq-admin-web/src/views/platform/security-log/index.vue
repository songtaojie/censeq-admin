<template>
	<div class="system-security-log-container layout-padding">
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
				<el-form-item label="操作类型">
					<el-select v-model="state.queryParam.action" placeholder="全部" clearable style="width: 160px">
						<el-option label="登录成功" value="LoginSucceeded" />
						<el-option label="登录失败（密码错误）" value="LoginFailed.InvalidUserNameOrPassword" />
						<el-option label="登录失败（账户锁定）" value="LoginFailed.LockedOut" />
						<el-option label="登出" value="Logout" />
					</el-select>
				</el-form-item>
				<el-form-item label="IP地址">
					<el-input v-model="state.queryParam.clientIpAddress" placeholder="IP地址" clearable style="width: 140px" @keyup.enter="onQuery" />
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
				<el-table-column label="时间" width="160" show-overflow-tooltip>
					<template #default="{ row }">
						{{ formatDateTime(row.creationTime) }}
					</template>
				</el-table-column>
				<el-table-column prop="userName" label="用户名" width="120" show-overflow-tooltip>
					<template #default="{ row }">
						<span style="font-weight: 600; color: var(--el-color-primary)">{{ row.userName || '—' }}</span>
					</template>
				</el-table-column>
				<el-table-column prop="action" label="操作" width="160" align="center">
					<template #default="{ row }">
						<el-tag size="small" :type="getActionTagType(row.action)" effect="light">
							{{ formatAction(row.action) }}
						</el-tag>
					</template>
				</el-table-column>
				<el-table-column prop="identity" label="登录来源" width="120" show-overflow-tooltip>
					<template #default="{ row }">
						<el-tag v-if="row.identity" size="small" type="info" effect="plain">{{ row.identity }}</el-tag>
						<span v-else>—</span>
					</template>
				</el-table-column>
				<el-table-column prop="clientIpAddress" label="IP地址" width="130" show-overflow-tooltip />
				<el-table-column prop="clientId" label="客户端ID" width="140" show-overflow-tooltip>
					<template #default="{ row }">{{ row.clientId || '—' }}</template>
				</el-table-column>
				<el-table-column prop="browserInfo" label="浏览器信息" min-width="180" show-overflow-tooltip>
					<template #default="{ row }">{{ row.browserInfo || '—' }}</template>
				</el-table-column>
				<el-table-column label="操作" width="90" fixed="right" align="center">
					<template #default="{ row }">
						<el-popconfirm title="确定删除该登录日志吗？" @confirm="onDelete(row)">
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
	</div>
</template>

<script setup lang="ts" name="systemSecurityLog">
import { reactive, onMounted, ref } from 'vue';
import { ElMessage } from 'element-plus';
import { useSecurityLogApi } from '/@/api/apis';
import type { IdentitySecurityLogDto } from '/@/api/models/identity';

const queryFormRef = ref();

const state = reactive({
	searchDateRange: [] as string[],
	queryParam: {
		userName: '',
		action: '',
		clientIpAddress: '',
	},
	tableData: {
		data: [] as IdentitySecurityLogDto[],
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

const ACTION_MAP: Record<string, string> = {
	LoginSucceeded: '登录成功',
	Logout: '登出',
};

const formatAction = (action: string) => {
	if (ACTION_MAP[action]) return ACTION_MAP[action];
	if (action?.startsWith('LoginFailed')) return '登录失败';
	return action ?? '—';
};

const getActionTagType = (action: string): 'success' | 'danger' | 'info' | 'warning' | '' => {
	if (action === 'LoginSucceeded') return 'success';
	if (action === 'Logout') return 'info';
	if (action?.startsWith('LoginFailed')) return 'danger';
	return '';
};

const getTableData = async () => {
	state.tableData.loading = true;
	try {
		const { getSecurityLogPage } = useSecurityLogApi();
		const data = await getSecurityLogPage({
			skipCount: (state.tableData.param.pageIndex - 1) * state.tableData.param.pageSize,
			maxResultCount: state.tableData.param.pageSize,
			startTime: state.searchDateRange?.[0],
			endTime: state.searchDateRange?.[1],
			userName: state.queryParam.userName || undefined,
			action: state.queryParam.action || undefined,
			clientIpAddress: state.queryParam.clientIpAddress || undefined,
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
	state.queryParam = { userName: '', action: '', clientIpAddress: '' };
	onQuery();
};

const onDelete = async (row: IdentitySecurityLogDto) => {
	try {
		const { deleteSecurityLog } = useSecurityLogApi();
		await deleteSecurityLog(row.id);
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
.system-security-log-container {
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