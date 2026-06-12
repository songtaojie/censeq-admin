import type { IdentitySessionDto } from '/@/api/models/identity';
import type { OnlineUserInfo } from '/@/api/models/signalr';

export interface SessionWithOnlineConnections extends IdentitySessionDto {
	onlineConnections: OnlineUserInfo[];
}

export function mergeSessionsWithOnlineConnections(sessions: IdentitySessionDto[], onlineUsers: OnlineUserInfo[]): SessionWithOnlineConnections[] {
	const onlineUsersBySessionId = onlineUsers.reduce<Record<string, OnlineUserInfo[]>>((map, user) => {
		if (!user.sessionId) return map;
		map[user.sessionId] ??= [];
		map[user.sessionId].push(user);
		return map;
	}, {});

	return sessions.map((session) => ({
		...session,
		onlineConnections: onlineUsersBySessionId[session.sessionId] ?? [],
	}));
}
