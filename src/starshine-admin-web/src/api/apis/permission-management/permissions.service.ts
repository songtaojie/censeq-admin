import type { GetPermissionListResponseDto, UpdatePermissionsDto } from '/@/api/models/permission';
import { useBaseApi } from '../base';
var permissionApi = useBaseApi('permission-management');
/**
 * 用户认证api接口集合
 * @method createRole 创建用户角色
 * @method getRolePage 获取角色分页列表
 */
export function usePermissionApi() {
	return {
		updatePermission: async (providerName: string, providerKey: string, input: UpdatePermissionsDto): Promise<void> => {
			return await permissionApi.update<void>('api/permission-management/permissions', input);
		},
		getPermissionList: async (providerName: string, providerKey: string): Promise<GetPermissionListResponseDto> => {
			return await permissionApi.list<GetPermissionListResponseDto>('api/permission-management/permissions', { providerName, providerKey });
		},
	};
}
