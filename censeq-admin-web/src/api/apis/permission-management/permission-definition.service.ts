import type {
	PermissionGroupDefinitionDto,
	PermissionDefinitionDto,
	UpdatePermissionGroupDefinitionDto,
	UpdatePermissionDefinitionDto,
} from '/@/api/models/permission/definition';
import { useBaseApi } from '../base';

const api = useBaseApi('permission-management');
const BASE = 'api/permission-management/definition';

export function usePermissionDefinitionApi() {
	return {
		getGroups: (): Promise<PermissionGroupDefinitionDto[]> =>
			api.list<PermissionGroupDefinitionDto[]>(`${BASE}/groups`, {}),

		updateGroup: (groupName: string, input: UpdatePermissionGroupDefinitionDto): Promise<PermissionGroupDefinitionDto> =>
			api.update<PermissionGroupDefinitionDto>(`${BASE}/groups/${encodeURIComponent(groupName)}`, input),

		getPermissions: (groupName: string): Promise<PermissionDefinitionDto[]> =>
			api.list<PermissionDefinitionDto[]>(`${BASE}/groups/${encodeURIComponent(groupName)}/permissions`, {}),

		updatePermission: (name: string, input: UpdatePermissionDefinitionDto): Promise<PermissionDefinitionDto> =>
			api.update<PermissionDefinitionDto>(`${BASE}/permissions/${encodeURIComponent(name)}`, input),
	};
}
