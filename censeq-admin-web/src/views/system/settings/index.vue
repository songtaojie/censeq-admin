<template>
	<div class="system-settings-container layout-padding">
		<div class="system-settings-padding layout-padding-auto layout-padding-view">
			<el-alert type="info" show-icon :closable="false" class="mb15">
				对应后端 <code>Censeq.SettingManagement</code>：宿主与租户按登录上下文读写；邮件设置受特性开关约束；需具备
				<code>SettingManagement</code> 相关权限。
			</el-alert>

			<el-tabs v-model="state.activeTab" type="border-card" @tab-change="onTabChange">
				<el-tab-pane label="邮件设置" name="email" lazy>
					<div v-loading="state.emailLoading" class="tab-body">
						<el-form ref="emailFormRef" :model="state.emailForm" :rules="emailRules" label-width="160px" class="max-w-900">
							<el-form-item label="SMTP 主机" prop="smtpHost">
								<el-input v-model="state.emailForm.smtpHost" clearable placeholder="例如 smtp.example.com" />
							</el-form-item>
							<el-form-item label="SMTP 端口" prop="smtpPort">
								<el-input-number v-model="state.emailForm.smtpPort" :min="1" :max="65535" controls-position="right" class="w-full" />
							</el-form-item>
							<el-form-item label="用户名" prop="smtpUserName">
								<el-input v-model="state.emailForm.smtpUserName" clearable autocomplete="off" />
							</el-form-item>
							<el-form-item label="密码" prop="smtpPassword">
								<el-input v-model="state.emailForm.smtpPassword" type="password" show-password clearable autocomplete="new-password" />
							</el-form-item>
							<el-form-item label="域" prop="smtpDomain">
								<el-input v-model="state.emailForm.smtpDomain" clearable />
							</el-form-item>
							<el-form-item label="启用 SSL">
								<el-switch v-model="state.emailForm.smtpEnableSsl" />
							</el-form-item>
							<el-form-item label="使用默认凭据">
								<el-switch v-model="state.emailForm.smtpUseDefaultCredentials" />
							</el-form-item>
							<el-form-item label="默认发件人地址" prop="defaultFromAddress">
								<el-input v-model="state.emailForm.defaultFromAddress" clearable placeholder="noreply@example.com" />
							</el-form-item>
							<el-form-item label="默认发件人显示名" prop="defaultFromDisplayName">
								<el-input v-model="state.emailForm.defaultFromDisplayName" clearable placeholder="系统通知" />
							</el-form-item>
							<el-form-item>
								<el-button type="primary" :loading="state.emailSaving" @click="onSaveEmail">保存</el-button>
								<el-button :loading="state.emailLoading" @click="loadEmail">重新加载</el-button>
								<el-button type="success" plain @click="openTestDialog">发送测试邮件</el-button>
							</el-form-item>
						</el-form>
					</div>
				</el-tab-pane>

				<el-tab-pane label="时区设置" name="timezone" lazy>
					<div v-loading="state.tzLoading" class="tab-body">
						<el-form label-width="160px" class="max-w-900">
							<el-form-item label="默认时区">
								<el-select
									v-model="state.timezone"
									filterable
									clearable
									placeholder="选择时区"
									class="w-full"
									:filter-method="filterTz"
								>
									<el-option
										v-for="opt in filteredTzOptions"
										:key="opt.value"
										:label="opt.name"
										:value="opt.value"
									/>
								</el-select>
							</el-form-item>
							<el-form-item>
								<el-button type="primary" :loading="state.tzSaving" :disabled="!state.timezone" @click="onSaveTimezone">保存</el-button>
								<el-button :loading="state.tzLoading" @click="loadTimezone">重新加载</el-button>
							</el-form-item>
						</el-form>
					</div>
				</el-tab-pane>
			</el-tabs>

			<el-dialog v-model="state.testVisible" title="发送测试邮件" width="520px" destroy-on-close @closed="resetTestForm">
				<el-form ref="testFormRef" :model="state.testForm" :rules="testRules" label-width="120px">
					<el-form-item label="发件人地址" prop="senderEmailAddress">
						<el-input v-model="state.testForm.senderEmailAddress" placeholder="须与 SMTP 配置一致" />
					</el-form-item>
					<el-form-item label="收件人地址" prop="targetEmailAddress">
						<el-input v-model="state.testForm.targetEmailAddress" />
					</el-form-item>
					<el-form-item label="主题" prop="subject">
						<el-input v-model="state.testForm.subject" />
					</el-form-item>
					<el-form-item label="正文" prop="body">
						<el-input v-model="state.testForm.body" type="textarea" :rows="4" />
					</el-form-item>
				</el-form>
				<template #footer>
					<el-button @click="state.testVisible = false">取消</el-button>
					<el-button type="primary" :loading="state.testSending" @click="onSendTest">发送</el-button>
				</template>
			</el-dialog>
		</div>
	</div>
</template>

<script setup lang="ts" name="systemSettings">
import { computed, reactive, ref, onMounted } from 'vue';
import type { FormInstance, FormRules } from 'element-plus';
import { ElMessage } from 'element-plus';
import { useSettingManagementApi } from '/@/api/apis';
import type { EmailSettingsDto, NameValueDto, UpdateEmailSettingsDto } from '/@/api/models/setting-management';

const api = useSettingManagementApi();
const emailFormRef = ref<FormInstance>();
const testFormRef = ref<FormInstance>();

const state = reactive({
	activeTab: 'email',
	emailLoading: false,
	emailSaving: false,
	emailForm: {
		smtpHost: '',
		smtpPort: 587,
		smtpUserName: '',
		smtpPassword: '',
		smtpDomain: '',
		smtpEnableSsl: false,
		smtpUseDefaultCredentials: false,
		defaultFromAddress: '',
		defaultFromDisplayName: '',
	} as UpdateEmailSettingsDto,
	tzLoading: false,
	tzSaving: false,
	timezone: '' as string,
	tzOptions: [] as NameValueDto[],
	tzFilter: '',
	testVisible: false,
	testSending: false,
	testForm: {
		senderEmailAddress: '',
		targetEmailAddress: '',
		subject: '测试邮件',
		body: '这是一封来自管理后台的测试邮件。',
	},
	emailInited: false,
	tzInited: false,
});

const emailRules: FormRules = {
	defaultFromAddress: [{ required: true, message: '请输入默认发件地址', trigger: 'blur' }],
	defaultFromDisplayName: [{ required: true, message: '请输入默认发件显示名', trigger: 'blur' }],
	smtpPort: [{ required: true, message: '请输入端口', trigger: 'change' }],
};

const testRules: FormRules = {
	senderEmailAddress: [{ required: true, message: '必填', trigger: 'blur' }],
	targetEmailAddress: [{ required: true, message: '必填', trigger: 'blur' }],
	subject: [{ required: true, message: '必填', trigger: 'blur' }],
};

const filteredTzOptions = computed(() => {
	const q = state.tzFilter.trim().toLowerCase();
	if (!q) return state.tzOptions;
	return state.tzOptions.filter((o) => o.name.toLowerCase().includes(q) || o.value.toLowerCase().includes(q));
});

function filterTz(q: string) {
	state.tzFilter = q;
}

function mapEmailToForm(d: EmailSettingsDto) {
	state.emailForm.smtpHost = d.smtpHost ?? '';
	state.emailForm.smtpPort = typeof d.smtpPort === 'number' ? d.smtpPort : 587;
	state.emailForm.smtpUserName = d.smtpUserName ?? '';
	state.emailForm.smtpPassword = d.smtpPassword ?? '';
	state.emailForm.smtpDomain = d.smtpDomain ?? '';
	state.emailForm.smtpEnableSsl = !!d.smtpEnableSsl;
	state.emailForm.smtpUseDefaultCredentials = !!d.smtpUseDefaultCredentials;
	state.emailForm.defaultFromAddress = d.defaultFromAddress ?? '';
	state.emailForm.defaultFromDisplayName = d.defaultFromDisplayName ?? '';
}

async function loadEmail() {
	state.emailLoading = true;
	try {
		const d = await api.getEmailSettings();
		mapEmailToForm(d);
		state.emailInited = true;
	} finally {
		state.emailLoading = false;
	}
}

async function loadTimezone() {
	state.tzLoading = true;
	try {
		const [current, options] = await Promise.all([api.getTimeZone(), api.getTimeZoneOptions()]);
		state.timezone = current ?? '';
		state.tzOptions = Array.isArray(options) ? options : [];
		state.tzInited = true;
	} finally {
		state.tzLoading = false;
	}
}

function onTabChange(name: string | number) {
	if (name === 'email' && !state.emailInited) loadEmail();
	if (name === 'timezone' && !state.tzInited) loadTimezone();
}

async function onSaveEmail() {
	const form = emailFormRef.value;
	if (!form) return;
	await form.validate(async (ok) => {
		if (!ok) return;
		state.emailSaving = true;
		try {
			await api.updateEmailSettings({ ...state.emailForm });
			ElMessage.success('邮件设置已保存');
			await loadEmail();
		} finally {
			state.emailSaving = false;
		}
	});
}

async function onSaveTimezone() {
	if (!state.timezone) {
		ElMessage.warning('请选择时区');
		return;
	}
	state.tzSaving = true;
	try {
		await api.updateTimeZone(state.timezone);
		ElMessage.success('时区已保存');
		await loadTimezone();
	} finally {
		state.tzSaving = false;
	}
}

function openTestDialog() {
	state.testForm.senderEmailAddress = state.emailForm.defaultFromAddress || '';
	state.testVisible = true;
}

function resetTestForm() {
	testFormRef.value?.resetFields();
}

async function onSendTest() {
	const form = testFormRef.value;
	if (!form) return;
	await form.validate(async (ok) => {
		if (!ok) return;
		state.testSending = true;
		try {
			await api.sendTestEmail({ ...state.testForm });
			ElMessage.success('测试邮件已发送');
			state.testVisible = false;
		} finally {
			state.testSending = false;
		}
	});
}

onMounted(() => {
	// 首屏邮件 tab：加载邮件；时区延迟到切换 tab
	loadEmail().catch(() => {});
});
</script>

<style scoped>
.tab-body {
	min-height: 240px;
	padding-top: 8px;
}

.max-w-900 {
	max-width: 900px;
}

.w-full {
	width: 100%;
}
</style>
