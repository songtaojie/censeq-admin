<template>
	<div class="group-table-container">
		<div class="table-header">
			<span class="table-title">权限组列表</span>
		</div>
		<el-table
			ref="tableRef"
			:data="state.groups"
			v-loading="state.loading"
			highlight-current-row
			@row-click="onRowClick"
			border
			style="width: 100%"
		>
			<el-table-column prop="name" label="名称" min-width="160" show-overflow-tooltip />
			<el-table-column prop="displayName" label="显示名称" min-width="160" show-overflow-tooltip />
			<el-table-column label="操作" width="80" align="center">
				<template #default="{ row }">
					<el-button type="primary" link size="small" @click.stop="onEdit(row)">编辑</el-button>
				</template>
			</el-table-column>
		</el-table>

		<EditDisplayNameDialog
			v-model:visible="state.dialog.visible"
			:title="`编辑权限组显示名称【${state.dialog.groupName}】`"
			:current-value="state.dialog.currentDisplayName"
			@confirm="onDialogConfirm"
			@cancel="onDialogCancel"
		/>
	</div>
</template>

<script setup lang="ts" name="GroupTable">
import { reactive, onMounted } from 'vue';
import { ElMessage } from 'element-plus';
import type { PermissionGroupDefinitionDto } from '/@/api/models/permission/definition';
import { usePermissionDefinitionApi } from '/@/api/apis';
import EditDisplayNameDialog from './EditDisplayNameDialog.vue';

const emit = defineEmits<{
	(e: 'select', group: PermissionGroupDefinitionDto): void;
}>();

const state = reactive({
	groups: [] as PermissionGroupDefinitionDto[],
	loading: false,
	dialog: {
		visible: false,
		groupName: '',
		currentDisplayName: '',
	},
});

const loadGroups = async () => {
	state.loading = true;
	try {
		const { getGroups } = usePermissionDefinitionApi();
		state.groups = await getGroups();
	} catch {
		ElMessage.error('加载权限组列表失败');
	} finally {
		state.loading = false;
	}
};

const onRowClick = (row: PermissionGroupDefinitionDto) => {
	emit('select', row);
};

const onEdit = (row: PermissionGroupDefinitionDto) => {
	state.dialog.groupName = row.name;
	state.dialog.currentDisplayName = row.displayName;
	state.dialog.visible = true;
};

const onDialogConfirm = async (displayName: string) => {
	try {
		const { updateGroup } = usePermissionDefinitionApi();
		await updateGroup(state.dialog.groupName, { displayName });
		ElMessage.success('更新成功');
		state.dialog.visible = false;
		await loadGroups();
	} catch {
		ElMessage.error('更新失败');
	}
};

const onDialogCancel = () => {
	state.dialog.visible = false;
};

onMounted(() => {
	loadGroups();
});

defineExpose({ loadGroups });
</script>

<style scoped lang="scss">
.group-table-container {
	.table-header {
		display: flex;
		align-items: center;
		margin-bottom: 12px;

		.table-title {
			font-size: 14px;
			font-weight: 600;
			color: var(--el-text-color-primary);
		}
	}
}
</style>
