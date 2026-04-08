import request from '/@/utils/request';
import type { ListResponseDto, PagedResultDto } from '../base';
import type {
	GetIdentityClaimTypesRequest,
	IdentityClaimTypeCreateDto,
	IdentityClaimTypeDto,
	IdentityClaimTypeUpdateDto,
} from '/@/api/models/identity';

export function useIdentityClaimTypeApi() {
	return {
		getList: (input: GetIdentityClaimTypesRequest) => {
			return request<PagedResultDto<IdentityClaimTypeDto>>({
				url: '/api/identity/claim-types',
				method: 'get',
				params: input,
			});
		},
		getAllList: () => {
			return request<ListResponseDto<IdentityClaimTypeDto>>({
				url: '/api/identity/claim-types/all',
				method: 'get',
			});
		},
		get: (id: string) => {
			return request<IdentityClaimTypeDto>({
				url: `/api/identity/claim-types/${id}`,
				method: 'get',
			});
		},
		create: (input: IdentityClaimTypeCreateDto) => {
			return request<IdentityClaimTypeDto>({
				url: '/api/identity/claim-types',
				method: 'post',
				data: input,
			});
		},
		update: (id: string, input: IdentityClaimTypeUpdateDto) => {
			return request<IdentityClaimTypeDto>({
				url: `/api/identity/claim-types/${id}`,
				method: 'put',
				data: input,
			});
		},
		delete: (id: string) => {
			return request({
				url: `/api/identity/claim-types/${id}`,
				method: 'delete',
			});
		},
	};
}