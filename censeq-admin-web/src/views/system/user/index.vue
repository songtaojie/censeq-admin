<template>
	<div class="system-user-container layout-padding">
		<div class="system-user-main layout-padding-auto layout-padding-view">
			<splitpanes class="default-theme user-splitpanes">
				<pane size="20" style="overflow: hidden; min-width: 180px">
					<OrgTree ref="orgTreeRef" @node-click="onOrganizationNodeClick" />
				</pane>
				<pane size="80" style="overflow-y: auto; padding-left: 6px; display: flex; flex-direction: column; min-width: 520px">
					<el-card shadow="hover" :body-style="{ paddingBottom: '0' }">
						<el-form ref="queryFormRef" :model="state.queryParam" :inline="true">
							<el-form-item label="账号">
								<el-input v-model="state.queryParam.userName" placeholder="账号" clearable style="width: 180px" @keyup.enter="onQuery" />
							</el-form-item>
							<el-form-item label="姓名">
								<el-input v-model="state.queryParam.name" placeholder="姓名" clearable style="width: 180px" @keyup.enter="onQuery" />
							</el-form-item>
							<el-form-item label="手机号">
								<el-input v-model="state.queryParam.phoneNumber" placeholder="手机号" clearable style="width: 180px" @keyup.enter="onQuery" />
							</el-form-item>
							<el-form-item label="邮箱">
								<el-input v-model="state.queryParam.email" placeholder="邮箱" clearable style="width: 180px" @keyup.enter="onQuery" />
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
						<template #header>
							<div class="table-toolbar">
								<div class="toolbar-left">
									<el-button type="primary" icon="ele-Plus" size="small" @click="onOpenAdd">新增</el-button>
								</div>
								<div class="toolbar-right">
									<el-tooltip content="刷新" placement="top">
										<el-button circle icon="ele-Refresh" size="small" @click="onRefresh" />
									</el-tooltip>
								</div>
							</div>
						</template>
						<el-table :data="state.tableData.data" v-loading="state.tableData.loading" style="width: 100%" border stripe>
							<el-table-column type="index" label="序号" width="60" align="center" fixed />
							<el-table-column label="头像" width="70" align="center">
								<template #default="{ row }">
									<UserAvatar :src="getAvatarSrc(row)" :name="getDisplayName(row)" :user-name="row.userName" :size="32" :font-size="14" />
								</template>
							</el-table-column>
							<el-table-column prop="userName" label="账号" min-width="120" show-overflow-tooltip>
								<template #default="{ row }">
									<span style="font-weight: 600; color: var(--el-color-primary)">{{ row.userName }}</span>
								</template>
							</el-table-column>
							<el-table-column label="姓名" min-width="110" show-overflow-tooltip>
								<template #default="{ row }">
									{{ [row.surname, row.name].filter(Boolean).join(' ') || '-' }}
								</template>
							</el-table-column>
							<el-table-column prop="email" label="邮箱" min-width="180" show-overflow-tooltip />
							<el-table-column prop="phoneNumber" label="手机号码" width="130" align="center" show-overflow-tooltip>
								<template #default="{ row }">
									{{ row.phoneNumber || '-' }}
								</template>
							</el-table-column>
							<el-table-column label="所属机构" min-width="150" show-overflow-tooltip>
								<template #default="{ row }">
									{{ row.organizationUnitNames || '-' }}
								</template>
							</el-table-column>
							<el-table-column label="状态" width="80" align="center">
								<template #default="{ row }">
									<el-switch
										size="small"
										:model-value="row.isActive"
										inline-prompt
										active-text="启用"
										inactive-text="禁用"
										@change="(val: boolean) => onStatusChange(row, val)"
									/>
								</template>
							</el-table-column>
							<el-table-column label="锁定策略" width="90" align="center">
								<template #default="{ row }">
									<el-tag :type="row.lockoutEnabled ? 'warning' : 'info'" size="small" effect="light">
										{{ row.lockoutEnabled ? '已开启' : '已关闭' }}
									</el-tag>
								</template>
							</el-table-column>
							<el-table-column label="创建时间" min-width="160" show-overflow-tooltip>
								<template #default="{ row }">
									{{ formatDate(row.creationTime) }}
								</template>
							</el-table-column>
							<el-table-column label="操作" width="220" fixed="right" align="center">
								<template #default="{ row }">
									<el-button icon="ele-Edit" size="small" text type="primary" @click="onOpenEdit(row)">编辑</el-button>
									<el-button icon="ele-Key" size="small" text type="warning" @click="onResetPassword(row)">重置密码</el-button>
									<el-button icon="ele-Delete" size="small" text type="danger" @click="onRowDel(row)">删除</el-button>
								</template>
							</el-table-column>
						</el-table>
						<el-pagination
							@size-change="onHandleSizeChange"
							@current-change="onHandleCurrentChange"
							class="pagination"
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
				</pane>
			</splitpanes>
		</div>

		<UserDialog ref="userDialogRef" @refresh="onRefresh()" />
	</div>
</template>

<script setup lang="ts" name="systemUser">
import { defineAsyncComponent, reactive, onMounted, ref } from 'vue';
import { ElMessageBox, ElMessage } from 'element-plus';
import type { FormInstance } from 'element-plus';
import { Splitpanes, Pane } from 'splitpanes';
import 'splitpanes/dist/splitpanes.css';
import { useIdentityApi } from '/@/api/apis';
import type { IdentityUserDto, IdentityUserOrganizationUnitDto, OrganizationUnitDto } from '/@/api/models/identity';
import UserAvatar from '/@/components/UserAvatar/index.vue';

type OuNode = OrganizationUnitDto & { children?: OuNode[] };
type UserTableRow = IdentityUserDto & {
	organizationUnitNames?: string;
	primaryOrganizationUnitName?: string;
};

const UserDialog = defineAsyncComponent(() => import('/@/views/system/user/dialog.vue'));
const OrgTree = defineAsyncComponent(() => import('/@/views/system/dept/orgTree.vue'));

const userDialogRef = ref();
const orgTreeRef = ref();
const queryFormRef = ref<FormInstance>();

const state = reactive({
	queryParam: {
		userName: '',
		name: '',
		phoneNumber: '',
		email: '',
	},
	organizationUnits: [] as OrganizationUnitDto[],
	selectedOrganizationUnitId: null as string | null,
	selectedOrganizationUnitName: '',
	tableData: {
		data: [] as UserTableRow[],
		total: 0,
		loading: false,
		param: {
			pageIndex: 1,
			pageSize: 10,
		},
	},
});

const getDisplayName = (row: IdentityUserDto) => [row.surname, row.name].filter(Boolean).join(' ') || row.userName || '?';
const getAvatarSrc = (row: IdentityUserDto) => {
	const extra = row.extraProperties ?? {};
	return (
		row.avatarUrl ??
		extra.avatarUrl ??
		extra.AvatarUrl ??
		extra.avatar ??
		extra.Avatar ??
		extra.photo ??
		extra.Photo ??
		extra.picture ??
		extra.Picture ??
		extra.profilePicture ??
		extra.ProfilePicture ??
		''
	);
};

const formatDate = (val?: string) => {
	if (!val) return '-';
	return val.replace('T', ' ').substring(0, 19);
};

const buildFilter = () => {
	const parts: string[] = [];
	if (state.queryParam.userName) parts.push(state.queryParam.userName);
	if (state.queryParam.name) parts.push(state.queryParam.name);
	if (state.queryParam.phoneNumber) parts.push(state.queryParam.phoneNumber);
	if (state.queryParam.email) parts.push(state.queryParam.email);
	return parts.join(' ') || undefined;
};

const getDescendantOrganizationUnitIds = (parentId: string): string[] => {
	const children = state.organizationUnits.filter((item) => item.parentId === parentId && item.id);
	return [...children.map((item) => item.id!), ...children.flatMap((item) => getDescendantOrganizationUnitIds(item.id!))];
};

const getSelectedOrganizationUnitIds = () => {
	if (!state.selectedOrganizationUnitId) return [];
	return [state.selectedOrganizationUnitId, ...getDescendantOrganizationUnitIds(state.selectedOrganizationUnitId)];
};

const loadOrganizationUnits = async () => {
	const { getOrganizationUnitAllList } = useIdentityApi();
	const res = await getOrganizationUnitAllList();
	state.organizationUnits = res.items ?? [];
};

const loadUserOrganizationNames = async (users: IdentityUserDto[]): Promise<UserTableRow[]> => {
	const { getUserOrganizationUnits } = useIdentityApi();
	const rows = users as UserTableRow[];
	await Promise.all(
		rows.map(async (row) => {
			if (!row.id) return;
			try {
				const res = await getUserOrganizationUnits(row.id);
				const items = (res.items ?? []) as IdentityUserOrganizationUnitDto[];
				row.organizationUnitNames = items.map((item) => item.displayName).filter(Boolean).join('、');
				row.primaryOrganizationUnitName = items.find((item) => item.isPrimary)?.displayName ?? items[0]?.displayName ?? '';
			} catch {
				row.organizationUnitNames = '';
				row.primaryOrganizationUnitName = '';
			}
		})
	);
	return rows;
};

const getTableData = async () => {
	state.tableData.loading = true;
	try {
		const { getUserPage } = useIdentityApi();
		const organizationUnitIds = getSelectedOrganizationUnitIds();
		const data = await getUserPage({
			filter: buildFilter(),
			organizationUnitIds: organizationUnitIds.length ? organizationUnitIds : undefined,
			skipCount: (state.tableData.param.pageIndex - 1) * state.tableData.param.pageSize,
			maxResultCount: state.tableData.param.pageSize,
		});
		state.tableData.data = await loadUserOrganizationNames(data.items ?? []);
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
	queryFormRef.value?.resetFields();
	state.queryParam.userName = '';
	state.queryParam.name = '';
	state.queryParam.phoneNumber = '';
	state.queryParam.email = '';
	state.tableData.param.pageIndex = 1;
	getTableData();
};

const onRefresh = async () => {
	await loadOrganizationUnits();
	orgTreeRef.value?.refresh?.();
	await getTableData();
};

const onOrganizationNodeClick = (node: OuNode | null) => {
	state.selectedOrganizationUnitId = node?.id ?? null;
	state.selectedOrganizationUnitName = node?.displayName ?? '';
	state.tableData.param.pageIndex = 1;
	getTableData();
};

const onOpenAdd = () => {
	userDialogRef.value.openDialog('add');
};

const onOpenEdit = (row: IdentityUserDto) => {
	userDialogRef.value.openDialog('edit', row);
};

const onStatusChange = async (row: IdentityUserDto, val: boolean) => {
	try {
		const api = useIdentityApi();
		const user = await api.getUser(row.id!);
		await api.updateUser(row.id!, {
			userName: user.userName!,
			name: user.name,
			surname: user.surname,
			email: user.email!,
			phoneNumber: user.phoneNumber,
			avatarUrl: user.avatarUrl,
			isActive: val,
			lockoutEnabled: user.lockoutEnabled,
			roleNames: [],
			concurrencyStamp: user.concurrencyStamp,
		});
		row.isActive = val;
		ElMessage.success(val ? '已启用' : '已禁用');
	} catch {
		ElMessage.error('操作失败');
	}
};

const onResetPassword = async (row: IdentityUserDto) => {
	const result = await ElMessageBox.prompt(`请输入用户「${row.userName}」的新密码（至少 6 位）`, '重置密码', {
		inputType: 'password',
		inputValidator: (v) => (!v || v.length < 6 ? '密码至少 6 位' : true),
		confirmButtonText: '确定',
		cancelButtonText: '取消',
	}).catch(() => null);
	if (!result) return;
	const api = useIdentityApi();
	await api.resetUserPassword(row.id!, result.value);
	ElMessage.success('密码已重置');
};

const onRowDel = (row: IdentityUserDto) => {
	ElMessageBox.confirm(`此操作将永久删除用户「${row.userName}」，是否继续？`, '提示', {
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

onMounted(async () => {
	await loadOrganizationUnits();
	await getTableData();
});
</script>

<style scoped lang="scss">
.system-user-container,
.system-user-main,
.user-splitpanes {
	height: 100%;
}

.system-user-main {
	overflow: hidden;
}

.full-table {
	flex: 1;
	min-height: 0;

	:deep(.el-card__body) {
		height: calc(100% - 49px);
		display: flex;
		flex-direction: column;
	}
}

.table-toolbar {
	display: flex;
	align-items: center;
	justify-content: space-between;
	gap: 12px;
}

.toolbar-left,
.toolbar-right {
	display: flex;
	align-items: center;
	gap: 8px;
}

:deep(.el-table) {
	flex: 1;
}
</style>
