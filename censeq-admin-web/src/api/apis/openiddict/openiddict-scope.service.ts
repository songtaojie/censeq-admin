import request from '/@/utils/request';
import type { PagedResultDto } from '../base';

export interface OpenIddictScopeDto {
  id: string;
  name: string;
  displayName?: string;
  description?: string;
  resources: string[];
  creationTime?: string;
  creatorId?: string;
  lastModificationTime?: string;
  lastModifierId?: string;
}

export interface OpenIddictScopeCreateDto {
  name: string;
  displayName?: string;
  description?: string;
  resources: string[];
}

export interface OpenIddictScopeUpdateDto {
  displayName?: string;
  description?: string;
  resources: string[];
}

export interface GetOpenIddictScopesInput {
  filter?: string;
  sorting?: string;
  skipCount?: number;
  maxResultCount?: number;
}

export function useOpenIddictScopeApi() {
  return {
    /**
     * 获取作用域列表
     */
    getList: (input: GetOpenIddictScopesInput) => {
      return request<PagedResultDto<OpenIddictScopeDto>>({
        url: '/api/openIddict/scopes',
        method: 'get',
        params: input,
      });
    },

    /**
     * 获取单个作用域
     */
    get: (id: string) => {
      return request<OpenIddictScopeDto>({
        url: `/api/openIddict/scopes/${id}`,
        method: 'get',
      });
    },

    /**
     * 创建作用域
     */
    create: (input: OpenIddictScopeCreateDto) => {
      return request<OpenIddictScopeDto>({
        url: '/api/openIddict/scopes',
        method: 'post',
        data: input,
      });
    },

    /**
     * 更新作用域
     */
    update: (id: string, input: OpenIddictScopeUpdateDto) => {
      return request<OpenIddictScopeDto>({
        url: `/api/openIddict/scopes/${id}`,
        method: 'put',
        data: input,
      });
    },

    /**
     * 删除作用域
     */
    delete: (id: string) => {
      return request({
        url: `/api/openIddict/scopes/${id}`,
        method: 'delete',
      });
    },

    /**
     * 检查名称是否存在
     */
    checkNameExists: (name: string, excludeId?: string) => {
      return request<boolean>({
        url: '/api/openIddict/scopes/check-name',
        method: 'get',
        params: { name, excludeId },
      });
    },
  };
}
