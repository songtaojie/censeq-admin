import type { EmailSettingsDto, NameValueDto, SendTestEmailInput, UpdateEmailSettingsDto } from '/@/api/models/setting-management';
import { useBaseApi } from '../base';

const api = useBaseApi('setting-management');

const emailingPath = 'api/setting-management/emailing';
const timezonePath = 'api/setting-management/timezone';

/**
 * 设置管理：与 EmailSettingsController、TimeZoneSettingsController 路由一致。
 */
export function useSettingManagementApi() {
	return {
		getEmailSettings: async (): Promise<EmailSettingsDto> => {
			return await api.request<EmailSettingsDto>(emailingPath, 'GET');
		},

		updateEmailSettings: async (input: UpdateEmailSettingsDto): Promise<void> => {
			return await api.request<void>(emailingPath, 'POST', input);
		},

		sendTestEmail: async (input: SendTestEmailInput): Promise<void> => {
			return await api.request<void>(`${emailingPath}/send-test-email`, 'POST', input);
		},

		getTimeZone: async (): Promise<string> => {
			return await api.request<string>(timezonePath, 'GET');
		},

		getTimeZoneOptions: async (): Promise<NameValueDto[]> => {
			return await api.request<NameValueDto[]>(`${timezonePath}/timezones`, 'GET');
		},

		updateTimeZone: async (timezone: string): Promise<void> => {
			const qs = `timezone=${encodeURIComponent(timezone)}`;
			return await api.request<void>(`${timezonePath}?${qs}`, 'POST', undefined);
		},
	};
}
