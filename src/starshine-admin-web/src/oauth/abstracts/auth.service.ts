// export interface IAuthService {
//   oidc: boolean;

//   get isInternalAuth(): boolean;

//   get isAuthenticated(): boolean;

//   init(): Promise<any>;

//   logout(queryParams?: Params): Observable<any>;

//   navigateToLogin(queryParams?: Params): void;

//   login(params: LoginParams): Observable<any>;

//   loginUsingGrant(
//     grantType: string,
//     parameters: object,
//     headers?: HttpHeaders,
//   ): Promise<AbpAuthResponse>;

//   getAccessTokenExpiration(): number;

//   getRefreshToken(): string;

//   getAccessToken(): string;

//   refreshToken(): Promise<AbpAuthResponse>;
// }
