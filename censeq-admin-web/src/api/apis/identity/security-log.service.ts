import type { GetSecurityLogsRequest, IdentitySecurityLogDto } from '/@/api/models/identity';
import type { PagedResponseDto } from '/@/api/models/core';
import { useBaseApi } from '../base';

const identityApi = useBaseApi('identity');

/**
 * 登录日志 API
 */
export function useSecurityLogApi() {
return {
getSecurityLogPage: async (input: GetSecurityLogsRequest): Promise<PagedResponseDto<IdentitySecurityLogDto>> => {
return await identityApi.page<IdentitySecurityLogDto>('api/identity/security-logs', input);
},
deleteSecurityLog: async (id: string): Promise<void> => {
return await identityApi.delete<void>(`api/identity/security-logs/${id}`, undefined);
},
};
}