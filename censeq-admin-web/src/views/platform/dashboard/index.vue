<template>
	<div class="platform-dashboard-container layout-padding">
		<div class="dashboard-scroll layout-padding-auto layout-padding-view">
			<section class="dashboard-shell">
			<section class="overview-banner section-card">
				<div class="banner-copy">
					<div class="banner-kicker">平台运营中心</div>
					<h1>平台概览</h1>
					<p>
						集中查看租户活跃度、身份链路状态、待处理事项与今日事件，让平台管理员在一个页面内完成判断和分发。
					</p>
					<div class="banner-tags">
						<span v-for="tag in signalTags" :key="tag">{{ tag }}</span>
					</div>
				</div>

				<div class="banner-side">
					<div class="availability-card">
						<div class="availability-label">今日平台可用性</div>
						<div class="availability-value">{{ signalSummary.value }}</div>
						<div class="availability-note">{{ signalSummary.label }}</div>
					</div>
					<div class="banner-actions">
						<el-button type="primary">查看租户列表</el-button>
						<el-button>处理今日告警</el-button>
					</div>
				</div>
			</section>

			<section class="metrics-grid">
				<article v-for="card in metricCards" :key="card.label" class="metric-card section-card">
					<div class="metric-head">
						<span>{{ card.label }}</span>
						<em :class="['metric-trend', card.trend > 0 ? 'up' : 'down']">
							{{ card.trend > 0 ? '+' : '' }}{{ card.trend }}%
						</em>
					</div>
					<div class="metric-value">{{ card.value }}</div>
					<div class="metric-note">{{ card.note }}</div>
				</article>
			</section>

			<section class="content-grid content-grid-main">
				<article class="section-card panel-card">
					<div class="panel-head">
						<div>
							<div class="panel-kicker">运行趋势</div>
							<h3>平台访问脉冲</h3>
						</div>
						<span class="panel-meta">最近 24 小时</span>
					</div>
					<div class="bar-chart">
						<div v-for="bar in pulseBars" :key="bar.label" class="bar-item">
							<div class="bar-track">
								<div class="bar-fill" :style="{ height: `${bar.value}%` }"></div>
							</div>
							<span>{{ bar.label }}</span>
						</div>
					</div>
				</article>

				<article class="section-card panel-card">
					<div class="panel-head">
						<div>
							<div class="panel-kicker">待办关注</div>
							<h3>重点事项</h3>
						</div>
					</div>
					<div class="task-list">
						<div v-for="item in riskItems" :key="item.title" class="task-item">
							<div class="task-dot" :class="item.levelClass"></div>
							<div class="task-copy">
								<strong>{{ item.title }}</strong>
								<span>{{ item.desc }}</span>
							</div>
							<em>{{ item.owner }}</em>
						</div>
					</div>
				</article>
			</section>

			<section class="content-grid content-grid-secondary">
				<article class="section-card panel-card">
					<div class="panel-head">
						<div>
							<div class="panel-kicker">租户结构</div>
							<h3>增长梯队</h3>
						</div>
					</div>
					<div class="tenant-list">
						<div v-for="tenant in tenantLadder" :key="tenant.name" class="tenant-item">
							<div class="tenant-copy">
								<strong>{{ tenant.name }}</strong>
								<span>{{ tenant.count }} 家租户</span>
							</div>
							<div class="tenant-progress">
								<div class="tenant-progress-bar" :style="{ width: `${tenant.ratio}%` }"></div>
							</div>
						</div>
					</div>
				</article>

				<article class="section-card panel-card">
					<div class="panel-head">
						<div>
							<div class="panel-kicker">链路状态</div>
							<h3>身份接入质量</h3>
						</div>
					</div>
					<div class="health-list">
						<div v-for="item in linkHealth" :key="item.label" class="health-item">
							<span>{{ item.label }}</span>
							<strong>{{ item.value }}</strong>
						</div>
					</div>
				</article>

				<article class="section-card panel-card">
					<div class="panel-head">
						<div>
							<div class="panel-kicker">快捷入口</div>
							<h3>常用操作</h3>
						</div>
					</div>
					<div class="action-grid">
						<button v-for="action in quickActions" :key="action.title" class="action-item" type="button">
							<strong>{{ action.title }}</strong>
							<span>{{ action.desc }}</span>
						</button>
					</div>
				</article>
			</section>

			<section class="content-grid">
				<article class="section-card panel-card">
					<div class="panel-head">
						<div>
							<div class="panel-kicker">今日动态</div>
							<h3>平台事件流</h3>
						</div>
					</div>
					<div class="event-list">
						<div v-for="event in events" :key="event.time + event.title" class="event-item">
							<div class="event-time">{{ event.time }}</div>
							<div class="event-copy">
								<strong>{{ event.title }}</strong>
								<span>{{ event.desc }}</span>
							</div>
							<div class="event-badge" :class="event.tone">{{ event.tag }}</div>
						</div>
					</div>
				</article>
			</section>
			</section>
		</div>
	</div>
</template>

<script setup lang="ts" name="platformDashboard">
interface MetricCard {
	label: string;
	value: string;
	note: string;
	trend: number;
}

const signalSummary = {
	value: '99.982%',
	label: '状态稳定，较昨日提升 0.03%',
};

const signalTags = ['身份稳定', '租户活跃', '告警可控'];

const metricCards: MetricCard[] = [
	{ label: '活跃租户', value: '1,248', note: '较昨日新增 36 家', trend: 6.8 },
	{ label: '今日登录次数', value: '82,416', note: '高峰出现在 10:20', trend: 12.4 },
	{ label: '待确认告警', value: '07', note: '其中高优先级 2 条', trend: -18.2 },
	{ label: '接口成功率', value: '99.94%', note: '主身份链路稳定', trend: 0.7 },
];

const pulseBars = [
	{ label: '00', value: 28 },
	{ label: '04', value: 20 },
	{ label: '08', value: 76 },
	{ label: '10', value: 92 },
	{ label: '12', value: 64 },
	{ label: '16', value: 71 },
	{ label: '20', value: 58 },
	{ label: 'Now', value: 84 },
];

const riskItems = [
	{ title: '租户审批积压', desc: '14 个企业租户仍处于待激活状态。', owner: '平台运营', levelClass: 'is-high' },
	{ title: '短信通道波动', desc: '华东区域验证码发送成功率低于阈值。', owner: '运维值守', levelClass: 'is-medium' },
	{ title: '权限模板待发布', desc: '新行业模版已完成审核，等待推送。', owner: '产品配置', levelClass: 'is-low' },
];

const tenantLadder = [
	{ name: '制造业', count: 352, ratio: 88 },
	{ name: '零售连锁', count: 271, ratio: 68 },
	{ name: '智慧园区', count: 193, ratio: 52 },
	{ name: '第三方服务', count: 126, ratio: 34 },
];

const linkHealth = [
	{ label: 'OIDC 单点登录', value: '正常 · 32ms' },
	{ label: '租户解析服务', value: '正常 · 18ms' },
	{ label: '短信验证码服务', value: '轻微抖动 · 126ms' },
	{ label: '审计日志写入', value: '正常 · 队列深度 4' },
];

const quickActions = [
	{ title: '新建租户', desc: '进入租户开通流程' },
	{ title: '权限审计', desc: '查看近期高危授权' },
	{ title: '登录追踪', desc: '筛选异常登录来源' },
	{ title: '广播公告', desc: '向租户发布维护通知' },
];

const events = [
	{ time: '09:18', title: '企业租户 IMES-021 完成初始化', desc: '组织、管理员、默认权限模板已就绪。', tag: '已完成', tone: 'tone-success' },
	{ time: '10:07', title: '华东短信线路延迟升高', desc: '验证码服务进入自动重试，当前已恢复 80%。', tag: '关注中', tone: 'tone-warning' },
	{ time: '11:42', title: '平台管理员执行权限批量调整', desc: '涉及 12 个角色、43 项资源点。', tag: '已记录', tone: 'tone-neutral' },
	{ time: '13:15', title: '单点登录成功率回升', desc: '最近 30 分钟已恢复到日常阈值区间。', tag: '稳定', tone: 'tone-success' },
];
</script>

<style scoped>
.platform-dashboard-container {
	--page-bg: #f5f7fb;
	--card-bg: #ffffff;
	--card-border: #e8edf5;
	--text-main: #1f2d3d;
	--text-secondary: #5f6b7a;
	--text-tertiary: #8b95a1;
	--brand: #3a7afe;
	--brand-soft: #edf4ff;
	--success: #16a34a;
	--warning: #d97706;
	--danger: #dc2626;
	background: linear-gradient(180deg, #f7f9fc 0%, #f3f6fb 100%);
	min-height: 100%;
	padding-bottom: 24px;
	color: var(--text-main);
	border-radius: 20px;
}

.dashboard-shell {
	display: grid;
	gap: 16px;
}

.dashboard-scroll {
	min-height: 100%;
	padding-bottom: 8px;
	overflow-y: auto;
	overflow-x: hidden;
}

.section-card {
	background: var(--card-bg);
	border: 1px solid var(--card-border);
	border-radius: 16px;
	box-shadow: 0 6px 18px rgba(31, 45, 61, 0.04);
}

.overview-banner {
	display: grid;
	grid-template-columns: minmax(0, 1.5fr) minmax(320px, 0.8fr);
	gap: 24px;
	padding: 24px 28px;
	align-items: center;
	position: relative;
	overflow: hidden;
}

.overview-banner::after {
	content: '';
	position: absolute;
	right: 0;
	top: 0;
	width: 280px;
	height: 100%;
	background: linear-gradient(135deg, rgba(58, 122, 254, 0.08), rgba(58, 122, 254, 0));
	pointer-events: none;
}

.banner-copy,
.banner-side {
	position: relative;
	z-index: 1;
}

.banner-copy {
	display: grid;
	gap: 14px;
}

.banner-kicker,
.panel-kicker,
.panel-meta,
.metric-head span,
.metric-note,
.task-copy span,
.tenant-copy span,
.health-item span,
.event-copy span,
.event-time,
.availability-label,
.availability-note {
	font-size: 12px;
	letter-spacing: 0.02em;
	color: var(--text-secondary);
}

.banner-kicker,
.panel-kicker {
	color: var(--brand);
	font-weight: 600;
}

.banner-copy h1,
.panel-head h3,
.metric-value,
.availability-value {
	margin: 0;
	font-family: 'Segoe UI', 'PingFang SC', 'Microsoft YaHei', sans-serif;
	font-weight: 700;
	color: var(--text-main);
}

.banner-copy h1 {
	font-size: 30px;
	line-height: 1.2;
}

.banner-copy p {
	margin: 0;
	max-width: 720px;
	font-size: 14px;
	line-height: 1.8;
	color: var(--text-secondary);
}

.banner-tags {
	display: flex;
	flex-wrap: wrap;
	gap: 10px;
}

.banner-tags span {
	padding: 6px 12px;
	border-radius: 999px;
	background: var(--brand-soft);
	color: var(--brand);
	font-size: 12px;
	font-weight: 500;
}

.banner-side {
	display: grid;
	gap: 16px;
}

.availability-card {
	padding: 18px 20px;
	border-radius: 14px;
	background: linear-gradient(135deg, #f7fbff 0%, #eef4ff 100%);
	border: 1px solid #dbe7ff;
	display: grid;
	gap: 6px;
}

.availability-value {
	font-size: 34px;
	line-height: 1.1;
	color: var(--brand);
}

.banner-actions {
	display: flex;
	gap: 12px;
	flex-wrap: wrap;
}

.metrics-grid,
.content-grid {
	display: grid;
	gap: 16px;
}

.metrics-grid {
	grid-template-columns: repeat(4, minmax(0, 1fr));
}

.metric-card {
	padding: 18px 20px;
	display: grid;
	gap: 10px;
}

.metric-head {
	display: flex;
	justify-content: space-between;
	align-items: center;
	gap: 12px;
}

.metric-trend {
	padding: 3px 8px;
	border-radius: 999px;
	font-size: 12px;
	font-style: normal;
	font-weight: 600;
	background: #f6f8fb;
}

.metric-trend.up {
	color: var(--success);
	background: #ecfdf3;
}

.metric-trend.down {
	color: var(--warning);
	background: #fff7ed;
}

.metric-value {
	font-size: 32px;
	line-height: 1.1;
}

.content-grid-main {
	grid-template-columns: minmax(0, 1.45fr) minmax(320px, 0.85fr);
}

.content-grid-secondary {
	grid-template-columns: repeat(3, minmax(0, 1fr));
}

.panel-card {
	padding: 20px;
	display: grid;
	gap: 18px;
}

.panel-head {
	display: flex;
	justify-content: space-between;
	gap: 12px;
	align-items: flex-start;
}

.panel-head h3 {
	margin-top: 4px;
	font-size: 20px;
	line-height: 1.3;
}

.bar-chart {
	height: 250px;
	display: grid;
	grid-template-columns: repeat(8, minmax(0, 1fr));
	gap: 12px;
	align-items: end;
}

.bar-item {
	height: 100%;
	display: grid;
	gap: 10px;
	justify-items: center;
}

.bar-track {
	height: 100%;
	width: 100%;
	display: flex;
	align-items: flex-end;
	padding-top: 14px;
	background: #f4f7fb;
	border-radius: 14px;
	overflow: hidden;
}

.bar-fill {
	width: 100%;
	border-radius: 12px 12px 0 0;
	background: linear-gradient(180deg, #78a8ff 0%, #3a7afe 100%);
}

.bar-item span {
	font-size: 12px;
	color: var(--text-tertiary);
}

.task-list,
.tenant-list,
.health-list,
.event-list {
	display: grid;
	gap: 12px;
}

.task-item,
.tenant-item,
.health-item,
.event-item {
	display: grid;
	gap: 12px;
	align-items: center;
	padding: 14px 16px;
	background: #fafbfd;
	border: 1px solid #edf1f6;
	border-radius: 12px;
}

.task-item {
	grid-template-columns: auto minmax(0, 1fr) auto;
}

.task-dot {
	width: 10px;
	height: 10px;
	border-radius: 50%;
}

.task-dot.is-high { background: var(--danger); }
.task-dot.is-medium { background: var(--warning); }
.task-dot.is-low { background: var(--success); }

.task-copy,
.tenant-copy,
.event-copy {
	display: grid;
	gap: 4px;
	min-width: 0;
}

.task-copy strong,
.tenant-copy strong,
.health-item strong,
.action-item strong,
.event-copy strong {
	font-size: 14px;
	font-weight: 600;
	color: var(--text-main);
}

.task-item em {
	font-size: 12px;
	font-style: normal;
	color: var(--text-tertiary);
}

.tenant-item {
	grid-template-columns: minmax(120px, 0.8fr) minmax(0, 1.2fr);
}

.tenant-progress {
	height: 8px;
	background: #edf2f8;
	border-radius: 999px;
	overflow: hidden;
}

.tenant-progress-bar {
	height: 100%;
	border-radius: inherit;
	background: linear-gradient(90deg, #8bb5ff 0%, #3a7afe 100%);
}

.health-item {
	grid-template-columns: minmax(0, 1fr) auto;
}

.action-grid {
	display: grid;
	grid-template-columns: repeat(2, minmax(0, 1fr));
	gap: 12px;
}

.action-item {
	padding: 16px;
	text-align: left;
	border-radius: 12px;
	border: 1px solid #eaf0f8;
	background: #f9fbff;
	cursor: pointer;
	display: grid;
	gap: 6px;
	transition: all 0.2s ease;
}

.action-item span {
	font-size: 12px;
	color: var(--text-secondary);
}

.action-item:hover {
	border-color: #cfe0ff;
	background: #f3f8ff;
	transform: translateY(-1px);
}

.event-item {
	grid-template-columns: 64px minmax(0, 1fr) auto;
}

.event-time {
	font-weight: 600;
	color: var(--brand);
}

.event-badge {
	padding: 5px 10px;
	border-radius: 999px;
	font-size: 12px;
	font-weight: 600;
	background: #f3f4f6;
}

.tone-success {
	color: var(--success);
	background: #ecfdf3;
}

.tone-warning {
	color: var(--warning);
	background: #fff7ed;
}

.tone-neutral {
	color: var(--brand);
	background: #eff6ff;
}

@media (max-width: 1440px) {
	.metrics-grid,
	.content-grid-secondary {
		grid-template-columns: repeat(2, minmax(0, 1fr));
	}
}

@media (max-width: 1100px) {
	.overview-banner,
	.content-grid-main,
	.content-grid-secondary {
		grid-template-columns: minmax(0, 1fr);
	}
}

@media (max-width: 768px) {
	.platform-dashboard-container {
		border-radius: 14px;
	}

	.overview-banner,
	.panel-card,
	.metric-card {
		padding: 16px;
	}

	.metrics-grid,
	.content-grid-secondary,
	.action-grid {
		grid-template-columns: minmax(0, 1fr);
	}

	.bar-chart {
		height: 220px;
		gap: 8px;
	}

	.task-item,
	.tenant-item,
	.health-item,
	.event-item {
		grid-template-columns: minmax(0, 1fr);
	}
}
</style>
