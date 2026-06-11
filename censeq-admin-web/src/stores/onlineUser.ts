import { defineStore } from 'pinia';
import { ElMessageBox } from 'element-plus';
import type { OnlineUserChange, OnlineUserInfo } from '/@/api/models/signalr';
import { useOidc } from '/@/composables/useOidc';

interface OnlineUserState {
	users: OnlineUserInfo[];
	forceOfflineReason: string;
}

export const useOnlineUserStore = defineStore('onlineUser', {
	state: (): OnlineUserState => ({
		users: [],
		forceOfflineReason: '',
	}),
	getters: {
		count: (state) => state.users.length,
	},
	actions: {
		replaceAll(users: OnlineUserInfo[]) {
			this.users = [...users].sort((a, b) => new Date(b.connectedAt).getTime() - new Date(a.connectedAt).getTime());
		},
		applyChange(change: OnlineUserChange) {
			if (change.online) {
				const exists = this.users.some((x) => x.connectionId === change.user.connectionId);
				this.replaceAll(exists ? this.users.map((x) => (x.connectionId === change.user.connectionId ? change.user : x)) : [...this.users, change.user]);
				return;
			}

			this.users = this.users.filter((x) => x.connectionId !== change.user.connectionId);
		},
		async handleForceOffline(reason?: string) {
			this.forceOfflineReason = reason || '您已被管理员强制下线';
			await ElMessageBox.alert(this.forceOfflineReason, '提示', {
				type: 'warning',
				confirmButtonText: '重新登录',
				closeOnClickModal: false,
				closeOnPressEscape: false,
			}).catch(() => {});
			await useOidc().logout();
		},
	},
});
