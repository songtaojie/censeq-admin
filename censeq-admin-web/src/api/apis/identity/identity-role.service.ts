import type { GetIdentityRolesRequest, IdentityRoleCreateDto, IdentityRoleDto, IdentityRoleUpdateDto } from '/@/api/models/identity';
import type { ListResponseDto, PagedResponseDto } from '/@/api/models/core';
import { useBaseApi } from '../base';
var identityApi = useBaseApi('identity');
/**
 * （不建议写成 request.post(xxx)，因为这样 post 时，无法 params 与 data 同时传参）
 *
 * 用户认证api接口集合
 * @method createRole 创建用户角色
 * @method getRolePage 获取角色分页列表
 */
export function useIdentityApi() {
	return {
		createRole: async (input: IdentityRoleCreateDto): Promise<IdentityRoleDto> => {
			return await identityApi.add<IdentityRoleDto>('api/identity/roles',input);
		},
		getRolePage: async ( input: GetIdentityRolesRequest): Promise<PagedResponseDto<IdentityRoleDto>> => {
			return await identityApi.page<IdentityRoleDto>('api/identity/roles',input);
		},
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

// 	getList = (input: GetIdentityRolesRequest) =>
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
