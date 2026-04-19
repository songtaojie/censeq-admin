export type MenuType = 1 | 2 | 3;

export type MenuAuthorizationMode = 1 | 2 | 3;

export interface CreateMenuDto {
	parentId?: string | null;
	name: string;
	title: string;
	routeName?: string;
	path?: string;
	component?: string;
	redirect?: string;
	icon?: string;
	type: MenuType;
	sort: number;
	visible: boolean;
	keepAlive: boolean;
	affix: boolean;
	isExternal: boolean;
	externalUrl?: string;
	isIframe: boolean;
	status: boolean;
	authorizationMode: MenuAuthorizationMode;
	remark?: string;
	buttonCode?: string;
	permissionGroups?: string | null;
	permissionNames: string[];
}

export interface UpdateMenuDto extends CreateMenuDto {
	concurrencyStamp: string;
}

export interface SetMenuStatusDto {
	status: boolean;
}

export interface MenuSortDto {
	parentId?: string | null;
	sort: number;
}

export interface CopyTenantMenusDto {
	clearExisting: boolean;
}

export interface MenuPermissionDefinitionDto {
	name: string;
	displayName: string;
	parentName?: string | null;
}

export interface MenuPermissionGroupDto {
	name: string;
	displayName: string;
	permissions: MenuPermissionDefinitionDto[];
}

export interface MenuTreeItemDto {
	id: string;
	parentId?: string | null;
	name: string;
	title: string;
	routeName?: string | null;
	path?: string | null;
	component?: string | null;
	icon?: string | null;
	type: MenuType;
	sort: number;
	visible: boolean;
	status: boolean;
	authorizationMode: MenuAuthorizationMode;
	buttonCode?: string | null;
	permissionGroups?: string | null;
	permissionNames: string[];
	children: MenuTreeItemDto[];
}

export interface MenuDetailDto extends MenuTreeItemDto {
	tenantId?: string | null;
	redirect?: string | null;
	keepAlive: boolean;
	affix: boolean;
	isExternal: boolean;
	externalUrl?: string | null;
	isIframe: boolean;
	remark?: string | null;
	concurrencyStamp: string;
}

export interface MenuRouteMetaDto {
	title: string;
	isLink: string;
	isHide: boolean;
	isKeepAlive: boolean;
	isAffix: boolean;
	isIframe: boolean;
	roles: string[];
	icon: string;
}

export interface MenuRouteDto {
	path: string;
	name?: string | null;
	redirect?: string | null;
	component?: string | null;
	meta: MenuRouteMetaDto;
	children: MenuRouteDto[];
}

export interface CurrentUserMenuResultDto {
	routes: MenuRouteDto[];
	authBtnList: string[];
	roles: string[];
}