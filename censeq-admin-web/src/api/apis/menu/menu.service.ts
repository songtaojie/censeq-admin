import type { ListResponseDto } from '/@/api/models/core';
import type {
	CopyTenantMenusDto,
	CreateMenuDto,
	CurrentUserMenuResultDto,
	MenuDetailDto,
	MenuPermissionGroupDto,
	MenuSortDto,
	MenuTreeItemDto,
	SetMenuStatusDto,
	UpdateMenuDto,
} from '/@/api/models/menu';
import { useBaseApi } from '../base';

const menuApi = useBaseApi('admin');

const menuBasePath = 'api/admin/menus';
const runtimeMenuPath = 'api/admin/runtime-menus/current-user';

export function useMenuApi() {
	return {
		getCurrentUserRuntimeMenu: async (): Promise<CurrentUserMenuResultDto> => {
			return await menuApi.request<CurrentUserMenuResultDto>(runtimeMenuPath, 'GET');
		},

		getMenuTree: async (): Promise<ListResponseDto<MenuTreeItemDto>> => {
			return await menuApi.request<ListResponseDto<MenuTreeItemDto>>(`${menuBasePath}/tree`, 'GET');
		},

		getMenuPermissionGroups: async (): Promise<ListResponseDto<MenuPermissionGroupDto>> => {
			return await menuApi.request<ListResponseDto<MenuPermissionGroupDto>>(`${menuBasePath}/permissions`, 'GET');
		},

		getMenu: async (id: string): Promise<MenuDetailDto> => {
			return await menuApi.request<MenuDetailDto>(`${menuBasePath}/${id}`, 'GET');
		},

		createMenu: async (input: CreateMenuDto): Promise<MenuDetailDto> => {
			return await menuApi.add<MenuDetailDto>(menuBasePath, input);
		},

		updateMenu: async (id: string, input: UpdateMenuDto): Promise<MenuDetailDto> => {
			return await menuApi.update<MenuDetailDto>(`${menuBasePath}/${id}`, input);
		},

		deleteMenu: async (id: string): Promise<void> => {
			return await menuApi.delete<void>(`${menuBasePath}/${id}`, undefined);
		},

		setMenuStatus: async (id: string, input: SetMenuStatusDto): Promise<MenuDetailDto> => {
			return await menuApi.update<MenuDetailDto>(`${menuBasePath}/${id}/status`, input);
		},

		moveMenu: async (id: string, input: MenuSortDto): Promise<MenuDetailDto> => {
			return await menuApi.update<MenuDetailDto>(`${menuBasePath}/${id}/sort`, input);
		},

		copyMenusFromHost: async (input: CopyTenantMenusDto): Promise<void> => {
			return await menuApi.add<void>(`${menuBasePath}/copy-from-host`, input);
		},
	};
}