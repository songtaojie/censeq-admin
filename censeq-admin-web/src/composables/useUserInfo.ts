import { storeToRefs } from 'pinia';
import { useUserInfoStore } from '/@/stores/userInfo';

export function useUserInfo() {
	const store = useUserInfoStore();
	const { userInfos } = storeToRefs(store);

	return {
		$id: store.$id,
		userInfos: userInfos,
		setUserInfos: store.setUserInfos,
		setAccessContext: store.setAccessContext,
	};
}
