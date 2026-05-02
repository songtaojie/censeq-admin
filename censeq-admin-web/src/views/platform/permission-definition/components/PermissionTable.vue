<template>
	<div class="permission-table-container">
		<div class="table-header">
			<span class="table-title">
				权限项列表
				<el-tag v-if="props.groupName" type="info" size="small" style="margin-left: 8px">{{ props.groupName }}</el-tag>
			</span>
		</div>
		<el-table
			:data="state.permissions"
			v-loading="state.loading"
			border
			style="width: 100%"
			empty-text="请先选择左侧权限组"
		>
			<el-table-column prop="name" label="名称" min-width="200" show-overflow-tooltip />
			<el-table-column prop="displayName" label="显示名称" min-width="160" show-overflow-tooltip />
			<el-table-column prop="parentName" label="父权限" min-width="160" show-overflow-tooltip>
				<template #default="{ row }">
					<span>{{ row.parentName || '-' }}</span>
				</template>
			</el-table-column>
			<el-table-column prop="isEnabled" label="是否启用" width="90" align="center">
				<template #default="{ row }">
					<el-tag :type="row.isEnabled ? 'success' : 'danger'" size="small">
						{{ row.isEnabled ? '启用' : '禁用' }}
					</el-tag>
				</template>
			</el-table-column>
			<el-table-column label="操作" width="80" align="center">
				<template #default="{ row }">
					<el-button type="primary" link size="small" @click="onEdit(row)">编辑</el-button>
				</template>
			</el-table-column>
		</el-table>

		<EditDisplayNameDialog
			v-model:visible="state.dialog.visible"
			:title="`编辑权限项显示名称【${state.dialog.permissionName}】`"
			:current-value="state.dialog.currentDisplayName"
			@confirm="onDialogConfirm"
			@cancel="onDialogCancel"
		/>
	</div>
</template>

<script setup lang="ts" name="PermissionTable">
import { reactive, watch } from 'vue';
import { ElMessage } from 'element-plus';
import type { PermissionDefinitionDto } from '/@/api/models/permission/definition';
import { usePermissionDefinitionApi } from '/@/api/apis';
import EditDisplayNameDialog from './EditDisplayNameDialog.vue';

const props = defineProps<{
	groupName: string;
}>();

const state = reactive({
	permissions: [] as PermissionDefinitionDto[],
	loading: false,
	dialog: {
		visible: false,
		permissionName: '',
		currentDisplayName: '',
	},
});

const loadPermissions = async () => {
	if (!props.groupName) {
		state.permissions = [];
		return;
	}
	state.loading = true;
	try {
		const { getPermissions } = usePermissionDefinitionApi();
		state.permissions = await getPermissions(props.groupName);
	} catch {
		ElMessage.error('加载权限项列表失败');
	} finally {
		state.loading = false;
	}
};

watch(() => props.groupName, loadPermissions, { immediate: true });

const onEdit = (row: PermissionDefinitionDto) => {
	state.dialog.permissionName = row.name;
	state.dialog.currentDisplayName = row.displayName;
	state.dialog.visible = true;
};

const onDialogConfirm = async (displayName: string) => {
	try {
		const { updatePermission } = usePermissionDefinitionApi();
		await updatePermission(state.dialog.permissionName, { displayName });
		ElMessage.success('更新成功');
		state.dialog.visible = false;
		await loadPermissions();
	} catch {
		ElMessage.error('更新失败');
	}
};

const onDialogCancel = () => {
	state.dialog.visible = false;
};
</script>

<style scoped lang="scss">
.permission-table-container {
	.table-header {
		display: flex;
		align-items: center;
		margin-bottom: 12px;

		.table-title {
			font-size: 14px;
			font-weight: 600;
			color: var(--el-text-color-primary);
			display: flex;
			align-items: center;
		}
	}
}
</style>
