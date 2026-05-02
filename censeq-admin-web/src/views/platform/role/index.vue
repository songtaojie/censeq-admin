<template>
	<SystemRole />
</template>

<script setup lang="ts" name="platformRole">
import { defineAsyncComponent } from 'vue';

const SystemRole = defineAsyncComponent(() => import('/@/views/system/role/index.vue'));
</script>
