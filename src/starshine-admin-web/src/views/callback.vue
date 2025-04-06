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
import { useOidcAuth } from '/@/composables/useOidcAuth';
const { handleRedirectCallback } = useOidcAuth();
onBeforeMount(async () => {
	handleRedirectCallback();
});
</script>
