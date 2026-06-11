<template>
	<div class="online-user-container layout-padding">
		<el-card shadow="hover" :body-style="{ paddingBottom: '0' }">
			<el-form :inline="true">
				<el-form-item label="当前在线">
					<el-tag type="success" effect="light">{{ onlineUsers.length }} 个连接</el-tag>
				</el-form-item>
				<el-form-item>
					<el-button-group>
						<el-button type="primary" icon="ele-Refresh" @click="refreshOnlineUsers">刷新</el-button>
						<el-button icon="ele-Connection" @click="reconnect">重连</el-button>
					</el-button-group>
				</el-form-item>
			</el-form>
		</el-card>

		<el-card class="full-table" shadow="hover" style="margin-top: 5px">
			<el-table :data="onlineUsers" style="width: 100%" border stripe>
				<el-table-column type="index" label="序号" width="60" align="center" fixed />
				<el-table-column prop="userName" label="账号" min-width="130" show-overflow-tooltip>
					<template #default="{ row }">
						<span class="online-user-name">{{ row.userName || '—' }}</span>
					</template>
				</el-table-column>
				<el-table-column prop="name" label="姓名" min-width="120" show-overflow-tooltip>
					<template #default="{ row }">{{ row.name || '—' }}</template>
				</el-table-column>
				<el-table-column prop="ip" label="IP" width="150" show-overflow-tooltip>
					<template #default="{ row }">{{ row.ip || '—' }}</template>
				</el-table-column>
				<el-table-column prop="userAgent" label="浏览器" min-width="260" show-overflow-tooltip>
					<template #default="{ row }">{{ simplifyUserAgent(row.userAgent) }}</template>
				</el-table-column>
				<el-table-column prop="sessionId" label="会话" min-width="150" show-overflow-tooltip>
					<template #default="{ row }">{{ row.sessionId || '—' }}</template>
				</el-table-column>
				<el-table-column label="上线时间" width="170" show-overflow-tooltip>
					<template #default="{ row }">{{ formatDate(row.connectedAt) }}</template>
				</el-table-column>
				<el-table-column label="状态" width="90" align="center">
					<template #default>
						<el-tag type="success" size="small" effect="dark">在线</el-tag>
					</template>
				</el-table-column>
				<el-table-column label="操作" width="120" fixed="right" align="center">
					<template #default="{ row }">
						<el-button icon="ele-SwitchButton" size="small" text type="danger" @click="onForceOffline(row)">强制下线</el-button>
					</template>
				</el-table-column>
			</el-table>
		</el-card>
	</div>
</template>

<script setup lang="ts" name="systemOnlineUser">
import { computed, onMounted } from 'vue';
import { ElMessage, ElMessageBox } from 'element-plus';
import { storeToRefs } from 'pinia';
import type { OnlineUserInfo } from '/@/api/models/signalr';
import { forceOffline, startOnlineUserHub, stopOnlineUserHub } from '/@/composables/useOnlineUserHub';
import { useOnlineUserStore } from '/@/stores/onlineUser';

const store = useOnlineUserStore();
const { users } = storeToRefs(store);
const onlineUsers = computed(() => users.value);

const formatDate = (val?: string) => {
	if (!val) return '—';
	const date = new Date(val);
	if (Number.isNaN(date.getTime())) return val.replace('T', ' ').substring(0, 19);
	return date.toLocaleString();
};

const simplifyUserAgent = (value?: string | null) => {
	if (!value) return '—';
	return value.length > 90 ? `${value.slice(0, 90)}...` : value;
};

const refreshOnlineUsers = async () => {
	await startOnlineUserHub();
	ElMessage.success('在线列表已刷新');
};

const reconnect = async () => {
	await stopOnlineUserHub();
	await startOnlineUserHub();
	ElMessage.success('SignalR 已重连');
};

const onForceOffline = async (row: OnlineUserInfo) => {
	const result = await ElMessageBox.prompt(`请输入强制下线「${row.userName}」的原因`, '强制下线', {
		inputValue: '您已被管理员强制下线',
		inputValidator: (v) => (!v || v.trim().length === 0 ? '请输入原因' : true),
		confirmButtonText: '确 定',
		cancelButtonText: '取 消',
		type: 'warning',
	}).catch(() => null);
	if (!result) return;

	await forceOffline(row.connectionId, result.value);
	ElMessage.success('已发送强制下线指令');
};

onMounted(() => {
	void startOnlineUserHub();
});
</script>

<style scoped lang="scss">
.online-user-container {
	display: flex;
	flex-direction: column;

	:deep(.full-table) {
		flex: 1;
	}
}

.online-user-name {
	color: var(--el-color-primary);
	font-weight: 600;
}
</style>
