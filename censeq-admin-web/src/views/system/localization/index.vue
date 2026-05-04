<template>
	<div class="localization-container layout-padding">
		<!-- 筛选栏 -->
		<el-card shadow="hover" :body-style="{ paddingBottom: '0' }">
			<el-form :model="state" ref="queryFormRef" :inline="true">
				<el-form-item label="资源">
					<el-select
						v-model="state.selectedResource"
						placeholder="选择资源"
						filterable
						clearable
						style="width: 220px"
						@change="onResourceChange"
					>
						<el-option
							v-for="r in state.resources"
							:key="r.name"
							:label="r.displayName ? `${r.displayName}（${r.name}）` : r.name"
							:value="r.name"
						/>
					</el-select>
				</el-form-item>
				<el-form-item label="语言">
					<el-select
						v-model="state.selectedCulture"
						placeholder="选择语言"
						filterable
						clearable
						style="width: 200px"
						@change="onCultureChange"
					>
						<el-option
							v-for="c in state.cultures"
							:key="c.cultureName"
							:label="`${c.displayName}（${c.cultureName}）`"
							:value="c.cultureName"
						/>
					</el-select>
				</el-form-item>
				<el-form-item label="关键字">
					<el-input
						v-model="state.filter"
						placeholder="搜索 Key 或翻译值"
						clearable
						style="width: 220px"
						@keyup.enter="onQuery"
						@clear="onQuery"
					/>
				</el-form-item>
				<el-form-item>
					<el-button-group>
						<el-button type="primary" icon="ele-Search" @click="onQuery">查询</el-button>
						<el-button icon="ele-Refresh" @click="onReset">重置</el-button>
					</el-button-group>
				</el-form-item>
				<el-form-item>
					<el-button
						type="primary"
						icon="ele-Plus"
						:disabled="!state.selectedResource || !state.selectedCulture"
						@click="onAdd"
					>新增</el-button>
				</el-form-item>
			</el-form>
		</el-card>

		<!-- 数据表格 -->
		<el-card class="full-table" shadow="hover" style="margin-top: 5px">
			<el-table
				:data="state.tableData"
				v-loading="state.loading"
				style="width: 100%"
				border
				stripe
				highlight-current-row
			>
				<el-table-column type="index" label="序号" width="60" align="center" fixed />
				<el-table-column prop="resourceName" label="资源" width="180" show-overflow-tooltip>
					<template #default="{ row }">
						<el-tag size="small" effect="plain">{{ resourceDisplayName(row.resourceName) }}</el-tag>
					</template>
				</el-table-column>
				<el-table-column prop="cultureName" label="语言" width="120" align="center" show-overflow-tooltip>
					<template #default="{ row }">
						<el-tag size="small" type="success" effect="light">{{ row.cultureName }}</el-tag>
					</template>
				</el-table-column>
				<el-table-column prop="key" label="Key" min-width="220" show-overflow-tooltip />
				<el-table-column prop="value" label="翻译值" min-width="260" show-overflow-tooltip>
					<template #default="{ row }">
						<span v-if="row.value">{{ row.value }}</span>
						<el-text v-else type="info" size="small">— 回退到 JSON 默认值</el-text>
					</template>
				</el-table-column>
				<el-table-column label="最后修改" width="155" align="center" show-overflow-tooltip>
					<template #default="{ row }">
						<el-text v-if="row.lastModificationTime" size="small">
							{{ row.lastModificationTime.substring(0, 16).replace('T', ' ') }}
						</el-text>
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
			<el-pagination
				class="pagination"
				background
				size="small"
				:pager-count="5"
				layout="total, sizes, prev, pager, next, jumper"
				:page-sizes="[20, 50, 100]"
				v-model:current-page="state.pageNum"
				v-model:page-size="state.pageSize"
				:total="state.total"
				@size-change="onHandleSizeChange"
				@current-change="onHandleCurrentChange"
			/>
		</el-card>

		<EditDialog ref="editDialogRef" @refresh="loadTexts" />
	</div>
</template>

<script setup lang="ts" name="platformLocalization">
import { defineAsyncComponent, reactive, ref, onMounted } from 'vue';
import { ElMessageBox, ElMessage } from 'element-plus';
import { useLocalizationManagementApi } from '/@/api/apis';
import type { LocalizationResourceDto, LocalizationCultureDto, LocalizationTextDto } from '/@/api/models/localization-management';

const EditDialog = defineAsyncComponent(() => import('./edit-dialog.vue'));

const localizationApi = useLocalizationManagementApi();
const editDialogRef = ref();
const queryFormRef = ref();

const state = reactive({
	resources: [] as LocalizationResourceDto[],
	cultures: [] as LocalizationCultureDto[],
	selectedResource: '' as string,
	selectedCulture: '' as string,
	filter: '',
	tableData: [] as LocalizationTextDto[],
	loading: false,
	pageNum: 1,
	pageSize: 20,
	total: 0,
});

onMounted(async () => {
	const [resources, cultures] = await Promise.all([
		localizationApi.getResources(),
		localizationApi.getCultures(),
	]);
	state.resources = resources;
	state.cultures = cultures;
	if (resources.length > 0) state.selectedResource = resources[0].name;
	if (cultures.length > 0) state.selectedCulture = cultures[0].cultureName;
	if (state.selectedResource && state.selectedCulture) await loadTexts();
});

async function loadTexts() {
	if (!state.selectedResource || !state.selectedCulture) return;
	state.loading = true;
	try {
		const result = await localizationApi.getTexts({
			resourceName: state.selectedResource,
			cultureName: state.selectedCulture,
			filter: state.filter || undefined,
			skipCount: (state.pageNum - 1) * state.pageSize,
			maxResultCount: state.pageSize,
			sorting: 'key',
		});
		state.tableData = result.items;
		state.total = result.totalCount;
	} finally {
		state.loading = false;
	}
}

function onQuery() {
	state.pageNum = 1;
	loadTexts();
}

function onReset() {
	state.selectedResource = state.resources[0]?.name ?? '';
	state.selectedCulture = state.cultures[0]?.cultureName ?? '';
	state.filter = '';
	onQuery();
}

function onResourceChange() { onQuery(); }
function onCultureChange() { onQuery(); }

function resourceDisplayName(name: string) {
	const r = state.resources.find(x => x.name === name);
	return r?.displayName || name;
}

function onHandleSizeChange(val: number) {
	state.pageSize = val;
	loadTexts();
}

function onHandleCurrentChange(val: number) {
	state.pageNum = val;
	loadTexts();
}

function onAdd() {
	editDialogRef.value.open(state.selectedResource, state.selectedCulture);
}

function onEdit(row: LocalizationTextDto) {
	editDialogRef.value.openEdit(row);
}

async function onDelete(row: LocalizationTextDto) {
	try {
		await ElMessageBox.confirm(`确定删除翻译条目 "${row.key}" 吗？`, '提示', {
			confirmButtonText: '确定',
			cancelButtonText: '取消',
			type: 'warning',
		});
		await localizationApi.deleteText(row.id);
		ElMessage.success('删除成功');
		await loadTexts();
	} catch (e: any) {
		if (e !== 'cancel') ElMessage.error('删除失败');
	}
}
</script>
