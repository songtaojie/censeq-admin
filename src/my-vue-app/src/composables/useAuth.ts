import { storeToRefs } from 'pinia';
import { useAuthStore } from '/@/stores/auth';

export function useAuth() {
	const store = useAuthStore();
	const { accessToken, expired } = storeToRefs(store);

	return {
		$id: store.$id,
		accessToken,
		expired,
		isAuthenticated: store.isAuthenticated,
		setAuth: store.setAuth,
		clearAuth: store.clearAuth,
		restoreFromStorage: store.restoreFromStorage,
	};
}
