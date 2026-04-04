<template>
	<div class="logout-page">
		<h2>您已成功退出系统</h2>
		<p>
			将在 <strong>{{ countdown }}</strong> 秒后跳转到登录页面
		</p>
		<button type="button" @click="redirectToOpenIdLogin">立即跳转</button>
	</div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useOidc } from '/@/composables/useOidc';

const countdown = ref(5);
const { login } = useOidc();

const redirectToOpenIdLogin = async () => {
	await login();
};

onMounted(() => {
	const timer = setInterval(() => {
		countdown.value--;
		if (countdown.value <= 0) {
			clearInterval(timer);
			redirectToOpenIdLogin();
		}
	}, 1000);
});
</script>

<style scoped>
.logout-page {
	padding: 80px 20px;
	text-align: center;
}
.logout-page h2 {
	font-size: 24px;
	margin-bottom: 16px;
}
.logout-page p {
	font-size: 16px;
	margin-bottom: 24px;
}
.logout-page button {
	padding: 10px 20px;
	font-size: 16px;
	cursor: pointer;
	background-color: #409eff;
	color: white;
	border: none;
	border-radius: 4px;
}
</style>
