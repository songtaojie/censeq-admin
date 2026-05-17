<template>
	<div class="system-dashboard-container layout-padding">
		<div class="dashboard-scroll layout-padding-auto layout-padding-view">
			<section class="brief-shell">
				<header class="hero-card">
					<div class="hero-copy brief-card">
						<div class="eyebrow">Enterprise Briefing</div>
						<div class="hero-head">
							<div>
								<p class="hero-kicker">企业经营概览</p>
								<h1>华东智造本周经营简报</h1>
							</div>
							<div class="hero-badge">Q2 / Week 20</div>
						</div>
						<p class="hero-summary">
							本页聚焦企业本周签约、项目履约、客户续约与现金回款表现，帮助管理层快速识别增长动能、协同阻塞与近期风险。
						</p>
						<div class="hero-tags">
							<span v-for="tag in heroTags" :key="tag">{{ tag }}</span>
						</div>
					</div>

					<div class="hero-aside brief-card">
						<div class="aside-label">经营健康指数</div>
						<div class="aside-score">92</div>
						<div class="aside-note">较上周提升 4 个点，项目履约和回款表现稳定。</div>
						<div class="aside-grid">
							<div v-for="item in headlineSignals" :key="item.label" class="aside-metric">
								<span>{{ item.label }}</span>
								<strong>{{ item.value }}</strong>
							</div>
						</div>
						<div class="hero-actions">
							<button type="button">查看项目中心</button>
							<button type="button" class="ghost">导出经营周报</button>
						</div>
					</div>
				</header>

				<section class="metrics-grid">
					<article v-for="card in metricCards" :key="card.label" class="brief-card metric-card">
						<div class="metric-top">
							<span>{{ card.label }}</span>
							<em :class="['metric-trend', card.trend > 0 ? 'up' : 'down']">
								{{ card.trend > 0 ? '+' : '' }}{{ card.trend }}%
							</em>
						</div>
						<div class="metric-value">{{ card.value }}</div>
						<p>{{ card.note }}</p>
					</article>
				</section>

				<section class="content-grid content-grid-main">
					<article class="brief-card panel-card panel-feature">
						<div class="panel-head">
							<div>
								<div class="panel-kicker">经营走势</div>
								<h3>近六月签约与回款节奏</h3>
							</div>
							<span class="panel-meta">单位：万元</span>
						</div>
						<div class="trend-chart">
							<div v-for="item in revenueTrend" :key="item.month" class="trend-item">
								<div class="trend-bars">
									<div class="trend-bar trend-bar-gold" :style="{ height: `${item.contract}%` }"></div>
									<div class="trend-bar trend-bar-ink" :style="{ height: `${item.receipt}%` }"></div>
								</div>
								<div class="trend-values">
									<strong>{{ item.contractValue }}</strong>
									<span>{{ item.receiptValue }}</span>
								</div>
								<label>{{ item.month }}</label>
							</div>
						</div>
						<div class="chart-legend">
							<span><i class="legend-dot gold"></i>签约额</span>
							<span><i class="legend-dot ink"></i>回款额</span>
						</div>
					</article>

					<article class="brief-card panel-card">
						<div class="panel-head">
							<div>
								<div class="panel-kicker">管理摘要</div>
								<h3>本周决策关注</h3>
							</div>
						</div>
						<div class="memo-list">
							<div v-for="memo in executiveMemos" :key="memo.title" class="memo-item">
								<div class="memo-index">{{ memo.index }}</div>
								<div class="memo-copy">
									<strong>{{ memo.title }}</strong>
									<p>{{ memo.desc }}</p>
								</div>
							</div>
						</div>
					</article>
				</section>

				<section class="content-grid content-grid-secondary">
					<article class="brief-card panel-card">
						<div class="panel-head">
							<div>
								<div class="panel-kicker">区域表现</div>
								<h3>重点市场贡献</h3>
							</div>
						</div>
						<div class="region-list">
							<div v-for="region in regionalPerformance" :key="region.name" class="region-item">
								<div class="region-top">
									<strong>{{ region.name }}</strong>
									<span>{{ region.value }}</span>
								</div>
								<div class="region-bar">
									<div class="region-fill" :style="{ width: `${region.ratio}%` }"></div>
								</div>
								<p>{{ region.note }}</p>
							</div>
						</div>
					</article>

					<article class="brief-card panel-card">
						<div class="panel-head">
							<div>
								<div class="panel-kicker">项目推进</div>
								<h3>核心项目里程碑</h3>
							</div>
						</div>
						<div class="milestone-list">
							<div v-for="project in projectMilestones" :key="project.name" class="milestone-item">
								<div class="milestone-copy">
									<strong>{{ project.name }}</strong>
									<p>{{ project.stage }}</p>
								</div>
								<div class="milestone-progress">
									<div class="milestone-progress-bar" :style="{ width: `${project.progress}%` }"></div>
								</div>
								<span>{{ project.progress }}%</span>
							</div>
						</div>
					</article>

					<article class="brief-card panel-card cash-card">
						<div class="panel-head">
							<div>
								<div class="panel-kicker">现金回款</div>
								<h3>账期与资金状态</h3>
							</div>
						</div>
						<div class="cash-summary">
							<div class="cash-figure">
								<span>应收回款达成</span>
								<strong>86%</strong>
							</div>
							<div class="cash-figure">
								<span>超 30 天账款</span>
								<strong>312 万</strong>
							</div>
						</div>
						<div class="cash-progress">
							<div v-for="item in cashFlowSignals" :key="item.label" class="cash-item">
								<div class="cash-label-row">
									<span>{{ item.label }}</span>
									<strong>{{ item.value }}</strong>
								</div>
								<div class="cash-track">
									<div class="cash-fill" :style="{ width: `${item.ratio}%` }"></div>
								</div>
							</div>
						</div>
					</article>
				</section>

				<section class="content-grid content-grid-bottom">
					<article class="brief-card panel-card">
						<div class="panel-head">
							<div>
								<div class="panel-kicker">协同焦点</div>
								<h3>本周待推进事项</h3>
							</div>
						</div>
						<div class="agenda-grid">
							<div v-for="item in focusAgenda" :key="item.title" class="agenda-item">
								<span>{{ item.label }}</span>
								<strong>{{ item.title }}</strong>
								<p>{{ item.desc }}</p>
							</div>
						</div>
					</article>

					<article class="brief-card panel-card">
						<div class="panel-head">
							<div>
								<div class="panel-kicker">风险提示</div>
								<h3>近期需要关注</h3>
							</div>
						</div>
						<div class="risk-list">
							<div v-for="risk in riskSignals" :key="risk.title" class="risk-item">
								<div class="risk-tone" :class="risk.tone"></div>
								<div class="risk-copy">
									<strong>{{ risk.title }}</strong>
									<p>{{ risk.desc }}</p>
								</div>
								<span>{{ risk.owner }}</span>
							</div>
						</div>
					</article>
				</section>
			</section>
		</div>
	</div>
</template>

<script setup lang="ts" name="systemDashboard">
interface MetricCard {
	label: string;
	value: string;
	note: string;
	trend: number;
}

const heroTags = ['客户续约稳定', '项目交付可控', '现金回款改善', '华东区域领跑'];

const headlineSignals = [
	{ label: '签约转化', value: '31.6%' },
	{ label: '客户续约', value: '88.4%' },
	{ label: '回款周期', value: '26 天' },
];

const metricCards: MetricCard[] = [
	{ label: '本月签约额', value: '4,860 万', note: '新签 18 个项目，制造业客户占比 46%。', trend: 12.8 },
	{ label: '在建项目', value: '27 个', note: '其中 6 个处于交付收尾阶段。', trend: 5.2 },
	{ label: '客户续约率', value: '88.4%', note: '重点客户续约推进总体稳定。', trend: 3.4 },
	{ label: '履约达成率', value: '94.1%', note: '交付质量保持在目标阈值以上。', trend: -1.3 },
];

const revenueTrend = [
	{ month: '1月', contract: 56, receipt: 44, contractValue: '3,240', receiptValue: '2,610' },
	{ month: '2月', contract: 62, receipt: 48, contractValue: '3,580', receiptValue: '2,940' },
	{ month: '3月', contract: 70, receipt: 57, contractValue: '4,020', receiptValue: '3,320' },
	{ month: '4月', contract: 78, receipt: 66, contractValue: '4,430', receiptValue: '3,860' },
	{ month: '5月', contract: 84, receipt: 72, contractValue: '4,860', receiptValue: '4,210' },
	{ month: '6月', contract: 68, receipt: 61, contractValue: '3,970', receiptValue: '3,540' },
];

const executiveMemos = [
	{ index: '01', title: '高毛利行业订单继续集中在华东', desc: '本周新增签约主要来自装备制造与智慧园区，建议保持该区域销售与交付资源倾斜。' },
	{ index: '02', title: '两个重点项目进入验收窗口', desc: '苏州工厂数字化改造与杭州园区集成项目将在 10 天内完成验收，需提前锁定客户培训与回款节点。' },
	{ index: '03', title: '续约客户对运维响应速度更敏感', desc: '本周续约沟通中，客户更关注 SLA 与实施顾问稳定性，建议同步优化服务承诺模板。' },
	{ index: '04', title: '账期压力主要集中于华南老项目', desc: '超过 30 天的账款仍集中在 3 个历史项目，适合专项跟进并绑定阶段性交付确认。' },
];

const regionalPerformance = [
	{ name: '华东区域', value: '1,960 万', ratio: 88, note: '新签贡献最高，制造业客户复购明显。' },
	{ name: '华南区域', value: '1,420 万', ratio: 72, note: '回款节奏改善，但历史账期仍需压降。' },
	{ name: '华北区域', value: '860 万', ratio: 46, note: '项目储备稳定，签约转化待提升。' },
	{ name: '西南区域', value: '620 万', ratio: 34, note: '以园区与政企项目为主，周期较长。' },
];

const projectMilestones = [
	{ name: '苏州工厂数字化改造', stage: '进入联调验收阶段', progress: 91 },
	{ name: '杭州园区一体化平台', stage: '完成主数据与设备接入', progress: 84 },
	{ name: '宁波仓储协同项目', stage: '推进二期排产与培训', progress: 67 },
	{ name: '无锡售后服务中心升级', stage: '客户侧需求冻结确认中', progress: 53 },
];

const cashFlowSignals = [
	{ label: '本月回款额', value: '4,210 万', ratio: 86 },
	{ label: '开票完成率', value: '93%', ratio: 93 },
	{ label: '到期账款回收', value: '78%', ratio: 78 },
];

const focusAgenda = [
	{ label: '经营协同', title: '续约客户名单复盘', desc: '梳理 12 家高价值客户续约进度，明确销售与交付联动策略。' },
	{ label: '项目交付', title: '验收资料集中校核', desc: '确保两个重点项目本周完成验收资料、培训记录与交付确认。' },
	{ label: '财务回款', title: '超期账款专项推进', desc: '对 3 个老项目设立专项推进机制，降低月末资金占压。' },
	{ label: '服务质量', title: 'SLA 响应指标优化', desc: '针对重点客户建立响应分层机制，提升续约沟通说服力。' },
];

const riskSignals = [
	{ title: '华南老项目账期偏长', desc: '3 个项目回款超过 30 天，若月底未回收将影响现金流预测。', owner: '财务 / 交付', tone: 'warning' },
	{ title: '验收窗口集中，资源排班紧张', desc: '两位核心实施顾问下周同时参与客户验收，需提前协调备份支持。', owner: '交付管理', tone: 'neutral' },
	{ title: '部分客户关注售后响应稳定性', desc: '续约谈判中对服务响应提出更高要求，需统一输出改进承诺。', owner: '客户成功', tone: 'safe' },
];
</script>

<style scoped>
.system-dashboard-container {
	--page-bg: linear-gradient(180deg, #f4efe7 0%, #ede5d9 100%);
	--card-bg: rgba(255, 252, 247, 0.92);
	--card-border: rgba(99, 79, 46, 0.14);
	--text-main: #1d2430;
	--text-secondary: #5f6670;
	--text-muted: #8a8075;
	--ink: #1f3653;
	--gold: #af8a4a;
	--gold-soft: rgba(175, 138, 74, 0.12);
	--line: rgba(40, 49, 62, 0.08);
	min-height: 100%;
	background: var(--page-bg);
	color: var(--text-main);
	border-radius: 24px;
	position: relative;
	overflow: hidden;
	box-shadow: inset 0 1px 0 rgba(255, 255, 255, 0.45);
}

.system-dashboard-container::before,
.system-dashboard-container::after {
	content: '';
	position: absolute;
	inset: 0;
	pointer-events: none;
}

.system-dashboard-container::before {
	background:
		radial-gradient(circle at top left, rgba(175, 138, 74, 0.18), transparent 32%),
		radial-gradient(circle at 88% 14%, rgba(31, 54, 83, 0.14), transparent 24%);
	opacity: 0.9;
}

.system-dashboard-container::after {
	background-image: linear-gradient(rgba(99, 79, 46, 0.05) 1px, transparent 1px),
		linear-gradient(90deg, rgba(99, 79, 46, 0.05) 1px, transparent 1px);
	background-size: 28px 28px;
	mask-image: linear-gradient(180deg, rgba(0, 0, 0, 0.4), transparent 85%);
}

.dashboard-scroll {
	min-height: 100%;
	overflow-y: auto;
	overflow-x: hidden;
	padding-bottom: 10px;
	position: relative;
	z-index: 1;
}

.brief-shell {
	display: grid;
	gap: 18px;
	padding-bottom: 12px;
}

.brief-card {
	background: var(--card-bg);
	backdrop-filter: blur(10px);
	border: 1px solid var(--card-border);
	border-radius: 22px;
	box-shadow: 0 20px 40px rgba(61, 48, 31, 0.08);
	animation: rise-in 0.55s ease both;
	position: relative;
	overflow: hidden;
}

.brief-card::after {
	content: '';
	position: absolute;
	inset: 0;
	background: linear-gradient(135deg, rgba(255, 255, 255, 0.38), transparent 40%);
	pointer-events: none;
}

.hero-card {
	display: grid;
	grid-template-columns: minmax(0, 1.5fr) minmax(340px, 0.82fr);
	gap: 18px;
	align-items: stretch;
}

.hero-copy,
.hero-aside,
.metric-card,
.panel-card {
	padding: 24px;
	min-width: 0;
}

.eyebrow,
.hero-kicker,
.panel-kicker,
.panel-meta,
.metric-top span,
.metric-card p,
.memo-copy p,
.region-item p,
.milestone-copy p,
.agenda-item p,
.risk-copy p,
.aside-label,
.aside-note,
.cash-figure span,
.cash-label-row span {
	font-size: 12px;
	letter-spacing: 0.12em;
	text-transform: uppercase;
	color: var(--text-muted);
	margin: 0;
}

.eyebrow,
.panel-kicker,
.hero-kicker {
	color: var(--gold);
	font-weight: 700;
	letter-spacing: 0.18em;
}

.hero-copy {
	display: grid;
	gap: 18px;
	padding-top: 28px;
	padding-bottom: 28px;
}

.hero-head {
	display: flex;
	justify-content: space-between;
	gap: 16px;
	align-items: flex-start;
}

.hero-head h1,
.panel-head h3,
.metric-value,
.aside-score,
.cash-figure strong {
	margin: 0;
	font-family: 'Baskerville Old Face', 'Palatino Linotype', 'Source Han Serif SC', 'Songti SC', serif;
	font-weight: 600;
	color: var(--text-main);
}

.hero-head h1 {
	font-size: 42px;
	line-height: 1.08;
	max-width: 580px;
}

.hero-badge {
	padding: 10px 14px;
	border-radius: 999px;
	background: rgba(31, 54, 83, 0.08);
	border: 1px solid rgba(31, 54, 83, 0.12);
	font-size: 12px;
	letter-spacing: 0.08em;
	color: var(--ink);
	white-space: nowrap;
}

.hero-summary {
	max-width: 760px;
	margin: 0;
	font-size: 15px;
	line-height: 1.9;
	color: var(--text-secondary);
}

.hero-tags {
	display: flex;
	flex-wrap: wrap;
	gap: 10px;
}

.hero-tags span,
.metric-trend,
.risk-item span {
	padding: 6px 12px;
	border-radius: 999px;
	font-size: 12px;
	line-height: 1;
	font-style: normal;
	white-space: nowrap;
}

.hero-tags span {
	background: var(--gold-soft);
	color: #7f6230;
	border: 1px solid rgba(175, 138, 74, 0.18);
}

.hero-aside {
	display: grid;
	gap: 16px;
	background: linear-gradient(180deg, rgba(33, 46, 67, 0.96) 0%, rgba(23, 35, 53, 0.96) 100%);
	color: #eef1f6;
}

.hero-aside::after {
	background: linear-gradient(135deg, rgba(255, 255, 255, 0.14), transparent 42%);
}

.hero-aside .aside-label,
.hero-aside .aside-note,
.hero-aside .aside-metric span {
	color: rgba(238, 241, 246, 0.68);
	letter-spacing: 0.08em;
	text-transform: none;
}

.aside-score {
	font-size: 76px;
	line-height: 0.95;
	color: #f1d39a;
}

.aside-note {
	font-size: 13px;
	line-height: 1.8;
	margin-top: -6px;
}

.aside-grid {
	display: grid;
	grid-template-columns: repeat(3, minmax(0, 1fr));
	gap: 12px;
}

.aside-metric {
	padding: 12px;
	border-radius: 16px;
	background: rgba(255, 255, 255, 0.05);
	border: 1px solid rgba(255, 255, 255, 0.08);
	display: grid;
	gap: 8px;
}

.aside-metric strong {
	font-size: 20px;
	font-weight: 600;
	color: #fff7e6;
}

.hero-actions {
	display: flex;
	gap: 12px;
	flex-wrap: wrap;
	margin-top: 4px;
}

.hero-actions button {
	border: none;
	padding: 11px 16px;
	border-radius: 999px;
	background: #f1d39a;
	color: #283246;
	font-weight: 700;
	cursor: pointer;
	transition: transform 0.2s ease, box-shadow 0.2s ease, background 0.2s ease;
	box-shadow: 0 10px 20px rgba(0, 0, 0, 0.14);
}

.hero-actions button.ghost {
	background: rgba(255, 255, 255, 0.08);
	color: #eef1f6;
	box-shadow: none;
	border: 1px solid rgba(255, 255, 255, 0.14);
}

.hero-actions button:hover {
	transform: translateY(-1px);
}

.metrics-grid,
.content-grid {
	display: grid;
	gap: 18px;
}

.metrics-grid {
	grid-template-columns: repeat(4, minmax(0, 1fr));
}

.metric-card {
	display: grid;
	gap: 12px;
	background: linear-gradient(180deg, rgba(255, 252, 247, 0.98), rgba(250, 245, 238, 0.94));
}

.metric-top {
	display: flex;
	justify-content: space-between;
	gap: 12px;
	align-items: center;
}

.metric-trend {
	background: rgba(31, 54, 83, 0.08);
	color: var(--ink);
	border: 1px solid rgba(31, 54, 83, 0.08);
}

.metric-trend.up {
	background: rgba(175, 138, 74, 0.14);
	color: #8d6a2f;
	border-color: rgba(175, 138, 74, 0.18);
}

.metric-trend.down {
	background: rgba(177, 91, 59, 0.1);
	color: #9d5b42;
	border-color: rgba(177, 91, 59, 0.16);
}

.metric-value {
	font-size: 34px;
	line-height: 1.06;
}

.metric-card p {
	font-size: 13px;
	line-height: 1.8;
	letter-spacing: 0;
	text-transform: none;
	color: var(--text-secondary);
}

.content-grid-main {
	grid-template-columns: minmax(0, 1.45fr) minmax(360px, 0.9fr);
}

.content-grid-secondary {
	grid-template-columns: repeat(3, minmax(0, 1fr));
}

.content-grid-bottom {
	grid-template-columns: minmax(0, 1.2fr) minmax(360px, 0.8fr);
}

.panel-card {
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
	font-size: 26px;
	line-height: 1.2;
	margin-top: 6px;
}

.panel-meta {
	color: var(--text-muted);
	white-space: nowrap;
}

.trend-chart {
	height: 280px;
	display: grid;
	grid-template-columns: repeat(6, minmax(0, 1fr));
	gap: 16px;
	align-items: end;
}

.trend-item {
	height: 100%;
	display: grid;
	justify-items: center;
	gap: 10px;
}

.trend-bars {
	height: 100%;
	width: 100%;
	display: flex;
	align-items: end;
	justify-content: center;
	gap: 8px;
	padding-top: 12px;
	border-radius: 18px;
	background: linear-gradient(180deg, rgba(31, 54, 83, 0.03), rgba(31, 54, 83, 0.01));
	border: 1px solid rgba(31, 54, 83, 0.05);
}

.trend-bar {
	width: calc(50% - 8px);
	border-radius: 12px 12px 0 0;
	min-height: 14px;
	box-shadow: inset 0 1px 0 rgba(255, 255, 255, 0.26);
}

.trend-bar-gold {
	background: linear-gradient(180deg, #d7b06f 0%, #af8a4a 100%);
}

.trend-bar-ink {
	background: linear-gradient(180deg, #486280 0%, #1f3653 100%);
}

.trend-values {
	display: grid;
	justify-items: center;
	gap: 4px;
}

.trend-values strong,
.region-top strong,
.milestone-copy strong,
.memo-copy strong,
.agenda-item strong,
.risk-copy strong {
	font-size: 15px;
	font-weight: 700;
	color: var(--text-main);
}

.trend-values span,
.trend-item label,
.region-top span,
.milestone-item span,
.risk-item span {
	font-size: 12px;
	color: var(--text-muted);
}

.chart-legend {
	display: flex;
	gap: 18px;
	align-items: center;
	flex-wrap: wrap;
	color: var(--text-secondary);
	font-size: 12px;
	letter-spacing: 0.06em;
	text-transform: uppercase;
}

.legend-dot {
	display: inline-block;
	width: 10px;
	height: 10px;
	border-radius: 50%;
	margin-right: 8px;
	vertical-align: middle;
}

.legend-dot.gold { background: #af8a4a; }
.legend-dot.ink { background: #1f3653; }

.memo-list,
.region-list,
.milestone-list,
.risk-list {
	display: grid;
	gap: 12px;
}

.memo-item,
.region-item,
.milestone-item,
.risk-item,
.agenda-item,
.cash-item {
	padding: 14px 16px;
	border-radius: 16px;
	background: rgba(255, 255, 255, 0.48);
	border: 1px solid var(--line);
}

.memo-item {
	display: grid;
	grid-template-columns: 44px minmax(0, 1fr);
	gap: 14px;
	align-items: start;
}

.memo-index {
	width: 44px;
	height: 44px;
	border-radius: 50%;
	display: grid;
	place-items: center;
	background: linear-gradient(180deg, rgba(175, 138, 74, 0.16), rgba(175, 138, 74, 0.06));
	color: #8a6830;
	font-family: 'Baskerville Old Face', 'Palatino Linotype', serif;
	font-size: 18px;
	font-weight: 700;
}

.memo-copy,
.milestone-copy,
.risk-copy {
	display: grid;
	gap: 6px;
	min-width: 0;
}

.memo-copy p,
.milestone-copy p,
.agenda-item p,
.risk-copy p,
.region-item p {
	font-size: 13px;
	line-height: 1.75;
	text-transform: none;
	letter-spacing: 0;
	color: var(--text-secondary);
}

.region-item,
.milestone-item {
	display: grid;
	gap: 10px;
}

.region-top,
.cash-label-row {
	display: flex;
	justify-content: space-between;
	gap: 12px;
	align-items: center;
}

.region-bar,
.milestone-progress,
.cash-track {
	height: 8px;
	border-radius: 999px;
	background: rgba(31, 54, 83, 0.08);
	overflow: hidden;
}

.region-fill,
.milestone-progress-bar,
.cash-fill {
	height: 100%;
	border-radius: inherit;
	background: linear-gradient(90deg, #d7b06f 0%, #9f793c 100%);
}

.milestone-item span {
	justify-self: end;
	color: var(--ink);
	font-weight: 600;
}

.cash-card {
	background: linear-gradient(180deg, rgba(255, 251, 244, 0.96), rgba(247, 239, 228, 0.92));
}

.cash-summary {
	display: grid;
	grid-template-columns: repeat(2, minmax(0, 1fr));
	gap: 12px;
}

.cash-figure {
	padding: 16px;
	border-radius: 18px;
	background: rgba(31, 54, 83, 0.05);
	border: 1px solid rgba(31, 54, 83, 0.08);
	display: grid;
	gap: 8px;
}

.cash-figure strong {
	font-size: 28px;
	line-height: 1.1;
	color: var(--ink);
}

.cash-progress {
	display: grid;
	gap: 12px;
}

.agenda-grid {
	display: grid;
	grid-template-columns: repeat(2, minmax(0, 1fr));
	gap: 12px;
}

.agenda-item {
	display: grid;
	gap: 8px;
	background: linear-gradient(180deg, rgba(255, 255, 255, 0.52), rgba(250, 244, 235, 0.7));
}

.agenda-item span {
	font-size: 11px;
	letter-spacing: 0.14em;
	text-transform: uppercase;
	color: #8d6a2f;
	font-weight: 700;
}

.risk-item {
	display: grid;
	grid-template-columns: 10px minmax(0, 1fr) auto;
	gap: 14px;
	align-items: start;
}

.risk-tone {
	width: 10px;
	height: 10px;
	margin-top: 7px;
	border-radius: 50%;
	background: #7d8793;
}

.risk-tone.warning { background: #a85b43; }
.risk-tone.neutral { background: #456482; }
.risk-tone.safe { background: #8d6a2f; }

@keyframes rise-in {
	from {
		opacity: 0;
		transform: translateY(18px);
	}
	to {
		opacity: 1;
		transform: translateY(0);
	}
}

.metrics-grid .brief-card:nth-child(2),
.content-grid .brief-card:nth-child(2) {
	animation-delay: 0.05s;
}

.metrics-grid .brief-card:nth-child(3),
.content-grid .brief-card:nth-child(3) {
	animation-delay: 0.1s;
}

.metrics-grid .brief-card:nth-child(4) {
	animation-delay: 0.15s;
}

@media (max-width: 1440px) {
	metrics-grid,
	.content-grid-secondary {
		grid-template-columns: repeat(2, minmax(0, 1fr));
	}

	.content-grid-bottom {
		grid-template-columns: minmax(0, 1fr);
	}
}

@media (max-width: 1180px) {
	.hero-card,
	.content-grid-main,
	.content-grid-secondary {
		grid-template-columns: minmax(0, 1fr);
	}

	.metrics-grid {
		grid-template-columns: repeat(2, minmax(0, 1fr));
	}

	.hero-head {
		flex-direction: column;
	}

	.hero-head h1 {
		max-width: none;
	}
}

@media (max-width: 768px) {
	.system-dashboard-container {
		border-radius: 16px;
	}

	.hero-copy,
	.hero-aside,
	.metric-card,
	.panel-card {
		padding: 18px;
	}

	.metrics-grid,
	.content-grid-secondary,
	.agenda-grid,
	.cash-summary,
	.aside-grid {
		grid-template-columns: minmax(0, 1fr);
	}

	.trend-chart {
		height: 240px;
		gap: 10px;
	}

	.memo-item,
	.risk-item {
		grid-template-columns: minmax(0, 1fr);
	}

	.risk-tone {
		margin-top: 0;
	}

	.hero-actions {
		grid-template-columns: minmax(0, 1fr);
	}

	.hero-actions button {
		width: 100%;
	}

	.hero-head h1 {
		font-size: 34px;
	}

	.aside-score {
		font-size: 60px;
	}

	.panel-head h3 {
		font-size: 22px;
	}
	}
</style>
