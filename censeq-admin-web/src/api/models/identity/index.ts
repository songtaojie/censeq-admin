import type { ExtensibleEntityDto, ExtensibleFullAuditedEntityDto, ExtensibleObject, PagedAndSortedRequestDto } from '../core';

export interface GetIdentityRolesRequest extends PagedAndSortedRequestDto {
	filter?: string;
}

export interface GetIdentityUsersRequest extends PagedAndSortedRequestDto {
	filter?: string;
}

/** 与后端 OrganizationUnitDto 一致（组织机构 / 部门） */
export interface OrganizationUnitDto extends ExtensibleEntityDto<string> {
	parentId?: string | null;
	code?: string;
	displayName?: string;
	entityVersion?: number;
}

export interface OrganizationUnitCreateDto {
	displayName: string;
	parentId?: string | null;
}

export interface OrganizationUnitUpdateDto {
	displayName: string;
}

/** 用户所属组织机构 Id 列表 */
export interface IdentityUserOrganizationUnitsDto {
	organizationUnitIds: string[];
}

export interface IdentityRoleCreateDto extends IdentityRoleCreateOrUpdateDtoBase {}

export interface IdentityRoleCreateOrUpdateDtoBase extends ExtensibleObject {
	name: string;
	isDefault: boolean;
	isPublic: boolean;
}

export interface IdentityRoleDto extends ExtensibleEntityDto<string> {
	name: string;
	isDefault: boolean;
	isStatic: boolean;
	isPublic: boolean;
	concurrencyStamp?: string;
}

export interface IdentityRoleUpdateDto extends IdentityRoleCreateOrUpdateDtoBase {
	concurrencyStamp?: string;
}

export interface IdentityUserCreateDto extends IdentityUserCreateOrUpdateDtoBase {
	password: string;
}

export interface IdentityUserCreateOrUpdateDtoBase extends ExtensibleObject {
	userName: string;
	name?: string;
	surname?: string;
	email: string;
	phoneNumber?: string;
	isActive: boolean;
	lockoutEnabled: boolean;
	roleNames: string[];
}

export interface IdentityUserDto extends ExtensibleFullAuditedEntityDto<string> {
	creationTime?: string;
	tenantId?: string;
	userName?: string;
	name?: string;
	surname?: string;
	email?: string;
	emailConfirmed: boolean;
	phoneNumber?: string;
	phoneNumberConfirmed: boolean;
	isActive: boolean;
	lockoutEnabled: boolean;
	lockoutEnd?: string;
	concurrencyStamp?: string;
}

export interface IdentityUserUpdateDto extends IdentityUserCreateOrUpdateDtoBase {
	password?: string;
	concurrencyStamp?: string;
}

export interface IdentityUserUpdateRolesDto {
	roleNames: string[];
}

export interface UserLookupCountRequest {
	filter?: string;
}

export interface UserLookupSearchRequest extends PagedAndSortedRequestDto {
	filter?: string;
}
