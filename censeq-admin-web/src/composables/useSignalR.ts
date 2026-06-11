import { HubConnection, HubConnectionBuilder, HubConnectionState, LogLevel } from '@microsoft/signalr';
import { useOidc } from '/@/composables/useOidc';

interface SignalROptions {
	hubUrl: string;
	automaticReconnect?: number[];
}

export function useSignalR({ hubUrl, automaticReconnect = [0, 2000, 5000, 10000, 30000] }: SignalROptions) {
	let connection: HubConnection | null = null;
	let starting: Promise<HubConnection> | null = null;

	const buildUrl = () => {
		const baseUrl = (import.meta.env.VITE_API_URL || '').replace(/\/+$/, '');
		return `${baseUrl}${hubUrl}`;
	};

	const start = async () => {
		if (connection?.state === HubConnectionState.Connected) return connection;
		if (starting) return starting;

		const { getAcessToken } = useOidc();
		connection = new HubConnectionBuilder()
			.withUrl(buildUrl(), {
				accessTokenFactory: async () => (await getAcessToken()) ?? '',
			})
			.withAutomaticReconnect(automaticReconnect)
			.configureLogging(LogLevel.Warning)
			.build();

		starting = connection
			.start()
			.then(() => connection!)
			.finally(() => {
				starting = null;
			});

		return starting;
	};

	const stop = async () => {
		if (connection && connection.state !== HubConnectionState.Disconnected) {
			await connection.stop();
		}
		connection = null;
		starting = null;
	};

	const on = async <T = unknown>(event: string, handler: (payload: T) => void) => {
		const conn = await start();
		conn.off(event);
		conn.on(event, handler as (...args: unknown[]) => void);
	};

	const off = (event: string) => {
		connection?.off(event);
	};

	const invoke = async <TResult = void>(method: string, ...args: unknown[]) => {
		const conn = await start();
		return (await conn.invoke(method, ...args)) as TResult;
	};

	return { start, stop, on, off, invoke };
}
