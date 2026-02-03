<template>
	<div class="system-role-dialog-container">
		<el-dialog :title="state.dialog.title" v-model="state.dialog.isShowDialog" width="769px">
			<el-form ref="roleDialogFormRef" :model="state.ruleForm" size="default" label-width="90px">
				<el-row :gutter="35">
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item label="菜单权限">
							<el-tree
								ref="treeRef"
								node-key="name"
								icon="ele-Menu"
								:data="state.menuData"
								:props="state.menuProps"
								show-checkbox
								class="menu-data-tree"
							/>
						</el-form-item>
					</el-col>
				</el-row>
			</el-form>
			<template #footer>
				<span class="dialog-footer">
					<el-button @click="onCancel" size="default">取 消</el-button>
					<el-button type="primary" @click="onSubmit" size="default">{{ state.dialog.submitTxt }}</el-button>
				</span>
			</template>
		</el-dialog>
	</div>
</template>

<script setup lang="ts" name="systemRoleDialog">
import { reactive, ref } from 'vue';
import type { ElTree } from 'element-plus';
import { IdentityRoleDto } from '/@/api/models/identity';
import { usePermissionApi } from '/@/api/apis';
import { useUserInfo } from '/@/composables/useUserInfo';
import { PermissionGroupDto } from '/@/api/models/permission';
// 定义子组件向父组件传值/事件
const emit = defineEmits(['refresh']);
const treeRef = ref<InstanceType<typeof ElTree>>();
// 定义变量内容
const roleDialogFormRef = ref();
const state = reactive({
	ruleForm: {} as IdentityRoleDto,
	menuData: [] as PermissionGroupDto[],
	menuProps: {
		children: 'permissions',
		label: 'displayName',
	},
	dialog: {
		isShowDialog: false,
		type: '',
		title: '',
		submitTxt: '',
	},
});

// 打开弹窗
const openDialog = (type: string, row: IdentityRoleDto) => {
	if (type === 'edit') {
		state.ruleForm = row;
		state.dialog.title = '修改角色';
		state.dialog.submitTxt = '修 改';
	} else {
		state.dialog.title = '新增角色';
		state.dialog.submitTxt = '新 增';
		// 清空表单，此项需加表单验证才能使用
		// nextTick(() => {
		// 	roleDialogFormRef.value.resetFields();
		// });
	}
	state.dialog.isShowDialog = true;
	getMenuData();
};
// 关闭弹窗
const closeDialog = () => {
	state.dialog.isShowDialog = false;
};
// 取消
const onCancel = () => {
	closeDialog();
};
// 提交
const onSubmit = () => {
	closeDialog();
	emit('refresh');
	// if (state.dialog.type === 'add') { }
};
// 获取菜单结构数据
const getMenuData = async () => {
	const { userInfos } = useUserInfo();
	const { getPermissionList } = usePermissionApi();
	var permissionList = await getPermissionList('R', userInfos.value.userName);
	state.menuData = permissionList.groups;
	treeRef.value?.setCheckedKeys([]); // 清空选中值
	var selectNames = extractGrantedPermissionNames(state.menuData);
	treeRef.value?.setCheckedKeys(selectNames);
	// state.menuData = [
	// 	{
	// 		id: 1,
	// 		label: '系统管理',
	// 		children: [
	// 			{
	// 				id: 11,
	// 				label: '菜单管理',
	// 				children: [
	// 					{
	// 						id: 111,
	// 						label: '菜单新增',
	// 					},
	// 					{
	// 						id: 112,
	// 						label: '菜单修改',
	// 					},
	// 					{
	// 						id: 113,
	// 						label: '菜单删除',
	// 					},
	// 					{
	// 						id: 114,
	// 						label: '菜单查询',
	// 					},
	// 				],
	// 			},
	// 			{
	// 				id: 12,
	// 				label: '角色管理',
	// 				children: [
	// 					{
	// 						id: 121,
	// 						label: '角色新增',
	// 					},
	// 					{
	// 						id: 122,
	// 						label: '角色修改',
	// 					},
	// 					{
	// 						id: 123,
	// 						label: '角色删除',
	// 					},
	// 					{
	// 						id: 124,
	// 						label: '角色查询',
	// 					},
	// 				],
	// 			},
	// 			{
	// 				id: 13,
	// 				label: '用户管理',
	// 				children: [
	// 					{
	// 						id: 131,
	// 						label: '用户新增',
	// 					},
	// 					{
	// 						id: 132,
	// 						label: '用户修改',
	// 					},
	// 					{
	// 						id: 133,
	// 						label: '用户删除',
	// 					},
	// 					{
	// 						id: 134,
	// 						label: '用户查询',
	// 					},
	// 				],
	// 			},
	// 		],
	// 	},
	// 	{
	// 		id: 2,
	// 		label: '权限管理',
	// 		children: [
	// 			{
	// 				id: 21,
	// 				label: '前端控制',
	// 				children: [
	// 					{
	// 						id: 211,
	// 						label: '页面权限',
	// 					},
	// 					{
	// 						id: 212,
	// 						label: '页面权限',
	// 					},
	// 				],
	// 			},
	// 			{
	// 				id: 22,
	// 				label: '后端控制',
	// 				children: [
	// 					{
	// 						id: 221,
	// 						label: '页面权限',
	// 					},
	// 				],
	// 			},
	// 		],
	// 	},
	// ];
};

function extractGrantedPermissionNames(data: PermissionGroupDto[]): string[] {
	const grantedNames = new Set<string>();

	for (const group of data) {
		let groupHasGranted = false;

		for (const permission of group.permissions) {
			if (permission.isGranted) {
				grantedNames.add(permission.name);
				groupHasGranted = true;
			}
		}

		if (groupHasGranted) {
			grantedNames.add(group.name);
		}
	}

	return Array.from(grantedNames);
}

// 暴露变量
defineExpose({
	openDialog,
});
</script>

<style scoped lang="scss">
.system-role-dialog-container {
	.menu-data-tree {
		width: 100%;
		border: 1px solid var(--el-border-color);
		border-radius: var(--el-input-border-radius, var(--el-border-radius-base));
		padding: 5px;
	}
}
</style>
