<template>
	<div class="personal-page layout-pd">
		<el-row :gutter="10" class="personal-grid">
			<el-col :xs="24" :sm="24" :md="8" :lg="7" :xl="6">
				<el-card shadow="hover" class="profile-card">
					<div class="account-center-avatarHolder">
						<el-upload class="avatar-upload" :show-file-list="false" :before-upload="beforeAvatarUpload" :http-request="uploadAvatar">
							<UserAvatar :src="avatarUrl" :name="displayName" :user-name="profile.userName" :size="104" :font-size="36" />
							<div class="avatar-upload__mask">
								<el-icon><ele-Camera /></el-icon>
								<span>更换头像</span>
							</div>
						</el-upload>
						<div class="username">{{ displayName }}</div>
						<div class="account">{{ profile.userName || '-' }}</div>
						<div class="profile-tags">
							<el-tag v-for="role in roleList" :key="role" size="small" effect="plain">{{ role }}</el-tag>
							<el-tag v-if="roleList.length === 0" size="small" effect="plain" type="info">暂无角色</el-tag>
						</div>
					</div>

					<div class="account-center-org">
						<p>
							<el-icon><ele-Message /></el-icon>
							<span>{{ profile.email || '-' }}</span>
						</p>
						<p>
							<el-icon><ele-Iphone /></el-icon>
							<span>{{ profile.phoneNumber || '-' }}</span>
						</p>
						<p>
							<el-icon><ele-OfficeBuilding /></el-icon>
							<span>{{ tenantDisplay }}</span>
						</p>
					</div>

					<div class="signature-box">
						<div v-if="signatureUrl" class="signature-image">
							<el-image :src="signatureUrl" fit="contain" alt="电子签名" />
						</div>
						<el-empty v-else description="暂无电子签名" :image-size="64" />
					</div>
					<div class="signature-actions">
						<el-button type="primary" icon="ele-Edit" @click="openSignatureDialog">电子签名</el-button>
						<el-upload
							:show-file-list="false"
							:before-upload="beforeSignatureUpload"
							:http-request="uploadSignatureFile"
							accept=".jpg,.jpeg,.png,.webp"
						>
							<el-button icon="ele-UploadFilled" :loading="signatureUploading">上传手写签名</el-button>
						</el-upload>
						<el-button v-if="signatureUrl" icon="ele-Delete" @click="removeSignature">移除签名</el-button>
					</div>
				</el-card>
			</el-col>

			<el-col :xs="24" :sm="24" :md="16" :lg="17" :xl="18">
				<el-card shadow="hover" class="detail-card">
					<el-tabs v-model="activeTab" class="profile-tabs">
						<el-tab-pane label="基础信息" name="basic" v-loading="loading">
							<el-form label-width="92px" class="personal-form">
								<el-row :gutter="35">
									<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
										<el-form-item label="登录账号">
											<el-input v-model="profile.userName" disabled />
										</el-form-item>
									</el-col>
									<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
										<el-form-item label="昵称">
											<el-input v-model="profile.name" placeholder="昵称" clearable />
										</el-form-item>
									</el-col>
									<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
										<el-form-item label="真实姓名">
											<el-input v-model="profile.surname" placeholder="真实姓名" clearable />
										</el-form-item>
									</el-col>
									<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
										<el-form-item label="邮箱">
											<el-input v-model="profile.email" placeholder="邮箱" clearable />
										</el-form-item>
									</el-col>
									<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
										<el-form-item label="手机号码">
											<el-input v-model="profile.phoneNumber" placeholder="手机号码" clearable />
										</el-form-item>
									</el-col>
									<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
										<el-form-item label="所属租户">
											<el-input :model-value="tenantDisplay" disabled />
										</el-form-item>
									</el-col>
									<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="form-actions">
										<el-form-item>
											<el-button type="primary" icon="ele-SuccessFilled" :loading="saving" @click="saveProfile">保存基本信息</el-button>
										</el-form-item>
									</el-col>
								</el-row>
							</el-form>
						</el-tab-pane>

						<el-tab-pane label="组织机构" name="org">
							<el-descriptions :column="1" border class="info-descriptions">
								<el-descriptions-item label="租户">{{ tenantDisplay }}</el-descriptions-item>
								<el-descriptions-item label="租户 ID">
									<el-tag v-if="isHostTenant" type="info" effect="plain">Host / 未进入租户上下文</el-tag>
									<span v-else>{{ tenantId }}</span>
								</el-descriptions-item>
								<el-descriptions-item label="角色">
									<span v-if="roleList.length === 0">-</span>
									<el-tag v-for="role in roleList" :key="role" class="mr5" size="small" effect="plain">{{ role }}</el-tag>
								</el-descriptions-item>
								<el-descriptions-item label="权限数量">{{ userInfos.authBtnList?.length || 0 }}</el-descriptions-item>
							</el-descriptions>
						</el-tab-pane>

						<el-tab-pane label="账号安全" name="security">
							<div class="security-list">
								<div class="security-list__item">
									<div>
										<div class="security-list__title">登录状态</div>
										<div class="security-list__desc">当前账号已完成身份认证</div>
									</div>
									<el-tag size="small" type="success">已登录</el-tag>
								</div>
								<div class="security-list__item">
									<div>
										<div class="security-list__title">用户 ID</div>
										<div class="security-list__desc code-text">{{ userId }}</div>
									</div>
								</div>
								<div class="security-list__item">
									<div>
										<div class="security-list__title">认证方式</div>
										<div class="security-list__desc">{{ claimValue('amr') }}</div>
									</div>
								</div>
								<div class="security-list__item">
									<div>
										<div class="security-list__title">令牌过期时间</div>
										<div class="security-list__desc">{{ expiresAt }}</div>
									</div>
								</div>
								<div class="security-list__item">
									<div>
										<div class="security-list__title">会话标识</div>
										<div class="security-list__desc code-text">{{ claimValue('sid') }}</div>
									</div>
								</div>
							</div>
						</el-tab-pane>
					</el-tabs>
				</el-card>
			</el-col>
		</el-row>

		<el-dialog v-model="signatureDialogVisible" title="电子签名" width="640px" draggable @opened="resizeSignatureCanvas">
			<div class="signature-editor">
				<canvas
					ref="signatureCanvasRef"
					class="signature-canvas"
					@pointerdown="startDraw"
					@pointermove="draw"
					@pointerup="endDraw"
					@pointerleave="endDraw"
					@pointercancel="endDraw"
				></canvas>
			</div>
			<div class="signature-tools">
				<div class="signature-tool">
					<span>画笔粗细</span>
					<el-input-number v-model="signaturePenWidth" :min="1" :max="8" :step="1" size="small" />
				</div>
				<div class="signature-tool">
					<span>画笔颜色</span>
					<el-color-picker v-model="signaturePenColor" color-format="hex" />
				</div>
			</div>
			<template #footer>
				<el-button @click="clearSignatureCanvas">清屏</el-button>
				<el-button @click="signatureDialogVisible = false">取消</el-button>
				<el-button type="primary" :loading="signatureUploading" @click="saveCanvasSignature">保存</el-button>
			</template>
		</el-dialog>
	</div>
</template>

<script setup lang="ts" name="personal">
import { computed, nextTick, onBeforeUnmount, onMounted, reactive, ref } from 'vue';
import type { UploadRequestOptions } from 'element-plus';
import { ElMessage } from 'element-plus';
import { useUserInfo } from '/@/composables/useUserInfo';
import { useOidc } from '/@/composables/useOidc';
import { resolveFileUrl, useFileApi, useProfileApi } from '/@/api/apis';
import type { ProfileDto } from '/@/api/models/account';
import UserAvatar from '/@/components/UserAvatar/index.vue';

const emptyGuid = '00000000-0000-0000-0000-000000000000';

const { userInfos, setUserInfos } = useUserInfo();
const { getCurrentUser } = useOidc();
const fileApi = useFileApi();
const profileApi = useProfileApi();

const activeTab = ref('basic');
const loading = ref(false);
const saving = ref(false);
const uploading = ref(false);
const signatureUploading = ref(false);
const signatureDialogVisible = ref(false);
const signatureCanvasRef = ref<HTMLCanvasElement>();
const signaturePenColor = ref('#000000');
const signaturePenWidth = ref(2);
const signatureHasContent = ref(false);
let signatureDrawing = false;
let signatureResizeObserver: ResizeObserver | undefined;

const state = reactive({
	claims: {} as Record<string, any>,
	expiresAt: '-',
	profile: {
		userName: '',
		email: '',
		name: '',
		surname: '',
		phoneNumber: '',
		avatarUrl: '',
		concurrencyStamp: '',
		extraProperties: {},
	} as ProfileDto,
});

const profile = computed(() => state.profile);
const displayName = computed(() => [state.profile.surname, state.profile.name].filter(Boolean).join(' ') || userInfos.value.displayName || state.profile.userName || '用户');
const avatarUrl = computed(() => resolveFileUrl(state.profile.avatarUrl || userInfos.value.photo));
const roleList = computed(() => (userInfos.value.roles || []).filter(Boolean));
const tenantId = computed(() => state.claims.tid || state.claims.tenantid || state.claims.tenantId || '');
const isHostTenant = computed(() => !tenantId.value || tenantId.value === emptyGuid);
const tenantDisplay = computed(() => state.claims.tenant_name || state.claims.tenantName || (isHostTenant.value ? 'Host（未进入租户）' : tenantId.value));
const userId = computed(() => claimValue('sub'));
const expiresAt = computed(() => state.expiresAt);
const signatureUrl = computed(() => {
	const value = (state.profile.extraProperties?.signature as string | undefined) || ((userInfos.value as any).signature as string | undefined);
	return resolveFileUrl(value);
});

const claimValue = (key: string) => {
	const value = state.claims[key] ?? (userInfos.value as any)[key];
	if (Array.isArray(value)) return value.length > 0 ? value.join(', ') : '-';
	return value || '-';
};

const loadProfile = async () => {
	loading.value = true;
	try {
		const [currentUser, profileData] = await Promise.all([getCurrentUser(), profileApi.getProfile()]);
		state.claims = ((currentUser?.profile ?? {}) as Record<string, any>) || {};
		state.expiresAt = currentUser?.expires_at ? new Date(currentUser.expires_at * 1000).toLocaleString() : '-';
		state.profile = {
			...profileData,
			name: profileData.name || '',
			surname: profileData.surname || '',
			phoneNumber: profileData.phoneNumber || '',
			avatarUrl: profileData.avatarUrl || '',
			extraProperties: profileData.extraProperties || {},
		};
	} finally {
		loading.value = false;
	}
};

const beforeAvatarUpload = (file: File) => {
	const isImage = ['image/jpeg', 'image/png', 'image/gif', 'image/bmp', 'image/webp'].includes(file.type);
	const isLt2M = file.size / 1024 / 1024 < 2;
	if (!isImage) ElMessage.error('头像仅支持常见图片格式');
	if (!isLt2M) ElMessage.error('头像大小不能超过 2MB');
	return isImage && isLt2M;
};

const uploadAvatar = async (options: UploadRequestOptions) => {
	if (uploading.value) return;
	uploading.value = true;
	try {
		const file = options.file as File;
		const result = await fileApi.uploadAvatar(file);
		state.profile.avatarUrl = result.url;
		await setUserInfos();
		ElMessage.success('头像已更新');
		options.onSuccess?.(result);
	} catch (error) {
		options.onError?.(error as Error);
	} finally {
		uploading.value = false;
	}
};

const beforeSignatureUpload = (file: File) => {
	const isImage = ['image/jpeg', 'image/png', 'image/webp'].includes(file.type);
	const isLt2M = file.size / 1024 / 1024 < 2;
	if (!isImage) ElMessage.error('签名仅支持 jpg、png、webp 图片');
	if (!isLt2M) ElMessage.error('签名图片大小不能超过 2MB');
	return isImage && isLt2M;
};

const persistSignature = async (signature: string | null) => {
	const extraProperties = { ...(state.profile.extraProperties || {}), signature };
	const saved = await profileApi.updateProfile({
		userName: state.profile.userName,
		email: state.profile.email,
		name: state.profile.name,
		surname: state.profile.surname,
		phoneNumber: state.profile.phoneNumber,
		avatarUrl: state.profile.avatarUrl,
		concurrencyStamp: state.profile.concurrencyStamp,
		extraProperties,
	});
	state.profile = {
		...state.profile,
		...saved,
		extraProperties: saved.extraProperties || extraProperties,
	};
	(userInfos.value as any).signature = signature || '';
};

const uploadSignatureFile = async (options: UploadRequestOptions) => {
	if (signatureUploading.value) return;
	signatureUploading.value = true;
	try {
		const result = await fileApi.uploadFile(options.file as File, { category: 'signature', isPublic: true, allowImageOnly: true });
		await persistSignature(result.url);
		ElMessage.success('电子签名已更新');
		options.onSuccess?.(result);
	} catch (error) {
		options.onError?.(error as Error);
	} finally {
		signatureUploading.value = false;
	}
};

const getCanvasPoint = (event: PointerEvent) => {
	const canvas = signatureCanvasRef.value!;
	const rect = canvas.getBoundingClientRect();
	return {
		x: event.clientX - rect.left,
		y: event.clientY - rect.top,
	};
};

const getSignatureContext = () => {
	const canvas = signatureCanvasRef.value;
	if (!canvas) return undefined;
	const context = canvas.getContext('2d');
	if (!context) return undefined;
	context.lineCap = 'round';
	context.lineJoin = 'round';
	context.strokeStyle = signaturePenColor.value;
	context.lineWidth = signaturePenWidth.value;
	return context;
};

const resizeSignatureCanvas = async () => {
	await nextTick();
	const canvas = signatureCanvasRef.value;
	if (!canvas) return;
	const rect = canvas.getBoundingClientRect();
	const ratio = window.devicePixelRatio || 1;
	canvas.width = Math.max(1, Math.floor(rect.width * ratio));
	canvas.height = Math.max(1, Math.floor(rect.height * ratio));
	const context = canvas.getContext('2d');
	if (!context) return;
	context.setTransform(ratio, 0, 0, ratio, 0, 0);
	context.fillStyle = '#ffffff';
	context.fillRect(0, 0, rect.width, rect.height);
	signatureHasContent.value = false;
};

const openSignatureDialog = async () => {
	signatureDialogVisible.value = true;
	await nextTick();
	if (!signatureResizeObserver && signatureCanvasRef.value) {
		signatureResizeObserver = new ResizeObserver(() => resizeSignatureCanvas());
		signatureResizeObserver.observe(signatureCanvasRef.value);
	}
};

const startDraw = (event: PointerEvent) => {
	const context = getSignatureContext();
	if (!context || !signatureCanvasRef.value) return;
	signatureCanvasRef.value.setPointerCapture(event.pointerId);
	const point = getCanvasPoint(event);
	signatureDrawing = true;
	signatureHasContent.value = true;
	context.beginPath();
	context.moveTo(point.x, point.y);
};

const draw = (event: PointerEvent) => {
	if (!signatureDrawing) return;
	const context = getSignatureContext();
	if (!context) return;
	const point = getCanvasPoint(event);
	context.lineTo(point.x, point.y);
	context.stroke();
};

const endDraw = (event: PointerEvent) => {
	if (!signatureDrawing) return;
	signatureDrawing = false;
	signatureCanvasRef.value?.releasePointerCapture(event.pointerId);
};

const clearSignatureCanvas = () => {
	const canvas = signatureCanvasRef.value;
	if (!canvas) return;
	const rect = canvas.getBoundingClientRect();
	const context = canvas.getContext('2d');
	if (!context) return;
	context.fillStyle = '#ffffff';
	context.fillRect(0, 0, rect.width, rect.height);
	signatureHasContent.value = false;
};

const canvasToFile = async () => {
	const canvas = signatureCanvasRef.value;
	if (!canvas) return undefined;
	return await new Promise<File | undefined>((resolve) => {
		canvas.toBlob((blob) => {
			if (!blob) return resolve(undefined);
			resolve(new File([blob], `${state.profile.userName || 'signature'}-signature.png`, { type: 'image/png' }));
		}, 'image/png');
	});
};

const saveCanvasSignature = async () => {
	if (!signatureHasContent.value) {
		ElMessage.warning('请先在画板中签名');
		return;
	}
	const file = await canvasToFile();
	if (!file) {
		ElMessage.error('生成签名图片失败');
		return;
	}
	signatureUploading.value = true;
	try {
		const result = await fileApi.uploadFile(file, { category: 'signature', isPublic: true, allowImageOnly: true });
		await persistSignature(result.url);
		signatureDialogVisible.value = false;
		ElMessage.success('电子签名已保存');
	} finally {
		signatureUploading.value = false;
	}
};

const removeSignature = async () => {
	signatureUploading.value = true;
	try {
		await persistSignature(null);
		ElMessage.success('电子签名已移除');
	} finally {
		signatureUploading.value = false;
	}
};

const saveProfile = async () => {
	saving.value = true;
	try {
		const saved = await profileApi.updateProfile({
			userName: state.profile.userName,
			email: state.profile.email,
			name: state.profile.name,
			surname: state.profile.surname,
			phoneNumber: state.profile.phoneNumber,
			avatarUrl: state.profile.avatarUrl,
			concurrencyStamp: state.profile.concurrencyStamp,
			extraProperties: state.profile.extraProperties,
		});
		state.profile = { ...state.profile, ...saved };
		await setUserInfos();
		ElMessage.success('资料已保存');
	} finally {
		saving.value = false;
	}
};

onMounted(loadProfile);

onBeforeUnmount(() => {
	signatureResizeObserver?.disconnect();
});
</script>

<style scoped lang="scss">
.personal-page {
	.personal-grid {
		width: 100%;
		align-items: stretch;
	}

	.profile-card,
	.detail-card {
		height: 100%;
		border: 1px solid var(--el-border-color-light);
		border-radius: 4px;
	}

	.profile-card {
		:deep(.el-card__body) {
			padding: 16px;
		}
	}

	.detail-card {
		:deep(.el-card__body) {
			min-height: 370px;
			padding: 16px 18px 18px;
		}
	}

	.account-center-avatarHolder {
		margin-bottom: 24px;
		text-align: center;
	}

	.avatar-upload {
		position: relative;
		display: inline-block;
		cursor: pointer;

		:deep(.el-upload) {
			position: relative;
			border-radius: 50%;
		}
	}

	.avatar-upload__mask {
		position: absolute;
		inset: 0;
		display: flex;
		flex-direction: column;
		align-items: center;
		justify-content: center;
		gap: 4px;
		border-radius: 50%;
		color: #fff;
		font-size: 12px;
		background: rgba(15, 23, 42, 0.58);
		opacity: 0;
		transition: opacity 0.18s ease;
	}

	.avatar-upload:hover .avatar-upload__mask {
		opacity: 1;
	}

	.username {
		margin-top: 14px;
		font-size: 21px;
		line-height: 30px;
		font-weight: 500;
		color: var(--el-text-color-primary);
	}

	.account {
		font-size: 13px;
		color: var(--el-text-color-secondary);
	}

	.profile-tags {
		display: flex;
		justify-content: center;
		flex-wrap: wrap;
		gap: 8px;
		margin-top: 12px;
	}

	.account-center-org {
		margin-bottom: 16px;
		padding-top: 10px;
		border-top: 1px solid var(--el-border-color-lighter);

		p {
			display: grid;
			grid-template-columns: 22px minmax(0, 1fr);
			align-items: center;
			min-height: 30px;
			margin: 7px 0;
			color: var(--el-text-color-regular);
		}

		.el-icon {
			color: var(--el-color-primary);
		}

		span {
			overflow-wrap: anywhere;
		}
	}

	.signature-box {
		display: flex;
		align-items: center;
		justify-content: center;
		width: 100%;
		height: 150px;
		margin-top: 14px;
		margin-bottom: 10px;
		background-color: var(--el-fill-color-blank);
		border: 1px solid var(--el-border-color);
	}

	.signature-image,
	.signature-image :deep(.el-image) {
		width: 100%;
		height: 100%;
	}

	.signature-actions {
		display: flex;
		flex-wrap: wrap;
		gap: 10px;

		.el-button + .el-button {
			margin-left: 0;
		}
	}

	.signature-editor {
		width: 100%;
		height: 260px;
		overflow: hidden;
		background-color: #fff;
		border: 1px dashed var(--el-border-color);
	}

	.signature-canvas {
		display: block;
		width: 100%;
		height: 100%;
		touch-action: none;
		cursor: crosshair;
	}

	.signature-tools {
		display: flex;
		flex-wrap: wrap;
		gap: 20px;
		margin-top: 12px;
	}

	.signature-tool {
		display: flex;
		align-items: center;
		gap: 8px;
		color: var(--el-text-color-regular);
		font-size: 13px;
	}

	.profile-tabs {
		:deep(.el-tabs__header) {
			margin-bottom: 20px;
		}
	}

	.personal-form {
		max-width: 940px;

		:deep(.el-form-item) {
			margin-bottom: 0;
		}

		:deep(.el-input),
		:deep(.el-select),
		:deep(.el-date-editor) {
			width: 100%;
		}
	}

	.form-actions {
		margin-top: 2px;
	}

	.info-descriptions {
		max-width: 940px;

		:deep(.el-descriptions__label) {
			width: 120px;
		}
	}

	.security-list {
		max-width: 940px;
	}

	.security-list__item {
		display: flex;
		align-items: center;
		justify-content: space-between;
		gap: 16px;
		padding: 15px 0;
		border-bottom: 1px solid var(--el-border-color-lighter);

		&:last-child {
			border-bottom: 0;
		}
	}

	.security-list__title {
		font-size: 14px;
		font-weight: 600;
		color: var(--el-text-color-primary);
	}

	.security-list__desc {
		margin-top: 6px;
		font-size: 13px;
		color: var(--el-text-color-secondary);
		overflow-wrap: anywhere;
	}

	.code-text {
		font-family: Consolas, 'Liberation Mono', monospace;
	}
}

@media (max-width: 768px) {
	.personal-page {
		.personal-grid {
			row-gap: 10px;
		}

		.detail-card {
			:deep(.el-card__body) {
				padding: 14px;
			}
		}

		.personal-form {
			:deep(.el-form-item__label) {
				width: 82px !important;
			}
		}
	}
}
</style>
