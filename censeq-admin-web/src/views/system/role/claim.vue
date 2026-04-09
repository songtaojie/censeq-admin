<template>
	<div class="role-claim-dialog-container">
		<el-dialog :title="`角色声明【${state.roleName}】`" v-model="state.dialog.isShowDialog" width="860px" destroy-on-close>
			<div class="dialog-intro">
				声明用于给角色附加结构化的业务信息，例如数据范围、部门标识或审批阈值。支持按声明类型动态维护值。
			</div>
			<div class="claim-toolbar">
				<el-button type="primary" size="small" @click="onAddClaim" :icon="Plus">添加声明</el-button>
				<el-button type="success" size="small" @click="openClaimTypeManage">维护声明类型</el-button>
			</div>
			<div class="claim-summary mb15">
				<el-tag type="primary">当前角色：{{ state.roleName || '-' }}</el-tag>
				<el-tag type="success">声明 {{ state.claims.length }} 条</el-tag>
				<el-tag type="info">类型 {{ state.claimTypes.length }} 种</el-tag>
			</div>
			<el-alert v-if="!state.claimTypes.length" type="warning" :closable="false" class="mb15" title="当前没有可用的声明类型，请先维护声明类型后再添加角色声明。" />
			<el-table :data="state.claims" border size="small" v-loading="state.loading" height="350">
				<el-table-column prop="claimType" label="声明类型" min-width="180">
					<template #default="scope">
						<el-select v-if="scope.row.isEditing" v-model="scope.row.claimType" placeholder="请选择声明类型" size="small" filterable @change="onClaimTypeChange(scope.row)">
							<el-option v-for="item in state.claimTypes" :key="item.id" :label="formatClaimTypeLabel(item)" :value="item.name" />
						</el-select>
						<el-tag v-else size="small" type="info">{{ scope.row.claimType }}</el-tag>
					</template>
				</el-table-column>
				<el-table-column prop="claimValue" label="声明值" min-width="200">
					<template #default="scope">
						<template v-if="scope.row.isEditing">
							<el-select v-if="getClaimTypeMeta(scope.row.claimType)?.name === 'DataScope'" v-model="scope.row.claimValue" placeholder="选择数据范围" size="small">
								<el-option label="全部数据 (All)" value="All" />
								<el-option label="本部门及以下数据 (DepartmentAndChild)" value="DepartmentAndChild" />
								<el-option label="本部门数据 (Department)" value="Department" />
								<el-option label="仅本人数据 (Self)" value="Self" />
								<el-option label="自定义数据 (Custom)" value="Custom" />
							</el-select>
							<el-select v-else-if="getClaimTypeMeta(scope.row.claimType)?.valueType === 'Boolean'" v-model="scope.row.claimValue" placeholder="请选择布尔值" size="small">
								<el-option label="是" value="true" />
								<el-option label="否" value="false" />
							</el-select>
							<el-input-number v-else-if="getClaimTypeMeta(scope.row.claimType)?.valueType === 'Int'" v-model="scope.row.claimValue" :controls="false" placeholder="请输入整数" size="small" />
							<el-date-picker v-else-if="getClaimTypeMeta(scope.row.claimType)?.valueType === 'DateTime'" v-model="scope.row.claimValue" type="datetime" placeholder="选择日期时间" value-format="YYYY-MM-DD HH:mm:ss" size="small" />
							<el-input v-else v-model="scope.row.claimValue" placeholder="请输入声明值" size="small" />
						</template>
						<span v-else>{{ scope.row.claimValue }}</span>
					</template>
				</el-table-column>
				<el-table-column label="操作" width="150" align="center">
					<template #default="scope">
						<template v-if="scope.row.isEditing">
							<el-button type="success" size="small" text @click="onSaveClaim(scope.row)">保存</el-button>
							<el-button size="small" text @click="onCancelClaim(scope.row, scope.$index)">取消</el-button>
						</template>
						<template v-else>
							<el-button type="primary" size="small" text @click="onEditClaim(scope.row)">修改</el-button>
							<el-popconfirm title="确定删除该声明吗？" @confirm="onDeleteClaim(scope.row)">
								<template #reference>
									<el-button type="danger" size="small" text>删除</el-button>
								</template>
							</el-popconfirm>
						</template>
					</template>
				</el-table-column>
			</el-table>
			<el-alert type="info" :closable="false" class="claim-tips">
				<template #title>
					<div class="tips-content">
						<p><strong>常用声明说明：</strong></p>
						<p>• <code>DataScope</code> - 数据范围：控制角色可以查看的数据范围</p>
						<p>• <code>MaxAmount</code> - 最大审批金额，如：100000</p>
						<p>• <code>DepartmentId</code> - 所属部门ID，用于数据过滤</p>
					</div>
				</template>
			</el-alert>
			<template #footer>
				<span class="dialog-footer">
					<el-button @click="onCancel" size="default">关 闭</el-button>
				</span>
			</template>
		</el-dialog>
	</div>
</template>

<script setup lang="ts" name="roleClaimDialog">
import { reactive } from 'vue';
import { useRouter } from 'vue-router';
import { ElMessage } from 'element-plus';
import { Plus } from '@element-plus/icons-vue';
import { IdentityRoleDto, IdentityClaimTypeDto } from '/@/api/models/identity';
import { useIdentityApi, useIdentityClaimTypeApi } from '/@/api/apis';

interface ClaimItem {
	id: string;
	claimType: string;
	claimValue: any;
	isEditing?: boolean;
	isNew?: boolean;
	originalType?: string;
	originalValue?: string;
}

const router = useRouter();

const state = reactive({
	roleId: '',
	roleName: '',
	claims: [] as ClaimItem[],
	claimTypes: [] as IdentityClaimTypeDto[],
	dialog: {
		isShowDialog: false,
	},
	loading: false,
});

const openDialog = async (row: IdentityRoleDto) => {
	state.roleId = row.id!;
	state.roleName = row.name!;
	state.dialog.isShowDialog = true;
	await Promise.all([loadRoleClaims(), loadClaimTypes()]);
};

const closeDialog = () => {
	state.dialog.isShowDialog = false;
	state.claims = [];
};

const onCancel = () => {
	closeDialog();
};

const loadRoleClaims = async () => {
	if (!state.roleId) return;

	state.loading = true;
	try {
		const { getRoleClaims } = useIdentityApi();
		const data = await getRoleClaims(state.roleId);
		state.claims = (data.items || []).map((item) => ({
			...item,
			claimValue: item.claimValue,
			isEditing: false,
			isNew: false,
		}));
	} finally {
		state.loading = false;
	}
};

const loadClaimTypes = async () => {
	const { getAllList } = useIdentityClaimTypeApi();
	const data = await getAllList();
	state.claimTypes = data.items || [];
};

const openClaimTypeManage = () => {
	router.push('/system/claim-type');
};

const getClaimTypeMeta = (claimType?: string) => {
	if (!claimType) return undefined;
	return state.claimTypes.find((item) => item.name === claimType);
};

const formatClaimTypeLabel = (item: IdentityClaimTypeDto) => {
	return `${item.name}（${formatValueType(item.valueType)}）`;
};

const normalizeClaimValue = (row: ClaimItem) => {
	const claimType = getClaimTypeMeta(row.claimType);
	if (claimType?.valueType === 'Int') {
		return row.claimValue === null || row.claimValue === undefined ? '' : String(row.claimValue);
	}
	if (claimType?.valueType === 'Boolean') {
		return row.claimValue === null || row.claimValue === undefined ? '' : String(row.claimValue);
	}
	return row.claimValue === null || row.claimValue === undefined ? '' : String(row.claimValue);
};

const onClaimTypeChange = (row: ClaimItem) => {
	row.claimValue = '';
};

const onAddClaim = () => {
	if (!state.claimTypes.length) {
		ElMessage.warning('请先维护声明类型');
		return;
	}

	state.claims.push({
		id: '',
		claimType: '',
		claimValue: '',
		isEditing: true,
		isNew: true,
	});
};

const onEditClaim = (row: ClaimItem) => {
	row.originalType = row.claimType;
	row.originalValue = String(row.claimValue ?? '');
	row.isEditing = true;
};

const onSaveClaim = async (row: ClaimItem) => {
	const claimValue = normalizeClaimValue(row);
	if (!row.claimType?.trim() || !claimValue.trim()) {
		ElMessage.warning('声明类型和值不能为空');
		return;
	}

	const { addRoleClaim, removeRoleClaim } = useIdentityApi();

	try {
		if (row.isNew) {
			await addRoleClaim(state.roleId, {
				claimType: row.claimType,
				claimValue,
			});
			ElMessage.success('添加成功');
		} else {
			if (row.id) {
				await removeRoleClaim(state.roleId, row.id);
				await addRoleClaim(state.roleId, {
					claimType: row.claimType,
					claimValue,
				});
			}
			ElMessage.success('修改成功');
		}
		await loadRoleClaims();
	} catch (error) {
		ElMessage.error('操作失败');
	}
};

const onCancelClaim = (row: ClaimItem, index: number) => {
	if (row.isNew) {
		state.claims.splice(index, 1);
	} else {
		row.claimType = row.originalType || row.claimType;
		row.claimValue = row.originalValue || row.claimValue;
		row.isEditing = false;
	}
};

const onDeleteClaim = async (row: ClaimItem) => {
	if (!row.id) return;

	try {
		const { removeRoleClaim } = useIdentityApi();
		await removeRoleClaim(state.roleId, row.id);
		ElMessage.success('删除成功');
		await loadRoleClaims();
	} catch (error) {
		ElMessage.error('删除失败');
	}
};

const formatValueType = (valueType: string) => {
	const displayMap: Record<string, string> = {
		String: '字符串',
		Int: '整数',
		Boolean: '布尔',
		DateTime: '日期时间',
	};
	return displayMap[valueType] || valueType;
};

defineExpose({
	openDialog,
});
</script>

<style scoped lang="scss">
.role-claim-dialog-container {
	.dialog-intro {
		margin-bottom: 12px;
		padding: 12px 14px;
		border-radius: 12px;
		background: var(--el-fill-color-light);
		color: var(--el-text-color-secondary);
		line-height: 1.7;
	}

	.claim-toolbar {
		display: flex;
		justify-content: flex-start;
		gap: 10px;
		margin-bottom: 12px;
	}

	.claim-summary {
		display: flex;
		flex-wrap: wrap;
		gap: 10px;
	}

	.claim-tips {
		margin-top: 15px;

		.tips-content {
			line-height: 1.8;
			font-size: 13px;

			code {
				background-color: #f4f4f5;
				padding: 2px 6px;
				border-radius: 3px;
				color: #409eff;
				font-family: monospace;
			}
		}
	}
}
</style>
