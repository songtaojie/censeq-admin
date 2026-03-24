/**
 * 与 Censeq.SettingManagement.Application.Contracts 中 DTO 的 JSON（camelCase）字段对齐。
 */

export interface EmailSettingsDto {
	smtpHost?: string | null;
	smtpPort: number;
	smtpUserName?: string | null;
	smtpPassword?: string | null;
	smtpDomain?: string | null;
	smtpEnableSsl: boolean;
	smtpUseDefaultCredentials: boolean;
	defaultFromAddress?: string | null;
	defaultFromDisplayName?: string | null;
}

export interface UpdateEmailSettingsDto {
	smtpHost: string;
	smtpPort: number;
	smtpUserName: string;
	smtpPassword: string;
	smtpDomain: string;
	smtpEnableSsl: boolean;
	smtpUseDefaultCredentials: boolean;
	defaultFromAddress: string;
	defaultFromDisplayName: string;
}

export interface SendTestEmailInput {
	senderEmailAddress: string;
	targetEmailAddress: string;
	subject: string;
	body: string;
}

/** Volo.Abp.NameValue 序列化结果 */
export interface NameValueDto {
	name: string;
	value: string;
}
