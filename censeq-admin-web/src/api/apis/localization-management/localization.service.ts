import type {
	LocalizationResourceDto,
	CreateUpdateLocalizationResourceDto,
	LocalizationCultureDto,
	CreateUpdateLocalizationCultureDto,
	LocalizationTextDto,
	CreateLocalizationTextDto,
	UpdateLocalizationTextDto,
	GetLocalizationTextsInput,
} from '/@/api/models/localization-management';
import type { PagedResponseDto } from '/@/api/models';
import { useBaseApi } from '../base';

const api = useBaseApi('localization-management');
const base = 'api/localization-management';

export function useLocalizationManagementApi() {
	return {
		// ---------- Resources ----------
		getResources: (): Promise<LocalizationResourceDto[]> =>
			api.request<LocalizationResourceDto[]>(`${base}/resources`, 'GET'),

		createResource: (input: CreateUpdateLocalizationResourceDto): Promise<LocalizationResourceDto> =>
			api.request<LocalizationResourceDto>(`${base}/resources`, 'POST', input),

		updateResource: (id: string, input: CreateUpdateLocalizationResourceDto): Promise<LocalizationResourceDto> =>
			api.request<LocalizationResourceDto>(`${base}/resources/${id}`, 'PUT', input),

		deleteResource: (id: string): Promise<void> =>
			api.request<void>(`${base}/resources/${id}`, 'DELETE'),

		// ---------- Cultures ----------
		getCultures: (isEnabled?: boolean): Promise<LocalizationCultureDto[]> =>
			api.request<LocalizationCultureDto[]>(`${base}/cultures`, 'GET', {
				isEnabled: isEnabled ?? undefined,
			}),

		createCulture: (input: CreateUpdateLocalizationCultureDto): Promise<LocalizationCultureDto> =>
			api.request<LocalizationCultureDto>(`${base}/cultures`, 'POST', input),

		updateCulture: (id: string, input: CreateUpdateLocalizationCultureDto): Promise<LocalizationCultureDto> =>
			api.request<LocalizationCultureDto>(`${base}/cultures/${id}`, 'PUT', input),

		deleteCulture: (id: string): Promise<void> =>
			api.request<void>(`${base}/cultures/${id}`, 'DELETE'),

		// ---------- Texts ----------
		getTexts: (input: GetLocalizationTextsInput): Promise<PagedResponseDto<LocalizationTextDto>> =>
			api.request<PagedResponseDto<LocalizationTextDto>>(`${base}/texts`, 'GET', input),

		createText: (input: CreateLocalizationTextDto): Promise<LocalizationTextDto> =>
			api.request<LocalizationTextDto>(`${base}/texts`, 'POST', input),

		updateText: (id: string, input: UpdateLocalizationTextDto): Promise<LocalizationTextDto> =>
			api.request<LocalizationTextDto>(`${base}/texts/${id}`, 'PUT', input),

		deleteText: (id: string): Promise<void> =>
			api.request<void>(`${base}/texts/${id}`, 'DELETE'),
	};
}
