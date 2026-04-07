import request from '/@/utils/request';
import type { PagedResultDto, ListResultDto } from '../base';

export interface OpenIddictApplicationDto {
  id: string;
  clientId: string;
  clientType: string;
  applicationType?: string;
  consentType?: string;
  displayName?: string;
  clientUri?: string;
  logoUri?: string;
  redirectUris: string[];
  postLogoutRedirectUris: string[];
  permissions: string[];
  requirements: string[];
  creationTime?: string;
  creatorId?: string;
  lastModificationTime?: string;
  lastModifierId?: string;
}

export interface OpenIddictApplicationCreateDto {
  clientId: string;
  displayName?: string;
  clientType: string;
  applicationType?: string;
  consentType?: string;
  clientSecret?: string;
  redirectUris: string[];
  postLogoutRedirectUris: string[];
  permissions: string[];
  requirements: string[];
  clientUri?: string;
  logoUri?: string;
}

export interface OpenIddictApplicationUpdateDto {
  displayName?: string;
  clientSecret?: string;
  consentType?: string;
  redirectUris: string[];
  postLogoutRedirectUris: string[];
  permissions: string[];
  requirements: string[];
  clientUri?: string;
  logoUri?: string;
}

export interface GetOpenIddictApplicationsInput {
  filter?: string;
  clientType?: string;
  sorting?: string;
  skipCount?: number;
  maxResultCount?: number;
}

export function useOpenIddictApplicationApi() {
  return {
    /**
     * 获取应用列表
     */
    getList: (input: GetOpenIddictApplicationsInput) => {
      return request<PagedResultDto<OpenIddictApplicationDto>>({
        url: '/api/openIddict/applications',
        method: 'get',
        params: input,
      });
    },

    /**
     * 获取单个应用
     */
    get: (id: string) => {
      return request<OpenIddictApplicationDto>({
        url: `/api/openIddict/applications/${id}`,
        method: 'get',
      });
    },

    /**
     * 创建应用
     */
    create: (input: OpenIddictApplicationCreateDto) => {
      return request<OpenIddictApplicationDto>({
        url: '/api/openIddict/applications',
        method: 'post',
        data: input,
      });
    },

    /**
     * 更新应用
     */
    update: (id: string, input: OpenIddictApplicationUpdateDto) => {
      return request<OpenIddictApplicationDto>({
        url: `/api/openIddict/applications/${id}`,
        method: 'put',
        data: input,
      });
    },

    /**
     * 删除应用
     */
    delete: (id: string) => {
      return request({
        url: `/api/openIddict/applications/${id}`,
        method: 'delete',
      });
    },

    /**
     * 生成客户端密钥
     */
    generateSecret: (id: string) => {
      return request<string>({
        url: `/api/openIddict/applications/${id}/generate-secret`,
        method: 'post',
      });
    },

    /**
     * 检查客户端ID是否存在
     */
    checkClientIdExists: (clientId: string, excludeId?: string) => {
      return request<boolean>({
        url: '/api/openIddict/applications/check-client-id',
        method: 'get',
        params: { clientId, excludeId },
      });
    },
  };
}
