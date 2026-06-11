import { ElNotification } from 'element-plus';
import { useSignalR } from '/@/composables/useSignalR';
import type { NotificationMessage } from '/@/api/models/signalr';
import { useNotificationStore } from '/@/stores/notification';

const notificationHub = useSignalR({ hubUrl: '/hubs/notification' });

export async function startNotificationHub() {
	const store = useNotificationStore();
	await notificationHub.start();
	await notificationHub.on<NotificationMessage>('ReceiveMessage', (message) => {
		store.receive(message);
		ElNotification({
			title: message.title || '通知',
			message: message.content,
			type: message.type as any,
		});
	});
}

export async function stopNotificationHub() {
	await notificationHub.stop();
}
