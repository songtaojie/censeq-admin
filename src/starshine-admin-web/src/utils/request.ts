import axios, { AxiosInstance } from 'axios';
import { ElMessage, ElMessageBox } from 'element-plus';
import { Session } from '/@/utils/storage';
import qs from 'qs';
import { useOidc } from '/@/composables/useOidc';
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

// 从 Cookie 获取 XSRF-TOKEN 的函数
function getXsrfToken() {
	return document.cookie
		.split('; ')
		.find((row) => row.startsWith('XSRF-TOKEN='))
		?.split('=')[1];
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
	(response) => {
		// 对响应数据做点什么
		const res = response.data;
		if (res.code && res.code !== 0) {
			// `token` 过期或者账号已在别处登录
			if (res.code === 401 || res.code === 4001) {
				Session.clear(); // 清除浏览器全部临时缓存
				window.location.href = '/'; // 去登录页
				ElMessageBox.alert('你已被登出，请重新登录', '提示', {})
					.then(() => {})
					.catch(() => {});
			}
			return Promise.reject(service.interceptors.response);
		} else {
			return res;
		}
	},
	(error) => {
		// 对响应错误做点什么
		if (error.message.indexOf('timeout') != -1) {
			ElMessage.error('网络超时');
		} else if (error.message == 'Network Error') {
			ElMessage.error('网络连接错误');
		} else {
			if (error.response.data) ElMessage.error(error.response.statusText);
			else ElMessage.error('接口路径找不到');
		}
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
