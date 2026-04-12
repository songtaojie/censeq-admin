import { defineStore } from 'pinia';
import Cookies from 'js-cookie';
import { Session } from '/@/utils/storage';
import { useOidc } from '/@/composables/useOidc';

/**
 * 用户信息
 * @methods setUserInfos 设置用户信息
 */
export const useUserInfoStore = defineStore('userInfo', {
	state: (): UserInfosState => ({
		userInfos: {
			userName: '',
			photo: '',
			time: 0,
			roles: [],
			authBtnList: [],
		},
	}),
	actions: {
		async setUserInfos(accessContext?: Partial<Pick<UserInfos, 'roles' | 'authBtnList'>>) {
			const userInfos = <UserInfos>await this.getApiUserInfo(accessContext);
			this.userInfos = userInfos;
			Session.set('userInfo', userInfos);
			return userInfos;
		},
		setAccessContext(roles: string[] = [], authBtnList: string[] = []) {
			this.userInfos = {
				...this.userInfos,
				roles: [...roles],
				authBtnList: [...authBtnList],
				time: new Date().getTime(),
			};
			Session.set('userInfo', this.userInfos);
		},
		async getApiUserInfo(accessContext?: Partial<Pick<UserInfos, 'roles' | 'authBtnList'>>) {
			const cached = (Session.get('userInfo') as Partial<UserInfos> | undefined) ?? {};
			const { getCurrentUser } = useOidc();
			const user = await getCurrentUser();
			const userName = user?.profile?.preferred_username ?? cached.userName ?? Cookies.get('userName') ?? '';
			return {
				userName,
				photo: user?.profile?.picture ?? cached.photo ?? '/upload/logo.png',
				time: new Date().getTime(),
				roles: accessContext?.roles ?? cached.roles ?? [],
				authBtnList: accessContext?.authBtnList ?? cached.authBtnList ?? [],
			};
		},
	},
});
