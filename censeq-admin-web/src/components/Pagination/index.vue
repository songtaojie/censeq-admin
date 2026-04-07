<template>
	<div class="pagination-container">
		<el-pagination
			:current-page="page"
			:page-size="limit"
			:pager-count="5"
			:page-sizes="[10, 20, 30, 50, 100]"
			:total="total"
			layout="total, sizes, prev, pager, next, jumper"
			background
			@size-change="handleSizeChange"
			@current-change="handleCurrentChange"
		/>
	</div>
</template>

<script setup lang="ts">
const props = defineProps({
	total: {
		type: Number,
		default: 0,
	},
	page: {
		type: Number,
		default: 1,
	},
	limit: {
		type: Number,
		default: 20,
	},
});

const emit = defineEmits<{
	(e: 'update:page', value: number): void;
	(e: 'update:limit', value: number): void;
	(e: 'pagination'): void;
}>();

const handleSizeChange = (value: number) => {
	emit('update:limit', value);
	emit('update:page', 1);
	emit('pagination');
};

const handleCurrentChange = (value: number) => {
	emit('update:page', value);
	emit('pagination');
};
</script>
