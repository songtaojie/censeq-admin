<template>
	<el-drawer
		v-model="state.dialogVisible"
		title="在线用户列表"
		size="900px"
		direction="rtl"
		:close-on-click-modal="false"
		destroy-on-close
	>
		<el-table :data="state.sessionList" v-loading="state.loading" border stripe>
			<el-table-column type="index" label="序号" width="60" align="center" />
			<el-table-column label="账号" min-width="120" show-overflow-tooltip>
				<template #default="{ row }">
					{{ userInfos.userName }}
					<el-tag v-if="row.isCurrentSession" type="success" size="small" class="ml5">当前</el-tag>
				</template>
			</el-table-column>
			<el-table-column label="姓名" min-width="100" show-overflow-tooltip>
				<template #default>
					{{ userInfos.realName || userInfos.userName }}
				</template>
			</el-table-column>
			<el-table-column label="登录时间" min-width="160" show-overflow-tooltip>
				<template #default="{ row }">
					{{ formatDateTime(row.signedIn) }}
				</template>
			</el-table-column>
			<el-table-column label="最后访问" min-width="160" show-overflow-tooltip>
				<template #default="{ row }">
					{{ row.lastAccessed ? formatDateTime(row.lastAccessed) : '-' }}
				</template>
			</el-table-column>
			<el-table-column label="IP地址" min-width="130" show-overflow-tooltip>
				<template #default="{ row }">
					{{ formatIpAddresses(row.ipAddresses) }}
				</template>
			</el-table-column>
			<el-table-column label="浏览器" min-width="150" show-overflow-tooltip>
				<template #default="{ row }">
					{{ formatDeviceInfo(row.deviceInfo) }}
				</template>
			</el-table-column>
			<el-table-column label="操作" width="120" fixed="right" align="center">
				<template #default="{ row }">
					<el-button
						v-if="!row.isCurrentSession"
						type="danger"
						link
						size="small"
						@click="handleForceOffline(row)"
						title="强制下线"
					>
						<el-icon><ele-CircleClose /></el-icon>
					</el-button>
					<el-button
						type="primary"
						link
						size="small"
						@click="handleViewDetail(row)"
						title="查看详情"
					>
						<el-icon><ele-View /></el-icon>
					</el-button>
				</template>
			</el-table-column>
		</el-table>
		<template #footer>
			<div class="drawer-footer">
				<el-button type="danger" @click="handleForceOfflineAll" :disabled="!hasOtherSessions">
					强制下线其他会话
				</el-button>
				<el-button @click="state.dialogVisible = false">关闭</el-button>
			</div>
		</template>
	</el-drawer>

	<!-- 详情弹窗 -->
	<el-dialog v-model="state.detailVisible" title="会话详情" width="500px" append-to-body>
		<el-descriptions :column="1" border>
			<el-descriptions-item label="会话ID">{{ state.currentSession?.sessionId }}</el-descriptions-item>
			<el-descriptions-item label="用户ID">{{ state.currentSession?.userId }}</el-descriptions-item>
			<el-descriptions-item label="设备类型">{{ state.currentSession?.device }}</el-descriptions-item>
			<el-descriptions-item label="设备信息">{{ state.currentSession?.deviceInfo || '-' }}</el-descriptions-item>
			<el-descriptions-item label="客户端ID">{{ state.currentSession?.clientId || '-' }}</el-descriptions-item>
			<el-descriptions-item label="IP地址">{{ state.currentSession?.ipAddresses || '-' }}</el-descriptions-item>
			<el-descriptions-item label="登录时间">{{ formatDateTime(state.currentSession?.signedIn) }}</el-descriptions-item>
			<el-descriptions-item label="最后访问">{{ formatDateTime(state.currentSession?.lastAccessed) }}</el-descriptions-item>
		</el-descriptions>
	</el-dialog>
</template>

<script setup lang="ts" name="sessionManager">
import { reactive, computed } from 'vue';
import { ElMessageBox, ElMessage } from 'element-plus';
import { useIdentityApi } from '/@/api/apis/identity/identity-role.service';
import { useUserInfo } from '/@/composables/useUserInfo';
import type { IdentitySessionDto } from '/@/api/models/identity';

const { getMySessions, revokeMySession, revokeAllOtherSessions } = useIdentityApi();
const { userInfos } = useUserInfo();

const state = reactive({
	dialogVisible: false,
	detailVisible: false,
	loading: false,
	sessionList: [] as IdentitySessionDto[],
	currentSession: null as IdentitySessionDto | null,
});

const hasOtherSessions = computed(() => {
	return state.sessionList.some((s) => !s.isCurrentSession);
});

// 打开弹窗
const open = async () => {
	state.dialogVisible = true;
	await loadSessions();
};

// 加载会话列表
const loadSessions = async () => {
	state.loading = true;
	try {
		const res = await getMySessions();
		state.sessionList = res || [];
	} catch (error) {
		console.error('加载会话列表失败', error);
		ElMessage.error('加载会话列表失败');
	} finally {
		state.loading = false;
	}
};

// 强制下线
const handleForceOffline = async (row: IdentitySessionDto) => {
	try {
		await ElMessageBox.confirm(
			`确定要强制下线该会话吗？\n登录时间：${formatDateTime(row.signedIn)}`,
			'提示',
			{
				confirmButtonText: '确定',
				cancelButtonText: '取消',
				type: 'warning',
			}
		);
		await revokeMySession(row.sessionId);
		ElMessage.success('强制下线成功');
		await loadSessions();
	} catch (error: any) {
		if (error !== 'cancel') {
			console.error('强制下线失败', error);
			ElMessage.error('强制下线失败');
		}
	}
};

// 强制下线所有其他会话
const handleForceOfflineAll = async () => {
	try {
		await ElMessageBox.confirm(
			'确定要强制下线所有其他会话吗？',
			'提示',
			{
				confirmButtonText: '确定',
				cancelButtonText: '取消',
				type: 'warning',
			}
		);
		await revokeAllOtherSessions();
		ElMessage.success('强制下线成功');
		await loadSessions();
	} catch (error: any) {
		if (error !== 'cancel') {
			console.error('强制下线失败', error);
			ElMessage.error('强制下线失败');
		}
	}
};

// 查看详情
const handleViewDetail = (row: IdentitySessionDto) => {
	state.currentSession = row;
	state.detailVisible = true;
};

// 格式化日期时间
const formatDateTime = (dateStr?: string) => {
	if (!dateStr) return '-';
	const date = new Date(dateStr);
	return date.toLocaleString('zh-CN');
};

// 格式化IP地址
const formatIpAddresses = (ipStr?: string) => {
	if (!ipStr) return '-';
	const ips = ipStr.split(',');
	return ips[0] || '-';
};

// 格式化设备信息
const formatDeviceInfo = (deviceInfo?: string) => {
	if (!deviceInfo) return '未知浏览器';
	// 提取浏览器名称和版本
	const browserMatch = deviceInfo.match(/(Chrome|Firefox|Safari|Edge|IE)[\/\s]([\d.]+)/i);
	if (browserMatch) {
		return `${browserMatch[1]} ${browserMatch[2]}`;
	}
	// 如果太长则截断
	if (deviceInfo.length > 30) {
		return deviceInfo.substring(0, 30) + '...';
	}
	return deviceInfo;
};

// 暴露方法
defineExpose({
	open,
});
</script>

<style scoped lang="scss">
.drawer-footer {
	display: flex;
	justify-content: flex-end;
	gap: 10px;
}
</style>
