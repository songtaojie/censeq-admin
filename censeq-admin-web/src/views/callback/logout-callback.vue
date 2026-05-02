<template>
	<div class="logout-container">
		<div class="logout-card">
			<div class="logout-icon">
				<el-icon size="56"><SuccessFilled /></el-icon>
			</div>

			<h2 class="logout-title">已安全退出</h2>
			<p class="logout-desc">感谢您的使用，期待下次再见</p>

			<div class="logout-countdown">
				<el-progress
					type="circle"
					:percentage="countdownPercent"
					:width="80"
					:stroke-width="6"
					color="#409eff"
					:show-text="false"
				/>
				<span class="countdown-number">{{ countdown }}</span>
			</div>

			<p class="logout-hint">{{ countdown }} 秒后自动跳转到登录页</p>

			<el-button type="primary" size="large" @click="redirectToOpenIdLogin" class="logout-btn">
				<el-icon><Right /></el-icon>
				立即登录
			</el-button>
		</div>
	</div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted } from 'vue';
import { useOidc } from '/@/composables/useOidc';
import { SuccessFilled, Right } from '@element-plus/icons-vue';

const TOTAL = 5;
const countdown = ref(TOTAL);
const { login } = useOidc();

const countdownPercent = computed(() => ((TOTAL - countdown.value) / TOTAL) * 100);

const redirectToOpenIdLogin = async () => {
	await login();
};

let timer: ReturnType<typeof setInterval> | null = null;

onMounted(() => {
	timer = setInterval(() => {
		countdown.value--;
		if (countdown.value <= 0) {
			clearInterval(timer!);
			redirectToOpenIdLogin();
		}
	}, 1000);
});

onUnmounted(() => {
	if (timer) clearInterval(timer);
});
</script>

<style scoped lang="scss">
.logout-container {
	display: flex;
	align-items: center;
	justify-content: center;
	min-height: 100vh;
	background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
}

.logout-card {
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

.logout-icon {
	color: #67c23a;
	margin-bottom: 20px;
	line-height: 1;
}

.logout-title {
	font-size: 22px;
	font-weight: 600;
	color: #303133;
	margin: 0 0 10px;
}

.logout-desc {
	font-size: 14px;
	color: #909399;
	margin: 0 0 32px;
}

.logout-countdown {
	position: relative;
	display: flex;
	align-items: center;
	justify-content: center;
	margin-bottom: 16px;
}

.countdown-number {
	position: absolute;
	font-size: 24px;
	font-weight: 700;
	color: #409eff;
}

.logout-hint {
	font-size: 13px;
	color: #c0c4cc;
	margin: 0 0 28px;
}

.logout-btn {
	width: 160px;
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
