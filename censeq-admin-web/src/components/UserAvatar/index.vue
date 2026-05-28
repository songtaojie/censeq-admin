<template>
	<el-avatar
		:size="size"
		:src="avatarSrc"
		:style="{ backgroundColor: avatarColor, fontSize: `${fontSize}px`, fontWeight: 600 }"
		@error="onAvatarError"
	>
		{{ avatarText }}
	</el-avatar>
</template>

<script setup lang="ts" name="UserAvatar">
import { computed, ref, watch } from 'vue';
import { resolveFileUrl } from '/@/api/apis';

const props = withDefaults(
	defineProps<{
		src?: string | null;
		name?: string | null;
		userName?: string | null;
		size?: number;
		fontSize?: number;
	}>(),
	{
		src: '',
		name: '',
		userName: '',
		size: 28,
		fontSize: 13,
	}
);

const imageLoadFailed = ref(false);
const defaultAvatarPaths = ['/upload/logo.png'];
const avatarColors = ['#409eff', '#67c23a', '#e6a23c', '#f56c6c', '#909399', '#8b5cf6', '#06b6d4'];

const cleanSrc = computed(() => props.src?.trim() ?? '');
const avatarSrc = computed(() => {
	if (!cleanSrc.value || imageLoadFailed.value) return '';
	if (defaultAvatarPaths.includes(cleanSrc.value)) return '';
	return resolveFileUrl(cleanSrc.value);
});
const displayName = computed(() => props.name?.trim() || props.userName?.trim() || '?');
const avatarText = computed(() => displayName.value.charAt(0).toUpperCase());
const avatarColor = computed(() => {
	const seed = props.userName?.trim() || displayName.value;
	let hash = 0;
	for (let i = 0; i < seed.length; i++) hash = seed.charCodeAt(i) + ((hash << 5) - hash);
	return avatarColors[Math.abs(hash) % avatarColors.length];
});

watch(cleanSrc, () => {
	imageLoadFailed.value = false;
});

const onAvatarError = () => {
	imageLoadFailed.value = true;
	return true;
};
</script>
