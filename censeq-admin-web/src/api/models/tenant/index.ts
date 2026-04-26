/**
 * 与 Censeq.TenantManagement.Application.Contracts 中的租户 DTO 对齐，供多租户管理页面与 HttpApi 联调使用。
 */
export interface TenantDto {
	id: string;
	name?: string;
	code?: string | null;
	isActive: boolean;
	concurrencyStamp?: string;
	extraProperties?: Record<string, unknown>;
}

export interface TenantCreateDto {
	name: string;
	code: string;
	adminEmailAddress: string;
	adminPassword: string;
}

export interface TenantUpdateDto {
	name: string;
	code?: string | null;
	isActive: boolean;
	concurrencyStamp?: string;
}

export interface TenantAdminUserDto {
	tenantId: string;
	userName?: string | null;
	email?: string | null;
}

/** 对应 GetTenantsInput（ABP 分页查询参数为 camelCase） */
export interface GetTenantsInput {
	filter?: string;
	sorting?: string;
	skipCount?: number;
	maxResultCount?: number;
}
