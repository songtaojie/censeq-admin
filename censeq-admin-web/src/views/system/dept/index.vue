<template>
	<div class="system-dept-container layout-padding">
		<div class="system-dept-padding layout-padding-auto layout-padding-view">
			<div class="system-dept-toolbar mb15">
				<el-button size="default" type="primary" @click="onRefresh">
					<el-icon><ele-Refresh /></el-icon>
					刷新
				</el-button>
				<el-button size="default" type="success" class="ml10" @click="onOpenAddRoot">
					<el-icon><ele-FolderAdd /></el-icon>
					新增根节点
				</el-button>
			</div>
			<el-table
				v-loading="state.loading"
				:data="state.treeData"
				row-key="id"
				default-expand-all
				:tree-props="{ children: 'children', hasChildren: 'hasChildren' }"
				style="width: 100%"
			>
				<el-table-column prop="displayName" label="显示名称" min-width="200" show-overflow-tooltip />
				<el-table-column prop="code" label="编码（层级）" min-width="200" show-overflow-tooltip />
				<el-table-column label="操作" width="200" fixed="right">
					<template #default="{ row }">
						<el-button size="small" text type="primary" @click="onAddChild(row)">新增子级</el-button>
						<el-button size="small" text type="primary" @click="onEdit(row)">编辑</el-button>
						<el-button size="small" text type="primary" @click="onDel(row)">删除</el-button>
					</template>
				</el-table-column>
			</el-table>
		</div>
		<DeptDialog ref="deptDialogRef" @refresh="loadTree()" />
	</div>
</template>

<script setup lang="ts" name="systemDept">
import { defineAsyncComponent, reactive, ref, onMounted } from 'vue';
import { ElMessageBox, ElMessage } from 'element-plus';
import { useIdentityApi } from '/@/api/apis';
import type { OrganizationUnitDto } from '/@/api/models/identity';

type OuNode = OrganizationUnitDto & { children?: OuNode[] };

const DeptDialog = defineAsyncComponent(() => import('/@/views/system/dept/dialog.vue'));

const deptDialogRef = ref();
const state = reactive({
	loading: false,
	flat: [] as OrganizationUnitDto[],
	treeData: [] as OuNode[],
});

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

const loadTree = async () => {
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

const onRefresh = () => loadTree();

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
	ElMessageBox.confirm(`将删除组织机构「${row.displayName}」及其子节点（若存在），是否继续？`, '提示', {
		type: 'warning',
		confirmButtonText: '删除',
		cancelButtonText: '取消',
	})
		.then(async () => {
			const { deleteOrganizationUnit } = useIdentityApi();
			await deleteOrganizationUnit(row.id!);
			ElMessage.success('已删除');
			await loadTree();
		})
		.catch(() => {});
};

onMounted(() => {
	loadTree();
});
</script>

<style scoped lang="scss">
.system-dept-container {
	.system-dept-padding {
		padding: 15px;
	}
}
</style>
