// composables/useOidcAuth.ts
import { useAuth } from './useAuth';
import { User, UserManager, WebStorageStateStore, UserManagerSettings, Log, Logger } from 'oidc-client-ts';
Log.setLogger(console);
Log.setLevel(Log.DEBUG);
const oidcConfig: UserManagerSettings = {
	authority: import.meta.env.VITE_OIDC_AUTHORITY, // IdentityServer 地址
	client_id: import.meta.env.VITE_OIDC_CLIENT_ID,
	redirect_uri: window.location.origin + '/callback',
	silent_redirect_uri: window.location.origin + '/silent-renew.html',
	response_type: import.meta.env.VITE_OIDC_RESPONSE_TYPE,
	scope: import.meta.env.VITE_OIDC_SCOPE,
	post_logout_redirect_uri: window.location.origin + '/logout-callback',
	userStore: new WebStorageStateStore({ store: window.sessionStorage }),
	automaticSilentRenew: true,
	monitorSession: true,
};

const userManager = new UserManager(oidcConfig);
const auth = useAuth();
console.log('useOidc,id', auth.$id);
export function useOidc() {
	// 发起登录
	const login = async () => {
		await userManager.signinRedirect();
	};

	const setAuthInfo = async (user: User | null) => {
		if (user != null) {
			auth.setAuth({
				accessToken: user.access_token,
				refreshToken: user.refresh_token,
				idToken: user.id_token,
				expired: user.expired,
				expiresIn: user.expires_in,
				expiresAt: user.expires_at ?? Date.now() + (user.expires_in ?? 0) * 1000,
			});
		}
	};

	// 处理回调（callback 页面中调用）
	const handleRedirectCallback = async () => {
		try {
			debugger;
			const user = await userManager.signinRedirectCallback();
			setAuthInfo(user);
			// 重定向到主页或上次页面
			window.location.replace('/');
		} catch (err) {
			Logger.error('OIDC 回调失败:', err);
		}
	};

	// 静默续期（你可以在全局路由守卫中使用）
	const trySilentRenew = async () => {
		try {
			const user = await userManager.signinSilent();
			setAuthInfo(user);
		} catch (err) {
			Logger.warn('Token 静默续期失败:', err);
		}
	};

	// 退出登录
	const logout = async () => {
		auth.clearAuth();
		await userManager.signoutRedirect();
	};

	// 处理静默续期回调（callback 页面中调用）
	const handleSilentCallback = async () => {
		try {
			userManager
				.signinSilentCallback()
				.then(() => {
					Logger.info('[silent-renew] 成功完成 Token 更新');
				})
				.catch((err) => {
					Logger.error('[silent-renew] 续期失败:', err);
				});
		} catch (err) {
			Logger.error('OIDC 回调失败:', err);
		}
	};
	return {
		login,
		handleRedirectCallback,
		handleSilentCallback,
		trySilentRenew,
		logout,
	};
}
