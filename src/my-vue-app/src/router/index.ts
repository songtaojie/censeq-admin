import { createRouter, createWebHistory, RouteRecordRaw } from 'vue-router';

const routes: RouteRecordRaw[] = [
	{
		path: '/',
		name: 'Login',
		// 注意这里要带上文件后缀.vue
		component: () => import('@/pages/login/Login.vue'),
		meta: {
			title: '登录',
		},
	},
];

const router = createRouter({
	history: createWebHistory(),
	routes,
	strict: true,
	// 期望滚动到哪个的位置
	scrollBehavior(to, from, savedPosition) {
		return new Promise((resolve) => {
			if (savedPosition) {
				return savedPosition;
			} else {
				if (from.meta.saveSrollTop) {
					const top: number = document.documentElement.scrollTop || document.body.scrollTop;
					resolve({ left: 0, top });
				}
			}
		});
	},
});

export default router;
