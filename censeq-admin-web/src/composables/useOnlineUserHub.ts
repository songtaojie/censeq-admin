import { useSignalR } from '/@/composables/useSignalR';
import type { ForceOfflineInput, OnlineUserChange, OnlineUserInfo } from '/@/api/models/signalr';
import { useOnlineUserStore } from '/@/stores/onlineUser';

const onlineUserHub = useSignalR({ hubUrl: '/hubs/online-user' });

export async function startOnlineUserHub() {
	const store = useOnlineUserStore();
	await onlineUserHub.start();
	await onlineUserHub.on<OnlineUserChange>('OnlineChanged', (change) => store.applyChange(change));
	await onlineUserHub.on<OnlineUserInfo[]>('OnlineList', (list) => store.replaceAll(list));
	await onlineUserHub.on<string>('ForceOffline', (reason) => void store.handleForceOffline(reason));

	const list = await onlineUserHub.invoke<OnlineUserInfo[]>('GetOnlineList');
	store.replaceAll(list);
}

export async function stopOnlineUserHub() {
	await onlineUserHub.stop();
}

export async function forceOffline(connectionId: string, reason?: string) {
	const input: ForceOfflineInput = { connectionId, reason };
	await onlineUserHub.invoke('ForceOffline', input);
}
