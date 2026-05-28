<template>
	<div class="system-file-container layout-padding">
		<el-card shadow="hover" :body-style="{ paddingBottom: '0' }">
			<el-form :model="state.queryParam" :inline="true">
				<el-form-item label="文件名">
					<el-input v-model="state.queryParam.filter" placeholder="文件名/原始名" clearable style="width: 180px" @keyup.enter="onQuery" />
				</el-form-item>
				<el-form-item label="分类">
					<el-input v-model="state.queryParam.category" placeholder="common/avatar..." clearable style="width: 150px" @keyup.enter="onQuery" />
				</el-form-item>
				<el-form-item>
					<el-button-group>
						<el-button type="primary" icon="ele-Search" @click="onQuery">查询</el-button>
						<el-button icon="ele-Refresh" @click="onReset">重置</el-button>
					</el-button-group>
				</el-form-item>
				<el-form-item>
					<el-upload :show-file-list="false" :http-request="uploadFile">
						<el-button type="primary" icon="ele-Upload">上传文件</el-button>
					</el-upload>
				</el-form-item>
			</el-form>
		</el-card>

		<el-card class="full-table" shadow="hover" style="margin-top: 5px">
			<el-table :data="state.tableData.data" v-loading="state.tableData.loading" style="width: 100%" border stripe>
				<el-table-column type="index" label="序号" width="60" align="center" fixed />
				<el-table-column label="预览" width="82" align="center">
					<template #default="{ row }">
						<el-image
							v-if="isImage(row)"
							:src="resolveFileUrl(row.url)"
							:preview-src-list="[resolveFileUrl(row.url)]"
							preview-teleported
							fit="cover"
							class="file-thumb"
						/>
						<el-icon v-else size="26" color="var(--el-color-info)"><ele-Document /></el-icon>
					</template>
				</el-table-column>
				<el-table-column prop="originalName" label="原始文件名" min-width="180" show-overflow-tooltip />
				<el-table-column prop="category" label="分类" width="110" show-overflow-tooltip />
				<el-table-column prop="contentType" label="类型" min-width="150" show-overflow-tooltip />
				<el-table-column label="大小" width="100" align="right">
					<template #default="{ row }">{{ formatSize(row.size) }}</template>
				</el-table-column>
				<el-table-column prop="url" label="访问地址" min-width="220" show-overflow-tooltip />
				<el-table-column label="上传时间" width="170" show-overflow-tooltip>
					<template #default="{ row }">{{ formatDate(row.creationTime) }}</template>
				</el-table-column>
				<el-table-column label="操作" width="180" fixed="right" align="center">
					<template #default="{ row }">
						<el-button icon="ele-View" size="small" text type="primary" @click="openFile(row)">查看</el-button>
						<el-button icon="ele-Delete" size="small" text type="danger" @click="deleteFile(row)">删除</el-button>
					</template>
				</el-table-column>
			</el-table>
			<el-pagination
				@size-change="onHandleSizeChange"
				@current-change="onHandleCurrentChange"
				class="pagination"
				:pager-count="5"
				:page-sizes="[10, 20, 50]"
				v-model:current-page="state.tableData.param.pageIndex"
				background
				size="small"
				v-model:page-size="state.tableData.param.pageSize"
				layout="total, sizes, prev, pager, next, jumper"
				:total="state.tableData.total"
			/>
		</el-card>
	</div>
</template>

<script setup lang="ts" name="systemFile">
import { onMounted, reactive } from 'vue';
import type { UploadRequestOptions } from 'element-plus';
import { ElMessage, ElMessageBox } from 'element-plus';
import { resolveFileUrl, useFileApi } from '/@/api/apis';
import type { FileRecordDto } from '/@/api/models/file';

const fileApi = useFileApi();

const state = reactive({
	queryParam: {
		filter: '',
		category: '',
	},
	tableData: {
		data: [] as FileRecordDto[],
		total: 0,
		loading: false,
		param: {
			pageIndex: 1,
			pageSize: 10,
		},
	},
});

const getTableData = async () => {
	state.tableData.loading = true;
	try {
		const data = await fileApi.getFilePage({
			filter: state.queryParam.filter || undefined,
			category: state.queryParam.category || undefined,
			skipCount: (state.tableData.param.pageIndex - 1) * state.tableData.param.pageSize,
			maxResultCount: state.tableData.param.pageSize,
		});
		state.tableData.data = data.items ?? [];
		state.tableData.total = data.totalCount ?? 0;
	} finally {
		state.tableData.loading = false;
	}
};

const isImage = (row: FileRecordDto) => row.contentType?.startsWith('image/') || ['.jpg', '.jpeg', '.png', '.gif', '.bmp', '.webp'].includes(row.extension);

const formatDate = (val?: string) => (val ? val.replace('T', ' ').substring(0, 19) : '-');

const formatSize = (size: number) => {
	if (!size) return '0 B';
	if (size < 1024) return `${size} B`;
	if (size < 1024 * 1024) return `${(size / 1024).toFixed(1)} KB`;
	return `${(size / 1024 / 1024).toFixed(1)} MB`;
};

const uploadFile = async (options: UploadRequestOptions) => {
	try {
		await fileApi.uploadFile(options.file as File, { category: state.queryParam.category || 'common', isPublic: true });
		ElMessage.success('上传成功');
		options.onSuccess?.({});
		await getTableData();
	} catch (error) {
		options.onError?.(error as Error);
	}
};

const openFile = (row: FileRecordDto) => {
	window.open(resolveFileUrl(row.url), '_blank');
};

const deleteFile = async (row: FileRecordDto) => {
	await ElMessageBox.confirm(`确认删除文件「${row.originalName}」？`, '提示', {
		confirmButtonText: '确认',
		cancelButtonText: '取消',
		type: 'warning',
	});
	await fileApi.deleteFile(row.id);
	ElMessage.success('删除成功');
	await getTableData();
};

const onQuery = () => {
	state.tableData.param.pageIndex = 1;
	getTableData();
};

const onReset = () => {
	state.queryParam.filter = '';
	state.queryParam.category = '';
	state.tableData.param.pageIndex = 1;
	getTableData();
};

const onHandleSizeChange = (val: number) => {
	state.tableData.param.pageSize = val;
	getTableData();
};

const onHandleCurrentChange = (val: number) => {
	state.tableData.param.pageIndex = val;
	getTableData();
};

onMounted(getTableData);
</script>

<style scoped lang="scss">
.system-file-container {
	display: flex;
	flex-direction: column;

	.file-thumb {
		width: 42px;
		height: 42px;
		border-radius: 6px;
		border: 1px solid var(--el-border-color-light);
	}
}
</style>
