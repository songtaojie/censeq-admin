<template>
	<div class="platform-cache-container layout-padding">
		<div class="cache-shell layout-padding-auto layout-padding-view">
			<el-card shadow="hover" class="cache-list-card">
				<template #header>
					<div class="card-header">
						<span>缓存列表</span>
						<div class="header-actions">
							<el-button icon="ele-Refresh" circle plain type="primary" :loading="state.loadingKeys" @click="loadKeys" />
							<el-button icon="ele-DeleteFilled" circle plain type="danger" @click="clearCache" v-auth="'CenseqAdmin.SystemMonitor.Cache.Clear'" />
						</div>
					</div>
				</template>
				<el-input v-model="state.filter" placeholder="输入关键字过滤缓存" clearable prefix-icon="ele-Search" />
				<el-tree
					ref="treeRef"
					class="cache-tree"
					:data="filteredTree"
					node-key="id"
					:props="{ children: 'children', label: 'name' }"
					highlight-current
					default-expand-all
					@node-click="nodeClick"
				/>
			</el-card>

			<el-card shadow="hover" class="cache-value-card" v-loading="state.loadingValue">
				<template #header>
					<div class="card-header">
						<span>{{ state.cacheKey ? `缓存数据 [${state.cacheKey}]` : '缓存数据' }}</span>
						<el-button
							icon="ele-Delete"
							type="danger"
							plain
							:disabled="!state.cacheKey"
							@click="deleteCache"
							v-auth="'CenseqAdmin.SystemMonitor.Cache.Delete'"
						>
							删除缓存
						</el-button>
					</div>
				</template>
				<el-empty v-if="!state.cacheKey" description="请选择左侧缓存键" />
				<pre v-else class="cache-value-pre">{{ formattedCacheValue }}</pre>
			</el-card>
		</div>
	</div>
</template>

<script setup lang="ts" name="platformCache">
import { computed, onMounted, reactive, ref } from 'vue';
import { ElMessage, ElMessageBox, ElTree } from 'element-plus';
import { useSystemMonitorApi } from '/@/api/apis';

interface CacheTreeNode {
	id: string;
	name: string;
	children?: CacheTreeNode[];
	disabled?: boolean;
}

const monitorApi = useSystemMonitorApi();
const treeRef = ref<InstanceType<typeof ElTree>>();

const state = reactive({
	loadingKeys: false,
	loadingValue: false,
	filter: '',
	cacheTree: [] as CacheTreeNode[],
	cacheKey: '',
	cacheValue: undefined as unknown,
});

const filteredTree = computed(() => {
	if (!state.filter) return state.cacheTree;
	const keyword = state.filter.toLowerCase();
	return state.cacheTree
		.map((group) => ({
			...group,
			children: group.children?.filter((item) => item.id.toLowerCase().includes(keyword)),
		}))
		.filter((group) => group.name.toLowerCase().includes(keyword) || (group.children?.length ?? 0) > 0);
});

const formattedCacheValue = computed(() => {
	if (typeof state.cacheValue === 'string') {
		return state.cacheValue;
	}
	return JSON.stringify(state.cacheValue, null, 2);
});

const buildTree = (keys: string[]) => {
	const groupMap = new Map<string, CacheTreeNode>();
	for (const key of keys) {
		const groupName = key.includes(':') ? key.split(':')[0] : 'default';
		if (!groupMap.has(groupName)) {
			groupMap.set(groupName, { id: `group:${groupName}`, name: groupName, disabled: true, children: [] });
		}
		groupMap.get(groupName)!.children!.push({
			id: key,
			name: groupName === 'default' ? key : key.slice(groupName.length + 1),
		});
	}
	return Array.from(groupMap.values()).sort((a, b) => a.name.localeCompare(b.name));
};

const normalizeValue = (value: unknown) => {
	if (typeof value !== 'string') return value;
	try {
		return JSON.parse(value);
	} catch {
		return value;
	}
};

const loadKeys = async () => {
	state.loadingKeys = true;
	try {
		const data = await monitorApi.getCacheKeys();
		state.cacheTree = buildTree(data.items ?? []);
		if (!state.cacheTree.length) {
			state.cacheKey = '';
			state.cacheValue = undefined;
		}
	} finally {
		state.loadingKeys = false;
	}
};

const nodeClick = async (node: CacheTreeNode) => {
	if (node.disabled || node.children?.length) return;
	state.cacheKey = node.id;
	state.loadingValue = true;
	try {
		state.cacheValue = normalizeValue(await monitorApi.getCacheValue(node.id));
	} finally {
		state.loadingValue = false;
	}
};

const deleteCache = async () => {
	if (!state.cacheKey) return;
	await ElMessageBox.confirm(`确认删除缓存 [${state.cacheKey}]？`, '提示', {
		confirmButtonText: '确认',
		cancelButtonText: '取消',
		type: 'warning',
	});
	await monitorApi.deleteCache(state.cacheKey);
	ElMessage.success('删除成功');
	state.cacheKey = '';
	state.cacheValue = undefined;
	await loadKeys();
};

const clearCache = async () => {
	await ElMessageBox.confirm('确认清空当前可枚举的本机缓存？', '提示', {
		confirmButtonText: '确认',
		cancelButtonText: '取消',
		type: 'warning',
	});
	await monitorApi.clearCache();
	ElMessage.success('清空成功');
	state.cacheKey = '';
	state.cacheValue = undefined;
	await loadKeys();
};

onMounted(loadKeys);
</script>

<style scoped lang="scss">
.platform-cache-container {
	min-height: 100%;
}

.cache-shell {
	display: grid;
	grid-template-columns: minmax(260px, 0.35fr) minmax(0, 1fr);
	gap: 12px;
	height: 100%;
	min-height: 560px;
}

.card-header,
.header-actions {
	display: flex;
	align-items: center;
	justify-content: space-between;
	gap: 10px;
	font-weight: 600;
}

.cache-list-card,
.cache-value-card {
	min-height: 0;

	:deep(.el-card__body) {
		height: calc(100% - 58px);
		overflow: auto;
	}
}

.cache-tree {
	margin-top: 12px;
}

.cache-value-pre {
	min-height: 100%;
	margin: 0;
	padding: 12px;
	border: 1px solid var(--el-border-color-lighter);
	border-radius: 8px;
	background: var(--el-fill-color-lighter);
	color: var(--el-text-color-primary);
	font-family: Consolas, 'Courier New', monospace;
	font-size: 13px;
	line-height: 1.6;
	white-space: pre-wrap;
	word-break: break-word;
}

@media (max-width: 960px) {
	.cache-shell {
		grid-template-columns: minmax(0, 1fr);
	}
}
</style>
