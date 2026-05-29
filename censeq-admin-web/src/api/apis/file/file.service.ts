import type { CreateUpdateFileProviderDto, FileProviderDto, FileRecordDto, GetFileProvidersRequest, GetFileRecordsRequest } from '/@/api/models/file';
import type { PagedResponseDto } from '/@/api/models/core';
import { useBaseApi } from '../base';

const fileApi = useBaseApi('admin');

/** 将后端返回的相对文件路径转换为浏览器可访问的完整地址。 */
export function resolveFileUrl(url?: string | null) {
	if (!url) return '';
	if (/^(https?:)?\/\//i.test(url) || url.startsWith('data:') || url.startsWith('blob:')) return url;
	const baseUrl = (import.meta.env.VITE_API_URL || '').replace(/\/$/, '');
	return `${baseUrl}${url.startsWith('/') ? url : `/${url}`}`;
}

/** 文件管理 API，包括分页查询、通用上传、头像上传和删除。 */
export function useFileApi() {
	return {
		/** 获取文件上传记录分页列表。 */
		getFilePage: async (input: GetFileRecordsRequest): Promise<PagedResponseDto<FileRecordDto>> => {
			return await fileApi.page<FileRecordDto>('api/admin/files', input);
		},
		/** 上传通用文件。 */
		uploadFile: async (file: File, extra: { category?: string; isPublic?: boolean; allowImageOnly?: boolean } = {}): Promise<FileRecordDto> => {
			return await fileApi.uploadFile<FileRecordDto>('api/admin/files', file, extra);
		},
		/** 上传当前用户头像。 */
		uploadAvatar: async (file: File): Promise<FileRecordDto> => {
			return await fileApi.uploadFile<FileRecordDto>('api/admin/files/avatar', file);
		},
		/** 删除指定文件记录及物理文件。 */
		deleteFile: async (id: string): Promise<void> => {
			return await fileApi.delete<void>(`api/admin/files/${id}`, undefined);
		},
	};
}

/** 文件存储服务商配置 API。 */
export function useFileProviderApi() {
	return {
		/** 获取文件存储服务商分页列表。 */
		getProviderPage: async (input: GetFileProvidersRequest): Promise<PagedResponseDto<FileProviderDto>> => {
			return await fileApi.page<FileProviderDto>('api/admin/file-providers', input);
		},
		/** 获取文件存储服务商详情。 */
		getProvider: async (id: string): Promise<FileProviderDto> => {
			return await fileApi.request<FileProviderDto>(`api/admin/file-providers/${id}`, 'GET');
		},
		/** 创建文件存储服务商。 */
		createProvider: async (input: CreateUpdateFileProviderDto): Promise<FileProviderDto> => {
			return await fileApi.add<FileProviderDto>('api/admin/file-providers', input);
		},
		/** 更新文件存储服务商。 */
		updateProvider: async (id: string, input: CreateUpdateFileProviderDto): Promise<FileProviderDto> => {
			return await fileApi.update<FileProviderDto>(`api/admin/file-providers/${id}`, input);
		},
		/** 删除文件存储服务商。 */
		deleteProvider: async (id: string): Promise<void> => {
			return await fileApi.delete<void>(`api/admin/file-providers/${id}`, undefined);
		},
		/** 设置默认文件存储服务商。 */
		setDefaultProvider: async (id: string): Promise<void> => {
			return await fileApi.add<void>(`api/admin/file-providers/${id}/default`, {});
		},
	};
}
