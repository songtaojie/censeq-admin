import { AuthService, SignInType } from './auth-service';
var loco = window.location;
var appRootUrl = `${loco.protocol}//${loco.host}`;
export const authService = new AuthService('main', SignInType.Window, appRootUrl, {
	authority: import.meta.env.VITE_OIDC_AUTHORITY,
	client_id: import.meta.env.VITE_OIDC_CLIENT_ID,
	redirect_uri: import.meta.env.VITE_OIDC_REDIRECT_URI + '/callback',
	response_type: import.meta.env.VITE_OIDC_RESPONSE_TYPE,
	scope: import.meta.env.VITE_OIDC_SCOPE,
});
