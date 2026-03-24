<template>
	<div>
		<div desktop="12" tablet="8">
			<dl>
				<dt>authorize successful</dt>
				<dt>Your browser should be redirected soon</dt>
			</dl>
		</div>
	</div>
</template>
<script setup lang="ts" name="callback">
import { onBeforeMount } from 'vue';
import { useRouter } from 'vue-router';
import { useOidc } from '/@/composables/useOidc';
import { Session } from '/@/utils/storage';
import { ensureAntiforgeryCookies } from '/@/utils/antiforgery';
const router = useRouter();
const { handleRedirectCallback } = useOidc();

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
	} finally {
		Session.remove('pre_auth_route');
	}
});
</script>
