<template>
	<div class="platform-file-provider-container layout-padding">
		<el-card shadow="hover" :body-style="{ paddingBottom: '0' }">
			<el-form :model="state.queryParam" :inline="true">
				<el-form-item label="关键字">
					<el-input v-model="state.queryParam.filter" placeholder="服务商/Bucket/备注" clearable style="width: 200px" @keyup.enter="onQuery" />
				</el-form-item>
				<el-form-item label="服务商">
					<el-select v-model="state.queryParam.provider" placeholder="全部" clearable style="width: 150px">
						<el-option v-for="item in providerOptions" :key="item.value" :label="item.label" :value="item.value" />
					</el-select>
				</el-form-item>
				<el-form-item label="状态">
					<el-select v-model="state.queryParam.isEnable" placeholder="全部" clearable style="width: 120px">
						<el-option label="启用" :value="true" />
						<el-option label="禁用" :value="false" />
					</el-select>
				</el-form-item>
				<el-form-item>
					<el-button-group>
						<el-button type="primary" icon="ele-Search" @click="onQuery">查询</el-button>
						<el-button icon="ele-Refresh" @click="onReset">重置</el-button>
					</el-button-group>
				</el-form-item>
				<el-form-item>
					<el-button type="primary" icon="ele-Plus" @click="openDialog()">新增服务商</el-button>
				</el-form-item>
			</el-form>
		</el-card>

		<el-card class="full-table" shadow="hover" style="margin-top: 5px">
			<el-table :data="state.tableData.data" v-loading="state.tableData.loading" style="width: 100%" border stripe>
				<el-table-column type="index" label="序号" width="60" align="center" fixed />
				<el-table-column prop="displayName" label="配置名称" min-width="180" show-overflow-tooltip />
				<el-table-column prop="provider" label="服务商" width="110" align="center">
					<template #default="{ row }">
						<el-tag effect="plain">{{ row.provider }}</el-tag>
					</template>
				</el-table-column>
				<el-table-column prop="bucketName" label="Bucket" min-width="160" show-overflow-tooltip />
				<el-table-column prop="endpoint" label="Endpoint" min-width="190" show-overflow-tooltip />
				<el-table-column prop="customDomain" label="自定义域名" min-width="180" show-overflow-tooltip />
				<el-table-column label="状态" width="130" align="center">
					<template #default="{ row }">
						<el-tag :type="row.isEnable ? 'success' : 'info'" effect="light">{{ row.isEnable ? '启用' : '禁用' }}</el-tag>
						<el-tag v-if="row.isDefault" type="warning" effect="light" style="margin-left: 6px">默认</el-tag>
					</template>
				</el-table-column>
				<el-table-column prop="orderNo" label="排序" width="80" align="center" />
				<el-table-column label="操作" width="240" fixed="right" align="center">
					<template #default="{ row }">
						<el-button icon="ele-Edit" size="small" text type="primary" @click="openDialog(row)">编辑</el-button>
						<el-button v-if="!row.isDefault" icon="ele-Star" size="small" text type="warning" @click="setDefault(row)">默认</el-button>
						<el-button icon="ele-Delete" size="small" text type="danger" @click="deleteProvider(row)">删除</el-button>
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

		<el-dialog
			v-model="state.dialog.visible"
			:title="state.dialog.form.id ? '编辑存储服务商' : '新增存储服务商'"
			width="820px"
			class="file-provider-dialog"
			destroy-on-close
		>
			<el-form ref="formRef" class="provider-dialog-form" :model="state.dialog.form" :rules="rules" label-width="88px">
				<el-row class="provider-form-row" :gutter="36">
					<el-col :xs="24" :sm="12">
						<el-form-item label="服务商" prop="provider">
							<el-select v-model="state.dialog.form.provider" placeholder="请选择服务商" style="width: 100%">
								<el-option v-for="item in providerOptions" :key="item.value" :label="item.label" :value="item.value" />
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12">
						<el-form-item label="Bucket" prop="bucketName">
							<el-input v-model="state.dialog.form.bucketName" placeholder="请输入 Bucket 名称" />
						</el-form-item>
					</el-col>
				</el-row>
				<el-row class="provider-form-row" :gutter="36">
					<el-col :xs="24" :sm="12">
						<el-form-item label="AccessKey">
							<el-input v-model="state.dialog.form.accessKey" placeholder="请输入 AccessKey" />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12">
						<el-form-item label="SecretKey">
							<el-input v-model="state.dialog.form.secretKey" type="password" show-password placeholder="留空则保留原值" />
						</el-form-item>
					</el-col>
				</el-row>
				<el-row class="provider-form-row" :gutter="36">
					<el-col :xs="24" :sm="12">
						<el-form-item label="Region">
							<el-input v-model="state.dialog.form.region" placeholder="请输入区域" />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12">
						<el-form-item label="Endpoint">
							<el-input v-model="state.dialog.form.endpoint" placeholder="请输入访问端点" />
						</el-form-item>
					</el-col>
				</el-row>
				<el-form-item class="provider-form-item--wide" label="自定义域名">
					<el-input v-model="state.dialog.form.customDomain" placeholder="例如 https://static.example.com" />
				</el-form-item>
				<el-row class="provider-form-row provider-form-row--settings" :gutter="36">
					<el-col :xs="24" :sm="12">
						<el-form-item label="排序">
							<el-input-number v-model="state.dialog.form.orderNo" :min="0" :max="9999" controls-position="right" style="width: 100%" />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12">
						<el-form-item class="provider-switches" label="开关">
							<div class="provider-switches__group">
								<el-checkbox v-model="state.dialog.form.isEnable">启用</el-checkbox>
								<el-checkbox v-model="state.dialog.form.isDefault">默认</el-checkbox>
								<el-checkbox v-model="state.dialog.form.isEnableHttps">HTTPS</el-checkbox>
								<el-checkbox v-model="state.dialog.form.isEnableCache">缓存</el-checkbox>
							</div>
						</el-form-item>
					</el-col>
				</el-row>
				<el-form-item class="provider-form-item--wide provider-form-item--remark" label="备注">
					<el-input v-model="state.dialog.form.remark" type="textarea" :rows="3" placeholder="请输入备注" />
				</el-form-item>
			</el-form>
			<template #footer>
				<div class="provider-dialog-footer">
					<el-button @click="state.dialog.visible = false">取消</el-button>
					<el-button type="primary" :loading="state.dialog.saving" @click="submit">保存</el-button>
				</div>
			</template>
		</el-dialog>
	</div>
</template>

<script setup lang="ts" name="platformFileProvider">
import { onMounted, reactive, ref } from 'vue';
import type { FormInstance, FormRules } from 'element-plus';
import { ElMessage, ElMessageBox } from 'element-plus';
import { useFileProviderApi } from '/@/api/apis';
import type { CreateUpdateFileProviderDto, FileProviderDto } from '/@/api/models/file';

type ProviderForm = CreateUpdateFileProviderDto & { id?: string };

const fileProviderApi = useFileProviderApi();
const formRef = ref<FormInstance>();

const providerOptions = [
	{ label: 'Minio', value: 'Minio' },
	{ label: '阿里云 OSS', value: 'Aliyun' },
	{ label: '腾讯云 COS', value: 'QCloud' },
	{ label: '七牛云 Kodo', value: 'Qiniu' },
	{ label: '华为云 OBS', value: 'HuaweiCloud' },
	{ label: '百度云 BOS', value: 'BaiduCloud' },
	{ label: '天翼云 OOS', value: 'Ctyun' },
];

const createDefaultForm = (): ProviderForm => ({
	provider: 'Minio',
	bucketName: '',
	accessKey: '',
	secretKey: '',
	region: '',
	endpoint: '',
	isEnableHttps: true,
	isEnableCache: true,
	isEnable: true,
	isDefault: false,
	customDomain: '',
	orderNo: 100,
	remark: '',
});

const state = reactive({
	queryParam: {
		filter: '',
		provider: '',
		isEnable: undefined as boolean | undefined,
	},
	tableData: {
		data: [] as FileProviderDto[],
		total: 0,
		loading: false,
		param: {
			pageIndex: 1,
			pageSize: 10,
		},
	},
	dialog: {
		visible: false,
		saving: false,
		form: createDefaultForm(),
	},
});

const rules: FormRules = {
	provider: [{ required: true, message: '请选择服务商', trigger: 'change' }],
	bucketName: [{ required: true, message: '请输入 Bucket 名称', trigger: 'blur' }],
};

const getTableData = async () => {
	state.tableData.loading = true;
	try {
		const data = await fileProviderApi.getProviderPage({
			filter: state.queryParam.filter || undefined,
			provider: state.queryParam.provider || undefined,
			isEnable: state.queryParam.isEnable,
			skipCount: (state.tableData.param.pageIndex - 1) * state.tableData.param.pageSize,
			maxResultCount: state.tableData.param.pageSize,
		});
		state.tableData.data = data.items ?? [];
		state.tableData.total = data.totalCount ?? 0;
	} finally {
		state.tableData.loading = false;
	}
};

const openDialog = (row?: FileProviderDto) => {
	state.dialog.form = row
		? {
				id: row.id,
				provider: row.provider,
				bucketName: row.bucketName,
				accessKey: row.accessKey ?? '',
				secretKey: '',
				region: row.region ?? '',
				endpoint: row.endpoint ?? '',
				isEnableHttps: row.isEnableHttps,
				isEnableCache: row.isEnableCache,
				isEnable: row.isEnable,
				isDefault: row.isDefault,
				customDomain: row.customDomain ?? '',
				orderNo: row.orderNo,
				remark: row.remark ?? '',
		  }
		: createDefaultForm();
	state.dialog.visible = true;
};

const toInput = (form: ProviderForm): CreateUpdateFileProviderDto => ({
	provider: form.provider,
	bucketName: form.bucketName,
	accessKey: form.accessKey || null,
	secretKey: form.secretKey || null,
	region: form.region || null,
	endpoint: form.endpoint || null,
	isEnableHttps: form.isEnableHttps,
	isEnableCache: form.isEnableCache,
	isEnable: form.isEnable,
	isDefault: form.isDefault,
	customDomain: form.customDomain || null,
	orderNo: form.orderNo,
	remark: form.remark || null,
});

const submit = async () => {
	await formRef.value?.validate();
	state.dialog.saving = true;
	try {
		if (state.dialog.form.id) {
			await fileProviderApi.updateProvider(state.dialog.form.id, toInput(state.dialog.form));
			ElMessage.success('更新成功');
		} else {
			await fileProviderApi.createProvider(toInput(state.dialog.form));
			ElMessage.success('创建成功');
		}
		state.dialog.visible = false;
		await getTableData();
	} finally {
		state.dialog.saving = false;
	}
};

const setDefault = async (row: FileProviderDto) => {
	await fileProviderApi.setDefaultProvider(row.id);
	ElMessage.success('默认服务商已更新');
	await getTableData();
};

const deleteProvider = async (row: FileProviderDto) => {
	await ElMessageBox.confirm(`确认删除存储服务商「${row.displayName}」？`, '提示', {
		confirmButtonText: '确认',
		cancelButtonText: '取消',
		type: 'warning',
	});
	await fileProviderApi.deleteProvider(row.id);
	ElMessage.success('删除成功');
	await getTableData();
};

const onQuery = () => {
	state.tableData.param.pageIndex = 1;
	getTableData();
};

const onReset = () => {
	state.queryParam.filter = '';
	state.queryParam.provider = '';
	state.queryParam.isEnable = undefined;
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
.platform-file-provider-container {
	display: flex;
	flex-direction: column;
}

:global(.file-provider-dialog) {
	width: min(820px, calc(100vw - 32px)) !important;
}

:global(.file-provider-dialog .el-dialog__body) {
	padding: 28px 36px 24px !important;
}

:global(.file-provider-dialog .el-dialog__footer) {
	padding: 0 36px 24px;
}

:global(.file-provider-dialog .provider-dialog-form) {
	padding: 0;
}

:global(.file-provider-dialog .provider-form-row) {
	margin-bottom: 12px;
}

:global(.file-provider-dialog .provider-form-row--settings) {
	margin-top: 6px;
	margin-bottom: 18px;
}

:global(.file-provider-dialog .el-form-item) {
	margin-bottom: 0;
}

:global(.file-provider-dialog .el-form-item__label) {
	padding-right: 16px;
}

:global(.file-provider-dialog .provider-form-item--wide) {
	margin-bottom: 20px;
}

:global(.file-provider-dialog .provider-form-item--remark) {
	margin-top: 4px;
}

:global(.file-provider-dialog .provider-form-item--remark .el-textarea__inner) {
	min-height: 82px !important;
}

:global(.file-provider-dialog .provider-switches .el-form-item__content) {
	align-items: flex-start;
}

.provider-switches__group {
	display: grid;
	grid-template-columns: repeat(2, minmax(82px, 1fr));
	column-gap: 18px;
	row-gap: 10px;
	width: 100%;
	padding-top: 2px;

	:deep(.el-checkbox) {
		height: 24px;
		margin-right: 0;
	}
}

.provider-dialog-footer {
	display: flex;
	justify-content: flex-end;
	gap: 10px;
}

@media (max-width: 768px) {
	:global(.file-provider-dialog .el-dialog__body) {
		padding: 22px 20px 20px !important;
	}

	:global(.file-provider-dialog .el-dialog__footer) {
		padding: 0 20px 20px;
	}

	:global(.file-provider-dialog .provider-form-row) {
		margin-bottom: 0;
	}

	:global(.file-provider-dialog .el-form-item) {
		margin-bottom: 18px;
	}

	.provider-switches__group {
		grid-template-columns: repeat(2, minmax(68px, 1fr));
	}
}
</style>
