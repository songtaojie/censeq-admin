import { defineStore, storeToRefs } from 'pinia';
import { isEmpty } from '/@/utils/other';
/**
 * 用户授权配置
 */
const useAuthStore = defineStore('auth', {
	state: (): AuthInfo => ({
		accessToken: undefined,
		expiresAt: 0,
		idToken: undefined,
		refreshToken: undefined,
		tokenType: undefined,
		expired: true,
		expiresIn: 0,
		scopes: undefined,
	}),
	getters: {
		isAuthenticated: (state): boolean => !isEmpty(state.accessToken) && !state.expired,
	},
	actions: {
		setAuth(data: Partial<AuthInfo>) {
			Object.assign(this, data);
		},
		clearAuth() {
			this.$reset();
		},
	},
});

export function useAuth() {
	const authStore = useAuthStore();
	const { accessToken, expired, refreshToken, tokenType } = storeToRefs(authStore);

	const setAuth = authStore.setAuth;
	const clearAuth = authStore.clearAuth;
	const isAuthenticated = authStore.isAuthenticated;

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
		isAuthenticated,
	};
}
