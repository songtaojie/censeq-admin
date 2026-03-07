import { Starshine } from './common';
import commonFunction from '/@/utils/commonFunction';
const { hasOwnProperty } = commonFunction();

export class ListResponseDto<T> {
	items?: T[];

	constructor(initialValues: Partial<ListResponseDto<T>> = {}) {
		for (const key in initialValues) {
			if (hasOwnProperty(initialValues, key)) {
				this[key] = initialValues[key];
			}
		}
	}
}
type ValueOf<T> = T[keyof T];
export class PagedResponseDto<T> extends ListResponseDto<T> {
	totalCount?: number;

	constructor(initialValues: Partial<PagedResponseDto<T>> = {}) {
		super(initialValues);
	}
}

export class ExtensibleObject {
	extraProperties?: Starshine.Dictionary<any>;

	constructor(initialValues: Partial<ExtensibleObject> = {}) {
		for (const key in initialValues) {
			if (hasOwnProperty(initialValues, key) && initialValues[key] !== undefined) {
				this[key] = initialValues[key];
			}
		}
	}
}

export class ExtensibleEntityDto<TKey = string> extends ExtensibleObject {
	id?: TKey;

	constructor(initialValues: Partial<ExtensibleEntityDto<TKey>> = {}) {
		super(initialValues);
	}
}

export class LimitedRequestDto {
	maxResultCount = 10;

	constructor(initialValues: Partial<LimitedRequestDto> = {}) {
		for (const key in initialValues) {
			if (hasOwnProperty(initialValues, key) && initialValues[key] !== undefined) {
				this[key] = initialValues[key] as ValueOf<LimitedRequestDto>;
			}
		}
	}
}

export class ExtensibleLimitedRequestDto extends ExtensibleEntityDto {
	maxResultCount = 10;

	constructor(initialValues: Partial<ExtensibleLimitedRequestDto> = {}) {
		super(initialValues);
	}
}

export class PagedRequestDto extends LimitedRequestDto {
	skipCount?: number;

	constructor(initialValues: Partial<PagedRequestDto> = {}) {
		super(initialValues);
	}
}

export class ExtensiblePagedRequestDto extends ExtensibleLimitedRequestDto {
	skipCount?: number;

	constructor(initialValues: Partial<ExtensiblePagedRequestDto> = {}) {
		super(initialValues);
	}
}

export class PagedAndSortedRequestDto extends PagedRequestDto {
	sorting?: string;

	constructor(initialValues: Partial<PagedAndSortedRequestDto> = {}) {
		super(initialValues);
	}
}

export class ExtensiblePagedAndSortedRequestDto extends ExtensiblePagedRequestDto {
	sorting?: string;

	constructor(initialValues: Partial<ExtensiblePagedAndSortedRequestDto> = {}) {
		super(initialValues);
	}
}

export class EntityDto<TKey = string> {
	id?: TKey;

	constructor(initialValues: Partial<EntityDto<TKey>> = {}) {
		for (const key in initialValues) {
			if (hasOwnProperty(initialValues, key)) {
				this[key] = initialValues[key];
			}
		}
	}
}

export class CreationAuditedEntityDto<TPrimaryKey = string> extends EntityDto<TPrimaryKey> {
	creationTime?: string | Date;
	creatorId?: string;

	constructor(initialValues: Partial<CreationAuditedEntityDto<TPrimaryKey>> = {}) {
		super(initialValues);
	}
}

export class CreationAuditedEntityWithUserDto<TPrimaryKey = string, TUserDto = any> extends CreationAuditedEntityDto<TPrimaryKey> {
	creator?: TUserDto;

	constructor(initialValues: Partial<CreationAuditedEntityWithUserDto<TPrimaryKey, TUserDto>> = {}) {
		super(initialValues);
	}
}

export class AuditedEntityDto<TPrimaryKey = string> extends CreationAuditedEntityDto<TPrimaryKey> {
	lastModificationTime?: string | Date;
	lastModifierId?: string;

	constructor(initialValues: Partial<AuditedEntityDto<TPrimaryKey>> = {}) {
		super(initialValues);
	}
}

/** @deprecated the class signature will change in v8.0 */
export class AuditedEntityWithUserDto<TPrimaryKey = string, TUserDto = any> extends AuditedEntityDto<TPrimaryKey> {
	creator?: TUserDto;
	lastModifier?: TUserDto;

	constructor(initialValues: Partial<AuditedEntityWithUserDto<TPrimaryKey, TUserDto>> = {}) {
		super(initialValues);
	}
}

export class FullAuditedEntityDto<TPrimaryKey = string> extends AuditedEntityDto<TPrimaryKey> {
	isDeleted?: boolean;
	deleterId?: string;
	deletionTime?: Date | string;

	constructor(initialValues: Partial<FullAuditedEntityDto<TPrimaryKey>> = {}) {
		super(initialValues);
	}
}
/** @deprecated the class signature will change in v8.0 */
export class FullAuditedEntityWithUserDto<TPrimaryKey = string, TUserDto = any> extends FullAuditedEntityDto<TPrimaryKey> {
	creator?: TUserDto;
	lastModifier?: TUserDto;
	deleter?: TUserDto;

	constructor(initialValues: Partial<FullAuditedEntityWithUserDto<TPrimaryKey, TUserDto>> = {}) {
		super(initialValues);
	}
}

export class ExtensibleCreationAuditedEntityDto<TPrimaryKey = string> extends ExtensibleEntityDto<TPrimaryKey> {
	creationTime?: Date | string;
	creatorId?: string;

	constructor(initialValues: Partial<ExtensibleCreationAuditedEntityDto<TPrimaryKey>> = {}) {
		super(initialValues);
	}
}

export class ExtensibleAuditedEntityDto<TPrimaryKey = string> extends ExtensibleCreationAuditedEntityDto<TPrimaryKey> {
	lastModificationTime?: Date | string;
	lastModifierId?: string;

	constructor(initialValues: Partial<ExtensibleAuditedEntityDto<TPrimaryKey>> = {}) {
		super(initialValues);
	}
}

export class ExtensibleAuditedEntityWithUserDto<TPrimaryKey = string, TUserDto = any> extends ExtensibleAuditedEntityDto<TPrimaryKey> {
	creator?: TUserDto;
	lastModifier?: TUserDto;

	constructor(initialValues: Partial<ExtensibleAuditedEntityWithUserDto<TPrimaryKey>> = {}) {
		super(initialValues);
	}
}

export class ExtensibleCreationAuditedEntityWithUserDto<
	TPrimaryKey = string,
	TUserDto = any,
> extends ExtensibleCreationAuditedEntityDto<TPrimaryKey> {
	creator?: TUserDto;

	constructor(initialValues: Partial<ExtensibleCreationAuditedEntityWithUserDto<TPrimaryKey>> = {}) {
		super(initialValues);
	}
}

export class ExtensibleFullAuditedEntityDto<TPrimaryKey = string> extends ExtensibleAuditedEntityDto<TPrimaryKey> {
	isDeleted?: boolean;
	deleterId?: string;
	deletionTime?: Date | string;

	constructor(initialValues: Partial<ExtensibleFullAuditedEntityDto<TPrimaryKey>> = {}) {
		super(initialValues);
	}
}

export class ExtensibleFullAuditedEntityWithUserDto<TPrimaryKey = string, TUserDto = any> extends ExtensibleFullAuditedEntityDto<TPrimaryKey> {
	creator?: TUserDto;
	lastModifier?: TUserDto;
	deleter?: TUserDto;

	constructor(initialValues: Partial<ExtensibleFullAuditedEntityWithUserDto<TPrimaryKey>> = {}) {
		super(initialValues);
	}
}
