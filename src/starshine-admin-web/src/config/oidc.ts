export const oidcConfig = {
  production: import.meta.env.VITE_OIDC_PRODUCTION === 'true',
  stsAuthority: import.meta.env.VITE_OIDC_STS_AUTHORITY,
  clientId: import.meta.env.VITE_OIDC_CLIENT_ID,
  clientRoot: import.meta.env.VITE_OIDC_CLIENT_ROOT,
  clientScope: import.meta.env.VITE_OIDC_CLIENT_SCOPE,
  apiRoot: import.meta.env.VITE_OIDC_API_ROOT
}