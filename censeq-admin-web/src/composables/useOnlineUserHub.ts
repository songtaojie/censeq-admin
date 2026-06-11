import { useSignalR } from '/@/composables/useSignalR';
import type { ForceOfflineInput, OnlineUserChange, OnlineUserInfo } from '/@/api/models/signalr';
import { useOnlineUserStore } from '/@/stores/onlineUser';

const onlineUserHub = useSignalR({ hubUrl: '/hubs/online-user' });
let forceOfflineRegistered = false;

export async function startOnlineUserHub() {
	const store = useOnlineUserStore();
	await onlineUserHub.start();
	if (!forceOfflineRegistered) {
		await onlineUserHub.on<string>('ForceOffline', (reason) => void store.handleForceOffline(reason));
		forceOfflineRegistered = true;
	}
}

export async function subscribeOnlineUsers() {
	const store = useOnlineUserStore();
	await startOnlineUserHub();
	await onlineUserHub.on<OnlineUserChange>('OnlineChanged', (change) => store.applyChange(change));
	await onlineUserHub.on<OnlineUserInfo[]>('OnlineList', (list) => store.replaceAll(list));

	const list = await onlineUserHub.invoke<OnlineUserInfo[]>('SubscribeOnlineUsers');
	store.replaceAll(list);
}

export async function unsubscribeOnlineUsers() {
	onlineUserHub.off('OnlineChanged');
	onlineUserHub.off('OnlineList');
	if (!onlineUserHub.isConnected()) return;
	try {
		await onlineUserHub.invoke('UnsubscribeOnlineUsers');
	} catch {
		// 页面离开或连接断开时无需打断用户流程。
	}
}

export async function stopOnlineUserHub() {
	await onlineUserHub.stop();
	forceOfflineRegistered = false;
}

export async function forceOffline(connectionId: string, reason?: string) {
	await startOnlineUserHub();
	const input: ForceOfflineInput = { connectionId, reason };
	await onlineUserHub.invoke('ForceOffline', input);
}
