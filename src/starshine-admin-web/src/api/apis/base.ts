import { Configuration } from './configuration';
import { service, cancelRequest } from '/@/utils/request';
import globalAxios, { AxiosResponse, AxiosRequestConfig, AxiosInstance } from 'axios';
import { PagedResultDto } from '../models';
export const BASE_PATH = '/'.replace(/\/+$/, '');

// 接口基类
export const useBaseApi = (module: string) => {
	const baseUrl = `/api/${module}/`;
	const request = <T>(config: AxiosRequestConfig<T>, cancel: boolean = false) => {
		if (cancel) {
			cancelRequest(config.url || '');
			return Promise.resolve({} as AxiosResponse<any, any>);
		}
		return service(config);
	};
	return {
		baseUrl: baseUrl,
		request: request,
		page: function (data: any, action: string, cancel: boolean = false) {
			return request(
				{
					url: baseUrl + action,
					method: 'get',
					data,
				},
				cancel
			);
		},
		detail: function (id: any, cancel: boolean = false) {
			return request(
				{
					url: baseUrl + 'detail',
					method: 'get',
					data: { id },
				},
				cancel
			);
		},
		dropdownData: function (data: any, cancel: boolean = false) {
			return request(
				{
					url: baseUrl + 'dropdownData',
					method: 'post',
					data,
				},
				cancel
			);
		},
		add: function (data: any, cancel: boolean = false) {
			return request(
				{
					url: baseUrl + 'add',
					method: 'post',
					data,
				},
				cancel
			);
		},
		update: function (data: any, cancel: boolean = false) {
			return request(
				{
					url: baseUrl + 'update',
					method: 'post',
					data,
				},
				cancel
			);
		},
		setStatus: function (data: any, cancel: boolean = false) {
			return request(
				{
					url: baseUrl + 'setStatus',
					method: 'post',
					data,
				},
				cancel
			);
		},
		delete: function (data: any, cancel: boolean = false) {
			return request(
				{
					url: baseUrl + 'delete',
					method: 'post',
					data,
				},
				cancel
			);
		},
		batchDelete: function (data: any, cancel: boolean = false) {
			return request(
				{
					url: baseUrl + 'batchDelete',
					method: 'post',
					data,
				},
				cancel
			);
		},
		exportData: function (data: any, cancel: boolean = false) {
			return request(
				{
					responseType: 'arraybuffer',
					url: baseUrl + 'export',
					method: 'post',
					data,
				},
				cancel
			);
		},
		downloadTemplate: function (cancel: boolean = false) {
			return request(
				{
					responseType: 'arraybuffer',
					url: baseUrl + 'import',
					method: 'get',
				},
				cancel
			);
		},
		importData: function (file: any, cancel: boolean = false) {
			const formData = new FormData();
			formData.append('file', file);
			return request(
				{
					headers: { 'Content-Type': 'multipart/form-data;charset=UTF-8' },
					responseType: 'arraybuffer',
					url: baseUrl + 'import',
					method: 'post',
					data: formData,
				},
				cancel
			);
		},
		uploadFile: function (params: any, action: string, cancel: boolean = false) {
			const formData = new FormData();
			formData.append('file', params.file);
			// 自定义参数
			if (params.data) {
				Object.keys(params.data).forEach((key) => {
					const value = params.data![key];
					if (Array.isArray(value)) {
						value.forEach((item) => formData.append(`${key}[]`, item));
						return;
					}
					formData.append(key, params.data![key]);
				});
			}
			return request(
				{
					url: baseUrl + action,
					method: 'POST',
					data: formData,
					headers: {
						'Content-Type': 'multipart/form-data;charset=UTF-8',
						ignoreCancelToken: true,
					},
				},
				cancel
			);
		},
	};
};

/**
 *
 * @export
 */
export const COLLECTION_FORMATS = {
	csv: ',',
	ssv: ' ',
	tsv: '\t',
	pipes: '|',
};

/**
 *
 * @export
 * @interface RequestArgs
 */
export interface RequestArgs {
	url: string;
	options: AxiosRequestConfig;
}

/**
 *
 * @export
 * @class BaseAPI
 */
export class BaseAPI {
	protected configuration: Configuration | undefined;

	constructor(
		configuration?: Configuration,
		protected basePath: string = BASE_PATH,
		protected axios: AxiosInstance = globalAxios
	) {
		if (configuration) {
			this.configuration = configuration;
			this.basePath = configuration.basePath || this.basePath;
		}
	}

	/**
	 *
	 * @summary 删除
	 * @param {*} [options] Override http request option.
	 * @throws {RequiredError}
	 */
	async Delete<Tout>(options?: AxiosRequestConfig): Promise<(axios?: AxiosInstance, basePath?: string) => Promise<AxiosResponse<Tout>>> {
		const newOptions = { method: 'DELETE', ...options };
		const localVarAxiosArgs = await ApiAxiosParamCreator(this.configuration).BuildParam(newOptions);
		return (axios: AxiosInstance = globalAxios, basePath: string = BASE_PATH) => {
			const axiosRequestArgs: AxiosRequestConfig = { ...localVarAxiosArgs.options, url: basePath + localVarAxiosArgs.url };
			return axios.request(axiosRequestArgs);
		};
	}
	/**
	 *
	 * @summary 删除
	 * @param {*} [options] Override http request option.
	 * @throws {RequiredError}
	 */
	async DeleteVoid(options?: AxiosRequestConfig): Promise<AxiosResponse<void>> {
		return this.Delete<void>(options).then((request) => request(this.axios, this.basePath));
	}

	/**
	 *
	 * @summary 删除
	 * @param {*} [options] Override http request option.
	 * @throws {RequiredError}
	 */
	async DeleteAdminResult<Tout>(options?: AxiosRequestConfig): Promise<AxiosResponse<Tout>> {
		return this.Delete<Tout>(options).then((request) => request(this.axios, this.basePath));
	}

	/**
	 *
	 * @summary 获取分页列表
	 * @param {*} [options] Override http request option.
	 * @throws {RequiredError}
	 */
	async Page<Tout>(
		options?: AxiosRequestConfig
	): Promise<(axios?: AxiosInstance, basePath?: string) => Promise<AxiosResponse<PagedResultDto<Tout>>>> {
		const newOptions = { method: 'GET', ...options };
		const localVarAxiosArgs = await ApiAxiosParamCreator(this.configuration).BuildParam(newOptions);
		return (axios: AxiosInstance = globalAxios, basePath: string = BASE_PATH) => {
			const axiosRequestArgs: AxiosRequestConfig = { ...localVarAxiosArgs.options, url: basePath + localVarAxiosArgs.url };
			return axios.request(axiosRequestArgs);
		};
	}
	/**
	 *
	 * @summary 获取分页列表
	 * @param {*} [options] Override http request option.
	 * @throws {RequiredError}
	 */
	async PageAdminResult<Tout>(options?: AxiosRequestConfig): Promise<AxiosResponse<PagedResultDto<Tout>>> {
		return this.Page<Tout>(options).then((request) => request(this.axios, this.basePath));
	}

	/**
	 *
	 * @summary 更新
	 * @param {*} [options] Override http request option.
	 * @throws {RequiredError}
	 */
	async Put<Tout>(options?: AxiosRequestConfig): Promise<(axios?: AxiosInstance, basePath?: string) => Promise<AxiosResponse<Tout>>> {
		const newOptions = { method: 'PUT', ...options };
		const localVarAxiosArgs = await ApiAxiosParamCreator(this.configuration).BuildParam(newOptions);
		return (axios: AxiosInstance = globalAxios, basePath: string = BASE_PATH) => {
			const axiosRequestArgs: AxiosRequestConfig = { ...localVarAxiosArgs.options, url: basePath + localVarAxiosArgs.url };
			return axios.request(axiosRequestArgs);
		};
	}
	/**
	 *
	 * @summary 更新
	 * @param {*} [options] Override http request option.
	 * @throws {RequiredError}
	 */
	async PutAdminResult<Tout>(options?: AxiosRequestConfig): Promise<AxiosResponse<Tout>> {
		return this.Put<Tout>(options).then((request) => request(this.axios, this.basePath));
	}

	/**
	 *
	 * @summary 获取
	 * @param {*} [options] Override http request option.
	 * @throws {RequiredError}
	 */
	async Get<Tout>(options?: AxiosRequestConfig): Promise<(axios?: AxiosInstance, basePath?: string) => Promise<AxiosResponse<Tout>>> {
		const newOptions = { method: 'GET', ...options };
		const localVarAxiosArgs = await ApiAxiosParamCreator(this.configuration).BuildParam(newOptions);
		return (axios: AxiosInstance = globalAxios, basePath: string = BASE_PATH) => {
			const axiosRequestArgs: AxiosRequestConfig = { ...localVarAxiosArgs.options, url: basePath + localVarAxiosArgs.url };
			return axios.request(axiosRequestArgs);
		};
	}
	/**
	 *
	 * @summary 获取
	 * @param {*} [options] Override http request option.
	 * @throws {RequiredError}
	 */
	async GetAdminResult<Tout>(options?: AxiosRequestConfig): Promise<AxiosResponse<Tout>> {
		return this.Get<Tout>(options).then((request) => request(this.axios, this.basePath));
	}
	/**
	 *
	 * @summary 更新
	 * @param {*} [options] Override http request option.
	 * @throws {RequiredError}
	 */
	async Post<Tout>(options?: AxiosRequestConfig): Promise<(axios?: AxiosInstance, basePath?: string) => Promise<AxiosResponse<Tout>>> {
		const newOptions = { method: 'POST', ...options };
		const localVarAxiosArgs = await ApiAxiosParamCreator(this.configuration).BuildParam(newOptions);
		return (axios: AxiosInstance = globalAxios, basePath: string = BASE_PATH) => {
			const axiosRequestArgs: AxiosRequestConfig = { ...localVarAxiosArgs.options, url: basePath + localVarAxiosArgs.url };
			return axios.request(axiosRequestArgs);
		};
	}

	/**
	 *
	 * @summary 更新
	 * @param {*} [options] Override http request option.
	 * @throws {RequiredError}
	 */
	async PostVoid(options?: AxiosRequestConfig): Promise<AxiosResponse<void>> {
		return this.Post<void>(options).then((request) => request(this.axios, this.basePath));
	}

	/**
	 *
	 * @summary 更新
	 * @param {*} [options] Override http request option.
	 * @throws {RequiredError}
	 */
	async PostAdminResult<Tout>(options?: AxiosRequestConfig): Promise<AxiosResponse<Tout>> {
		return this.Post<Tout>(options).then((request) => request(this.axios, this.basePath));
	}
}

/**
 *
 * @export
 * @class RequiredError
 * @extends {Error}
 */
export class RequiredError extends Error {
	name: 'RequiredError' = 'RequiredError';
	constructor(
		public field: string,
		msg?: string
	) {
		super(msg);
	}
}

export const ApiAxiosParamCreator = function (configuration?: Configuration) {
	return {
		/**
		 *
		 * @summary 构建参数
		 * @param {*} [options] Override http request option.
		 * @throws {RequiredError}
		 */
		BuildParam: async (options: AxiosRequestConfig = {}): Promise<RequestArgs> => {
			// use dummy base URL string because the URL constructor only accepts absolute URLs.
			const localVarUrlObj = new URL(options.url || '', 'https://example.com');
			let baseOptions;
			if (configuration) {
				baseOptions = configuration.baseOptions;
			}
			const localVarHeaderParameter = {} as any;
			const localVarQueryParameter = { ...options.params } as any;
			options.params = null;
			const localVarRequestOptions: AxiosRequestConfig = { method: 'POST', ...baseOptions, ...options };
			// authentication Bearer required

			localVarHeaderParameter['Content-Type'] = 'application/json-patch+json';

			const query = new URLSearchParams(localVarUrlObj.search);
			for (const key in localVarQueryParameter) {
				if (localVarQueryParameter[key] != undefined) {
					query.set(key, localVarQueryParameter[key]);
				}
			}

			localVarUrlObj.search = new URLSearchParams(query).toString();
			let headersFromBaseOptions = baseOptions && baseOptions.headers ? baseOptions.headers : {};

			localVarRequestOptions.headers = { ...localVarHeaderParameter, ...headersFromBaseOptions, ...options.headers };
			let needsSerialization;
			if (options.data instanceof FormData) {
				needsSerialization = false;
			} else {
				needsSerialization =
					typeof options.data !== 'string' ||
					(localVarRequestOptions.headers != undefined && localVarRequestOptions.headers['Content-Type'] === 'application/json');
			}
			localVarRequestOptions.data = needsSerialization ? JSON.stringify(options.data !== undefined ? options.data : {}) : options.data || '';

			return {
				url: localVarUrlObj.pathname + localVarUrlObj.search + localVarUrlObj.hash,
				// url: localVarUrlObj.pathname + localVarUrlObj.hash,
				options: localVarRequestOptions,
			};
		},
	};
};
