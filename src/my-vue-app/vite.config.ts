import { defineConfig, loadEnv, ConfigEnv } from 'vite';
import vue from '@vitejs/plugin-vue';
import { resolve } from 'path';
const pathResolve = (dir: string) => {
	return resolve(__dirname, '.', dir);
};
const alias: Record<string, string> = {
	'/@': pathResolve('./src/'),
	'vue-i18n': 'vue-i18n/dist/vue-i18n.cjs.js',
};
// https://vite.dev/config/
export default defineConfig((mode: ConfigEnv) => {
	const env = loadEnv(mode.mode, process.cwd());
	return {
		plugins: [vue()],
		resolve: { alias },
		server: {
			host: '0.0.0.0',
			port: env.VITE_PORT as unknown as number,
			open: JSON.parse(env.VITE_OPEN),
			hmr: true,
			proxy: {
				'^/[Uu]pload': {
					target: env.VITE_API_URL,
					changeOrigin: true,
				},
			},
		},
	};
});
