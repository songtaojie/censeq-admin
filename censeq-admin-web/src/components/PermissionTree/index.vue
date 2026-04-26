<template>
	<div class="permission-tree-wrapper" v-loading="loading">
		<div v-if="showSummary" class="permission-tree__summary">
			<el-tag type="success">已授权 {{ grantedCount }} 项</el-tag>
			<el-tag type="info">共 {{ totalCount }} 项</el-tag>
			<slot name="summary-extra" />
		</div>
		<el-tree
			ref="treeRef"
			node-key="name"
			:data="treeData"
			:props="TREE_PROPS"
			show-checkbox
			:check-strictly="false"
			:default-expand-all="defaultExpandAll"
			class="permission-tree"
			@check="onCheck"
		>
			<template #default="{ data: node }">
				<span class="permission-tree__node">
					<span>{{ node.displayName }}</span>
					<el-tag
						v-if="node.name && referencedNames?.has(node.name)"
						type="warning"
						size="small"
						effect="plain"
						class="permission-tree__tag"
					>菜单引用</el-tag>
				</span>
			</template>
		</el-tree>
		<div v-if="!loading && !treeData.length" class="permission-tree__empty">
			<el-empty description="暂无权限数据" :image-size="60" />
		</div>
	</div>
</template>

<script setup lang="ts">
import { ref, computed, watch, nextTick } from 'vue';
import type { ElTree } from 'element-plus';
import type { PermissionGroupDto } from '/@/api/models/permission';

// ── Props ──────────────────────────────────────────────────────────────────
const props = withDefaults(
	defineProps<{
		/** 权限分组数据，来自 GET /permission-management/permissions */
		data: PermissionGroupDto[];
		/** 当前已授权的权限名称列表（v-model） */
		modelValue?: string[];
		/** 是否显示顶部已授权/总数统计 */
		showSummary?: boolean;
		/** 是否默认展开所有节点 */
		defaultExpandAll?: boolean;
		/** 加载态 */
		loading?: boolean;
		/** 需要标记"菜单引用"的权限名称集合（可选） */
		referencedNames?: Set<string>;
	}>(),
	{
		modelValue: () => [],
		showSummary: true,
		defaultExpandAll: true,
		loading: false,
	}
);

const emit = defineEmits<{
	(e: 'update:modelValue', value: string[]): void;
}>();

// ── 常量 ──────────────────────────────────────────────────────────────────
const TREE_PROPS = {
	children: 'permissions',
	label: 'displayName',
} as const;

// ── Refs ──────────────────────────────────────────────────────────────────
const treeRef = ref<InstanceType<typeof ElTree>>();

/** 防止 watch(modelValue) 和 onCheck emit 循环触发 */
let _skipSync = false;

// ── Computed ──────────────────────────────────────────────────────────────
/** 所有叶子权限名称集合（分组内 permissions[].name） */
const allPermNames = computed<Set<string>>(() => {
	const s = new Set<string>();
	for (const group of props.data) {
		for (const p of group.permissions) {
			s.add(p.name);
		}
	}
	return s;
});

const totalCount = computed(() => allPermNames.value.size);

const grantedCount = computed(
	() => (props.modelValue ?? []).filter((n) => allPermNames.value.has(n)).length
);

const treeData = computed(() => props.data);

// ── 同步工具 ──────────────────────────────────────────────────────────────
function syncChecked(names: string[]) {
	treeRef.value?.setCheckedKeys([]);
	treeRef.value?.setCheckedKeys(names);
}

/** data 加载/更换后，将 modelValue 同步到树 */
watch(
	() => props.data,
	() => {
		nextTick(() => syncChecked(props.modelValue ?? []));
	},
	{ deep: false }
);

/** 父组件外部修改 modelValue（如全选/清空）→ 同步到树 */
watch(
	() => props.modelValue,
	(val) => {
		if (_skipSync) {
			_skipSync = false;
			return;
		}
		if (!props.data.length) return;
		nextTick(() => syncChecked(val ?? []));
	}
);

// ── 事件 ──────────────────────────────────────────────────────────────────
function onCheck() {
	_skipSync = true;
	emit('update:modelValue', getGrantedNames());
}

// ── 公开方法 ──────────────────────────────────────────────────────────────

/**
 * 获取当前已授权的叶子权限名称列表。
 * 调用时机：父组件点击"保存"时。
 */
function getGrantedNames(): string[] {
	const checked = (treeRef.value?.getCheckedKeys() ?? []) as string[];
	const halfChecked = (treeRef.value?.getHalfCheckedKeys() ?? []) as string[];
	const all = new Set([...checked, ...halfChecked]);
	// 只返回真实权限（叶子），排除分组节点
	return Array.from(allPermNames.value).filter((n) => all.has(n));
}

/** 全选所有权限 */
function checkAll() {
	syncChecked(Array.from(allPermNames.value));
	_skipSync = true;
	emit('update:modelValue', Array.from(allPermNames.value));
}

/** 清空所有勾选 */
function uncheckAll() {
	treeRef.value?.setCheckedKeys([]);
	_skipSync = true;
	emit('update:modelValue', []);
}

defineExpose({ getGrantedNames, checkAll, uncheckAll });
</script>

<style scoped lang="scss">
.permission-tree-wrapper {
	width: 100%;
}

.permission-tree__summary {
	display: flex;
	flex-wrap: wrap;
	gap: 8px;
	margin-bottom: 10px;
	align-items: center;
}

.permission-tree {
	border: 1px solid var(--el-border-color);
	border-radius: var(--el-border-radius-base);
	padding: 10px;
	max-height: 460px;
	overflow-y: auto;
}

.permission-tree__node {
	display: inline-flex;
	align-items: center;
}

.permission-tree__tag {
	margin-left: 6px;
}

.permission-tree__empty {
	display: flex;
	justify-content: center;
	padding: 20px 0;
}
</style>
