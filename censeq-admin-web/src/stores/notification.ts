import { defineStore } from 'pinia';
import type { NotificationMessage } from '/@/api/models/signalr';

interface NotificationState {
	messages: NotificationMessage[];
	unreadCount: number;
}

export const useNotificationStore = defineStore('notification', {
	state: (): NotificationState => ({
		messages: [],
		unreadCount: 0,
	}),
	actions: {
		receive(message: NotificationMessage) {
			this.messages = [message, ...this.messages].slice(0, 100);
			this.unreadCount += 1;
		},
		markAllRead() {
			this.unreadCount = 0;
		},
		clear() {
			this.messages = [];
			this.unreadCount = 0;
		},
	},
});
