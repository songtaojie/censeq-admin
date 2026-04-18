<template>
	<div>
		<div desktop="12" tablet="8">
			<dl>
				<dt>{{ callbackTitle }}</dt>
				<dt>{{ callbackDescription }}</dt>
			</dl>
		</div>
	</div>
</template>
<script setup lang="ts" name="callback">
import { computed, onBeforeMount, ref } from 'vue';
import { useRouter } from 'vue-router';
import { useOidc } from '/@/composables/useOidc';
import { Session } from '/@/utils/storage';
import { ensureAntiforgeryCookies } from '/@/utils/antiforgery';
const router = useRouter();
const { handleRedirectCallback } = useOidc();
const callbackError = ref('');

const callbackTitle = computed(() => (callbackError.value ? 'authorize failed' : 'authorize successful'));
const callbackDescription = computed(() => {
	if (callbackError.value) return callbackError.value;
	return 'Your browser should be redirected soon';
});

const getErrorMessage = (error: unknown) => {
	if (error instanceof Error) return error.message;
	if (typeof error === 'string') return error;
	try {
		return JSON.stringify(error);
	} catch {
		return 'Unknown callback error';
	}
};

onBeforeMount(async () => {
	try {
		await handleRedirectCallback();
		try {
			await ensureAntiforgeryCookies();
		} catch {
			/* 由后续 API 报错提示 */
		}
		const redirect = Session.get('pre_auth_route');
		if (redirect) {
			const { path, query } = JSON.parse(redirect);
			await router.replace({ path, query });
		} else {
			await router.replace('/');
		}
	} catch (err) {
		console.error(' 登录回调失败:', err);
		callbackError.value = `OIDC callback failed: ${getErrorMessage(err)}`;
	} finally {
		Session.remove('pre_auth_route');
	}
});
</script>
