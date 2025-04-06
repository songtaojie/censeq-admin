// // auth-flow-strategy.ts
// import { inject } from 'vue'
// export const LoginServiceToken = Symbol('LoginService')
// export abstract class AuthFlowStrategy {
//   constructor() {
//     this.httpErrorReporter = inject.get(HttpErrorReporterService);
//     this.environment = injector.get(EnvironmentService);
//     this.configState = injector.get(ConfigStateService);
//     this.oAuthService = injector.get(OAuthService2);
//     this.sessionState = injector.get(SessionStateService);
//     this.localStorageService = injector.get(AbpLocalStorageService);
//     this.oAuthConfig = this.environment.getEnvironment().oAuthConfig || {};
//     this.tenantKey = injector.get(TENANT_KEY);
//     this.router = injector.get(Router);
//     this.oAuthErrorFilterService = injector.get(OAuthErrorFilterService);
//     this.rememberMeService = injector.get(RememberMeService);
//     this.windowService = injector.get(AbpWindowService);

//     this.listenToOauthErrors();
//   }

//   abstract signIn(): Promise<void>;
//   abstract signOut(): Promise<void>;
//   abstract handleCallback(): Promise<void>;
//   abstract isAuthenticated(): boolean;
// }
