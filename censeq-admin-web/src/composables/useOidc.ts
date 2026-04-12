import { Session } from '/@/utils/storage';

import { User, UserManager, WebStorageStateStore, UserManagerSettings, Log, Logger } from 'oidc-client-ts';
Log.setLogger(console);
Log.setLevel(Log.DEBUG);
const webStorageStateStore = new WebStorageStateStore({ store: window.sessionStorage });
const oidcConfig: UserManagerSettings = {
	authority: import.meta.env.VITE_OIDC_AUTHORITY, // IdentityServer 地址
	client_id: import.meta.env.VITE_OIDC_CLIENT_ID,
	redirect_uri: `${window.location.origin}/callback`,
	silent_redirect_uri: window.location.origin + '/silent-renew.html',
	response_type: import.meta.env.VITE_OIDC_RESPONSE_TYPE,
	scope: import.meta.env.VITE_OIDC_SCOPE,
	post_logout_redirect_uri: window.location.origin + '/logout-callback',
	userStore: webStorageStateStore,
	stateStore: webStorageStateStore,
	automaticSilentRenew: true, //此标记用于指示是否应在访问令牌到期前自动尝试续订。自动续订尝试在访问令牌到期前 1 分钟开始（默认值：true）
	monitorSession: true, //当用户在 OP 上执行退出时将引发事件（默认值：false）
	loadUserInfo: true,
};
const userManager = new UserManager(oidcConfig);
export function useOidc() {
	// 发起登录
	const login = async () => {
		await userManager.signinRedirect();
	};

	const setUser = async (user: User | null) => {
		if (user != null && user.profile != null) {
			const cached = (Session.get('userInfo') as Partial<UserInfos> | undefined) ?? {};
			var userInfos = {
				authBtnList: cached.authBtnList ?? [],
				userName: user.profile.preferred_username ?? cached.userName ?? '',
				time: new Date().getTime(),
				photo: user.profile.picture ?? cached.photo ?? '/upload/logo.png',
				roles: cached.roles ?? [],
			};
			Session.set('userInfo', userInfos);
		}
	};

	const getCurrentUser = async () => {
		return await userManager.getUser();
	};

	// 处理回调（callback 页面中调用）
	const handleRedirectCallback = async () => {
		try {
			const user = await userManager.signinCallback();
			console.log('回调中的user', user);
		} catch (err) {
			Logger.error('OIDC 回调失败:', err);
		}
	};

	// 静默续期（你可以在全局路由守卫中使用）
	const trySilentRenew = async () => {
		try {
			const user = await userManager.signinSilent();
			setUser(user);
		} catch (err) {
			Logger.warn('Token 静默续期失败:', err);
		}
	};

	// 退出登录
	const logout = async () => {
		await userManager.signoutRedirect();
	};

	// 处理静默续期回调（callback 页面中调用）
	const signinSilentCallback = async () => {
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
	// 事件
	userManager.events.addUserLoaded(async (user) => {
		await setUser(user);
	});
	const isAuthenticated = async () => {
		var user = await userManager.getUser();
		return !!(user && !user.expired);
	};

	// 获取登录用户的访问令牌
	const getAcessToken = () => {
		return new Promise<string | null>((resolve, reject) => {
			userManager
				.getUser()
				.then(async (user) => {
					if (user == null) {
						await login();
						return resolve(null);
					}
					return resolve(user.access_token);
				})
				.catch((err) => {
					Logger.error(err);
					return reject(err);
				});
		});
	};
	return {
		login,
		handleRedirectCallback,
		signinSilentCallback,
		trySilentRenew,
		logout,
		getCurrentUser,
		getAcessToken,
		isAuthenticated,
	};
}
