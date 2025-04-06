// composables/useAuth.ts
import { useAuthStore } from './authInfo';
import { storeToRefs } from 'pinia';

export function useAuth() {
	const authStore = useAuthStore();
	const { accessToken, expired, refreshToken, tokenType } = storeToRefs(authStore);

	const setAuth = authStore.setAuth;
	const clearAuth = authStore.clearAuth;
	const isLoggedIn = authStore.isLoggedIn;

	return {
		// state
		accessToken,
		expired,
		refreshToken,
		tokenType,

		// actions
		setAuth,
		clearAuth,

		// getters
		isLoggedIn,
	};
}
