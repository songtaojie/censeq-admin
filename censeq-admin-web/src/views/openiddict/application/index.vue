<template>
	<div class="openiddict-application-container layout-padding">
		<div class="openiddict-application-padding layout-padding-auto layout-padding-view page-shell">
			<el-card shadow="hover" :body-style="{ paddingBottom: '0' }" class="page-filter-card">
				<el-form :model="state.searchForm" size="default" inline class="page-toolbar">
					<el-form-item label="关键词">
						<el-input v-model="state.searchForm.filter" class="page-search" placeholder="请输入客户端ID或名称" clearable />
					</el-form-item>
					<el-form-item label="客户端类型">
						<el-select v-model="state.searchForm.clientType" placeholder="全部" clearable style="width: 120px">
							<el-option label="机密" value="confidential" />
							<el-option label="公开" value="public" />
						</el-select>
					</el-form-item>
					<el-form-item>
						<el-button-group>
							<el-button size="default" type="primary" @click="handleSearch">
								<el-icon><ele-Search /></el-icon>查询
							</el-button>
							<el-button size="default" @click="resetSearch">
								<el-icon><ele-RefreshLeft /></el-icon>重置
							</el-button>
						</el-button-group>
					</el-form-item>
					<el-form-item>
						<el-button type="success" size="default" @click="handleCreate">
							<el-icon><ele-Plus /></el-icon>新建应用
						</el-button>
					</el-form-item>
				</el-form>
			</el-card>

			<el-card class="page-content-card" shadow="hover" style="margin-top: 5px">
				<div class="page-table-body">
					<el-table :data="state.dataList" v-loading="state.loading" border>
				<el-table-column type="index" label="序号" width="60" align="center" />
				<el-table-column prop="clientId" label="客户端ID" min-width="150" show-overflow-tooltip />
				<el-table-column prop="displayName" label="显示名称" min-width="150" show-overflow-tooltip>
					<template #default="{ row }">
						{{ row.displayName || '-' }}
					</template>
				</el-table-column>
				<el-table-column prop="clientType" label="客户端类型" width="100" align="center">
					<template #default="{ row }">
						<el-tag v-if="row.clientType === 'confidential'" type="primary">机密</el-tag>
						<el-tag v-else-if="row.clientType === 'public'" type="success">公开</el-tag>
						<el-tag v-else>{{ row.clientType }}</el-tag>
					</template>
				</el-table-column>
				<el-table-column prop="applicationType" label="应用类型" width="100" align="center">
					<template #default="{ row }">
						<span>{{ row.applicationType === 'native' ? '原生' : 'Web' }}</span>
					</template>
				</el-table-column>
				<el-table-column prop="redirectUris" label="回调地址" min-width="200" show-overflow-tooltip>
					<template #default="{ row }">
						{{ formatArray(row.redirectUris) }}
					</template>
				</el-table-column>
				<el-table-column prop="creationTime" label="创建时间" width="160" align="center">
					<template #default="{ row }">
						{{ formatDateTime(row.creationTime) }}
					</template>
				</el-table-column>
					<el-table-column label="操作" width="180" align="center" fixed="right">
					<template #default="{ row }">
						<el-button type="primary" link size="small" @click="handleEdit(row)">编辑</el-button>
						<el-button type="danger" link size="small" @click="handleDelete(row)">删除</el-button>
					</template>
				</el-table-column>
					</el-table>

					<el-pagination
				v-model:current-page="state.searchForm.skipCount"
				v-model:page-size="state.searchForm.maxResultCount"
				:pager-count="5"
				:page-sizes="[10, 20, 30, 50, 100]"
				:total="state.total"
				layout="total, sizes, prev, pager, next, jumper"
				background
				@size-change="handlePageSizeChange"
					@current-change="getList"
					></el-pagination>
				</div>
			</el-card>
		</div>

		<!-- 编辑弹窗 -->
		<el-dialog
			v-model="state.dialogVisible"
			:title="state.isEdit ? '编辑应用' : '新建应用'"
			width="700px"
			destroy-on-close
		>
			<el-form
				ref="formRef"
				:model="state.form"
				:rules="formRules"
				label-width="120px"
				label-position="right"
			>
				<el-form-item label="客户端ID" prop="clientId">
					<el-input v-model="state.form.clientId" :disabled="state.isEdit" placeholder="请输入客户端ID" />
				</el-form-item>
				<el-form-item label="显示名称" prop="displayName">
					<el-input v-model="state.form.displayName" placeholder="请输入显示名称" />
				</el-form-item>
				<el-form-item label="客户端类型" prop="clientType">
					<el-radio-group v-model="state.form.clientType" :disabled="state.isEdit">
						<el-radio label="confidential">机密 (Confidential)</el-radio>
						<el-radio label="public">公开 (Public)</el-radio>
					</el-radio-group>
				</el-form-item>
				<el-form-item label="应用类型" prop="applicationType">
					<el-radio-group v-model="state.form.applicationType">
						<el-radio label="web">Web应用</el-radio>
						<el-radio label="native">原生应用</el-radio>
					</el-radio-group>
				</el-form-item>
				<el-form-item label="同意类型" prop="consentType">
					<el-select v-model="state.form.consentType" style="width: 100%">
						<el-option label="显式同意 (Explicit)" value="explicit" />
						<el-option label="隐式同意 (Implicit)" value="implicit" />
						<el-option label="外部同意 (External)" value="external" />
					</el-select>
				</el-form-item>
				<el-form-item label="客户端密钥" prop="clientSecret">
					<el-input
						v-model="state.form.clientSecret"
						type="password"
						show-password
						placeholder="留空则自动生成"
					/>
					<div class="form-tip">{{ state.isEdit ? '留空表示不修改' : '留空则自动生成32位随机密钥' }}</div>
				</el-form-item>
				<el-form-item label="回调地址" prop="redirectUris">
					<el-tag
						v-for="(tag, index) in state.form.redirectUris"
						:key="index"
						closable
						@close="removeRedirectUri(index)"
						class="mr5 mb5"
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
					<el-button v-else size="small" @click="showInput('redirectUri')">+ 添加</el-button>
				</el-form-item>
				<el-form-item label="登出回调地址" prop="postLogoutRedirectUris">
					<el-tag
						v-for="(tag, index) in state.form.postLogoutRedirectUris"
						:key="index"
						closable
						@close="removePostLogoutUri(index)"
						class="mr5 mb5"
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
					<el-button v-else size="small" @click="showInput('postLogoutUri')">+ 添加</el-button>
				</el-form-item>
				<el-form-item label="权限" prop="permissions">
					<el-checkbox-group v-model="state.form.permissions">
						<el-checkbox label="ept:token">Token端点</el-checkbox>
						<el-checkbox label="ept:authorization">授权端点</el-checkbox>
						<el-checkbox label="ept:logout">登出端点</el-checkbox>
						<el-checkbox label="gt:authorization_code">授权码模式</el-checkbox>
						<el-checkbox label="gt:client_credentials">客户端凭证模式</el-checkbox>
						<el-checkbox label="gt:password">密码模式</el-checkbox>
						<el-checkbox label="gt:refresh_token">刷新令牌</el-checkbox>
						<el-checkbox label="scp:openid">OpenID</el-checkbox>
						<el-checkbox label="scp:profile">Profile</el-checkbox>
						<el-checkbox label="scp:email">Email</el-checkbox>
						<el-checkbox label="scp:phone">Phone</el-checkbox>
						<el-checkbox label="scp:roles">Roles</el-checkbox>
					</el-checkbox-group>
				</el-form-item>
			</el-form>
			<template #footer>
				<div class="dialog-footer">
					<el-button @click="state.dialogVisible = false">取消</el-button>
					<el-button type="primary" @click="handleSubmit" :loading="state.submitLoading">确定</el-button>
				</div>
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
.openiddict-application-container {
	.form-tip {
		font-size: 12px;
		color: #909399;
		margin-top: 4px;
	}

	.mr5 {
		margin-right: 5px;
	}

	.mb5 {
		margin-bottom: 5px;
	}
}
</style>