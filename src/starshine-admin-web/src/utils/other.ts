import { nextTick, defineAsyncComponent } from 'vue';
import type { App } from 'vue';
import * as svg from '@element-plus/icons-vue';
import router from '/@/router/index';
import pinia from '/@/stores/index';
import { storeToRefs } from 'pinia';
import { useThemeConfig } from '/@/stores/themeConfig';
import { i18n } from '/@/i18n/index';
import { Local } from '/@/utils/storage';
import { verifyUrl } from '/@/utils/toolsValidate';

// 引入组件
const SvgIcon = defineAsyncComponent(() => import('/@/components/svgIcon/index.vue'));

/**
 * 导出全局注册 element plus svg 图标
 * @param app vue 实例
 * @description 使用：https://element-plus.gitee.io/zh-CN/component/icon.html
 */
export function elSvg(app: App) {
	const icons = svg as any;
	for (const i in icons) {
		app.component(`ele-${icons[i].name}`, icons[i]);
	}
	app.component('SvgIcon', SvgIcon);
}

/**
 * 设置浏览器标题国际化
 * @method const title = useTitle(); ==> title()
 */
export function useTitle() {
	const stores = useThemeConfig(pinia);
	const { themeConfig } = storeToRefs(stores);
	nextTick(() => {
		let webTitle = '';
		let globalTitle: string = themeConfig.value.globalTitle;
		const { path, meta } = router.currentRoute.value;
		if (path === '/login') {
			webTitle = <string>meta.title;
		} else {
			webTitle = setTagsViewNameI18n(router.currentRoute.value);
		}
		document.title = `${webTitle} - ${globalTitle}` || globalTitle;
	});
}

/**
 * 设置 自定义 tagsView 名称、 自定义 tagsView 名称国际化
 * @param params 路由 query、params 中的 tagsViewName
 * @returns 返回当前 tagsViewName 名称
 */
export function setTagsViewNameI18n(item: any) {
	let tagsViewName: string = '';
	const { query, params, meta } = item;
	// 修复tagsViewName匹配到其他含下列单词的路由
	const pattern = /^\{("(zh-cn|en|zh-tw)":"[^,]+",?){1,3}}$/;
	if (query?.tagsViewName || params?.tagsViewName) {
		if (pattern.test(query?.tagsViewName) || pattern.test(params?.tagsViewName)) {
			// 国际化
			const urlTagsParams = (query?.tagsViewName && JSON.parse(query?.tagsViewName)) || (params?.tagsViewName && JSON.parse(params?.tagsViewName));
			tagsViewName = urlTagsParams[i18n.global.locale.value];
		} else {
			// 非国际化
			tagsViewName = query?.tagsViewName || params?.tagsViewName;
		}
	} else {
		// 非自定义 tagsView 名称
		tagsViewName = i18n.global.t(meta.title);
	}
	return tagsViewName;
}

/**
 * 图片懒加载
 * @param el dom 目标元素
 * @param arr 列表数据
 * @description data-xxx 属性用于存储页面或应用程序的私有自定义数据
 */
export const lazyImg = (el: string, arr: EmptyArrayType) => {
	const io = new IntersectionObserver((res) => {
		res.forEach((v: any) => {
			if (v.isIntersecting) {
				const { img, key } = v.target.dataset;
				v.target.src = img;
				v.target.onload = () => {
					io.unobserve(v.target);
					arr[key]['loading'] = false;
				};
			}
		});
	});
	nextTick(() => {
		document.querySelectorAll(el).forEach((img) => io.observe(img));
	});
};

/**
 * 全局组件大小
 * @returns 返回 `window.localStorage` 中读取的缓存值 `globalComponentSize`
 */
export const globalComponentSize = (): string => {
	const stores = useThemeConfig(pinia);
	const { themeConfig } = storeToRefs(stores);
	return Local.get('themeConfig')?.globalComponentSize || themeConfig.value?.globalComponentSize;
};

/**
 * 对象深克隆
 * @param obj 源对象
 * @returns 克隆后的对象
 */
export function deepClone(obj: EmptyObjectType) {
	let newObj: EmptyObjectType;
	try {
		newObj = obj.push ? [] : {};
	} catch (error) {
		newObj = {};
	}
	for (let attr in obj) {
		if (obj[attr] && typeof obj[attr] === 'object') {
			newObj[attr] = deepClone(obj[attr]);
		} else {
			newObj[attr] = obj[attr];
		}
	}
	return newObj;
}

/**
 * 判断是否是移动端
 */
export function isMobile() {
	if (navigator.userAgent.match(/('phone|pad|pod|iPhone|iPod|ios|iPad|Android|Mobile|BlackBerry|IEMobile|MQQBrowser|JUC|Fennec|wOSBrowser|BrowserNG|WebOS|Symbian|Windows Phone')/i)) {
		return true;
	} else {
		return false;
	}
}

/**
 * 判断数组对象中所有属性是否为空，为空则删除当前行对象
 * @description @感谢大黄
 * @param list 数组对象
 * @returns 删除空值后的数组对象
 */
export function handleEmpty(list: EmptyArrayType) {
	const arr = [];
	for (const i in list) {
		const d = [];
		for (const j in list[i]) {
			d.push(list[i][j]);
		}
		const leng = d.filter((item) => item === '').length;
		if (leng !== d.length) {
			arr.push(list[i]);
		}
	}
	return arr;
}

/**
 * 打开外部链接
 * @param val 当前点击项菜单
 */
export function handleOpenLink(val: RouteItem) {
	const { origin, pathname } = window.location;
	router.push(val.path);
	if (verifyUrl(<string>val.meta?.isLink)) window.open(val.meta?.isLink);
	else window.open(`${origin}${pathname}#${val.meta?.isLink}`);
}

/**
 * 判断给定值是否是字符串
 * @param {any} value 要验证的值
 * @returns {Boolean} true代表是字符串，false代表不是字符串
 */
export const isString = function (value: any) {
	return typeof value === 'string';
};
/**
 * 如果传递的值是JavaScript数组则返回true，否则返回false。
 * @param {Object} value 要测试的目标。
 * @returns {Boolean}
 */
export const isArray = function (value: any) {
	if ('isArray' in Array) {
		return Array.isArray(value);
	}
	return toString.call(value) === '[object Array]';
};
/**
 * 判断给定值是否为空
 * @param {any} value 要判断的值
 * @param {Boolean} allowEmptyString 是否允许空字符串
 * @returns {Boolean} true为空，false不为空
 */
export const isEmpty = function (value: any, allowEmptyString: boolean = false) {
	return value === undefined || value === null || (!allowEmptyString ? value === '' : false) || (isArray(value) && value.length === 0);
};

/**
 * 判断给定制是否是对象
 * @param {any} value 要验证的值
 * @returns {Boolean} true代表是对象，false代表不是对象
 */
export const isObject = function (value: any) {
	// eslint-disable-next-line no-useless-call
	if (toString.call(null) === '[object Object]') {
		// 在这里检查ownerDocument以排除DOM节点
		return value !== null && toString.call(value) === '[object Object]' && value.ownerDocument === undefined;
	}
	return toString.call(value) === '[object Object]';
};

/**
 * 是否是简单地对象
 * @param {any} value 要验证的值
 * @returns {Boolean} true代表是简单对象，false代表不是简单对象
 */
export const isSimpleObject = function (value: any) {
	return value instanceof Object && value.constructor === Object;
};
/**
 * 监察对象是否为空
 * @param {Object} object 要检查的对象
 * @return {Boolean} true不为空
 */
export const isEmptyObject = function (object: any) {
	var key;
	for (key in object) {
		if (object.hasOwnProperty(key)) {
			return false;
		}
	}
	return true;
};
/**
 * 是否是boolean
 * @param {any} value 要验证的值
 * @returns {Boolean} true代表是Bool值，false代表不是Bool值
 */
export const isBoolean = function (value: any) {
	return typeof value === 'boolean';
};
/**
 * 如果传递的值是数字，则返回true。 对于非有限数，返回false。
 * @param {Object} value 要测试的值。
 * @return {Boolean} 如果传递的值是数字，则返回true。 对于非有限数，返回false。
 */
export const isNumber = function (value: any) {
	return typeof value === 'number' && isFinite(value);
};
/**
 * 如果传递的值是JavaScript函数则返回“true”，否则返回“false”。
 * @param {Object} value 要测试的值.
 * @return {Boolean} 如果传递的值是JavaScript函数则返回“true”，否则返回“false”。
 */
export const isFunction = function (value: any) {
	if (typeof document !== 'undefined' && typeof document.getElementsByTagName('body') === 'function') {
		return !!value && toString.call(value) === '[object Function]';
	}

	return !!value && typeof value === 'function';
};
/**
 * 生成四位随机的十六进制数字
 */
function s4() {
	return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
}
export const newGuid = function () {
	return `${s4() + s4()}-${s4()}-${s4()}-${s4() + s4() + s4()}`;
};

export function dateFormat(time: string, format: string) {
	var date = new Date(time);
	if (isEmpty(format)) {
		format = 'yyyy-MM-dd';
	}
	var o = {
		'M+': date.getMonth() + 1, // 月份
		'd+': date.getDate(), // 日
		'H+': date.getHours(), // 小时
		'h+': date.getHours(), // 小时
		'm+': date.getMinutes(), // 分
		's+': date.getSeconds(), // 秒
		'q+': Math.floor((date.getMonth() + 3) / 3), // 季度
		S: date.getMilliseconds(), // 毫秒
	};
	if (/(y+)/.test(format)) {
		format = format.replace(RegExp.$1, `${date.getFullYear()}`.substr(4 - RegExp.$1.length));
	}
	for (var k in o) {
		if (new RegExp(`(${k})`).test(format)) {
			format = format.replace(RegExp.$1, RegExp.$1.length === 1 ? o[k] : `00${o[k]}`.substr(`${o[k]}`.length));
		}
	}
	return format;
}
/**
 * 字符串或者对象
 * @param {*} value 值
 * @param {*} isObj 是否是对象
 * @returns
 */
export function toHex(value: any, isObj: boolean = false) {
	var result = '';
	var str = value;
	if (isObj && isObject(value)) {
		str = JSON.stringify(value);
	}
	if (!isEmpty(str)) {
		str = str.toString();
		for (var i = 0; i < str.length; i++) {
			var temp = str.charCodeAt(i).toString(16);
			if (temp.length === 1) temp = `0${temp}00`;
			else if (temp.length === 2) temp = `${temp}00`;
			else if (temp.length === 3) temp = `${temp.substring(1, 3)}0${temp.substring(0, 1)}`;
			else if (temp.length === 4) temp = temp.substring(2, 4) + temp.substring(0, 2);
			result += temp;
		}
	}
	return result.toLowerCase();
}
export function formHex(value: any, isObj: boolean = false) {
	var result = '';
	if (!isEmpty(value)) {
		value = value.toString();
		for (var i = 0; i < value.length; i += 4) {
			result = result + String.fromCharCode(Number(`0x${value.substring(i + 2, i + 4)}${value.substring(i, i + 2)}`));
		}
	}
	if (!isEmpty(result) && isObj) {
		return JSON.parse(result);
	}
	return result;
}

/**
 * 统一批量导出
 * @method elSvg 导出全局注册 element plus svg 图标
 * @method useTitle 设置浏览器标题国际化
 * @method setTagsViewNameI18n 设置 自定义 tagsView 名称、 自定义 tagsView 名称国际化
 * @method lazyImg 图片懒加载
 * @method globalComponentSize() element plus 全局组件大小
 * @method deepClone 对象深克隆
 * @method isMobile 判断是否是移动端
 * @method handleEmpty 判断数组对象中所有属性是否为空，为空则删除当前行对象
 * @method handleOpenLink 打开外部链接
 */
const other = {
	elSvg: (app: App) => {
		elSvg(app);
	},
	useTitle: () => {
		useTitle();
	},
	setTagsViewNameI18n(route: RouteToFrom) {
		return setTagsViewNameI18n(route);
	},
	lazyImg: (el: string, arr: EmptyArrayType) => {
		lazyImg(el, arr);
	},
	globalComponentSize: () => {
		return globalComponentSize();
	},
	deepClone: (obj: EmptyObjectType) => {
		return deepClone(obj);
	},
	isMobile: () => {
		return isMobile();
	},
	handleEmpty: (list: EmptyArrayType) => {
		return handleEmpty(list);
	},
	handleOpenLink: (val: RouteItem) => {
		handleOpenLink(val);
	},
};

// 统一批量导出
export default other;
