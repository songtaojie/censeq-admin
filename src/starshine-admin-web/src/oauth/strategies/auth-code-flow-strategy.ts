// auth-code-flow-strategy.ts
import { AuthFlowStrategy } from './auth-flow-strategy';
import { UserManager, UserManagerSettings, WebStorageStateStore } from 'oidc-client-ts';

export class AuthCodeFlowStrategy extends AuthFlowStrategy {
	private userManager: UserManager;

	constructor(oidcConfig: UserManagerSettings) {
		super();
		const config: UserManagerSettings = {
			automaticSilentRenew: true,
			monitorSession: true,
			userStore: new WebStorageStateStore({ store: sessionStorage }),
			...oidcConfig,
		};
		this.userManager = new UserManager(config);
	}

	public async signIn(): Promise<void> {
		try {
			// 使用授权码流进行重定向认证
			await this.userManager.signinRedirect();
		} catch (error) {
			console.error('AuthorizationCodeFlow authentication failed', error);
		}
	}

	public async handleCallback(): Promise<void> {
		try {
			const user = await this.userManager.signinRedirectCallback();
			console.log('User authenticated with AuthorizationCodeFlow', user);
		} catch (error) {
			console.error('Callback handling failed', error);
		}
	}

	public async signOut(): Promise<void> {
		try {
			await this.userManager.signoutRedirect();
			console.log('User logged out');
		} catch (error) {
			console.error('SignOut failed', error);
		}
	}

	public isAuthenticated(): boolean {
		return this.userManager.getUser() !== null;
	}
}
