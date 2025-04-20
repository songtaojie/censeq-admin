export namespace Starshine {
	export interface Root {
		registerLocaleFn: (locale: string) => Promise<any>;
		skipGetAppConfiguration?: boolean;
		skipInitAuthService?: boolean;
		sendNullsAsQueryParam?: boolean;
		tenantKey?: string;
		localizations?: Localization[];
		othersGroup?: string;
		dynamicLayouts?: Map<string, string>;
		disableProjectNameInTitle?: boolean;
	}

	export interface Child {
		localizations?: Localization[];
	}

	export interface Localization {
		culture: string;
		resources: LocalizationResource[];
	}

	export interface LocalizationResource {
		resourceName: string;
		texts: Record<string, string>;
	}

	export interface HasPolicy {
		requiredPolicy?: string;
	}

	export type PagedResponse<T> = {
		totalCount: number;
	} & PagedItemsResponse<T>;

	export interface PagedItemsResponse<T> {
		items: T[];
	}

	export interface PageQueryParams {
		filter?: string;
		sorting?: string;
		skipCount?: number;
		maxResultCount?: number;
	}

	export interface Lookup {
		id: string;
		displayName: string;
	}

	export interface Nav {
		name: string;
		parentName?: string;
		requiredPolicy?: string;
		order?: number;
		invisible?: boolean;
	}

	export interface Route extends Nav {
		path?: string;
		layout?: string;
		iconClass?: string;
		group?: string;
		breadcrumbText?: string;
	}

	export interface BasicItem {
		id: string;
		name: string;
	}

	export interface Option<T> {
		key: Extract<keyof T, string>;
		value: T[Extract<keyof T, string>];
	}

	export interface Dictionary<T = any> {
		[key: string]: T;
	}
}
