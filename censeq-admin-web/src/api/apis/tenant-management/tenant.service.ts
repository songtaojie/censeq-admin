import type { GetTenantsInput, TenantCreateDto, TenantDto, TenantUpdateDto } from '/@/api/models/tenant';
import type { PagedResponseDto } from '/@/api/models/core';
import { useBaseApi } from '../base';

const tenantApi = useBaseApi('tenant-management');

const tenantsBasePath = 'api/multi-tenancy/tenants';

/**
 * 租户管理 API，路由与 Censeq.TenantManagement.HttpApi.TenantController 一致。
 */
export function useTenantApi() {
	return {
		getTenant: async (id: string): Promise<TenantDto> => {
			return await tenantApi.request<TenantDto>(`${tenantsBasePath}/${id}`, 'GET');
		},

		getTenantPage: async (input: GetTenantsInput): Promise<PagedResponseDto<TenantDto>> => {
			return await tenantApi.page<TenantDto>(tenantsBasePath, input);
		},

		createTenant: async (input: TenantCreateDto): Promise<TenantDto> => {
			return await tenantApi.add<TenantDto>(tenantsBasePath, input);
		},

		updateTenant: async (id: string, input: TenantUpdateDto): Promise<TenantDto> => {
			return await tenantApi.update<TenantDto>(`${tenantsBasePath}/${id}`, input);
		},

		deleteTenant: async (id: string): Promise<void> => {
			return await tenantApi.delete<void>(`${tenantsBasePath}/${id}`, undefined);
		},

		getDefaultConnectionString: async (id: string): Promise<string> => {
			return await tenantApi.request<string>(`${tenantsBasePath}/${id}/default-connection-string`, 'GET');
		},

		updateDefaultConnectionString: async (id: string, defaultConnectionString: string): Promise<void> => {
			// ASP.NET [FromBody] string 需要 JSON 字符串字面量（含引号转义），与 axios 对象序列化行为区分
			return await tenantApi.request<void>(`${tenantsBasePath}/${id}/default-connection-string`, 'PUT', JSON.stringify(defaultConnectionString), {
				headers: { 'Content-Type': 'application/json' },
			});
		},

		deleteDefaultConnectionString: async (id: string): Promise<void> => {
			return await tenantApi.request<void>(`${tenantsBasePath}/${id}/default-connection-string`, 'DELETE', undefined);
		},

		resetAdminPassword: async (id: string, newPassword: string): Promise<void> => {
			return await tenantApi.request<void>(`api/admin/tenants/${id}/reset-admin-password`, 'POST', { newPassword });
		},

		getPermissions: async (id: string): Promise<string[]> => {
			return await tenantApi.request<string[]>(`api/admin/tenants/${id}/permissions`, 'GET');
		},

		updatePermissions: async (id: string, grantedPermissions: string[]): Promise<void> => {
			return await tenantApi.request<void>(`api/admin/tenants/${id}/permissions`, 'PUT', { grantedPermissions });
		},
	};
}
