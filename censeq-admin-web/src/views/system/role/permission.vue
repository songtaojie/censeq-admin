<template>
	<div class="role-permission-dialog-container">
		<el-dialog :title="`授权角色菜单【${state.roleName}】`" v-model="state.dialog.isShowDialog" width="600px" destroy-on-close>
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

		// 等待DOM更新后设置选中状态
		nextTick(() => {
			treeRef.value?.setCheckedKeys([]);
			const selectNames = extractGrantedPermissionNames(state.menuData);
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
	.menu-data-tree {
		width: 100%;
		border: 1px solid var(--el-border-color);
		border-radius: var(--el-input-border-radius, var(--el-border-radius-base));
		padding: 10px;
		max-height: 400px;
		overflow-y: auto;
	}
}
</style>
