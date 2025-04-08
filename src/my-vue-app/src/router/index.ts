import { createRouter, createWebHistory, RouteRecordRaw } from 'vue-router';
import NProgress from 'nprogress';
import 'nprogress/nprogress.css';
import { staticRoutes, notFoundAndNoPower, dynamicRoutes } from './route';
import { useKeepALiveNames } from '/@/stores/keepAliveNames';
import pinia from '/@/stores/index';
import { useOidc } from '/@/composables/useOidc';
import { useAuth } from '/@/composables/useAuth';

/**
 * 一维数组处理成多级嵌套数组（只保留二级：也就是二级以上全部处理成只有二级，keep-alive 支持二级缓存）
 * @description isKeepAlive 处理 `name` 值，进行缓存。顶级关闭，全部不缓存
 * @link 参考：https://v3.cn.vuejs.org/api/built-in-components.html#keep-alive
 * @param arr 处理后的一维路由菜单数组
 * @returns 返回将一维数组重新处理成 `定义动态路由（dynamicRoutes）` 的格式
 */
export function formatTwoStageRoutes(arr: any) {
	if (arr.length <= 0) return false;
	const newArr: any = [];
	const cacheList: Array<string> = [];
	arr.forEach((v: any) => {
		if (v.path == null || v.path == undefined) return;

		if (v.path === '/') {
			newArr.push({ component: v.component, name: v.name, path: v.path, redirect: v.redirect, meta: v.meta, children: [] });
		} else {
			// 判断是否是动态路由（xx/:id/:name），用于 tagsView 等中使用
			// 修复：https://gitee.com/lyt-top/vue-next-admin/issues/I3YX6G
			if (v.path.indexOf('/:') > -1) {
				v.meta['isDynamic'] = true;
				v.meta['isDynamicPath'] = v.path;
			}
			newArr[0].children.push({ ...v });
			// 存 name 值，keep-alive 中 include 使用，实现路由的缓存
			// 路径：/@/layout/routerView/parent.vue
			if (newArr[0].meta.isKeepAlive && v.meta.isKeepAlive) {
				cacheList.push(v.name);
			}
		}
	});
	const stores = useKeepALiveNames(pinia);
	stores.setCacheKeepAlive(cacheList);
	return newArr;
}

/**
 * 路由多级嵌套数组处理成一维数组
 * @param arr 传入路由菜单数据数组
 * @returns 返回处理后的一维路由菜单数组
 */
export function formatFlatteningRoutes(arr: any) {
	if (arr.length <= 0) return false;
	for (let i = 0; i < arr.length; i++) {
		if (arr[i].children) {
			arr = arr.slice(0, i + 1).concat(arr[i].children, arr.slice(i + 1));
		}
	}
	return arr;
}

export const router = createRouter({
	history: createWebHistory(),
	routes: [...notFoundAndNoPower, ...staticRoutes, ...dynamicRoutes],
	strict: true,
	// 期望滚动到哪个的位置
	// scrollBehavior(to, from, savedPosition) {
	// 	return new Promise((resolve) => {
	// 		if (savedPosition) {
	// 			return savedPosition;
	// 		} else {
	// 			if (from.meta.saveSrollTop) {
	// 				const top: number = document.documentElement.scrollTop || document.body.scrollTop;
	// 				resolve({ left: 0, top });
	// 			}
	// 		}
	// 	});
	// },
});
// 路由加载前
router.beforeEach(async (to, from, next) => {
	NProgress.configure({ showSpinner: false });
	if (to.meta.title) NProgress.start();
	debugger;
	if (to.meta.requireAuth) {
		const { isAuthenticated } = useAuth();
		const { login } = useOidc();
		if (isAuthenticated) {
			next();
		} else {
			await login();
			NProgress.done();
		}
	} else {
		next();
	}
});

// 路由加载后
router.afterEach(() => {
	NProgress.done();
});

// 导出路由
export default router;
