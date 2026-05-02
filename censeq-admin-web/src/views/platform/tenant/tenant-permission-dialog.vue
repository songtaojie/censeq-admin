<template>
	<el-dialog
		v-model="visible"
		width="760px"
		destroy-on-close
		draggable
		:close-on-click-modal="false"
	>
		<template #header>
			<div style="color: #fff">
				<el-icon size="16" style="margin-right: 4px; vertical-align: middle"><ele-Key /></el-icon>
				<span>租户权限配置【{{ tenantName }}】</span>
			</div>
		</template>

		<div class="dialog-intro">
			配置平台向该租户开放的权限范围。租户管理员只能将范围内的权限分配给其角色，保存后立即生效。
		</div>

		<div class="toolbar">
			<el-button size="small" @click="onCheckAll">全选</el-button>
			<el-button size="small" @click="onUncheckAll">清空</el-button>
		</div>

		<PermissionTree
			ref="treeRef"
			v-model="checkedPermissions"
			:data="permissionGroups"
			:loading="loading"
			:show-summary="true"
		/>

		<template #footer>
			<el-button @click="visible = false">取 消</el-button>
			<el-button type="primary" :loading="submitLoading" @click="onSubmit">保 存</el-button>
		</template>
	</el-dialog>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { ElMessage } from 'element-plus';
import PermissionTree from '/@/components/PermissionTree/index.vue';
import { usePermissionDefinitionApi, useTenantApi, useMenuApi } from '/@/api/apis';
import type { PermissionGroupDto, PermissionGrantInfoDto } from '/@/api/models/permission';
import type { PermissionGroupDefinitionDto, PermissionDefinitionDto } from '/@/api/models/permission/definition';

// PermissionGrantInfoDto 扩展：支持子级树节点
interface PermNode extends PermissionGrantInfoDto {
	permissions: PermNode[];
}

const emit = defineEmits<{ (e: 'saved'): void }>();

const visible = ref(false);
const loading = ref(false);
const submitLoading = ref(false);

const tenantId = ref('');
const tenantName = ref('');
const permissionGroups = ref<PermissionGroupDto[]>([]);
const checkedPermissions = ref<string[]>([]);

const treeRef = ref<InstanceType<typeof PermissionTree>>();

// ── 公开 ──────────────────────────────────────────────────────────────────
async function open(id: string, name: string) {
	tenantId.value = id;
	tenantName.value = name;
	visible.value = true;
	await loadData();
}

defineExpose({ open });

// ── 数据加载 ──────────────────────────────────────────────────────────────
async function loadData() {
	loading.value = true;
	try {
		const { getGroups, getPermissions: getPermDefs } = usePermissionDefinitionApi();
		const { getPermissions } = useTenantApi();
		const { getTenantScopePermissionNames } = useMenuApi();

		// 并发加载：权限分组定义 + 该租户当前授权 + 租户作用域菜单引用的权限集合
		const [groups, granted, scopeResult] = await Promise.all([
			getGroups(),
			getPermissions(tenantId.value),
			getTenantScopePermissionNames(),
		]);

		// 只展示被租户菜单引用过的权限（过滤掉平台专属权限如 TenantManagement）
		const tenantScopeSet = new Set<string>(scopeResult.items ?? []);

		// 逐组加载权限定义，构建 PermissionGroupDto[] 树结构
		const fullGroups = await Promise.all(
			groups.map(async (group: PermissionGroupDefinitionDto) => {
				const perms: PermissionDefinitionDto[] = await getPermDefs(group.name);
				// 构建层级树：先处理根权限，再挂子权限
				const permMap = new Map<string, PermNode>();
				const rootPerms: PermNode[] = [];

				// 过滤：若 tenantScopeSet 非空，只保留租户作用域菜单引用的权限及其子孙节点
				// 判断标准：当前节点或任意祖先节点在 tenantScopeSet 中
				const filteredPerms = tenantScopeSet.size > 0
					? perms.filter((p) => {
						let cur: PermissionDefinitionDto | undefined = p;
						while (cur) {
							if (tenantScopeSet.has(cur.name)) return true;
							cur = cur.parentName ? perms.find((x) => x.name === cur!.parentName) : undefined;
						}
						return false;
					})
					: perms;

				for (const p of filteredPerms) {
					const node: PermNode = {
						name: p.name,
						displayName: p.displayName,
						parentName: p.parentName,
						isGranted: false,
						allowedProviders: [],
						grantedProviders: [],
						permissions: [],
					};
					permMap.set(p.name, node);
				}

				for (const node of permMap.values()) {
					if (node.parentName && permMap.has(node.parentName)) {
						permMap.get(node.parentName)!.permissions.push(node);
					} else {
						rootPerms.push(node);
					}
				}

				return {
					name: group.name,
					displayName: group.displayName,
					permissions: rootPerms,
				} as PermissionGroupDto;
			})
		);

		permissionGroups.value = fullGroups.filter((g) => g.permissions.length > 0);
		checkedPermissions.value = granted;
	} finally {
		loading.value = false;
	}
}

// ── 操作 ──────────────────────────────────────────────────────────────────
function onCheckAll() {
	treeRef.value?.checkAll();
}

function onUncheckAll() {
	treeRef.value?.uncheckAll();
}

async function onSubmit() {
	submitLoading.value = true;
	try {
		const { updatePermissions } = useTenantApi();
		const names = treeRef.value?.getGrantedNames() ?? checkedPermissions.value;
		await updatePermissions(tenantId.value, names);
		ElMessage.success('权限配置已保存');
		visible.value = false;
		emit('saved');
	} finally {
		submitLoading.value = false;
	}
}
</script>

<style scoped lang="scss">
.dialog-intro {
	margin-bottom: 12px;
	padding: 10px 14px;
	border-radius: 8px;
	background: var(--el-fill-color-light);
	color: var(--el-text-color-secondary);
	line-height: 1.7;
	font-size: 13px;
}

.toolbar {
	display: flex;
	gap: 8px;
	margin-bottom: 10px;
}
</style>
