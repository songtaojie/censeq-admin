<template>
	<SystemUser />
</template>

<script setup lang="ts" name="platformUser">
import { defineAsyncComponent } from 'vue';

const SystemUser = defineAsyncComponent(() => import('/@/views/system/user/index.vue'));
</script>
