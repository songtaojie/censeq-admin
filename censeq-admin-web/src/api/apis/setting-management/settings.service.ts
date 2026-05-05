import type {
	EmailSettingsDto,
	NameValueDto,
	SendTestEmailInput,
	UpdateEmailSettingsDto,
	SettingDefinitionDto,
	SettingDefinitionGetListInput,
	CreateSettingDefinitionDto,
	UpdateSettingDefinitionDto,
	SettingValueDto,
	SetSettingValueInput,
} from '/@/api/models/setting-management';
import type { PagedResponseDto } from '/@/api/models/core/dto';
import { useBaseApi } from '../base';

const api = useBaseApi('setting-management');

const emailingPath = 'api/setting-management/emailing';
const timezonePath = 'api/setting-management/timezone';
const settingDefinitionPath = 'api/setting-management/setting-definitions';
const settingValuePath = 'api/setting-management/setting-values';

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

		// ── SettingDefinition ─────────────────────────────────────────────
		getSettingDefinitions: async (input: SettingDefinitionGetListInput): Promise<PagedResponseDto<SettingDefinitionDto>> => {
			const params = new URLSearchParams();
			if (input.filter) params.append('filter', input.filter);
			if (input.skipCount !== undefined) params.append('skipCount', String(input.skipCount));
			if (input.maxResultCount !== undefined) params.append('maxResultCount', String(input.maxResultCount));
			const qs = params.toString() ? `?${params.toString()}` : '';
			return await api.request<PagedResponseDto<SettingDefinitionDto>>(`${settingDefinitionPath}${qs}`, 'GET');
		},

		getSettingDefinition: async (id: string): Promise<SettingDefinitionDto> => {
			return await api.request<SettingDefinitionDto>(`${settingDefinitionPath}/${id}`, 'GET');
		},

		createSettingDefinition: async (input: CreateSettingDefinitionDto): Promise<SettingDefinitionDto> => {
			return await api.request<SettingDefinitionDto>(settingDefinitionPath, 'POST', input);
		},

		updateSettingDefinition: async (id: string, input: UpdateSettingDefinitionDto): Promise<SettingDefinitionDto> => {
			return await api.request<SettingDefinitionDto>(`${settingDefinitionPath}/${id}`, 'PUT', input);
		},

		deleteSettingDefinition: async (id: string): Promise<void> => {
			return await api.request<void>(`${settingDefinitionPath}/${id}`, 'DELETE');
		},

		// ── SettingValue ───────────────────────────────────────────────────
		getSettingValue: async (name: string): Promise<SettingValueDto> => {
			return await api.request<SettingValueDto>(`${settingValuePath}?name=${encodeURIComponent(name)}`, 'GET');
		},

		setSettingValue: async (input: SetSettingValueInput): Promise<void> => {
			return await api.request<void>(settingValuePath, 'PUT', input);
		},

		deleteSettingValue: async (name: string, providerName: string, providerKey?: string | null): Promise<void> => {
			const params = new URLSearchParams({ name, providerName });
			if (providerKey) params.append('providerKey', providerKey);
			return await api.request<void>(`${settingValuePath}?${params.toString()}`, 'DELETE');
		},
	};
}
