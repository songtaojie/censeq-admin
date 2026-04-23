<template>
	<el-card class="org-tree-card" shadow="hover" :body-style="{ padding: '8px', height: 'calc(100% - 57px)', overflowY: 'auto' }">
		<template #header>
			<div class="tree-header">
				<el-input v-model="filterText" placeholder="机构名称" :prefix-icon="Search" size="small" clearable style="flex: 1" />
				<el-dropdown @command="handleCommand" style="margin-left: 8px; flex-shrink: 0">
					<el-button size="small" style="padding: 0 8px">
						<el-icon><ele-MoreFilled /></el-icon>
					</el-button>
					<template #dropdown>
						<el-dropdown-menu>
							<el-dropdown-item command="expandAll">全部展开</el-dropdown-item>
							<el-dropdown-item command="collapseAll">全部折叠</el-dropdown-item>
							<el-dropdown-item divided command="reset">显示全部</el-dropdown-item>
							<el-dropdown-item command="refresh">刷 新</el-dropdown-item>
						</el-dropdown-menu>
					</template>
				</el-dropdown>
			</div>
		</template>
		<el-tree
			ref="treeRef"
			:data="treeData"
			node-key="id"
			:props="treeProps"
			:filter-node-method="filterNode"
			highlight-current
			:expand-on-click-node="false"
			v-loading="loading"
			@node-click="onNodeClick"
		>
			<template #default="{ node, data }">
				<span class="tree-node">
					<el-icon class="tree-icon icon-level1" v-if="node.level === 1"><ele-OfficeBuilding /></el-icon>
					<el-icon class="tree-icon icon-level2" v-else-if="node.level === 2"><ele-PriceTag /></el-icon>
					<el-icon class="tree-icon icon-level3" v-else><ele-CollectionTag /></el-icon>
					<span>{{ data.displayName }}</span>
				</span>
			</template>
		</el-tree>
	</el-card>
</template>

<script setup lang="ts" name="systemOrgTree">
import { ref, watch, onMounted } from 'vue';
import { Search } from '@element-plus/icons-vue';
import { useIdentityApi } from '/@/api/apis';
import type { OrganizationUnitDto } from '/@/api/models/identity';

type OuNode = OrganizationUnitDto & { children?: OuNode[] };

const emit = defineEmits<{
	'node-click': [node: OuNode | null];
}>();

const treeRef = ref<any>();
const filterText = ref('');
const loading = ref(false);
const treeData = ref<OuNode[]>([]);

const treeProps = {
	children: 'children',
	label: 'displayName',
};

watch(filterText, (val) => {
	treeRef.value?.filter(val);
});

function filterNode(value: string, data: OuNode): boolean {
	if (!value) return true;
	return (data.displayName ?? '').toLowerCase().includes(value.toLowerCase());
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

async function refresh() {
	loading.value = true;
	try {
		const { getOrganizationUnitAllList } = useIdentityApi();
		const res = await getOrganizationUnitAllList();
		treeData.value = buildOuTree(res.items ?? []);
	} catch {
		treeData.value = [];
	} finally {
		loading.value = false;
	}
}

function onNodeClick(data: OuNode) {
	emit('node-click', data);
}

function setAllExpanded(expand: boolean) {
	const traverse = (nodes: OuNode[]) => {
		nodes.forEach((node) => {
			const n = treeRef.value?.getNode(node.id!);
			if (n) n.expanded = expand;
			if (node.children?.length) traverse(node.children);
		});
	};
	traverse(treeData.value);
}

function handleCommand(cmd: string) {
	switch (cmd) {
		case 'expandAll':
			setAllExpanded(true);
			break;
		case 'collapseAll':
			setAllExpanded(false);
			break;
		case 'reset':
			treeRef.value?.setCurrentKey(undefined);
			emit('node-click', null);
			break;
		case 'refresh':
			refresh();
			break;
	}
}

onMounted(() => {
	refresh();
});

defineExpose({ refresh });
</script>

<style scoped lang="scss">
.org-tree-card {
	height: 100%;

	:deep(.el-card__header) {
		padding: 8px 12px;
		border-bottom: 1px solid var(--el-border-color-lighter);
	}

	:deep(.el-card__body) {
		padding: 6px 4px;
		height: calc(100% - 57px);
		overflow-y: auto;
	}

	:deep(.el-tree-node__content) {
		height: 32px;
		border-radius: 4px;
		margin-bottom: 1px;

		&:hover {
			background-color: var(--el-color-primary-light-9);
		}
	}

	:deep(.el-tree-node.is-current > .el-tree-node__content) {
		background-color: var(--el-color-primary-light-8);
		color: var(--el-color-primary);
		font-weight: 600;
	}

	.tree-header {
		display: flex;
		align-items: center;
	}

	.tree-node {
		display: flex;
		align-items: center;
		font-size: 13px;
	}

	.tree-icon {
		margin-right: 5px;
		font-size: 15px;
	}

	.icon-level1 {
		color: var(--el-color-primary);
	}

	.icon-level2 {
		color: var(--el-color-success);
	}

	.icon-level3 {
		color: var(--el-color-warning);
	}
}
</style>
