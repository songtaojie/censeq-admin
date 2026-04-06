<template>
	<div class="role-claim-dialog-container">
		<el-dialog :title="`角色声明管理【${state.roleName}】`" v-model="state.dialog.isShowDialog" width="800px" destroy-on-close>
			<div class="claim-toolbar">
				<el-button type="primary" size="small" @click="onAddClaim" :icon="Plus">添加声明</el-button>
			</div>
			<el-table :data="state.claims" border size="small" v-loading="state.loading" height="350">
				<el-table-column prop="claimType" label="声明类型" min-width="180">
					<template #default="scope">
						<el-select v-if="scope.row.isEditing" v-model="scope.row.claimType" placeholder="选择或输入声明类型" size="small" allow-create filterable>
							<el-option label="数据范围 (DataScope)" value="DataScope" />
							<el-option label="最大金额 (MaxAmount)" value="MaxAmount" />
							<el-option label="部门ID (DepartmentId)" value="DepartmentId" />
						</el-select>
						<el-tag v-else size="small" type="info">{{ scope.row.claimType }}</el-tag>
					</template>
				</el-table-column>
				<el-table-column prop="claimValue" label="声明值" min-width="200">
					<template #default="scope">
						<template v-if="scope.row.isEditing">
							<el-select v-if="scope.row.claimType === 'DataScope'" v-model="scope.row.claimValue" placeholder="选择数据范围" size="small">
								<el-option label="全部数据 (All)" value="All" />
								<el-option label="本部门及以下数据 (DepartmentAndChild)" value="DepartmentAndChild" />
								<el-option label="本部门数据 (Department)" value="Department" />
								<el-option label="仅本人数据 (Self)" value="Self" />
								<el-option label="自定义数据 (Custom)" value="Custom" />
							</el-select>
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
import { reactive, ref } from 'vue';
import { ElMessage } from 'element-plus';
import { Plus } from '@element-plus/icons-vue';
import { IdentityRoleDto, IdentityRoleClaimDto } from '/@/api/models/identity';
import { useIdentityApi } from '/@/api/apis';

interface ClaimItem extends IdentityRoleClaimDto {
	isEditing?: boolean;
	isNew?: boolean;
	originalType?: string;
	originalValue?: string;
}

const state = reactive({
	roleId: '',
	roleName: '',
	claims: [] as ClaimItem[],
	dialog: {
		isShowDialog: false,
	},
	loading: false,
});

// 打开弹窗
const openDialog = (row: IdentityRoleDto) => {
	state.roleId = row.id!;
	state.roleName = row.name!;
	state.dialog.isShowDialog = true;
	loadRoleClaims();
};

// 关闭弹窗
const closeDialog = () => {
	state.dialog.isShowDialog = false;
	state.claims = [];
};

// 取消/关闭
const onCancel = () => {
	closeDialog();
};

// 加载角色声明
const loadRoleClaims = async () => {
	if (!state.roleId) return;

	state.loading = true;
	try {
		const { getRoleClaims } = useIdentityApi();
		const data = await getRoleClaims(state.roleId);
		state.claims = (data.items || []).map(item => ({
			...item,
			isEditing: false,
			isNew: false,
		}));
	} finally {
		state.loading = false;
	}
};

// 添加声明
const onAddClaim = () => {
	state.claims.push({
		id: '',
		claimType: '',
		claimValue: '',
		isEditing: true,
		isNew: true,
	});
};

// 编辑声明
const onEditClaim = (row: ClaimItem) => {
	row.originalType = row.claimType;
	row.originalValue = row.claimValue;
	row.isEditing = true;
};

// 保存声明
const onSaveClaim = async (row: ClaimItem) => {
	if (!row.claimType?.trim() || !row.claimValue?.trim()) {
		ElMessage.warning('声明类型和值不能为空');
		return;
	}

	const { addRoleClaim, removeRoleClaim } = useIdentityApi();

	try {
		if (row.isNew) {
			await addRoleClaim(state.roleId, {
				claimType: row.claimType,
				claimValue: row.claimValue,
			});
			ElMessage.success('添加成功');
		} else {
			// 先删除旧声明，再添加新声明
			if (row.id) {
				await removeRoleClaim(state.roleId, row.id);
				await addRoleClaim(state.roleId, {
					claimType: row.claimType,
					claimValue: row.claimValue,
				});
			}
			ElMessage.success('修改成功');
		}
		// 重新加载以获取最新数据
		await loadRoleClaims();
	} catch (error) {
		ElMessage.error('操作失败');
	}
};

// 取消编辑
const onCancelClaim = (row: ClaimItem, index: number) => {
	if (row.isNew) {
		state.claims.splice(index, 1);
	} else {
		row.claimType = row.originalType || row.claimType;
		row.claimValue = row.originalValue || row.claimValue;
		row.isEditing = false;
	}
};

// 删除声明
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

// 暴露变量
defineExpose({
	openDialog,
});
</script>

<style scoped lang="scss">
.role-claim-dialog-container {
	.claim-toolbar {
		margin-bottom: 15px;
		display: flex;
		justify-content: flex-end;
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
