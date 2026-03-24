import service from '/@/utils/request';

/**
 * 从 API 站点拉取防伪 Cookie 对（.AspNetCore.Antiforgery + XSRF-TOKEN）。须在跨域 POST/PUT/DELETE 之前、且 axios withCredentials 已开启时调用。
 */
export async function ensureAntiforgeryCookies(): Promise<void> {
	await service.get('api/app/antiforgery');
}
