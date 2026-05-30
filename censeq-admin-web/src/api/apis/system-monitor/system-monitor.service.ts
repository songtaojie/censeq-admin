import type { ListResponseDto } from '/@/api/models/core';
import type { AssemblyInfoDto, SystemBaseInfoDto, SystemDiskInfoDto, SystemUsageInfoDto } from '/@/api/models/system-monitor';
import { useBaseApi } from '../base';

const monitorApi = useBaseApi('admin');
const basePath = 'api/admin/system-monitor';

export function useSystemMonitorApi() {
	return {
		getServerBase: async (): Promise<SystemBaseInfoDto> => {
			return await monitorApi.request<SystemBaseInfoDto>(`${basePath}/server/base`, 'GET');
		},

		getServerUsage: async (): Promise<SystemUsageInfoDto> => {
			return await monitorApi.request<SystemUsageInfoDto>(`${basePath}/server/usage`, 'GET');
		},

		getServerDisks: async (): Promise<ListResponseDto<SystemDiskInfoDto>> => {
			return await monitorApi.request<ListResponseDto<SystemDiskInfoDto>>(`${basePath}/server/disks`, 'GET');
		},

		getAssemblyList: async (): Promise<ListResponseDto<AssemblyInfoDto>> => {
			return await monitorApi.request<ListResponseDto<AssemblyInfoDto>>(`${basePath}/server/assemblies`, 'GET');
		},

		getCacheKeys: async (): Promise<ListResponseDto<string>> => {
			return await monitorApi.request<ListResponseDto<string>>(`${basePath}/cache/keys`, 'GET');
		},

		getCacheValue: async (key: string): Promise<unknown> => {
			return await monitorApi.request<unknown>(`${basePath}/cache/value/${encodeURIComponent(key)}`, 'GET');
		},

		deleteCache: async (key: string): Promise<void> => {
			return await monitorApi.delete<void>(`${basePath}/cache/${encodeURIComponent(key)}`, undefined);
		},

		clearCache: async (): Promise<void> => {
			return await monitorApi.delete<void>(`${basePath}/cache`, undefined);
		},
	};
}
