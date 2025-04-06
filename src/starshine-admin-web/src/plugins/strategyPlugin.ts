import { App, inject } from 'vue';

export class StrategyContainer<T> {
	private strategies: Map<string, T> = new Map();
	private defaultKey: string | null = null;

	register(key: string, impl: T) {
		this.strategies.set(key, impl);
	}

	get(key: string): T | undefined {
		return this.strategies.get(key) ?? this.getDefault();
	}

	setDefaultKey(key: string) {
		if (!this.strategies.has(key)) {
			throw new Error(`Default key "${key}" is not registered.`);
		}
		this.defaultKey = key;
	}

	getDefault(): T | undefined {
		return this.defaultKey ? this.strategies.get(this.defaultKey) : undefined;
	}

	getAllKeys(): string[] {
		return [...this.strategies.keys()];
	}
}

export function createStrategyPlugin<T>(
	token: string | symbol,
	options: {
		implementations: Record<string, T>;
		defaultKey: string;
	}
) {
	return {
		install(app: App) {
			const container = new StrategyContainer<T>();
			for (const [key, impl] of Object.entries(options.implementations)) {
				container.register(key, impl);
			}
			container.setDefaultKey(options.defaultKey);
			app.provide(token, container);
		},
	};
}

export function injectStrategy<T>(token: string | symbol): StrategyContainer<T> {
	const container = inject(token) as StrategyContainer<T>;
	if (!container) throw new Error(`StrategyContainer "${String(token)}" not found`);
	return container;
}
