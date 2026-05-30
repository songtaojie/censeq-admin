<template>
	<div class="platform-server-container layout-padding">
		<div class="monitor-shell layout-padding-auto layout-padding-view">
			<section class="summary-grid">
				<el-card class="summary-card" shadow="hover">
					<div class="summary-label">CPU 使用率</div>
					<div class="summary-main">
						<el-progress type="dashboard" :percentage="percentageOf(state.usage.cpuRate)" :color="progressColor(percentageOf(state.usage.cpuRate))" :width="116" />
						<div>
							<strong>{{ state.usage.cpuRate || '0%' }}</strong>
							<span>{{ state.base.processorCount || '-' }}</span>
						</div>
					</div>
				</el-card>

				<el-card class="summary-card" shadow="hover">
					<div class="summary-label">内存使用率</div>
					<div class="summary-main">
						<el-progress type="dashboard" :percentage="percentageOf(state.usage.ramRate)" :color="progressColor(percentageOf(state.usage.ramRate))" :width="116" />
						<div>
							<strong>{{ state.usage.ramRate || '0%' }}</strong>
							<span>已用 {{ state.usage.usedRam || '-' }}</span>
							<span>剩余 {{ state.usage.freeRam || '-' }}</span>
						</div>
					</div>
				</el-card>

				<el-card class="summary-card summary-card--wide" shadow="hover">
					<div class="summary-label">运行状态</div>
					<div class="status-stack">
						<div>
							<span>服务启动</span>
							<strong>{{ state.usage.startTime || '-' }}</strong>
						</div>
						<div>
							<span>服务运行</span>
							<strong>{{ state.usage.runTime || '-' }}</strong>
						</div>
						<div>
							<span>系统运行</span>
							<strong>{{ state.base.sysRunTime || '-' }}</strong>
						</div>
					</div>
				</el-card>
			</section>

			<section class="content-grid">
				<el-card shadow="hover">
					<template #header>
						<div class="card-header">
							<span>系统信息</span>
							<el-button icon="ele-Refresh" circle plain type="primary" :loading="state.loading" @click="loadAll" />
						</div>
					</template>
					<el-descriptions :column="2" border>
						<el-descriptions-item label="主机名称">{{ state.base.hostName || '-' }}</el-descriptions-item>
						<el-descriptions-item label="运行环境">{{ state.base.environment || '-' }}</el-descriptions-item>
						<el-descriptions-item label="操作系统">{{ state.base.systemOs || '-' }}</el-descriptions-item>
						<el-descriptions-item label="系统架构">{{ state.base.osArchitecture || '-' }}</el-descriptions-item>
						<el-descriptions-item label="运行框架">{{ state.base.frameworkDescription || '-' }}</el-descriptions-item>
						<el-descriptions-item label="Stage 状态">{{ state.base.stage || '-' }}</el-descriptions-item>
						<el-descriptions-item label="内网地址">{{ state.base.localIp || '-' }}</el-descriptions-item>
						<el-descriptions-item label="远端地址">{{ state.base.remoteIp || '-' }}</el-descriptions-item>
						<el-descriptions-item label="站点目录" :span="2">{{ state.base.wwwroot || '-' }}</el-descriptions-item>
					</el-descriptions>
				</el-card>

				<el-card shadow="hover">
					<template #header>
						<div class="card-header">
							<span>磁盘信息</span>
							<span class="header-meta">{{ state.disks.length }} 个分区</span>
						</div>
					</template>
					<div class="disk-list">
						<div v-for="disk in state.disks" :key="disk.diskName" class="disk-item">
							<div class="disk-head">
								<strong>{{ disk.diskName }}</strong>
								<el-tag size="small" effect="plain">{{ disk.diskType }}</el-tag>
							</div>
							<el-progress :percentage="disk.usedPercent" :color="progressColor(disk.usedPercent)" />
							<div class="disk-meta">
								<span>已用 {{ disk.used }} GB</span>
								<span>剩余 {{ disk.availableFreeSpace }} GB</span>
								<span>总量 {{ disk.totalSize }} GB</span>
							</div>
						</div>
					</div>
				</el-card>
			</section>

			<el-card shadow="hover">
				<template #header>
					<div class="card-header">
						<span>程序集信息</span>
						<span class="header-meta">{{ state.assemblies.length }} 个程序集</span>
					</div>
				</template>
				<div class="assembly-list">
					<el-tag v-for="assembly in state.assemblies" :key="assembly.name" effect="plain" round>
						{{ assembly.name }}
						<span class="assembly-version">v{{ assembly.version }}</span>
					</el-tag>
				</div>
			</el-card>
		</div>
	</div>
</template>

<script setup lang="ts" name="platformServer">
import { onActivated, onBeforeUnmount, onDeactivated, onMounted, reactive } from 'vue';
import { useSystemMonitorApi } from '/@/api/apis';
import type { AssemblyInfoDto, SystemBaseInfoDto, SystemDiskInfoDto, SystemUsageInfoDto } from '/@/api/models/system-monitor';

const monitorApi = useSystemMonitorApi();

const state = reactive({
	loading: false,
	base: {} as SystemBaseInfoDto,
	usage: {} as SystemUsageInfoDto,
	disks: [] as SystemDiskInfoDto[],
	assemblies: [] as AssemblyInfoDto[],
	timer: 0,
});

const percentageOf = (value?: string) => {
	const parsed = Number.parseFloat((value || '0').replace('%', ''));
	return Number.isFinite(parsed) ? Math.max(0, Math.min(100, Math.round(parsed))) : 0;
};

const progressColor = (value: number) => {
	if (value >= 85) return '#f56c6c';
	if (value >= 65) return '#e6a23c';
	return '#409eff';
};

const loadUsage = async () => {
	state.usage = await monitorApi.getServerUsage();
};

const loadAll = async () => {
	state.loading = true;
	try {
		const [base, usage, disks, assemblies] = await Promise.all([
			monitorApi.getServerBase(),
			monitorApi.getServerUsage(),
			monitorApi.getServerDisks(),
			monitorApi.getAssemblyList(),
		]);
		state.base = base;
		state.usage = usage;
		state.disks = disks.items ?? [];
		state.assemblies = assemblies.items ?? [];
	} finally {
		state.loading = false;
	}
};

const startTimer = () => {
	stopTimer();
	state.timer = window.setInterval(loadUsage, 10000);
};

const stopTimer = () => {
	if (state.timer) {
		window.clearInterval(state.timer);
		state.timer = 0;
	}
};

onMounted(async () => {
	await loadAll();
	startTimer();
});
onActivated(startTimer);
onDeactivated(stopTimer);
onBeforeUnmount(stopTimer);
</script>

<style scoped lang="scss">
.platform-server-container {
	min-height: 100%;
}

.monitor-shell {
	display: grid;
	gap: 12px;
	overflow: auto;
}

.summary-grid {
	display: grid;
	grid-template-columns: repeat(3, minmax(0, 1fr));
	gap: 12px;
}

.summary-card {
	:deep(.el-card__body) {
		display: grid;
		gap: 12px;
		min-height: 160px;
	}
}

.summary-label,
.header-meta,
.disk-meta,
.assembly-version,
.status-stack span {
	color: var(--el-text-color-secondary);
	font-size: 12px;
}

.summary-main {
	display: flex;
	align-items: center;
	gap: 16px;

	strong {
		display: block;
		margin-bottom: 6px;
		font-size: 28px;
		line-height: 1;
	}

	span {
		display: block;
		margin-top: 5px;
		color: var(--el-text-color-secondary);
	}
}

.status-stack {
	display: grid;
	grid-template-columns: repeat(3, minmax(0, 1fr));
	gap: 12px;

	div {
		padding: 14px;
		border: 1px solid var(--el-border-color-lighter);
		border-radius: 8px;
		background: var(--el-fill-color-lighter);
	}

	strong {
		display: block;
		margin-top: 8px;
		font-size: 16px;
		color: var(--el-text-color-primary);
	}
}

.content-grid {
	display: grid;
	grid-template-columns: minmax(0, 1.2fr) minmax(360px, 0.8fr);
	gap: 12px;
}

.card-header {
	display: flex;
	align-items: center;
	justify-content: space-between;
	gap: 12px;
	font-weight: 600;
}

.disk-list {
	display: grid;
	gap: 14px;
}

.disk-item {
	display: grid;
	gap: 10px;
	padding: 12px;
	border: 1px solid var(--el-border-color-lighter);
	border-radius: 8px;
	background: var(--el-fill-color-blank);
}

.disk-head,
.disk-meta {
	display: flex;
	justify-content: space-between;
	gap: 8px;
}

.disk-meta {
	flex-wrap: wrap;
}

.assembly-list {
	display: flex;
	flex-wrap: wrap;
	gap: 8px;
	max-height: 280px;
	overflow: auto;
}

.assembly-version {
	margin-left: 4px;
}

@media (max-width: 1200px) {
	.summary-grid,
	.content-grid {
		grid-template-columns: minmax(0, 1fr);
	}
}

@media (max-width: 768px) {
	.summary-main,
	.status-stack {
		display: grid;
		grid-template-columns: minmax(0, 1fr);
	}
}
</style>
