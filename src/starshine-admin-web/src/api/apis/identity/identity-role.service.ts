import type { GetIdentityRolesInput, IdentityRoleCreateDto, IdentityRoleDto, IdentityRoleUpdateDto } from '/@/api/models/identity';
import type { ListResultDto, PagedResultDto } from '/@/api/models/core';
import request from '/@/utils/request';
import { useBaseApi } from '../base';
var identityApi = useBaseApi('identity');
/**
 * （不建议写成 request.post(xxx)，因为这样 post 时，无法 params 与 data 同时传参）
 *
 * 登录api接口集合
 * @method signIn 用户登录
 * @method signOut 用户退出登录
 */
export function useIdentityApi() {
	return {
		create: (input: IdentityRoleCreateDto) => identityApi.add(input),
		page: (input: GetIdentityRolesInput) => identityApi.page(input, 'roles'),
	};
}

// export class IdentityRoleService {
// 	apiName = 'AbpIdentity';

// 	create = (input: IdentityRoleCreateDto) => identityApi.add(input);

// 	delete = (id: string) =>
// 		this.restService.request<any, void>(
// 			{
// 				method: 'DELETE',
// 				url: `/api/identity/roles/${id}`,
// 			},
// 			{ apiName: this.apiName }
// 		);

// 	get = (id: string) =>
// 		this.restService.request<any, IdentityRoleDto>(
// 			{
// 				method: 'GET',
// 				url: `/api/identity/roles/${id}`,
// 			},
// 			{ apiName: this.apiName }
// 		);

// 	getAllList = () =>
// 		this.restService.request<any, ListResultDto<IdentityRoleDto>>(
// 			{
// 				method: 'GET',
// 				url: '/api/identity/roles/all',
// 			},
// 			{ apiName: this.apiName }
// 		);

// 	getList = (input: GetIdentityRolesInput) =>
// 		this.restService.request<any, PagedResultDto<IdentityRoleDto>>(
// 			{
// 				method: 'GET',
// 				url: '/api/identity/roles',
// 				params: { filter: input.filter, sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
// 			},
// 			{ apiName: this.apiName }
// 		);

// 	update = (id: string, input: IdentityRoleUpdateDto) =>
// 		this.restService.request<any, IdentityRoleDto>(
// 			{
// 				method: 'PUT',
// 				url: `/api/identity/roles/${id}`,
// 				body: input,
// 			},
// 			{ apiName: this.apiName }
// 		);

// 	constructor(private restService: RestService) {}
// }
