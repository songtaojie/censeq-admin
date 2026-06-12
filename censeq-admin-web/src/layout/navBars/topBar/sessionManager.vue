<template>
	<el-drawer
		v-model="state.dialogVisible"
		title="在线用户列表"
		size="900px"
		direction="rtl"
		:close-on-click-modal="false"
		destroy-on-close
		@closed="handleDrawerClosed"
	>
		<el-table :data="sessionRows" v-loading="state.loading" border stripe>
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
			<el-table-column label="在线状态" width="130" align="center">
				<template #default="{ row }">
					<el-tag v-if="row.onlineConnections.length > 0" type="success" size="small" effect="light">
						在线 {{ row.onlineConnections.length }} 个连接
					</el-tag>
					<el-tag v-else type="info" size="small" effect="plain">离线</el-tag>
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
	<el-dialog v-model="state.detailVisible" title="会话详情" width="720px" append-to-body>
		<el-descriptions :column="1" border>
			<el-descriptions-item label="会话ID">{{ state.currentSession?.sessionId }}</el-descriptions-item>
			<el-descriptions-item label="用户名">{{ userInfos.userName || '-' }}</el-descriptions-item>
			<el-descriptions-item label="设备类型">{{ state.currentSession?.device }}</el-descriptions-item>
			<el-descriptions-item label="设备信息">{{ state.currentSession?.deviceInfo || '-' }}</el-descriptions-item>
			<el-descriptions-item label="客户端ID">{{ state.currentSession?.clientId || '-' }}</el-descriptions-item>
			<el-descriptions-item label="IP地址">{{ state.currentSession?.ipAddresses || '-' }}</el-descriptions-item>
			<el-descriptions-item label="登录时间">{{ formatDateTime(state.currentSession?.signedIn) }}</el-descriptions-item>
			<el-descriptions-item label="最后访问">{{ formatDateTime(state.currentSession?.lastAccessed) }}</el-descriptions-item>
		</el-descriptions>
		<div class="session-connections">
			<div class="session-connections__title">当前 SignalR 连接</div>
			<el-table v-if="currentSessionConnections.length > 0" :data="currentSessionConnections" border size="small">
				<el-table-column label="连接ID" min-width="220" show-overflow-tooltip>
					<template #default="{ row }">{{ row.connectionId }}</template>
				</el-table-column>
				<el-table-column label="上线时间" width="170" show-overflow-tooltip>
					<template #default="{ row }">{{ formatDateTime(row.connectedAt) }}</template>
				</el-table-column>
				<el-table-column label="IP地址" width="120" show-overflow-tooltip>
					<template #default="{ row }">{{ row.ip || '-' }}</template>
				</el-table-column>
				<el-table-column label="浏览器" min-width="220" show-overflow-tooltip>
					<template #default="{ row }">{{ formatDeviceInfo(row.userAgent) }}</template>
				</el-table-column>
			</el-table>
			<el-empty v-else description="当前会话暂无实时连接，可能是浏览器已关闭但登录会话仍有效。" :image-size="80" />
		</div>
	</el-dialog>
</template>

<script setup lang="ts" name="sessionManager">
import { reactive, computed } from 'vue';
import { ElMessageBox, ElMessage } from 'element-plus';
import { storeToRefs } from 'pinia';
import { useIdentityApi } from '/@/api/apis/identity/identity-role.service';
import { useUserInfo } from '/@/composables/useUserInfo';
import { forceOffline, subscribeMyOnlineUsers, unsubscribeMyOnlineUsers } from '/@/composables/useOnlineUserHub';
import { useOnlineUserStore } from '/@/stores/onlineUser';
import type { IdentitySessionDto } from '/@/api/models/identity';
import type { SessionWithOnlineConnections } from './sessionOnlineStatus';
import { mergeSessionsWithOnlineConnections } from './sessionOnlineStatus';

const { getMySessions, revokeMySession, revokeAllOtherSessions } = useIdentityApi();
const { userInfos } = useUserInfo();
const onlineUserStore = useOnlineUserStore();
const { users: onlineUsers } = storeToRefs(onlineUserStore);

const state = reactive({
	dialogVisible: false,
	detailVisible: false,
	loading: false,
	sessionList: [] as IdentitySessionDto[],
	currentSession: null as SessionWithOnlineConnections | null,
});

const sessionRows = computed(() => mergeSessionsWithOnlineConnections(state.sessionList, onlineUsers.value));

const hasOtherSessions = computed(() => {
	return state.sessionList.some((s) => !s.isCurrentSession);
});

const currentSessionConnections = computed(() => {
	if (!state.currentSession) return [];
	const match = sessionRows.value.find((session) => session.sessionId === state.currentSession?.sessionId);
	return match?.onlineConnections ?? [];
});

// 打开弹窗
const open = async () => {
	state.dialogVisible = true;
	await Promise.all([loadSessions(), loadOnlineUsers()]);
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

// 加载实时在线连接
const loadOnlineUsers = async () => {
	try {
		await subscribeMyOnlineUsers();
	} catch (error) {
		console.error('加载实时连接失败', error);
		ElMessage.warning('实时在线状态加载失败，会话列表仍可正常查看');
	}
};

// 强制下线
const handleForceOffline = async (row: SessionWithOnlineConnections) => {
	try {
		const result = await ElMessageBox.prompt(
			`请输入强制下线提示内容\n登录时间：${formatDateTime(row.signedIn)}`,
			'强制下线',
			{
				confirmButtonText: '强制下线',
				cancelButtonText: '取消',
				type: 'warning',
				inputType: 'textarea',
				inputValue: '您的登录会话已被管理员强制下线，请重新登录',
				inputValidator: (value) => {
					if (!value || !value.trim()) return '请输入下线提示内容';
					return true;
				},
			}
		);
		const reason = result.value.trim();
		await forceOfflineSessionConnections(row, reason);
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
		const result = await ElMessageBox.prompt(
			'请输入强制下线其他会话的提示内容',
			'强制下线其他会话',
			{
				confirmButtonText: '强制下线',
				cancelButtonText: '取消',
				type: 'warning',
				inputType: 'textarea',
				inputValue: '您的其他登录会话已被管理员强制下线，请重新登录',
				inputValidator: (value) => {
					if (!value || !value.trim()) return '请输入下线提示内容';
					return true;
				},
			}
		);
		const reason = result.value.trim();
		const otherSessions = sessionRows.value.filter((session) => !session.isCurrentSession);
		await Promise.all(otherSessions.map((session) => forceOfflineSessionConnections(session, reason)));
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

const forceOfflineSessionConnections = async (session: SessionWithOnlineConnections, reason: string) => {
	if (session.onlineConnections.length === 0) return;
	await Promise.all(session.onlineConnections.map((connection) => forceOffline(connection.connectionId, reason)));
};

// 查看详情
const handleViewDetail = (row: SessionWithOnlineConnections) => {
	state.currentSession = row;
	state.detailVisible = true;
};

const handleDrawerClosed = () => {
	state.detailVisible = false;
	state.currentSession = null;
	void unsubscribeMyOnlineUsers();
};

// 格式化日期时间
const formatDateTime = (dateStr?: string) => {
	if (!dateStr) return '-';
	const date = new Date(dateStr);
	if (Number.isNaN(date.getTime())) return '-';
	const pad = (value: number) => value.toString().padStart(2, '0');
	return `${date.getFullYear()}-${date.getMonth() + 1}-${date.getDate()} ${pad(date.getHours())}:${pad(date.getMinutes())}:${pad(date.getSeconds())}`;
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

.session-connections {
	margin-top: 16px;
}

.session-connections__title {
	margin-bottom: 8px;
	font-weight: 600;
	color: var(--el-text-color-primary);
}
</style>
