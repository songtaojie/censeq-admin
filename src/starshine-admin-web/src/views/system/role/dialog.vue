<template>
	<div class="system-role-dialog-container">
		<el-dialog :title="state.dialog.title" v-model="state.dialog.isShowDialog" width="769px">
			<el-form ref="roleDialogFormRef" :model="state.ruleForm" size="default" label-width="90px">
				<el-row :gutter="35">
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="角色名称">
							<el-input v-model="state.ruleForm.name" placeholder="请输入角色名称" clearable></el-input>
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="是否默认">
							<el-switch v-model="state.ruleForm.isDefault" inline-prompt active-text="是" inactive-text="否" />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="是否公共">
							<el-switch v-model="state.ruleForm.isPublic" inline-prompt active-text="是" inactive-text="否" />
						</el-form-item>
					</el-col>
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
import { IdentityRoleDto, IdentityRoleCreateDto } from '/@/api/models/identity';
import { usePermissionApi, useIdentityApi } from '/@/api/apis';
import { useUserInfo } from '/@/composables/useUserInfo';
import { PermissionGroupDto, UpdatePermissionDto } from '/@/api/models/permission';
// 定义子组件向父组件传值/事件
const emit = defineEmits(['refresh']);
const treeRef = ref<InstanceType<typeof ElTree>>();
// 定义变量内容
const roleDialogFormRef = ref();
const state = reactive({
	ruleForm: {
		isDefault: false,
		isPublic: false,
		isStatic: false,
	} as IdentityRoleDto,
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
	const { createRole } = useIdentityApi();
	const { updatePermission } = usePermissionApi();
	roleDialogFormRef.value.validate(async (valid: boolean) => {
		if (!valid) return;
		var data = await createRole({
			name: state.ruleForm.name,
			isDefault: state.ruleForm.isDefault,
			isPublic: state.ruleForm.isPublic,
		});
		if (data.id) {
			const updatePermissions: UpdatePermissionDto[] = state.menuData.flatMap((group) =>
				group.permissions.map((p) => ({
					name: p.name,
					isGranted: p.isGranted,
				}))
			);
			updatePermission('R', data.id, {
				permissions: updatePermissions,
			});
		}
		closeDialog();
		emit('refresh');
	});
};
// 获取菜单结构数据
const getMenuData = async () => {
	const { userInfos } = useUserInfo();
	const { getPermissionList } = usePermissionApi();
	var permissionList = await getPermissionList('R', '');
	state.menuData = permissionList.groups;
	treeRef.value?.setCheckedKeys([]); // 清空选中值
	var selectNames = extractGrantedPermissionNames(state.menuData);
	treeRef.value?.setCheckedKeys(selectNames);
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
