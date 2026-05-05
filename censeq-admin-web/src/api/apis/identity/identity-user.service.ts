import type { GetIdentityUsersRequest, IdentityUserDto } from '/@/api/models/identity';
import type { PagedResponseDto } from '/@/api/models/core';
import { useBaseApi } from '../base';

const identityApi = useBaseApi('identity');

const usersBasePath = 'api/identity/users';

export function useIdentityUserApi() {
	return {
		getIdentityUserPage: async (input: GetIdentityUsersRequest): Promise<PagedResponseDto<IdentityUserDto>> => {
			return await identityApi.page<IdentityUserDto>(usersBasePath, input);
		},
	};
}