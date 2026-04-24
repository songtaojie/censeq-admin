<template>
	<div class="app-container layout-padding">
		<el-card shadow="hover" :body-style="{ paddingBottom: '0' }">
			<el-form ref="queryFormRef" :model="state.searchForm" :inline="true">
				<el-form-item label="关键词">
					<el-input v-model="state.searchForm.filter" placeholder="请输入客户端ID或名称" clearable style="width: 200px" />
				</el-form-item>
				<el-form-item label="客户端类型">
					<el-select v-model="state.searchForm.clientType" placeholder="全部" clearable style="width: 130px">
						<el-option label="机密" value="confidential" />
						<el-option label="公开" value="public" />
					</el-select>
				</el-form-item>
				<el-form-item>
					<el-button-group>
						<el-button type="primary" @click="handleSearch">
							<el-icon><ele-Search /></el-icon>查询
						</el-button>
						<el-button @click="resetSearch">
							<el-icon><ele-RefreshLeft /></el-icon>重置
						</el-button>
					</el-button-group>
				</el-form-item>
				<el-form-item>
					<el-button type="primary" @click="handleCreate">
						<el-icon><ele-Plus /></el-icon>新建应用
					</el-button>
				</el-form-item>
			</el-form>
		</el-card>

		<el-card shadow="hover" style="margin-top: 8px">
			<el-table :data="state.dataList" v-loading="state.loading" border stripe>
				<el-table-column type="index" label="序号" width="60" align="center" />
				<el-table-column prop="clientId" label="客户端ID" min-width="160" show-overflow-tooltip>
					<template #default="{ row }">
						<span class="client-id-text">{{ row.clientId }}</span>
					</template>
				</el-table-column>
				<el-table-column prop="displayName" label="显示名称" min-width="140" show-overflow-tooltip>
					<template #default="{ row }">
						{{ row.displayName || '-' }}
					</template>
				</el-table-column>
				<el-table-column prop="clientType" label="客户端类型" width="110" align="center">
					<template #default="{ row }">
						<el-tag v-if="row.clientType === 'confidential'" type="primary" effect="light">机密</el-tag>
						<el-tag v-else-if="row.clientType === 'public'" type="success" effect="light">公开</el-tag>
						<el-tag v-else effect="light">{{ row.clientType }}</el-tag>
					</template>
				</el-table-column>
				<el-table-column prop="applicationType" label="应用类型" width="100" align="center">
					<template #default="{ row }">
						<el-tag :type="row.applicationType === 'native' ? 'warning' : 'info'" effect="light">
							{{ row.applicationType === 'native' ? '原生' : 'Web' }}
						</el-tag>
					</template>
				</el-table-column>
				<el-table-column prop="redirectUris" label="回调地址" min-width="200" show-overflow-tooltip>
					<template #default="{ row }">
						<span class="uri-text">{{ formatArray(row.redirectUris) }}</span>
					</template>
				</el-table-column>
				<el-table-column prop="creationTime" label="创建时间" width="160" align="center">
					<template #default="{ row }">
						{{ formatDateTime(row.creationTime) }}
					</template>
				</el-table-column>
				<el-table-column label="操作" width="150" align="center" fixed="right">
					<template #default="{ row }">
						<el-button icon="ele-Edit" size="small" text type="primary" @click="handleEdit(row)">编辑</el-button>
						<el-button icon="ele-Delete" size="small" text type="danger" @click="handleDelete(row)">删除</el-button>
					</template>
				</el-table-column>
			</el-table>
			<el-pagination
				class="pagination"
				v-model:current-page="state.searchForm.skipCount"
				v-model:page-size="state.searchForm.maxResultCount"
				:pager-count="5"
				:page-sizes="[10, 20, 50, 100]"
				:total="state.total"
				layout="total, sizes, prev, pager, next, jumper"
				background
				size="small"
				@size-change="handlePageSizeChange"
				@current-change="getList"
			/>
		</el-card>

		<!-- 编辑弹窗 -->
		<el-dialog
			v-model="state.dialogVisible"
			width="700px"
			destroy-on-close
			draggable
			:close-on-click-modal="false"
		>
			<template #header>
				<div class="dialog-header">
					<el-icon v-if="state.isEdit" color="#409EFF" size="18"><ele-Edit /></el-icon>
					<el-icon v-else color="#67C23A" size="18"><ele-Plus /></el-icon>
					<span>{{ state.isEdit ? '编辑应用' : '新建应用' }}</span>
				</div>
			</template>
			<el-form
				ref="formRef"
				:model="state.form"
				:rules="formRules"
				label-width="120px"
				label-position="right"
				class="dialog-form"
			>
				<el-row :gutter="20">
					<el-col :span="12">
						<el-form-item label="客户端ID" prop="clientId">
							<el-input v-model="state.form.clientId" :disabled="state.isEdit" placeholder="请输入客户端ID" />
						</el-form-item>
					</el-col>
					<el-col :span="12">
						<el-form-item label="显示名称" prop="displayName">
							<el-input v-model="state.form.displayName" placeholder="请输入显示名称" />
						</el-form-item>
					</el-col>
				</el-row>
				<el-row :gutter="20">
					<el-col :span="12">
						<el-form-item label="客户端类型" prop="clientType">
							<el-radio-group v-model="state.form.clientType" :disabled="state.isEdit">
								<el-radio label="confidential">机密</el-radio>
								<el-radio label="public">公开</el-radio>
							</el-radio-group>
						</el-form-item>
					</el-col>
					<el-col :span="12">
						<el-form-item label="应用类型" prop="applicationType">
							<el-radio-group v-model="state.form.applicationType">
								<el-radio label="web">Web</el-radio>
								<el-radio label="native">原生</el-radio>
							</el-radio-group>
						</el-form-item>
					</el-col>
				</el-row>
				<el-row :gutter="20">
					<el-col :span="12">
						<el-form-item label="同意类型" prop="consentType">
							<el-select v-model="state.form.consentType" style="width: 100%">
								<el-option label="显式同意" value="explicit" />
								<el-option label="隐式同意" value="implicit" />
								<el-option label="外部同意" value="external" />
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :span="12">
						<el-form-item label="客户端密钥" prop="clientSecret">
							<el-input
								v-model="state.form.clientSecret"
								type="password"
								show-password
								:placeholder="state.isEdit ? '留空表示不修改' : '留空则自动生成'"
							/>
						</el-form-item>
					</el-col>
				</el-row>
				<el-form-item label="回调地址" prop="redirectUris">
					<div class="tag-input-area">
						<el-tag
							v-for="(tag, index) in state.form.redirectUris"
							:key="index"
							closable
							effect="light"
							@close="removeRedirectUri(index)"
						>
							{{ tag }}
						</el-tag>
						<el-input
							v-if="state.inputVisible.redirectUri"
							ref="redirectUriInputRef"
							v-model="state.inputValue.redirectUri"
							size="small"
							@keyup.enter="handleInputConfirm('redirectUri')"
							@blur="handleInputConfirm('redirectUri')"
							style="width: 200px"
						/>
						<el-button v-else size="small" plain @click="showInput('redirectUri')">
							<el-icon><ele-Plus /></el-icon>添加
						</el-button>
					</div>
				</el-form-item>
				<el-form-item label="登出回调" prop="postLogoutRedirectUris">
					<div class="tag-input-area">
						<el-tag
							v-for="(tag, index) in state.form.postLogoutRedirectUris"
							:key="index"
							closable
							type="info"
							effect="light"
							@close="removePostLogoutUri(index)"
						>
							{{ tag }}
						</el-tag>
						<el-input
							v-if="state.inputVisible.postLogoutUri"
							ref="postLogoutUriInputRef"
							v-model="state.inputValue.postLogoutUri"
							size="small"
							@keyup.enter="handleInputConfirm('postLogoutUri')"
							@blur="handleInputConfirm('postLogoutUri')"
							style="width: 200px"
						/>
						<el-button v-else size="small" plain @click="showInput('postLogoutUri')">
							<el-icon><ele-Plus /></el-icon>添加
						</el-button>
					</div>
				</el-form-item>
				<el-form-item label="权限" prop="permissions">
					<div class="permission-group">
						<div class="permission-section">
							<div class="permission-label">端点</div>
							<el-checkbox v-model="state.form.permissions" label="ept:token">Token端点</el-checkbox>
							<el-checkbox v-model="state.form.permissions" label="ept:authorization">授权端点</el-checkbox>
							<el-checkbox v-model="state.form.permissions" label="ept:logout">登出端点</el-checkbox>
						</div>
						<div class="permission-section">
							<div class="permission-label">授权模式</div>
							<el-checkbox v-model="state.form.permissions" label="gt:authorization_code">授权码</el-checkbox>
							<el-checkbox v-model="state.form.permissions" label="gt:client_credentials">客户端凭证</el-checkbox>
							<el-checkbox v-model="state.form.permissions" label="gt:password">密码</el-checkbox>
							<el-checkbox v-model="state.form.permissions" label="gt:refresh_token">刷新令牌</el-checkbox>
						</div>
						<div class="permission-section">
							<div class="permission-label">作用域</div>
							<el-checkbox v-model="state.form.permissions" label="scp:openid">OpenID</el-checkbox>
							<el-checkbox v-model="state.form.permissions" label="scp:profile">Profile</el-checkbox>
							<el-checkbox v-model="state.form.permissions" label="scp:email">Email</el-checkbox>
							<el-checkbox v-model="state.form.permissions" label="scp:phone">Phone</el-checkbox>
							<el-checkbox v-model="state.form.permissions" label="scp:roles">Roles</el-checkbox>
						</div>
					</div>
				</el-form-item>
			</el-form>
			<template #footer>
				<el-button icon="ele-CircleClose" @click="state.dialogVisible = false">取消</el-button>
				<el-button type="primary" icon="ele-Select" @click="handleSubmit" :loading="state.submitLoading">保存</el-button>
			</template>
		</el-dialog>
	</div>
</template>

<script setup lang="ts" name="openiddictApplication">
import { reactive, ref, nextTick } from 'vue';
import { ElMessage, ElMessageBox } from 'element-plus';
import type { FormInstance, FormRules } from 'element-plus';
import { useOpenIddictApplicationApi, type OpenIddictApplicationDto } from '/@/api/apis/openiddict';
const { getList: getApplicationList, create, update, delete: deleteApplication } = useOpenIddictApplicationApi();

const formRef = ref<FormInstance>();
const redirectUriInputRef = ref<HTMLInputElement>();
const postLogoutUriInputRef = ref<HTMLInputElement>();

const state = reactive({
	loading: false,
	submitLoading: false,
	total: 0,
	dialogVisible: false,
	isEdit: false,
	dataList: [] as OpenIddictApplicationDto[],
	searchForm: {
		filter: '',
		clientType: '',
		skipCount: 1,
		maxResultCount: 20,
	},
	form: {
		id: '',
		clientId: '',
		displayName: '',
		clientType: 'confidential',
		applicationType: 'web',
		consentType: 'explicit',
		clientSecret: '',
		redirectUris: [] as string[],
		postLogoutRedirectUris: [] as string[],
		permissions: [] as string[],
	},
	inputVisible: {
		redirectUri: false,
		postLogoutUri: false,
	},
	inputValue: {
		redirectUri: '',
		postLogoutUri: '',
	},
});

const formRules = reactive<FormRules>({
	clientId: [{ required: true, message: '请输入客户端ID', trigger: 'blur' }],
	clientType: [{ required: true, message: '请选择客户端类型', trigger: 'change' }],
});

// 获取列表
const getList = async () => {
	state.loading = true;
	try {
		const res = await getApplicationList({
			...state.searchForm,
			skipCount: (state.searchForm.skipCount - 1) * state.searchForm.maxResultCount,
		});
		state.dataList = res.items || [];
		state.total = res.totalCount || 0;
	} finally {
		state.loading = false;
	}
};

// 搜索
const handleSearch = () => {
	state.searchForm.skipCount = 1;
	getList();
};

// 重置搜索
const resetSearch = () => {
	state.searchForm.filter = '';
	state.searchForm.clientType = '';
	state.searchForm.skipCount = 1;
	getList();
};

const handlePageSizeChange = () => {
	state.searchForm.skipCount = 1;
	getList();
};

// 格式化数组
const formatArray = (arr?: string[]) => {
	if (!arr || arr.length === 0) return '-';
	return arr.join(', ');
};

// 格式化日期
const formatDateTime = (dateStr?: string) => {
	if (!dateStr) return '-';
	const date = new Date(dateStr);
	return date.toLocaleString('zh-CN');
};

// 新建
const handleCreate = () => {
	state.isEdit = false;
	state.form = {
		id: '',
		clientId: '',
		displayName: '',
		clientType: 'confidential',
		applicationType: 'web',
		consentType: 'explicit',
		clientSecret: '',
		redirectUris: [],
		postLogoutRedirectUris: [],
		permissions: ['ept:token', 'ept:authorization', 'gt:authorization_code', 'scp:openid', 'scp:profile'],
	};
	state.dialogVisible = true;
};

// 编辑
const handleEdit = (row: OpenIddictApplicationDto) => {
	state.isEdit = true;
	state.form = {
		id: row.id,
		clientId: row.clientId,
		displayName: row.displayName || '',
		clientType: row.clientType,
		applicationType: row.applicationType || 'web',
		consentType: row.consentType || 'explicit',
		clientSecret: '',
		redirectUris: [...row.redirectUris],
		postLogoutRedirectUris: [...row.postLogoutRedirectUris],
		permissions: [...row.permissions],
	};
	state.dialogVisible = true;
};

// 删除
const handleDelete = async (row: OpenIddictApplicationDto) => {
	try {
		await ElMessageBox.confirm(`确定要删除应用 "${row.clientId}" 吗？`, '提示', {
			confirmButtonText: '确定',
			cancelButtonText: '取消',
			type: 'warning',
		});
		await deleteApplication(row.id);
		ElMessage.success('删除成功');
		getList();
	} catch (error: any) {
		if (error !== 'cancel') {
			console.error('删除失败', error);
			ElMessage.error('删除失败');
		}
	}
};

// 提交
const handleSubmit = async () => {
	if (!formRef.value) return;
	await formRef.value.validate(async (valid) => {
		if (!valid) return;
		state.submitLoading = true;
		try {
			if (state.isEdit) {
				await update(state.form.id, {
					displayName: state.form.displayName,
					clientSecret: state.form.clientSecret || undefined,
					consentType: state.form.consentType,
					redirectUris: state.form.redirectUris,
					postLogoutRedirectUris: state.form.postLogoutRedirectUris,
					permissions: state.form.permissions,
					requirements: [],
				});
				ElMessage.success('更新成功');
			} else {
				await create({
					clientId: state.form.clientId,
					displayName: state.form.displayName,
					clientType: state.form.clientType,
					applicationType: state.form.applicationType,
					consentType: state.form.consentType,
					clientSecret: state.form.clientSecret,
					redirectUris: state.form.redirectUris,
					postLogoutRedirectUris: state.form.postLogoutRedirectUris,
					permissions: state.form.permissions,
					requirements: [],
				});
				ElMessage.success('创建成功');
			}
			state.dialogVisible = false;
			getList();
		} catch (error) {
			console.error('提交失败', error);
			ElMessage.error('提交失败');
		} finally {
			state.submitLoading = false;
		}
	});
};

// Tag输入相关方法
const removeRedirectUri = (index: number) => {
	state.form.redirectUris.splice(index, 1);
};

const removePostLogoutUri = (index: number) => {
	state.form.postLogoutRedirectUris.splice(index, 1);
};

const showInput = (type: 'redirectUri' | 'postLogoutUri') => {
	state.inputVisible[type] = true;
	state.inputValue[type] = '';
	nextTick(() => {
		if (type === 'redirectUri') {
			redirectUriInputRef.value?.focus();
		} else {
			postLogoutUriInputRef.value?.focus();
		}
	});
};

const handleInputConfirm = (type: 'redirectUri' | 'postLogoutUri') => {
	const value = state.inputValue[type]?.trim();
	if (value) {
		if (type === 'redirectUri') {
			if (!state.form.redirectUris.includes(value)) {
				state.form.redirectUris.push(value);
			}
		} else {
			if (!state.form.postLogoutRedirectUris.includes(value)) {
				state.form.postLogoutRedirectUris.push(value);
			}
		}
	}
	state.inputVisible[type] = false;
	state.inputValue[type] = '';
};

// 初始化
getList();
</script>

<style scoped lang="scss">
.app-container {
	display: flex;
	flex-direction: column;
}

.pagination {
	margin-top: 14px;
	justify-content: flex-end;
}

.client-id-text {
	color: var(--el-color-primary);
	font-weight: 500;
}

.uri-text {
	font-size: 12px;
	color: #606266;
}

.dialog-header {
	display: flex;
	align-items: center;
	gap: 8px;
	font-size: 16px;
	font-weight: 600;
}

.dialog-form {
	:deep(.el-form-item) {
		margin-bottom: 20px !important;
	}
}

.tag-input-area {
	display: flex;
	flex-wrap: wrap;
	gap: 6px;
	align-items: center;
}

.permission-group {
	display: flex;
	flex-direction: column;
	gap: 10px;
	width: 100%;
}

.permission-section {
	display: flex;
	flex-wrap: wrap;
	align-items: center;
	gap: 4px;
	padding: 8px 10px;
	background: var(--el-fill-color-lighter);
	border-radius: 6px;
}

.permission-label {
	font-size: 12px;
	color: #909399;
	font-weight: 600;
	width: 52px;
	flex-shrink: 0;
}
</style>