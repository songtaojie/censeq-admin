import type {
	GetIdentityRolesRequest,
	GetIdentityUsersRequest,
	IdentityRoleClaimCreateDto,
	IdentityRoleClaimDto,
	IdentityRoleCreateDto,
	IdentityRoleDto,
	IdentityRoleUpdateDto,
	IdentityUserCreateDto,
	IdentityUserDto,
	IdentityUserOrganizationUnitsDto,
	IdentityUserUpdateDto,
	IdentityUserUpdateRolesDto,
	OrganizationUnitCreateDto,
	OrganizationUnitDto,
	OrganizationUnitUpdateDto,
} from '/@/api/models/identity';
import type { ListResponseDto, PagedResponseDto } from '/@/api/models/core';
import { useBaseApi } from '../base';

const identityApi = useBaseApi('identity');

/**
 * Identity 远程服务：角色、用户、组织机构（ABP 组织单元）API。
 */
export function useIdentityApi() {
	return {
		createRole: async (input: IdentityRoleCreateDto): Promise<IdentityRoleDto> => {
			return await identityApi.add<IdentityRoleDto>('api/identity/roles', input);
		},
		getRolePage: async (input: GetIdentityRolesRequest): Promise<PagedResponseDto<IdentityRoleDto>> => {
			return await identityApi.page<IdentityRoleDto>('api/identity/roles', input);
		},
		getAllRoles: async (): Promise<ListResponseDto<IdentityRoleDto>> => {
			return await identityApi.request<ListResponseDto<IdentityRoleDto>>('api/identity/roles/all', 'GET');
		},
		updateRole: async (id: string, input: IdentityRoleUpdateDto): Promise<IdentityRoleDto> => {
			return await identityApi.update<IdentityRoleDto>(`api/identity/roles/${id}`, input);
		},
		deleteRole: async (id: string): Promise<void> => {
			return await identityApi.delete<void>(`api/identity/roles/${id}`, undefined);
		},

		getRoleClaims: async (roleId: string): Promise<ListResponseDto<IdentityRoleClaimDto>> => {
			return await identityApi.request<ListResponseDto<IdentityRoleClaimDto>>(`api/identity/roles/${roleId}/claims`, 'GET');
		},
		addRoleClaim: async (roleId: string, input: IdentityRoleClaimCreateDto): Promise<void> => {
			return await identityApi.request<void>(`api/identity/roles/${roleId}/claims`, 'POST', input);
		},
		removeRoleClaim: async (roleId: string, claimId: string): Promise<void> => {
			return await identityApi.delete<void>(`api/identity/roles/${roleId}/claims/${claimId}`, undefined);
		},

		getUserPage: async (input: GetIdentityUsersRequest): Promise<PagedResponseDto<IdentityUserDto>> => {
			return await identityApi.page<IdentityUserDto>('api/identity/users', input);
		},
		getUser: async (id: string): Promise<IdentityUserDto> => {
			return await identityApi.request<IdentityUserDto>(`api/identity/users/${id}`, 'GET');
		},
		createUser: async (input: IdentityUserCreateDto): Promise<IdentityUserDto> => {
			return await identityApi.add<IdentityUserDto>('api/identity/users', input);
		},
		updateUser: async (id: string, input: IdentityUserUpdateDto): Promise<IdentityUserDto> => {
			return await identityApi.update<IdentityUserDto>(`api/identity/users/${id}`, input);
		},
		deleteUser: async (id: string): Promise<void> => {
			return await identityApi.delete<void>(`api/identity/users/${id}`, undefined);
		},
		getUserRoles: async (id: string): Promise<ListResponseDto<IdentityRoleDto>> => {
			return await identityApi.request<ListResponseDto<IdentityRoleDto>>(`api/identity/users/${id}/roles`, 'GET');
		},
		updateUserRoles: async (id: string, input: IdentityUserUpdateRolesDto): Promise<void> => {
			return await identityApi.request<void>(`api/identity/users/${id}/roles`, 'PUT', input);
		},
		getUserOrganizationUnits: async (id: string): Promise<ListResponseDto<OrganizationUnitDto>> => {
			return await identityApi.request<ListResponseDto<OrganizationUnitDto>>(`api/identity/users/${id}/organization-units`, 'GET');
		},
		updateUserOrganizationUnits: async (id: string, input: IdentityUserOrganizationUnitsDto): Promise<void> => {
			return await identityApi.request<void>(`api/identity/users/${id}/organization-units`, 'PUT', input);
		},

		getOrganizationUnitAllList: async (): Promise<ListResponseDto<OrganizationUnitDto>> => {
			return await identityApi.request<ListResponseDto<OrganizationUnitDto>>('api/identity/organization-units', 'GET');
		},
		getOrganizationUnit: async (id: string): Promise<OrganizationUnitDto> => {
			return await identityApi.request<OrganizationUnitDto>(`api/identity/organization-units/${id}`, 'GET');
		},
		createOrganizationUnit: async (input: OrganizationUnitCreateDto): Promise<OrganizationUnitDto> => {
			return await identityApi.add<OrganizationUnitDto>('api/identity/organization-units', input);
		},
		updateOrganizationUnit: async (id: string, input: OrganizationUnitUpdateDto): Promise<OrganizationUnitDto> => {
			return await identityApi.update<OrganizationUnitDto>(`api/identity/organization-units/${id}`, input);
		},
		deleteOrganizationUnit: async (id: string): Promise<void> => {
			return await identityApi.delete<void>(`api/identity/organization-units/${id}`, undefined);
		},
	};
}
