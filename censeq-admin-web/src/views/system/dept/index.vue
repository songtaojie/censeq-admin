<template>
	<div class="system-org-container layout-padding">
		<div class="system-org-main layout-padding-auto layout-padding-view">
			<splitpanes class="default-theme" style="height: 100%; width: 100%">
				<pane size="20" style="overflow: hidden; min-width: 160px">
					<OrgTree ref="orgTreeRef" @node-click="onNodeClick" />
				</pane>
				<pane size="80" style="overflow-y: auto; padding: 0 0 0 5px; display: flex; flex-direction: column; min-width: 300px">
					<!-- 搜索栏 -->
					<el-card shadow="hover" :body-style="{ paddingBottom: '0' }">
						<el-form :inline="true" :model="state.queryParams" @keyup.enter="handleQuery">
							<el-form-item label="机构名称">
								<el-input v-model="state.queryParams.name" placeholder="机构名称" clearable style="width: 180px" />
							</el-form-item>
							<el-form-item>
								<el-button type="primary" icon="ele-Search" @click="handleQuery">查 询</el-button>
								<el-button icon="ele-RefreshRight" @click="resetQuery">重 置</el-button>
							</el-form-item>
						</el-form>
					</el-card>

					<!-- 操作工具栏 + 表格 -->
					<el-card shadow="hover" style="margin-top: 5px">
						<template #header>
							<div class="table-toolbar">
								<div class="toolbar-left">
									<el-button type="primary" icon="ele-Plus" size="small" @click="onOpenAddRoot">新 增</el-button>
									<el-button plain icon="ele-Expand" size="small" @click="setTableExpand(true)">全部展开</el-button>
									<el-button plain icon="ele-Fold" size="small" @click="setTableExpand(false)">全部折叠</el-button>
								</div>
								<div class="toolbar-right">
									<el-tooltip content="刷新" placement="top">
										<el-button circle icon="ele-Refresh" size="small" @click="onRefresh" />
									</el-tooltip>
								</div>
							</div>
						</template>
						<el-table
							ref="tableRef"
							v-loading="state.loading"
							:data="displayData"
							row-key="id"
							default-expand-all
							:tree-props="{ children: 'children', hasChildren: 'hasChildren' }"
							style="width: 100%"
							stripe
							border
							highlight-current-row
						>
							<el-table-column type="index" label="序号" width="60" align="center" />
							<el-table-column prop="displayName" label="机构名称" min-width="200" show-overflow-tooltip>
								<template #default="{ row }">
									<el-icon v-if="!row.parentId" style="vertical-align: middle; margin-right: 4px; color: var(--el-color-primary)"><ele-OfficeBuilding /></el-icon>
									<el-icon v-else style="vertical-align: middle; margin-right: 4px; color: var(--el-color-success)"><ele-Connection /></el-icon>
									<span>{{ row.displayName }}</span>
								</template>
							</el-table-column>
							<el-table-column prop="code" label="机构编码" align="center" min-width="160" show-overflow-tooltip />
							<el-table-column label="级别" align="center" width="80">
								<template #default="{ row }">
									<el-tag v-if="row.code" size="small" :type="levelTagType(row.code)" effect="plain">
										{{ getLevel(row.code) }} 级
									</el-tag>
								</template>
							</el-table-column>						<el-table-column label="状态" align="center" width="80">
							<template #default="{ row }">
								<el-tag v-if="row.status === 1" type="success" size="small" effect="light">启 用</el-tag>
								<el-tag v-else type="info" size="small" effect="light">禁 用</el-tag>
							</template>
						</el-table-column>
						<el-table-column prop="remark" label="备注" min-width="120" show-overflow-tooltip>
							<template #default="{ row }">
								<span style="color: #999">{{ row.remark || '—' }}</span>
							</template>
						</el-table-column>							<el-table-column label="操作" width="220" fixed="right" align="center">
								<template #default="{ row }">
									<el-button icon="ele-Plus" size="small" text type="primary" @click="onAddChild(row)">新增子级</el-button>
									<el-divider direction="vertical" />
									<el-button icon="ele-Edit" size="small" text type="primary" @click="onEdit(row)">编 辑</el-button>
									<el-divider direction="vertical" />
									<el-button icon="ele-Delete" size="small" text type="danger" @click="onDel(row)">删 除</el-button>
								</template>
							</el-table-column>
						</el-table>
					</el-card>
				</pane>
			</splitpanes>
		</div>
		<DeptDialog ref="deptDialogRef" @refresh="onRefresh()" />
	</div>
</template>

<script setup lang="ts" name="systemOrg">
import { defineAsyncComponent, reactive, ref, computed, onMounted } from 'vue';
import { ElMessageBox, ElMessage } from 'element-plus';
import { Splitpanes, Pane } from 'splitpanes';
import 'splitpanes/dist/splitpanes.css';
import { useIdentityApi } from '/@/api/apis';
import type { OrganizationUnitDto } from '/@/api/models/identity';

type OuNode = OrganizationUnitDto & { children?: OuNode[] };

const DeptDialog = defineAsyncComponent(() => import('/@/views/system/dept/dialog.vue'));
const OrgTree = defineAsyncComponent(() => import('/@/views/system/dept/orgTree.vue'));

const deptDialogRef = ref();
const orgTreeRef = ref();
const tableRef = ref<any>();

const state = reactive({
	loading: false,
	flat: [] as OrganizationUnitDto[],
	treeData: [] as OuNode[],
	selectedNodeId: null as string | null,
	queryParams: {
		name: '',
	},
});

function getLevel(code: string): number {
	return (code.match(/\./g)?.length ?? 0) + 1;
}

function levelTagType(code: string): '' | 'success' | 'warning' | 'info' | 'danger' {
	const lv = getLevel(code);
	if (lv === 1) return '';
	if (lv === 2) return 'success';
	if (lv === 3) return 'warning';
	return 'info';
}

function buildOuTree(items: OrganizationUnitDto[]): OuNode[] {
	const byId = new Map<string, OuNode>();
	items.forEach((i) => {
		if (!i.id) return;
		byId.set(i.id, { ...i, children: [] });
	});
	const roots: OuNode[] = [];
	byId.forEach((node) => {
		const pid = node.parentId;
		if (pid && byId.has(pid)) {
			byId.get(pid)!.children!.push(node);
		} else {
			roots.push(node);
		}
	});
	const sortTree = (nodes: OuNode[]) => {
		nodes.sort((a, b) => (a.code ?? '').localeCompare(b.code ?? '', undefined, { numeric: true }));
		nodes.forEach((n) => {
			if (n.children?.length) sortTree(n.children);
		});
	};
	sortTree(roots);
	return roots;
}

function getAllDescendantIds(parentId: string, flat: OrganizationUnitDto[]): string[] {
	const children = flat.filter((x) => x.parentId === parentId);
	return [...children.map((x) => x.id!), ...children.flatMap((x) => getAllDescendantIds(x.id!, flat))];
}

const displayData = computed<OuNode[]>(() => {
	let items = state.flat;

	if (state.selectedNodeId) {
		const ids = new Set([state.selectedNodeId, ...getAllDescendantIds(state.selectedNodeId, state.flat)]);
		items = items.filter((x) => ids.has(x.id!));
	}

	if (state.queryParams.name) {
		const q = state.queryParams.name.toLowerCase();
		const matchingIds = new Set(items.filter((x) => (x.displayName ?? '').toLowerCase().includes(q)).map((x) => x.id!));
		const ancestorIds = new Set<string>();
		matchingIds.forEach((id) => {
			let parentId = items.find((x) => x.id === id)?.parentId;
			while (parentId) {
				ancestorIds.add(parentId);
				parentId = items.find((x) => x.id === parentId)?.parentId;
			}
		});
		items = items.filter((x) => matchingIds.has(x.id!) || ancestorIds.has(x.id!));
	}

	return buildOuTree(items);
});

const loadData = async () => {
	state.loading = true;
	try {
		const { getOrganizationUnitAllList } = useIdentityApi();
		const res = await getOrganizationUnitAllList();
		state.flat = res.items ?? [];
		state.treeData = buildOuTree(state.flat);
	} catch {
		state.flat = [];
		state.treeData = [];
	} finally {
		state.loading = false;
	}
};

const onRefresh = async () => {
	await loadData();
	orgTreeRef.value?.refresh();
};

const onNodeClick = (node: OuNode | null) => {
	state.selectedNodeId = node?.id ?? null;
};

const handleQuery = () => {};

const resetQuery = () => {
	state.queryParams.name = '';
	state.selectedNodeId = null;
	orgTreeRef.value?.clearSelection?.();
};

function setTableExpand(expand: boolean) {
	const table = tableRef.value;
	if (!table) return;
	const traverse = (nodes: OuNode[]) => {
		nodes.forEach((n) => {
			table.store.states.treeData.value[n.id!] = { ...(table.store.states.treeData.value[n.id!] ?? {}), expanded: expand };
			if (n.children?.length) traverse(n.children);
		});
	};
	traverse(displayData.value);
}

const onOpenAddRoot = () => {
	deptDialogRef.value.openDialog('add', null, null);
};

const onAddChild = (row: OrganizationUnitDto) => {
	deptDialogRef.value.openDialog('add', null, row.id ?? null);
};

const onEdit = (row: OrganizationUnitDto) => {
	deptDialogRef.value.openDialog('edit', row, null);
};

const onDel = (row: OrganizationUnitDto) => {
	ElMessageBox.confirm(`确定删除机构：【${row.displayName}】？`, '提示', {
		type: 'warning',
		confirmButtonText: '确 定',
		cancelButtonText: '取 消',
	})
		.then(async () => {
			const { deleteOrganizationUnit } = useIdentityApi();
			await deleteOrganizationUnit(row.id!);
			ElMessage.success('删除成功');
			await onRefresh();
		})
		.catch(() => {});
};

onMounted(() => {
	loadData();
});
</script>

<style scoped lang="scss">
.system-org-container {
	display: flex;
	flex-direction: column;
	height: 100%;

	.system-org-main {
		padding: 0 !important;
		overflow: hidden;
	}

	:deep(.splitpanes.default-theme .splitpanes__pane) {
		background: transparent;
	}

	:deep(.splitpanes__splitter) {
		background: var(--el-border-color-light);
		width: 4px !important;
		margin: 0 2px;

		&:hover {
			background: var(--el-color-primary-light-5);
		}
	}

	.table-toolbar {
		display: flex;
		align-items: center;
		justify-content: space-between;
	}

	.toolbar-left {
		display: flex;
		align-items: center;
		gap: 6px;
	}

	.toolbar-right {
		display: flex;
		align-items: center;
		gap: 6px;
	}
}
</style>
