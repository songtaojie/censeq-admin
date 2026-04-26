<template>
	<div class="system-tenant-menu-container layout-pd">
		<el-card shadow="hover" :body-style="{ paddingBottom: '0' }">
			<el-form :inline="true">
				<el-form-item label="菜单名称">
					<el-input v-model="state.keyword" placeholder="菜单名称/路由名称" clearable @keyup.enter="getTableData" />
				</el-form-item>
				<el-form-item label="菜单类型">
					<el-select v-model="state.filterType" placeholder="全部" clearable style="width: 110px">
						<el-option label="目录" :value="1" />
						<el-option label="菜单" :value="2" />
						<el-option label="按钮" :value="3" />
					</el-select>
				</el-form-item>
				<el-form-item>
					<el-button-group>
						<el-button type="primary" icon="ele-Search" @click="getTableData">查询</el-button>
						<el-button icon="ele-Refresh" @click="onReset">重置</el-button>
					</el-button-group>
				</el-form-item>
				<el-form-item>
					<el-button type="primary" icon="ele-Plus" @click="onOpenAddMenu">新增</el-button>
				</el-form-item>
				<el-form-item>
					<el-button
						:type="state.hasMenus ? 'warning' : 'success'"
						:icon="state.hasMenus ? 'ele-RefreshRight' : 'ele-CopyDocument'"
						:loading="state.copyLoading"
						@click="onCopyFromHost"
					>
						{{ state.hasMenus ? '从平台重新初始化' : '从平台复制菜单' }}
					</el-button>
				</el-form-item>
			</el-form>
		</el-card>

		<!-- 空状态提示 -->
		<el-card v-if="!state.tableData.loading && !state.hasMenus" shadow="hover" style="margin-top: 5px">
			<el-empty description="当前租户暂无菜单配置">
				<el-button type="primary" icon="ele-CopyDocument" :loading="state.copyLoading" @click="onCopyFromHost">
					从平台复制菜单
				</el-button>
			</el-empty>
		</el-card>

		<el-card v-else class="full-table" shadow="hover" style="margin-top: 5px">
			<el-table
				:data="state.tableData.data"
				v-loading="state.tableData.loading"
				style="width: 100%"
				row-key="id"
				:tree-props="{ children: 'children', hasChildren: 'hasChildren' }"
				border
				default-expand-all
			>
				<el-table-column label="菜单名称" min-width="220" show-overflow-tooltip>
					<template #default="scope">
						<SvgIcon v-if="scope.row.icon" :name="scope.row.icon" />
						<span class="ml5">{{ resolveTitle(scope.row.title) }}</span>
					</template>
				</el-table-column>
				<el-table-column label="类型" width="75" align="center">
					<template #default="scope">
						<el-tag size="small" :type="tagTypeMap[scope.row.type]" effect="light">{{ typeTextMap[scope.row.type] }}</el-tag>
					</template>
				</el-table-column>
				<el-table-column prop="name" label="菜单编码" width="180" show-overflow-tooltip />
				<el-table-column prop="path" label="路由路径" min-width="180" show-overflow-tooltip />
				<el-table-column label="组件路径" min-width="180" show-overflow-tooltip>
					<template #default="scope">
						<span>{{ scope.row.component || '—' }}</span>
					</template>
				</el-table-column>
				<el-table-column label="权限标识" min-width="220" show-overflow-tooltip>
					<template #default="scope">
						<span>{{ formatPermissions(scope.row.permissionNames) }}</span>
					</template>
				</el-table-column>
				<el-table-column label="排序" width="70" align="center">
					<template #default="scope">{{ scope.row.sort }}</template>
				</el-table-column>
				<el-table-column label="状态" width="80" align="center">
					<template #default="scope">
						<el-switch size="small" :model-value="scope.row.status" @change="(value: boolean) => onStatusChange(scope.row, value)" />
					</template>
				</el-table-column>
				<el-table-column label="操作" width="230" fixed="right" align="center">
					<template #default="scope">
						<el-button icon="ele-Plus" size="small" text type="primary" :disabled="scope.row.type === 3" @click="onOpenAddMenu(scope.row)">新增下级</el-button>
						<el-button icon="ele-Edit" size="small" text type="primary" @click="onOpenEditMenu(scope.row)">编辑</el-button>
						<el-button icon="ele-Delete" size="small" text type="danger" @click="onTabelRowDel(scope.row)">删除</el-button>
					</template>
				</el-table-column>
			</el-table>
		</el-card>

		<MenuDialog ref="menuDialogRef" @refresh="onDialogRefresh" />
	</div>
</template>

<script setup lang="ts" name="systemTenantMenu">
import { defineAsyncComponent, ref, onMounted, reactive } from 'vue';
import { ElMessageBox, ElMessage } from 'element-plus';
import { useMenuApi } from '/@/api/apis';
import type { MenuTreeItemDto } from '/@/api/models/menu';
import { setBackEndControlRefreshRoutes } from '/@/router/backEnd';
import { i18n } from '/@/i18n/index';

const MenuDialog = defineAsyncComponent(() => import('/@/views/system/menu/dialog.vue'));

const menuApi = useMenuApi();
const menuDialogRef = ref();
const typeTextMap: Record<number, string> = { 1: '目录', 2: '菜单', 3: '按钮' };
const tagTypeMap: Record<number, 'success' | 'warning' | 'info'> = { 1: 'success', 2: 'warning', 3: 'info' };

const state = reactive({
	keyword: '',
	filterType: undefined as number | undefined,
	hasMenus: false,
	copyLoading: false,
	tableData: {
		data: [] as MenuTreeItemDto[],
		loading: false,
	},
});

const resolveTitle = (title: string) => {
	return title?.startsWith('message.') ? i18n.global.t(title) : title;
};

const formatPermissions = (permissionNames: string[] | undefined) => {
	if (!permissionNames || permissionNames.length === 0) return '—';
	return permissionNames.join(', ');
};

const filterTree = (menus: MenuTreeItemDto[], keyword: string, type?: number): MenuTreeItemDto[] => {
	return menus
		.map((menu) => {
			const children = filterTree(menu.children ?? [], keyword, type);
			const keywordMatched =
				!keyword ||
				[menu.name, menu.title, menu.path, menu.routeName]
					.filter(Boolean)
					.some((item) => String(item).toLowerCase().includes(keyword.trim().toLowerCase()));
			const typeMatched = type === undefined || menu.type === type;
			if ((keywordMatched && typeMatched) || children.length > 0) {
				return { ...menu, children };
			}
			return null;
		})
		.filter((item): item is MenuTreeItemDto => item !== null);
};

const getTableData = async () => {
	state.tableData.loading = true;
	try {
		const data = await menuApi.getMenuTree();
		const all = data.items ?? [];
		state.hasMenus = all.length > 0;
		state.tableData.data = filterTree(all, state.keyword, state.filterType);
	} finally {
		state.tableData.loading = false;
	}
};

const onReset = () => {
	state.keyword = '';
	state.filterType = undefined;
	getTableData();
};

const onDialogRefresh = async () => {
	await getTableData();
	await setBackEndControlRefreshRoutes();
};

const onOpenAddMenu = (row?: MenuTreeItemDto) => {
	menuDialogRef.value.openDialog('add', row);
};

const onOpenEditMenu = (row: MenuTreeItemDto) => {
	menuDialogRef.value.openDialog('edit', row);
};

const onStatusChange = async (row: MenuTreeItemDto, value: boolean) => {
	try {
		await menuApi.setMenuStatus(row.id, { status: value });
		ElMessage.success('状态更新成功');
		await onDialogRefresh();
	} catch {
		await getTableData();
	}
};

const onTabelRowDel = (row: MenuTreeItemDto) => {
	ElMessageBox.confirm(`此操作将永久删除菜单「${resolveTitle(row.title)}」，是否继续?`, '提示', {
		confirmButtonText: '删除',
		cancelButtonText: '取消',
		type: 'warning',
	})
		.then(async () => {
			await menuApi.deleteMenu(row.id);
			ElMessage.success('删除成功');
			await onDialogRefresh();
		})
		.catch(() => {});
};

const onCopyFromHost = () => {
	if (state.hasMenus) {
		ElMessageBox.confirm('此操作将清除当前租户所有已有菜单，并从平台重新复制，是否继续？', '警告', {
			confirmButtonText: '确认重置',
			cancelButtonText: '取消',
			type: 'warning',
		})
			.then(async () => {
				state.copyLoading = true;
				try {
					await menuApi.copyMenusFromHost({ clearExisting: true });
					ElMessage.success('菜单已从平台重新初始化');
					await onDialogRefresh();
				} finally {
					state.copyLoading = false;
				}
			})
			.catch(() => {});
	} else {
		ElMessageBox.confirm('将从平台模板复制菜单到当前租户，是否继续？', '提示', {
			confirmButtonText: '确认',
			cancelButtonText: '取消',
			type: 'info',
		})
			.then(async () => {
				state.copyLoading = true;
				try {
					await menuApi.copyMenusFromHost({ clearExisting: false });
					ElMessage.success('菜单已从平台复制成功');
					await onDialogRefresh();
				} finally {
					state.copyLoading = false;
				}
			})
			.catch(() => {});
	}
};

onMounted(() => {
	getTableData();
});
</script>
