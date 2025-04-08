import vuePlugin from 'eslint-plugin-vue';
import typescriptEslint from '@typescript-eslint/eslint-plugin';
import globals from 'globals';
import parser from 'vue-eslint-parser';
import js from '@eslint/js';

export default [
	{
		ignores: [
			'**/*.sh',
			'**/node_modules',
			'**/lib',
			'**/*.md',
			'**/*.scss',
			'**/*.woff',
			'**/*.ttf',
			'**/.vscode',
			'**/.idea',
			'**/dist',
			'**/mock',
			'**/public',
			'**/bin',
			'**/build',
			'**/config',
			'**/index.html',
			'src/assets',
		],
	},
	// 使用 @eslint/js 的推荐配置
	js.configs.recommended,
	// 直接加载 vue 插件配置（不再需要 compat）
	{
		files: ['**/*.vue'],
		...vuePlugin.configs['vue3-essential'],
	},
	{
		plugins: {
			vue: vuePlugin,
			'@typescript-eslint': typescriptEslint,
		},
		languageOptions: {
			globals: {
				...globals.browser,
				...globals.node,
			},
			parser: parser,
			ecmaVersion: 12,
			sourceType: 'module',
			parserOptions: {
				parser: '@typescript-eslint/parser',
			},
		},
		rules: {
			// 你的规则保持不变
		},
	},
	{
		files: ['**/*.ts', '**/*.tsx', '**/*.vue'],
		rules: {
			'no-undef': 'off',
		},
	},
];
