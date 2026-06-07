<template>
	<div class="tenant-permission-drawer-container">
		<el-drawer
			v-model="visible"
			direction="rtl"
			size="720px"
			destroy-on-close
			:close-on-click-modal="false"
			class="tenant-permission-drawer"
		>
			<template #header>
				<div class="drawer-title">
					<el-icon size="16"><ele-Key /></el-icon>
					<span>租户权限配置【{{ tenantName }}】</span>
				</div>
			</template>

			<div class="drawer-content">
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
			</div>

			<template #footer>
				<span class="drawer-footer">
					<el-button @click="visible = false" size="default">取 消</el-button>
					<el-button type="primary" :loading="submitLoading" @click="onSubmit" size="default">保 存</el-button>
				</span>
			</template>
		</el-drawer>
	</div>
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

		const allPermissions = (
			await Promise.all(groups.map((group: PermissionGroupDefinitionDto) => getPermDefs(group.name)))
		).flat();

		permissionGroups.value = buildTenantScopeGroup(allPermissions, tenantScopeSet);
		checkedPermissions.value = granted;
	} finally {
		loading.value = false;
	}
}

function buildTenantScopeGroup(perms: PermissionDefinitionDto[], tenantScopeSet: Set<string>): PermissionGroupDto[] {
	const permMap = new Map<string, PermNode>();
	const rootPerms: PermNode[] = [];

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
		permMap.set(p.name, {
			name: p.name,
			displayName: p.displayName,
			parentName: p.parentName,
			isGranted: false,
			allowedProviders: [],
			grantedProviders: [],
			permissions: [],
		});
	}

	for (const node of permMap.values()) {
		if (node.parentName && permMap.has(node.parentName)) {
			permMap.get(node.parentName)!.permissions.push(node);
		} else {
			rootPerms.push(node);
		}
	}

	if (rootPerms.length === 0) {
		return [];
	}

	return [{
		name: '__tenant_scope_permissions__',
		displayName: '租户可用权限',
		permissions: rootPerms,
	}];
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
.tenant-permission-drawer-container {
	:deep(.tenant-permission-drawer) {
		max-width: 92vw;
	}

	:deep(.el-drawer__header) {
		margin-bottom: 0;
		padding: 16px 20px;
		border-bottom: 1px solid var(--el-border-color-lighter);
		color: var(--el-text-color-primary);
	}

	:deep(.el-drawer__body) {
		display: flex;
		flex-direction: column;
		padding: 18px 20px;
		overflow: hidden;
	}

	:deep(.el-drawer__footer) {
		padding: 14px 20px;
		border-top: 1px solid var(--el-border-color-lighter);
	}

	.drawer-title {
		display: inline-flex;
		align-items: center;
		gap: 6px;
		font-weight: 600;
	}

	.drawer-content {
		display: flex;
		min-height: 0;
		flex: 1;
		flex-direction: column;
	}

	.dialog-intro {
		margin-bottom: 12px;
		padding: 12px 14px;
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

	:deep(.permission-tree-wrapper) {
		display: flex;
		min-height: 0;
		flex: 1;
		flex-direction: column;
	}

	:deep(.permission-tree) {
		flex: 1;
		max-height: none;
	}

	.drawer-footer {
		display: flex;
		justify-content: flex-end;
		gap: 10px;
	}
}
</style>
