<template>
	<SystemDept />
</template>

<script setup lang="ts" name="platformDept">
import { defineAsyncComponent } from 'vue';

const SystemDept = defineAsyncComponent(() => import('/@/views/system/dept/index.vue'));
</script>
