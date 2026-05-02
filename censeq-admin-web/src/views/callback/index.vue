<template>
	<div class="callback-container">
		<div class="callback-card">
			<div class="callback-icon" :class="callbackError ? 'is-error' : 'is-success'">
				<el-icon v-if="!callbackError && !loading" size="56"><CircleCheckFilled /></el-icon>
				<el-icon v-else-if="callbackError" size="56"><CircleCloseFilled /></el-icon>
				<el-icon v-else size="56" class="spin"><Loading /></el-icon>
			</div>

			<h2 class="callback-title">{{ callbackTitle }}</h2>
			<p class="callback-desc">{{ callbackDescription }}</p>

			<div v-if="!callbackError && loading" class="callback-progress">
				<el-progress :percentage="progress" :show-text="false" status="success" striped striped-flow :duration="10" />
			</div>

			<el-button v-if="callbackError" type="primary" size="large" @click="retryLogin" class="callback-btn">
				<el-icon><RefreshRight /></el-icon>
				重新登录
			</el-button>
		</div>
	</div>
</template>

<script setup lang="ts" name="callback">
import { computed, onBeforeMount, ref } from 'vue';
import { useRouter } from 'vue-router';
import { useOidc } from '/@/composables/useOidc';
import { Session } from '/@/utils/storage';
import { ensureAntiforgeryCookies } from '/@/utils/antiforgery';
import { CircleCheckFilled, CircleCloseFilled, Loading, RefreshRight } from '@element-plus/icons-vue';

const router = useRouter();
const { handleRedirectCallback, login } = useOidc();
const callbackError = ref('');
const loading = ref(true);
const progress = ref(0);

const callbackTitle = computed(() => {
	if (loading.value) return '正在验证身份…';
	return callbackError.value ? '登录失败' : '登录成功';
});

const callbackDescription = computed(() => {
	if (loading.value) return '请稍候，正在处理授权回调';
	if (callbackError.value) return callbackError.value;
	return '即将跳转到目标页面';
});

const getErrorMessage = (error: unknown) => {
	if (error instanceof Error) return error.message;
	if (typeof error === 'string') return error;
	try {
		return JSON.stringify(error);
	} catch {
		return '未知错误';
	}
};

const retryLogin = async () => {
	await login();
};

// 模拟进度条推进，让等待体验更流畅
const startProgress = () => {
	const timer = setInterval(() => {
		if (progress.value < 90) {
			progress.value += Math.random() * 15;
		} else {
			clearInterval(timer);
		}
	}, 300);
	return timer;
};

onBeforeMount(async () => {
	const progressTimer = startProgress();
	try {
		await handleRedirectCallback();
		try {
			await ensureAntiforgeryCookies();
		} catch {
			/* 由后续 API 报错提示 */
		}
		clearInterval(progressTimer);
		progress.value = 100;
		loading.value = false;

		const redirect = Session.get('pre_auth_route');
		if (redirect) {
			const { path, query } = JSON.parse(redirect);
			await router.replace({ path, query });
		} else {
			await router.replace('/');
		}
	} catch (err) {
		clearInterval(progressTimer);
		loading.value = false;
		console.error('登录回调失败:', err);
		callbackError.value = `授权失败：${getErrorMessage(err)}`;
	} finally {
		Session.remove('pre_auth_route');
	}
});
</script>

<style scoped lang="scss">
.callback-container {
	display: flex;
	align-items: center;
	justify-content: center;
	min-height: 100vh;
	background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
}

.callback-card {
	display: flex;
	flex-direction: column;
	align-items: center;
	padding: 56px 64px;
	background: #fff;
	border-radius: 16px;
	box-shadow: 0 20px 60px rgba(0, 0, 0, 0.15);
	min-width: 380px;
	text-align: center;
	animation: fadeInUp 0.4s ease;
}

.callback-icon {
	margin-bottom: 24px;
	line-height: 1;

	&.is-success {
		color: #67c23a;
	}

	&.is-error {
		color: #f56c6c;
	}

	.spin {
		color: #409eff;
		animation: rotate 1.2s linear infinite;
	}
}

.callback-title {
	font-size: 22px;
	font-weight: 600;
	color: #303133;
	margin: 0 0 12px;
}

.callback-desc {
	font-size: 14px;
	color: #909399;
	margin: 0 0 28px;
	line-height: 1.6;
	max-width: 280px;
}

.callback-progress {
	width: 100%;
	margin-bottom: 8px;
}

.callback-btn {
	margin-top: 8px;
	width: 160px;
}

@keyframes rotate {
	from { transform: rotate(0deg); }
	to { transform: rotate(360deg); }
}

@keyframes fadeInUp {
	from {
		opacity: 0;
		transform: translateY(24px);
	}
	to {
		opacity: 1;
		transform: translateY(0);
	}
}
</style>
