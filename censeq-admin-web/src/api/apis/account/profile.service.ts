import type { ProfileDto, UpdateProfileDto } from '/@/api/models/account';
import { useBaseApi } from '../base';

const accountApi = useBaseApi('account');

/** 当前登录用户个人资料 API。 */
export function useProfileApi() {
	return {
		/** 获取当前登录用户个人资料。 */
		getProfile: async (): Promise<ProfileDto> => {
			return await accountApi.request<ProfileDto>('api/account/my-profile', 'GET');
		},
		/** 更新当前登录用户个人资料。 */
		updateProfile: async (input: UpdateProfileDto): Promise<ProfileDto> => {
			return await accountApi.update<ProfileDto>('api/account/my-profile', input);
		},
	};
}
