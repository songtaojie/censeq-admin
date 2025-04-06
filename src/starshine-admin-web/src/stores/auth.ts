import { defineStore } from 'pinia';
import { Session } from '/@/utils/storage';

/**
 * 授权信息结构
 */
export interface AuthInfo {
	accessToken?: string;
	expiresAt: number | undefined;
	idToken?: string;
	refreshToken?: string;
	tokenType?: string;
	expired: boolean | undefined;
	expiresIn: number | undefined;
	scopes?: string[];
}

/**
 * 用户授权 Store
 */
export const useAuthStore = defineStore('auth', {
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
		isAuthenticated(state) {
			debugger;
			return !!state.accessToken && !state.expired;
		},
		// isAuthenticated: (state): boolean => !!state.accessToken && !state.expired,
	},

	actions: {
		setAuth(data: Partial<AuthInfo>) {
			debugger;
			this.accessToken = data.accessToken;
			this.expiresAt = data.expiresAt;
			this.idToken = data.idToken;
			this.refreshToken = data.refreshToken;
			this.tokenType = data.tokenType;
			this.expired = data.expired;
			this.expiresIn = data.expiresIn;
			this.scopes = data.scopes;
			Session.set('auth', data);
		},
		clearAuth() {
			this.$reset();
		},
		restoreFromStorage() {
			var data = Session.get('auth') as AuthInfo;
			if (data !== null) {
				this.setAuth(data);
			}
		},
	},
});
