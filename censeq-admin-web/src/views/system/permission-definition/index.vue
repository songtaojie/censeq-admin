<template>
	<div class="permission-definition-container">
		<el-row :gutter="16">
			<el-col :span="8">
				<el-card shadow="never">
					<GroupTable @select="onGroupSelect" />
				</el-card>
			</el-col>
			<el-col :span="16">
				<el-card shadow="never">
					<PermissionTable :group-name="state.selectedGroupName" />
				</el-card>
			</el-col>
		</el-row>
	</div>
</template>

<script setup lang="ts" name="permissionDefinition">
import { reactive } from 'vue';
import GroupTable from './components/GroupTable.vue';
import PermissionTable from './components/PermissionTable.vue';
import type { PermissionGroupDefinitionDto } from '/@/api/models/permission/definition';

const state = reactive({
	selectedGroupName: '',
});

const onGroupSelect = (group: PermissionGroupDefinitionDto) => {
	state.selectedGroupName = group.name;
};
</script>

<style scoped lang="scss">
.permission-definition-container {
	padding: 16px;
}
</style>
