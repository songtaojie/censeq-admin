// auth-password-flow-strategy.ts
import { AuthFlowStrategy } from './auth-flow-strategy';
import { UserManager, UserManagerSettings, WebStorageStateStore } from 'oidc-client-ts';

/**
 * @internal
 */
export declare interface PasswordFlowStrategyArgs {
	client_id: string;
	client_secret?: string;
	scope: string;
	username: string;
	password: string;
	redirectUri: string;
}

export class AuthPasswordFlowStrategy extends AuthFlowStrategy {
	private userManager: UserManager;
	private clientId: string;
	private scope: string;
	private redirectUri: string;
	private username: string;
	private password: string;

	constructor(args: PasswordFlowStrategyArgs, oidcConfig: UserManagerSettings) {
		super();
		this.clientId = args.client_id;
		this.scope = args.scope;
		this.redirectUri = args.redirectUri;
		this.username = args.username;
		this.password = args.password;
		const config: UserManagerSettings = {
			automaticSilentRenew: true,
			monitorSession: true,
			userStore: new WebStorageStateStore({ store: sessionStorage }),
			...oidcConfig,
		};
		this.userManager = new UserManager(config);
	}

	public async signIn(): Promise<void> {
		const tokenRequest = {
			client_id: this.clientId,
			grant_type: 'password',
			username: this.username,
			password: this.password,
			scope: this.scope,
			redirect_uri: this.redirectUri,
		};

		try {
			// 使用 PasswordFlow 登录
			const user = await this.userManager.signinRedirect(tokenRequest);
			console.log('User authenticated with PasswordFlow', user);
		} catch (error) {
			console.error('PasswordFlow authentication failed', error);
		}
	}

	public async handleCallback(): Promise<void> {
		// 对于 PasswordFlow，通常没有回调逻辑
		console.warn('No callback handling for PasswordFlow');
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
