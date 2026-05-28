import type { PagedAndSortedRequestDto } from '../core';

/** 文件上传记录。 */
export interface FileRecordDto {
	/** 文件记录主键。 */
	id: string;
	/** 文件所属租户。 */
	tenantId?: string | null;
	/** 上传用户标识。 */
	ownerUserId?: string | null;
	/** 用户上传时的原始文件名。 */
	originalName: string;
	/** 服务端保存的文件基础名。 */
	fileName: string;
	/** 文件扩展名。 */
	extension: string;
	/** 文件 MIME 类型。 */
	contentType: string;
	/** 相对于 wwwroot 的存储路径。 */
	relativePath: string;
	/** 前端可访问的文件地址。 */
	url: string;
	/** 文件大小，单位字节。 */
	size: number;
	/** 文件内容哈希。 */
	hash?: string | null;
	/** 业务分类，例如 avatar、common。 */
	category?: string | null;
	/** 是否公开访问。 */
	isPublic: boolean;
	/** 创建时间。 */
	creationTime?: string;
}

/** 文件记录分页查询条件。 */
export interface GetFileRecordsRequest extends PagedAndSortedRequestDto {
	/** 文件名关键字。 */
	filter?: string;
	/** 文件业务分类。 */
	category?: string;
}
