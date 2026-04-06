import type { PagedAndSortedRequestDto } from '../core';

export interface GetAuditLogsRequest extends PagedAndSortedRequestDto {
	startTime?: string;
	endTime?: string;
	httpMethod?: string;
	url?: string;
	userName?: string;
	applicationName?: string;
	clientIpAddress?: string;
	hasException?: boolean;
	minExecutionDuration?: number;
	maxExecutionDuration?: number;
}

export interface AuditLogDto {
	id: string;
	applicationName?: string;
	userId?: string;
	userName?: string;
	tenantId?: string;
	tenantName?: string;
	executionTime: string;
	executionDuration: number;
	clientIpAddress?: string;
	clientName?: string;
	clientId?: string;
	correlationId?: string;
	browserInfo?: string;
	httpMethod?: string;
	url?: string;
	exceptions?: string;
	comments?: string;
	httpStatusCode?: number;
	hasException: boolean;
	entityChanges: EntityChangeDto[];
	actions: AuditLogActionDto[];
}

export interface EntityChangeDto {
	id: string;
	auditLogId: string;
	changeTime: string;
	changeType: number;
	entityId?: string;
	entityTypeFullName?: string;
	propertyChanges: EntityPropertyChangeDto[];
}

export interface EntityPropertyChangeDto {
	id: string;
	entityChangeId: string;
	propertyName?: string;
	propertyTypeFullName?: string;
	originalValue?: string;
	newValue?: string;
}

export interface AuditLogActionDto {
	id: string;
	auditLogId: string;
	serviceName?: string;
	methodName?: string;
	parameters?: string;
	executionTime: string;
	executionDuration: number;
}
