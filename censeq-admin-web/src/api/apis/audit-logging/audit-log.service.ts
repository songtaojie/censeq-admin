import type { AuditLogDto, GetAuditLogsRequest } from '/@/api/models/audit-logging';
import type { PagedResponseDto } from '/@/api/models/core';
import { useBaseApi } from '../base';

const auditLogApi = useBaseApi('audit-logging');

/**
 * 审计日志API
 */
export function useAuditLogApi() {
	return {
		getAuditLogPage: async (input: GetAuditLogsRequest): Promise<PagedResponseDto<AuditLogDto>> => {
			return await auditLogApi.page<AuditLogDto>('api/audit-logging/audit-logs', input);
		},
		getAuditLog: async (id: string): Promise<AuditLogDto> => {
			return await auditLogApi.request<AuditLogDto>(`api/audit-logging/audit-logs/${id}`, 'GET');
		},
		deleteAuditLog: async (id: string): Promise<void> => {
			return await auditLogApi.delete<void>(`api/audit-logging/audit-logs/${id}`, undefined);
		},
	};
}
