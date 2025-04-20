<template>
	<slot v-if="getUserAuthBtnList" />
</template>

<script setup lang="ts" name="auths">
import { computed } from 'vue';
import { useUserInfo } from '/@/composables/useUserInfo';

// 定义父组件传过来的值
const props = defineProps({
	value: {
		type: Array,
		default: () => [],
	},
});

// 定义变量内容
const { userInfos } = useUserInfo();

// 获取 pinia 中的用户权限
const getUserAuthBtnList = computed(() => {
	let flag = false;
	userInfos.value.authBtnList.map((val: string) => {
		props.value.map((v) => {
			if (val === v) flag = true;
		});
	});
	return flag;
});
</script>
