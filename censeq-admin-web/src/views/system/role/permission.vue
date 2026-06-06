<template>
	<div class="role-permission-dialog-container">
		<el-drawer
			v-model="state.dialog.isShowDialog"
			direction="rtl"
			size="720px"
			destroy-on-close
			:close-on-click-modal="false"
			class="role-permission-drawer"
		>
			<template #header>
				<div class="drawer-title">
					<el-icon size="16"><ele-Menu /></el-icon>
					<span>角色菜单授权【{{ state.roleName }}】</span>
				</div>
			</template>
			<div class="drawer-content">
				<div class="dialog-intro">
					为当前角色配置可访问的菜单权限。树形勾选会同步影响角色可见菜单，保存后立即生效。
				</div>
				<PermissionTree
					ref="permTreeRef"
					v-model="checkedPermissions"
					:data="state.menuData"
					:loading="state.loading"
					:show-summary="true"
					:referenced-names="referencedPermissionNames"
				/>
			</div>
			<template #footer>
				<span class="drawer-footer">
					<el-button @click="onCancel" size="default">取 消</el-button>
					<el-button type="primary" @click="onSubmit" size="default" :loading="state.submitLoading">
						保 存
					</el-button>
				</span>
			</template>
		</el-drawer>
	</div>
</template>

<script setup lang="ts" name="rolePermissionDialog">
import { reactive, ref } from 'vue';
import { ElMessage } from 'element-plus';
import { IdentityRoleDto } from '/@/api/models/identity';
import { usePermissionApi, useTenantApi } from '/@/api/apis';
import { useMenuApi } from '/@/api/apis';
import { PermissionGroupDto, UpdatePermissionDto } from '/@/api/models/permission';
import { useOidc } from '/@/composables/useOidc';
import PermissionTree from '/@/components/PermissionTree/index.vue';

const permTreeRef = ref<InstanceType<typeof PermissionTree>>();
const checkedPermissions = ref<string[]>([]);

const state = reactive({
	roleKey: '',
	roleName: '',
	menuData: [] as PermissionGroupDto[],
	dialog: {
		isShowDialog: false,
	},
	loading: false,
	submitLoading: false,
});

const referencedPermissionNames = ref<Set<string>>(new Set());

// 打开弹窗
const openDialog = (row: IdentityRoleDto) => {
	state.roleKey = row.id!;
	state.roleName = row.name!;
	state.dialog.isShowDialog = true;
	getMenuData();
};

// 关闭弹窗
const closeDialog = () => {
	state.dialog.isShowDialog = false;
	state.roleKey = '';
	state.roleName = '';
	state.menuData = [];
	checkedPermissions.value = [];
	referencedPermissionNames.value = new Set();
};

const onCancel = () => closeDialog();

// 提交
const onSubmit = async () => {
	if (!state.roleKey) return;
	state.submitLoading = true;
	try {
		const { updatePermission } = usePermissionApi();
		const names = permTreeRef.value?.getGrantedNames() ?? checkedPermissions.value;

		// 以完整 menuData 为基准，构建全量 isGranted 列表
		const grantedSet = new Set(names);
		const updatePayload: UpdatePermissionDto[] = state.menuData.flatMap((group) =>
			group.permissions.map((p) => ({
				name: p.name,
				isGranted: grantedSet.has(p.name),
			}))
		);

		await updatePermission('R', state.roleKey, { permissions: updatePayload });
		ElMessage.success('授权成功');
		closeDialog();
	} finally {
		state.submitLoading = false;
	}
};

// 获取权限数据，并在租户上下文时过滤为租户已被授权的范围
const getMenuData = async () => {
	if (!state.roleKey) return;
	state.loading = true;
	try {
		const { getPermissionList } = usePermissionApi();
		const menuApi = useMenuApi();

		const { getCurrentTenantId } = useOidc();
		const tenantId = await getCurrentTenantId();

		const requests: Promise<any>[] = [
			getPermissionList('R', state.roleKey),
			menuApi.getReferencedPermissionNames(),
		];
		// 租户上下文：额外加载当前租户被平台授权的权限范围
		if (tenantId) {
			const { getPermissions } = useTenantApi();
			requests.push(getPermissions(tenantId));
		}

		const [permissionList, referencedResult, tenantGranted] = await Promise.all(requests);

		referencedPermissionNames.value = new Set(referencedResult.items ?? []);

		let groups: PermissionGroupDto[] = permissionList.groups;

		// 租户侧：过滤掉租户未被授权的权限项（隐藏超出范围的权限，防止误配置）
		if (tenantId && tenantGranted) {
			const allowedSet = new Set<string>(tenantGranted as string[]);
			groups = groups
				.map((group) => ({
					...group,
					permissions: group.permissions.filter((p) => allowedSet.has(p.name)),
				}))
				.filter((group) => group.permissions.length > 0);
		}

		state.menuData = groups;
		checkedPermissions.value = extractGrantedPermissionNames(groups);
	} finally {
		state.loading = false;
	}
};

function extractGrantedPermissionNames(data: PermissionGroupDto[]): string[] {
	return data.flatMap((group) =>
		group.permissions.filter((p) => p.isGranted).map((p) => p.name)
	);
}

defineExpose({ openDialog });
</script>

<style scoped lang="scss">
.role-permission-dialog-container {
	:deep(.role-permission-drawer) {
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
		border-radius: 12px;
		background: var(--el-fill-color-light);
		color: var(--el-text-color-secondary);
		line-height: 1.7;
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
