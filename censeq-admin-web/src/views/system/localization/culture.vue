<template>
	<div class="culture-container layout-padding">
		<!-- 筛选栏 -->
		<el-card shadow="hover" :body-style="{ paddingBottom: '0' }">
			<el-form :model="state" :inline="true">
				<el-form-item label="启用状态">
					<el-select v-model="state.filterEnabled" placeholder="全部" clearable style="width: 110px" @change="loadList">
						<el-option label="已启用" :value="true" />
						<el-option label="已禁用" :value="false" />
					</el-select>
				</el-form-item>
				<el-form-item label="关键字">
					<el-input
						v-model="state.filter"
						placeholder="搜索语言代码/显示名"
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
				<el-table-column prop="cultureName" label="语言代码" width="130" align="center" show-overflow-tooltip>
					<template #default="{ row }">
						<el-tag size="small" effect="plain">{{ row.cultureName }}</el-tag>
					</template>
				</el-table-column>
				<el-table-column prop="displayName" label="显示名称" min-width="180" show-overflow-tooltip />
				<el-table-column prop="uiCultureName" label="UI 语言代码" width="130" align="center" show-overflow-tooltip>
					<template #default="{ row }">
						<el-tag v-if="row.uiCultureName" size="small" type="info" effect="light">{{ row.uiCultureName }}</el-tag>
						<el-text v-else type="info" size="small">—</el-text>
					</template>
				</el-table-column>
				<el-table-column prop="isEnabled" label="状态" width="90" align="center" show-overflow-tooltip>
					<template #default="{ row }">
						<el-tag :type="row.isEnabled ? 'success' : 'danger'" size="small" effect="light">
							{{ row.isEnabled ? '启用' : '禁用' }}
						</el-tag>
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
		<CultureDialog ref="dialogRef" @refresh="loadList" />
	</div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted } from 'vue';
import { ElMessage, ElMessageBox } from 'element-plus';
import { useLocalizationManagementApi } from '/@/api/apis';
import type { LocalizationCultureDto } from '/@/api/models/localization-management';
import CultureDialog from './culture-dialog.vue';

defineOptions({ name: 'platformLocalizationCulture' });

const localizationApi = useLocalizationManagementApi();

const state = reactive({
	filter: '',
	filterEnabled: null as boolean | null,
	loading: false,
	list: [] as LocalizationCultureDto[],
});

const dialogRef = ref<InstanceType<typeof CultureDialog>>();

const filteredList = computed(() => {
	let result = state.list;
	if (state.filterEnabled !== null) {
		result = result.filter((c) => c.isEnabled === state.filterEnabled);
	}
	if (state.filter) {
		const kw = state.filter.toLowerCase();
		result = result.filter(
			(c) => c.cultureName.toLowerCase().includes(kw) || c.displayName.toLowerCase().includes(kw)
		);
	}
	return result;
});

async function loadList() {
	state.loading = true;
	try {
		state.list = await localizationApi.getCultures();
	} finally {
		state.loading = false;
	}
}

function onReset() {
	state.filter = '';
	state.filterEnabled = null;
	loadList();
}

function onAdd() {
	dialogRef.value?.open();
}

function onEdit(row: LocalizationCultureDto) {
	dialogRef.value?.openEdit(row);
}

async function onDelete(row: LocalizationCultureDto) {
	await ElMessageBox.confirm(`确定删除语言 "${row.displayName}（${row.cultureName}）" 吗？`, '警告', {
		type: 'warning',
		confirmButtonText: '删 除',
		cancelButtonText: '取 消',
	});
	await localizationApi.deleteCulture(row.id);
	ElMessage.success('删除成功');
	loadList();
}

onMounted(loadList);
</script>

<style scoped lang="scss">
.culture-container {
	:deep(.full-table) {
		.el-card__body {
			padding-bottom: 0;
		}
	}
}
</style>
