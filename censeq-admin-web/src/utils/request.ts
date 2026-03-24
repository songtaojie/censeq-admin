import axios, { AxiosError, AxiosInstance, AxiosResponse } from 'axios';
import { ElMessage, ElMessageBox } from 'element-plus';
import { Session } from '/@/utils/storage';
import qs from 'qs';
import { useOidc } from '/@/composables/useOidc';

export interface ApiResponse<T = any> {
	code: number;
	message: string;
	data: T;
}

/**
 * 从 ABP RemoteServiceErrorResponse（body 多为 `{ error: { message, details, validationErrors } }`）
 * 或直连字段中解析可读错误文案；避免 HTTP/2 下 `statusText` 为空导致 ElMessage 只有图标无文字。
 */
function getHttpErrorMessage(error: AxiosError | any): string {
	if (error?.message === 'canceled' || axios.isCancel?.(error)) {
		return '请求已取消';
	}

	const response = error?.response as AxiosResponse<any> | undefined;
	const status = response?.status;

	if (error?.message?.indexOf?.('timeout') !== -1) {
		return '网络超时';
	}
	if (error?.message === 'Network Error') {
		return '网络连接错误';
	}

	if (!response) {
		return error?.message && typeof error.message === 'string' ? error.message : '请求失败';
	}

	const data = response.data;
	const errObj = data?.error && typeof data.error === 'object' ? data.error : data;

	if (errObj && typeof errObj === 'object') {
		const pieces: string[] = [];
		if (typeof errObj.message === 'string' && errObj.message.trim()) {
			pieces.push(errObj.message.trim());
		}
		if (typeof errObj.details === 'string' && errObj.details.trim()) {
			pieces.push(errObj.details.trim());
		}
		if (Array.isArray(errObj.validationErrors)) {
			for (const ve of errObj.validationErrors) {
				if (ve?.message) pieces.push(String(ve.message).trim());
			}
		}
		if (pieces.length > 0) {
			const combined = pieces.join('；').replace(/\r\n/g, ' ');
			return combined.length > 800 ? `${combined.slice(0, 800)}…` : combined;
		}
	}

	if (typeof data === 'string' && data.trim()) {
		return data.trim().length > 800 ? `${data.trim().slice(0, 800)}…` : data.trim();
	}

	const statusText = typeof response.statusText === 'string' ? response.statusText.trim() : '';
	if (statusText) return statusText;

	if (status === 404) return '接口路径找不到';

	return typeof status === 'number' ? `请求失败（${status}）` : '请求失败';
}

// 定义请求中止控制器映射表
const abortControllerMap: Map<string, AbortController> = new Map();

// 配置新建一个 axios 实例
export const service: AxiosInstance = axios.create({
	baseURL: import.meta.env.VITE_API_URL,
	timeout: 50000,
	headers: { 'Content-Type': 'application/json' },
	withCredentials: true,
	paramsSerializer: {
		serialize(params) {
			return qs.stringify(params, { allowDots: true });
		},
	},
});

// 从 Cookie 获取 XSRF-TOKEN（与 RequestVerificationToken 头一致，须 URL 解码）
function getXsrfToken(): string | undefined {
	const raw = document.cookie
		.split('; ')
		.find((row) => row.startsWith('XSRF-TOKEN='))
		?.split('=')
		.slice(1)
		.join('=');
	if (!raw) return undefined;
	try {
		return decodeURIComponent(raw);
	} catch {
		return raw;
	}
}

// 添加请求拦截器
service.interceptors.request.use(
	async (config) => {
		// 在发送请求之前做些什么 token
		const { isAuthenticated, getAcessToken } = useOidc();
		var xsrfToken = getXsrfToken();
		if (xsrfToken) {
			config.headers!['RequestVerificationToken'] = `${xsrfToken}`;
		}
		var acessToken = await getAcessToken();
		if(!config.headers.has('Authorization') && acessToken)
		{
			config.headers['Authorization'] = `Bearer ${acessToken}`;
		}
		// 记录中止控制信息
		const controller = new AbortController();
		config.signal = controller.signal;
		const url = config.url || '';
		abortControllerMap.set(url, controller);
		return config;
	},
	(error) => {
		// 对请求错误做些什么
		return Promise.reject(error);
	}
);

// 添加响应拦截器
service.interceptors.response.use(
	(response : AxiosResponse<any>) => {
		// 对响应数据做点什么（2xx 均为成功：如 DELETE 常返回 204、POST 可能 201）
		const status = response.status;
		if (status < 200 || status >= 300) {
			// `token` 过期或者账号已在别处登录
			if (status === 401) {
				Session.clear(); // 清除浏览器全部临时缓存
				window.location.href = '/'; // 去登录页
				ElMessageBox.alert('你已被登出，请重新登录', '提示', {})
					.then(() => {})
					.catch(() => {});
			}
			return Promise.reject(response);
		}
		return response.data;
	},
	(error: AxiosError) => {
		ElMessage.error(getHttpErrorMessage(error));
		return Promise.reject(error);
	}
);

// 取消指定请求
export const cancelRequest = (url: string | string[]) => {
	const urlList = Array.isArray(url) ? url : [url];
	for (const _url of urlList) {
		abortControllerMap.get(_url)?.abort();
		abortControllerMap.delete(_url);
	}
};

// 取消全部请求
export const cancelAllRequest = () => {
	for (const [_, controller] of abortControllerMap) {
		controller.abort();
	}
	abortControllerMap.clear();
};

// 导出 axios 实例
export default service;
