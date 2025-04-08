import { RouteRecordRaw } from 'vue-router';

/**
 * 定义动态路由
 * 前端添加路由，请在顶级节点的 `children 数组` 里添加
 * @description 未开启 isRequestRoutes 为 true 时使用（前端控制路由），开启时第一个顶级 children 的路由将被替换成接口请求回来的路由数据
 * @description 各字段请查看 `/@/views/system/menu/component/addMenu.vue 下的 ruleForm`
 * @returns 返回路由菜单数据
 */
export const dynamicRoutes: Array<RouteRecordRaw> = [
	{
		path: '/',
		name: '/',
		component: () => import('/@/layout/index.vue'),
		redirect: '/dashboard/home',
		meta: {
			isKeepAlive: true,
			requireAuth: true,
		},
		children: [],
	},
	{
		path: '/callback',
		name: 'callback',
		component: () => import('/@/views/callback.vue'),
		meta: {
			isKeepAlive: true,
			requireAuth: false,
		},
		children: [],
	},
	// {
	// 	path: '/platform/job/dashboard',
	// 	name: 'jobDashboard',
	// 	component: () => import('/@/views/system/job/dashboard.vue'),
	// 	meta: {
	// 		title: '任务看板',
	// 		isLink: import.meta.env.VITE_API_URL + '/schedule',
	// 		isHide: true,
	// 		isKeepAlive: true,
	// 		isAffix: false,
	// 		isIframe: true,
	// 		icon: 'ele-Clock',
	// 	},
	// },
];

/**
 * 定义静态路由（默认路由）
 * 此路由不要动，前端添加路由的话，请在 `dynamicRoutes 数组` 中添加
 * @description 前端控制直接改 dynamicRoutes 中的路由，后端控制不需要修改，请求接口路由数据时，会覆盖 dynamicRoutes 第一个顶级 children 的内容（全屏，不包含 layout 中的路由出口）
 * @returns 返回路由菜单数据
 */
export const staticRoutes: Array<RouteRecordRaw> = [
	// {
	// 	path: '/callback',
	// 	name: 'callback',
	// 	component: () => import('/@/views/callback.vue'),
	// 	meta: {
	// 		isKeepAlive: true,
	// 		requireAuth: false,
	// 	},
	// 	children: [],
	// },
];

/**
 * 定义404、401界面
 * @link 参考：https://next.router.vuejs.org/zh/guide/essentials/history-mode.html#netlify
 */
export const notFoundAndNoPower = [
	// {
	// 	path: '/:path(.*)*',
	// 	name: 'notFound',
	// 	component: () => import('/@/views/errors/404.vue'),
	// 	meta: {
	// 		title: 'message.staticRoutes.notFound',
	// 		isHide: true,
	// 	},
	// },
	{
		path: '/401',
		name: 'noPower',
		component: () => import('/@/views/errors/401.vue'),
		meta: {
			title: 'message.staticRoutes.noPower',
			isHide: true,
		},
	},
];
