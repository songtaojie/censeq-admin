<template>
	<div class="resource-container layout-padding">
		<!-- 筛选栏 -->
		<el-card shadow="hover" :body-style="{ paddingBottom: '0' }">
			<el-form :model="state" :inline="true">
				<el-form-item label="关键字">
					<el-input
						v-model="state.filter"
						placeholder="搜索资源名称/显示名"
						clearable
						style="width: 220px"
						@clear="loadList"
						@keyup.enter="loadList"
					/>
				</el-form-item>
				<el-form-item>
					<el-button-group>
						<el-button type="primary" icon="ele-Search" @click="loadList">查询</el-button>
						<el-button icon="ele-Refresh" @click="onReset">重置</el-button>
					</el-button-group>
				</el-form-item>
				<el-form-item>
					<el-button type="primary" icon="ele-Plus" @click="onAdd">新增</el-button>
				</el-form-item>
			</el-form>
		</el-card>

		<!-- 数据表格 -->
		<el-card class="full-table" shadow="hover" style="margin-top: 5px">
			<el-table :data="filteredList" v-loading="state.loading" style="width: 100%" border stripe highlight-current-row>
				<el-table-column type="index" label="序号" width="60" align="center" fixed />
				<el-table-column prop="name" label="资源名称" min-width="200" show-overflow-tooltip>
					<template #default="{ row }">
						<el-tag size="small" effect="plain">{{ row.name }}</el-tag>
					</template>
				</el-table-column>
				<el-table-column prop="displayName" label="显示名称" min-width="200" show-overflow-tooltip />
				<el-table-column prop="defaultCultureName" label="默认语言" width="120" align="center" show-overflow-tooltip>
					<template #default="{ row }">
						<el-tag v-if="row.defaultCultureName" size="small" type="info" effect="light">{{ row.defaultCultureName }}</el-tag>
						<el-text v-else type="info" size="small">—</el-text>
					</template>
				</el-table-column>
				<el-table-column label="创建时间" width="155" align="center" show-overflow-tooltip>
					<template #default="{ row }">
						<el-text v-if="row.creationTime" size="small">{{ row.creationTime.substring(0, 16).replace('T', ' ') }}</el-text>
						<el-text v-else type="info" size="small">—</el-text>
					</template>
				</el-table-column>
				<el-table-column label="操作" width="130" fixed="right" align="center">
					<template #default="{ row }">
						<el-button icon="ele-Edit" size="small" text type="primary" @click="onEdit(row)">编辑</el-button>
						<el-button icon="ele-Delete" size="small" text type="danger" @click="onDelete(row)">删除</el-button>
					</template>
				</el-table-column>
			</el-table>
		</el-card>

		<!-- 弹窗 -->
		<ResourceDialog ref="dialogRef" @refresh="loadList" />
	</div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted } from 'vue';
import { ElMessage, ElMessageBox } from 'element-plus';
import { useLocalizationManagementApi } from '/@/api/apis';
import type { LocalizationResourceDto } from '/@/api/models/localization-management';
import ResourceDialog from './resource-dialog.vue';

defineOptions({ name: 'platformLocalizationResource' });

const localizationApi = useLocalizationManagementApi();

const state = reactive({
	filter: '',
	loading: false,
	list: [] as LocalizationResourceDto[],
});

const dialogRef = ref<InstanceType<typeof ResourceDialog>>();

const filteredList = computed(() => {
	if (!state.filter) return state.list;
	const kw = state.filter.toLowerCase();
	return state.list.filter(
		(r) => r.name.toLowerCase().includes(kw) || (r.displayName ?? '').toLowerCase().includes(kw)
	);
});

async function loadList() {
	state.loading = true;
	try {
		state.list = await localizationApi.getResources();
	} finally {
		state.loading = false;
	}
}

function onReset() {
	state.filter = '';
	loadList();
}

function onAdd() {
	dialogRef.value?.open();
}

function onEdit(row: LocalizationResourceDto) {
	dialogRef.value?.openEdit(row);
}

async function onDelete(row: LocalizationResourceDto) {
	await ElMessageBox.confirm(`确定删除资源 "${row.displayName || row.name}" 吗？`, '警告', {
		type: 'warning',
		confirmButtonText: '删 除',
		cancelButtonText: '取 消',
	});
	await localizationApi.deleteResource(row.id);
	ElMessage.success('删除成功');
	loadList();
}

onMounted(loadList);
</script>

<style scoped lang="scss">
.resource-container {
	:deep(.full-table) {
		.el-card__body {
			padding-bottom: 0;
		}
	}
}
</style>
