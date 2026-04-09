<template>
	<div class="role-permission-dialog-container">
		<el-dialog :title="`角色菜单授权【${state.roleName}】`" v-model="state.dialog.isShowDialog" width="720px" destroy-on-close>
			<div class="dialog-intro">
				为当前角色配置可访问的菜单权限。树形勾选会同步影响角色可见菜单，保存后立即生效。
			</div>
			<div class="permission-summary">
				<el-tag type="primary">角色：{{ state.roleName || '-' }}</el-tag>
				<el-tag type="success">已授权 {{ grantedCount }} 项</el-tag>
				<el-tag type="info">总权限 {{ permissionCount }} 项</el-tag>
			</div>
			<el-tree
				ref="treeRef"
				node-key="name"
				icon="ele-Menu"
				:data="state.menuData"
				:props="state.menuProps"
				show-checkbox
				class="menu-data-tree"
				v-loading="state.loading"
			/>
			<template #footer>
				<span class="dialog-footer">
					<el-button @click="onCancel" size="default">取 消</el-button>
					<el-button type="primary" @click="onSubmit" size="default" :loading="state.submitLoading">
						保 存
					</el-button>
				</span>
			</template>
		</el-dialog>
	</div>
</template>

<script setup lang="ts" name="rolePermissionDialog">
import { reactive, ref, nextTick } from 'vue';
import type { ElTree } from 'element-plus';
import { ElMessage } from 'element-plus';
import { IdentityRoleDto } from '/@/api/models/identity';
import { usePermissionApi } from '/@/api/apis';
import { PermissionGroupDto, UpdatePermissionDto } from '/@/api/models/permission';

const treeRef = ref<InstanceType<typeof ElTree>>();

const state = reactive({
	roleId: '',
	roleName: '',
	menuData: [] as PermissionGroupDto[],
	menuProps: {
		children: 'permissions',
		label: 'displayName',
	},
	dialog: {
		isShowDialog: false,
	},
	loading: false,
	submitLoading: false,
});

const permissionCount = ref(0);
const grantedCount = ref(0);

// 打开弹窗
const openDialog = (row: IdentityRoleDto) => {
	state.roleId = row.id!;
	state.roleName = row.name!;
	state.dialog.isShowDialog = true;
	getMenuData();
};

// 关闭弹窗
const closeDialog = () => {
	state.dialog.isShowDialog = false;
	state.menuData = [];
	permissionCount.value = 0;
	grantedCount.value = 0;
};

// 取消
const onCancel = () => {
	closeDialog();
};

// 提交
const onSubmit = async () => {
	if (!state.roleId) return;

	state.submitLoading = true;
	try {
		const { updatePermission } = usePermissionApi();
		const checkedKeys = treeRef.value?.getCheckedKeys() || [];
		const halfCheckedKeys = treeRef.value?.getHalfCheckedKeys() || [];

		// 合并选中和半选中的节点
		const allGrantedKeys = [...checkedKeys, ...halfCheckedKeys];

		// 构建权限更新数据
		const updatePermissions: UpdatePermissionDto[] = state.menuData.flatMap((group) =>
			group.permissions.map((p) => ({
				name: p.name,
				isGranted: allGrantedKeys.includes(p.name),
			}))
		);

		await updatePermission('R', state.roleId, { permissions: updatePermissions });
		ElMessage.success('授权成功');
		closeDialog();
	} finally {
		state.submitLoading = false;
	}
};

// 获取菜单结构数据
const getMenuData = async () => {
	if (!state.roleId) return;

	state.loading = true;
	try {
		const { getPermissionList } = usePermissionApi();
		const permissionList = await getPermissionList('R', state.roleId);
		state.menuData = permissionList.groups;
		permissionCount.value = state.menuData.reduce((total, group) => total + group.permissions.length, 0);

		// 等待DOM更新后设置选中状态
		nextTick(() => {
			treeRef.value?.setCheckedKeys([]);
			const selectNames = extractGrantedPermissionNames(state.menuData);
			grantedCount.value = selectNames.length;
			treeRef.value?.setCheckedKeys(selectNames);
		});
	} finally {
		state.loading = false;
	}
};

function extractGrantedPermissionNames(data: PermissionGroupDto[]): string[] {
	const grantedNames: string[] = [];

	for (const group of data) {
		for (const permission of group.permissions) {
			if (permission.isGranted) {
				grantedNames.push(permission.name);
			}
		}
	}

	return grantedNames;
}

// 暴露变量
defineExpose({
	openDialog,
});
</script>

<style scoped lang="scss">
.role-permission-dialog-container {
	.dialog-intro {
		margin-bottom: 12px;
		padding: 12px 14px;
		border-radius: 12px;
		background: var(--el-fill-color-light);
		color: var(--el-text-color-secondary);
		line-height: 1.7;
	}

	.permission-summary {
		display: flex;
		flex-wrap: wrap;
		gap: 10px;
		margin-bottom: 12px;
	}

	.menu-data-tree {
		width: 100%;
		border: 1px solid var(--el-border-color);
		border-radius: var(--el-input-border-radius, var(--el-border-radius-base));
		padding: 10px;
		max-height: 460px;
		overflow-y: auto;
	}
}
</style>
