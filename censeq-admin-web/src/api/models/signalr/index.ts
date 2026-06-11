/** 当前在线用户的单个 SignalR 连接信息。 */
export interface OnlineUserInfo {
	/** SignalR 当前连接 ID。 */
	connectionId: string;
	/** 登录用户 ID。 */
	userId: string;
	/** 登录用户名。 */
	userName: string;
	/** 用户显示名称。 */
	name?: string | null;
	/** 当前租户 ID，宿主侧为空。 */
	tenantId?: string | null;
	/** 当前登录会话 ID。 */
	sessionId?: string | null;
	/** 客户端 IP 地址。 */
	ip?: string | null;
	/** 客户端 User-Agent 信息。 */
	userAgent?: string | null;
	/** 连接建立时间。 */
	connectedAt: string;
}

/** 在线用户连接状态变更事件。 */
export interface OnlineUserChange {
	/** 发生变更的在线用户连接信息。 */
	user: OnlineUserInfo;
	/** true 表示上线，false 表示离线。 */
	online: boolean;
}

/** 管理员强制指定连接下线时提交的参数。 */
export interface ForceOfflineInput {
	/** 需要强制下线的 SignalR 连接 ID。 */
	connectionId: string;
	/** 展示给客户端的下线原因。 */
	reason?: string;
}

/** 服务端推送到前端的实时通知消息。 */
export interface NotificationMessage {
	/** 通知唯一标识。 */
	id: string;
	/** 通知标题。 */
	title: string;
	/** 通知正文内容。 */
	content: string;
	/** 通知类型，前端可据此展示不同样式。 */
	type: 'info' | 'success' | 'warning' | 'error' | string;
	/** 通知创建时间。 */
	createdAt: string;
	/** 业务扩展字段。 */
	extraProperties?: Record<string, string | null>;
}
