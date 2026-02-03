import { Configuration } from './configuration';
import { service, cancelRequest } from '/@/utils/request';
import globalAxios, { AxiosResponse, AxiosRequestConfig, AxiosInstance } from 'axios';
import { PagedResponseDto,PagedRequestDto } from '../models';
export const BASE_PATH = '/'.replace(/\/+$/, '');

type HttpMethod = 'GET' | 'POST' | 'PUT' | 'DELETE' | 'PATCH';

export interface ApiOptions {
  cancel?: boolean;
  responseType?: AxiosRequestConfig['responseType'];
  headers?: Record<string, string>;
}


export function useBaseApi<TModule extends string>(apiName: TModule) {

  const request = <TResult>(
    url: string,
    method: HttpMethod,
    data?: any,
    options: ApiOptions = {}
  ): Promise<TResult> => {
    // const url = baseUrl + url; // TODO根据apiName获取域名
    if (options.cancel) {
      cancelRequest(url);
      return Promise.resolve({} as TResult);
    }

    return service({
      url,
      method,
      data: ['POST', 'PUT', 'PATCH'].includes(method) ? data : undefined,
      params: ['GET', 'DELETE'].includes(method) ? data : undefined,
      headers: options.headers,
      responseType: options.responseType,
    });
  };

  return {
    request,

    // 常规封装
    page: <TResult>(url:string, data: PagedRequestDto, options?: ApiOptions) =>
      request<PagedResponseDto<TResult>>(url, 'GET', data, options),

    list: <TResult>(url :string, data: any, options?: ApiOptions) =>
      request<TResult>(url, 'GET', data, options),

    detail: <TResult>(url :string,id: any,  options?: ApiOptions) =>
      request<TResult>(url, 'GET', { id }, options),

    add: <TResult>(url :string, data: any, options?: ApiOptions) =>
      request<TResult>(url, 'POST', data, options),

    update: <TResult>(url :string,data: any,  options?: ApiOptions) =>
      request<TResult>(url, 'PUT', data, options),

    delete: <TResult>(url :string,data: any,  options?: ApiOptions) =>
      request<TResult>(url, 'DELETE', data, options),

    batchDelete: <TResult>(url :string,data: any,  options?: ApiOptions) =>
      request<TResult>(url, 'POST', data, options),

    exportData: (url :string, data: any, options?: ApiOptions) =>
      request<Blob>(url, 'POST', data, {
        ...options,
        responseType: 'arraybuffer',
      }),

    downloadTemplate: (url :string, options?: ApiOptions) =>
      request<Blob>(url, 'GET', undefined, {
        ...options,
        responseType: 'arraybuffer',
      }),

    importData: (url :string,file: File,  options?: ApiOptions) => {
      const formData = new FormData();
      formData.append('file', file);
      return request<Blob>(url, 'POST', formData, {
        ...options,
        headers: { 'Content-Type': 'multipart/form-data' },
        responseType: 'arraybuffer',
      });
    },

    uploadFile: <TResult>(
      url :string,
      file: File,
      extra: Record<string, any> = {},
      options?: ApiOptions
    ) => {
      const formData = new FormData();
      formData.append('file', file);
      Object.entries(extra).forEach(([key, val]) => {
        if (Array.isArray(val)) {
          val.forEach((item) => formData.append(`${key}[]`, item));
        } else {
          formData.append(key, val);
        }
      });

      return request<TResult>(url, 'POST', formData, {
        ...options,
        headers: { 'Content-Type': 'multipart/form-data' },
      });
    },
  };
}

