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
import { useOidc } from '/@/composables/useOidc';
const { handleRedirectCallback } = useOidc();
onBeforeMount(async () => {
	handleRedirectCallback();
});
</script>
