/**
 * 与 Censeq.TenantManagement.Application.Contracts 中的租户 DTO 对齐，供多租户管理页面与 HttpApi 联调使用。
 */
export interface TenantProfileFields {
	domain?: string | null;
	icon?: string | null;
	copyright?: string | null;
	icpNo?: string | null;
	icpAddress?: string | null;
	remark?: string | null;
	maxUserCount: number;
}

export interface TenantDto extends TenantProfileFields {
	id: string;
	name?: string;
	code?: string | null;
	isActive: boolean;
	concurrencyStamp?: string;
	extraProperties?: Record<string, unknown>;
}

export interface TenantCreateDto extends TenantProfileFields {
	name: string;
	code: string;
	adminUserName: string;
	adminName: string;
	adminEmailAddress: string;
	adminPassword: string;
}

export interface TenantUpdateDto extends TenantProfileFields {
	name: string;
	code?: string | null;
	isActive: boolean;
	concurrencyStamp?: string;
}

export interface TenantAdminUserDto {
	tenantId: string;
	userName?: string | null;
	name?: string | null;
	email?: string | null;
}

/** 对应 GetTenantsInput（ABP 分页查询参数为 camelCase） */
export interface GetTenantsInput {
	filter?: string;
	sorting?: string;
	skipCount?: number;
	maxResultCount?: number;
}
