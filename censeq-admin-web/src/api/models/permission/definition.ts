export interface PermissionGroupDefinitionDto {
	id: string;
	name: string;
	displayName: string;
}

export interface PermissionDefinitionDto {
	id: string;
	groupName: string;
	name: string;
	parentName?: string;
	displayName: string;
	isEnabled: boolean;
}

export interface UpdatePermissionGroupDefinitionDto {
	displayName: string;
}

export interface UpdatePermissionDefinitionDto {
	displayName: string;
}
