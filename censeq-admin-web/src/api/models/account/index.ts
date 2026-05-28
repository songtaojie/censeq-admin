/** 当前登录用户的个人资料。 */
export interface ProfileDto {
	/** 登录账号。 */
	userName: string;
	/** 邮箱。 */
	email: string;
	/** 名。 */
	name?: string;
	/** 姓。 */
	surname?: string;
	/** 手机号。 */
	phoneNumber?: string;
	/** 头像地址。 */
	avatarUrl?: string;
	/** 是否外部账号。 */
	isExternal: boolean;
	/** 是否已设置密码。 */
	hasPassword: boolean;
	/** 并发标记。 */
	concurrencyStamp?: string;
	/** ABP 扩展属性。 */
	extraProperties?: Record<string, unknown>;
}

/** 更新当前登录用户个人资料的请求。 */
export interface UpdateProfileDto {
	/** 登录账号。 */
	userName?: string;
	/** 邮箱。 */
	email?: string;
	/** 名。 */
	name?: string;
	/** 姓。 */
	surname?: string;
	/** 手机号。 */
	phoneNumber?: string;
	/** 头像地址。 */
	avatarUrl?: string;
	/** 并发标记。 */
	concurrencyStamp?: string;
	/** ABP 扩展属性。 */
	extraProperties?: Record<string, unknown>;
}
