// services/AuthService.ts
import { UserManager, User, UserManagerSettings, WebStorageStateStore, UserManagerEvents, Log, Logger } from 'oidc-client-ts';
import { Ref, ref, computed } from 'vue';
import { useAuthStore } from '../stores/authInfo';
import { storeToRefs } from 'pinia';

const authStore = useAuthStore();
const { authInfo } = storeToRefs(authStore);
authStore.setAuthInfo({
	authInfo: {},
});
Log.setLogger(console);
export enum SignInType {
	Window,
	Popup,
}

export class AuthService {
	private readonly authName: string;
	private readonly appUrl: string;
	private readonly defaultSignInType: SignInType;
	private readonly userManager: UserManager;
	private _inited = false;
	private router: any = null;

	public readonly user: Ref<User | null> = ref(null);

	constructor(authName: string, defaultSignInType: SignInType, appUrl: string, oidcConfig: UserManagerSettings) {
		if (!authName || !appUrl || !oidcConfig) {
			throw new Error('Missing required OIDC config');
		}

		this.authName = authName;
		this.appUrl = appUrl;
		this.defaultSignInType = defaultSignInType;
		const config: UserManagerSettings = {
			automaticSilentRenew: true,
			monitorSession: true,
			userStore: new WebStorageStateStore({ store: window.sessionStorage }),
			...oidcConfig,
		};
		this.userManager = new UserManager(config);
		this._initEvents();
	}

	public get isAuthenticated() {
		return computed(() => !!this.user.value && !this.user.value.expired);
	}

	public get accessToken() {
		return computed(() => (this.isAuthenticated.value ? this.user.value!.access_token : ''));
	}

	public setRouter(router: any) {
		this.router = router;
	}

	public async init(): Promise<boolean> {
		if (this._isCallback()) return false;
		if (this._inited) return true;

		try {
			const existingUser = await this.userManager.getUser();
			if (existingUser && !existingUser.expired) {
				this.user.value = existingUser;
			}
			this._inited = true;
			return true;
		} catch {
			return false;
		}
	}

	public signIn(args?: any) {
		return this._signIn(this.defaultSignInType, args);
	}

	public signOut(args?: any) {
		if (this.defaultSignInType === SignInType.Popup) {
			return this.userManager.signoutPopup(args).finally(() => {
				this._redirectAfterSignout();
			});
		}
		return this.userManager.signoutRedirect(args);
	}

	/**
	 * 处理来自授权端点的响应.
	 */
	public signinCallback(url?: string): Promise<User | undefined> {
		return this.userManager.signinCallback(url);
	}

	/**
	 * 为当前已验证的用户加载“用户”对象。
	 *
	 * @returns A promise
	 */
	public getUser(): Promise<User | null> {
		return this.userManager.getUser();
	}

	private _signIn(type: SignInType, args?: any) {
		if (type === SignInType.Popup) {
			return this.userManager.signinPopup(args);
		}
		return this.userManager.signinRedirect(args);
	}

	private _isCallback(): boolean {
		debugger;
		const path = window.location.pathname.toLowerCase();
		const match = (url?: string) => url && path === this._getPath(url).toLowerCase();

		if (match(this.userManager.settings.popup_redirect_uri)) {
			this.userManager.signinPopupCallback();
			return true;
		} else if (match(this.userManager.settings.silent_redirect_uri)) {
			this.userManager.signinSilentCallback();
			return true;
		} else if (match(this.userManager.settings.popup_post_logout_redirect_uri)) {
			this.userManager.signoutPopupCallback();
			return true;
		}
		return false;
	}

	private _redirectAfterSignout() {
		if (this.router) {
			// const current = this.router.currentRoute
			// if (current?.meta?.authName === this.authName) {

			// }
			this.router.replace('/');
			return;
		}
		window.location.href = this.appUrl;
	}

	private _initEvents() {
		const events: UserManagerEvents = this.userManager.events;
		events.addUserSignedIn(() => {
			Logger.info('User signed in:');
			// 在这里执行你需要的操作，例如存储用户信息、更新 UI 等
		});

		events.addUserLoaded((user) => {
			Logger.info('User Loaded:');
			this.user.value = user;
			authStore.setAuthInfo({
				authInfo: {},
			});
		});

		events.addUserUnloaded(() => {
			console.log('aaddUserUnloaded:');
			this.user.value = null;
		});

		events.addAccessTokenExpired(() => {
			console.log('addAccessTokenExpired:');
			this.user.value = null;
			this._signInIfNecessary();
		});

		events.addUserSignedOut(() => {
			this.user.value = null;
			this._signInIfNecessary();
		});
	}

	private _signInIfNecessary() {
		if (this.router) {
			const current = this.router.currentRoute;
			// if (current?.meta?.authName === this.authName) {
			//   this._signIn(this.defaultSignInType, { state: { current } })
			//     .catch(() => setTimeout(() => this._signInIfNecessary(), 5000))
			// }
			this._signIn(this.defaultSignInType, { state: { current } }).catch(() => setTimeout(() => this._signInIfNecessary(), 5000));
		}
	}

	private _getPath(url: string) {
		const a = document.createElement('a');
		a.href = url;
		return a.pathname.startsWith('/') ? a.pathname : '/' + a.pathname;
	}
}
