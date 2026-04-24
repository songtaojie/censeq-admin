<template>
	<div class="system-menu-dialog-container">
		<el-dialog v-model="state.dialog.isShowDialog" width="860px" destroy-on-close draggable :close-on-click-modal="false">
			<template #header>
				<div style="color: #fff">
					<el-icon size="16" style="margin-right: 4px; display: inline; vertical-align: middle">
						<ele-Edit v-if="state.dialog.type === 'edit'" />
						<ele-Plus v-else />
					</el-icon>
					<span>{{ state.dialog.title }}</span>
				</div>
			</template>
			<el-form ref="menuDialogFormRef" :model="state.ruleForm" :rules="rules" size="default" label-width="90px" v-loading="state.loading">
				<el-tabs v-model="state.activeTab" class="menu-dialog-tabs">
					<!-- ========== Tab 1: 基本信息 ========== -->
					<el-tab-pane label="基本信息" name="basic">
						<el-row :gutter="20" class="tab-pane-row">
							<!-- 上级菜单 - 全宽 -->
							<el-col :span="24" class="mb16">
								<el-form-item label="上级菜单" prop="parentId">
									<el-tree-select
										v-model="state.ruleForm.parentId"
										:data="parentOptions"
										:props="{ label: 'displayTitle', value: 'id', children: 'children', disabled: 'disabled' }"
										check-strictly
										clearable
										default-expand-all
										class="w100"
										placeholder="请选择上级菜单（留空为顶级）"
									/>
								</el-form-item>
							</el-col>
							<!-- 菜单类型 -->
							<el-col :span="24" class="mb16">
								<el-form-item label="菜单类型" prop="type">
									<el-radio-group v-model="state.ruleForm.type" @change="onTypeChange">
										<el-radio :label="1">目录</el-radio>
										<el-radio :label="2">菜单</el-radio>
										<el-radio :label="3">按钮</el-radio>
									</el-radio-group>
								</el-form-item>
							</el-col>
							<!-- 菜单名称 / 菜单编码 -->
							<el-col :xs="24" :sm="12" class="mb16">
								<el-form-item label="菜单名称" prop="title">
									<el-input v-model="state.ruleForm.title" placeholder="支持 i18n key 或直接标题" clearable />
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="12" class="mb16">
								<el-form-item label="菜单编码" prop="name">
									<el-input v-model="state.ruleForm.name" placeholder="唯一编码，如 systemMenu" clearable />
								</el-form-item>
							</el-col>
							<!-- 按钮编码（仅 type=3） -->
							<el-col v-if="state.ruleForm.type === 3" :xs="24" :sm="12" class="mb16">
								<el-form-item label="按钮编码" prop="buttonCode">
									<el-input v-model="state.ruleForm.buttonCode" placeholder="如 system.menu.create" clearable />
								</el-form-item>
							</el-col>
							<!-- 排序 -->
							<el-col :xs="24" :sm="12" class="mb16">
								<el-form-item label="菜单排序" prop="sort">
									<el-input-number v-model="state.ruleForm.sort" controls-position="right" placeholder="排序值" class="w100" :min="0" />
								</el-form-item>
							</el-col>
							<!-- 授权方式 -->
							<el-col :xs="24" :sm="12" class="mb16">
								<el-form-item label="授权方式" prop="authorizationMode">
									<el-select v-model="state.ruleForm.authorizationMode" class="w100">
										<el-option label="匿名访问" :value="1" />
										<el-option label="命中任一权限" :value="2" />
										<el-option label="命中全部权限" :value="3" />
									</el-select>
								</el-form-item>
							</el-col>
							<!-- 开关区 -->
							<el-col :xs="24" :sm="12" class="mb16">
								<el-form-item label="是否显示">
									<el-switch v-model="state.ruleForm.visible" active-text="显示" inactive-text="隐藏" inline-prompt />
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="12" class="mb16">
								<el-form-item label="启用状态">
									<el-switch v-model="state.ruleForm.status" active-text="启用" inactive-text="停用" inline-prompt />
								</el-form-item>
							</el-col>
						</el-row>
					</el-tab-pane>

					<!-- ========== Tab 2: 路由配置（仅目录/菜单） ========== -->
					<el-tab-pane v-if="state.ruleForm.type !== 3" label="路由配置" name="route">
						<el-row :gutter="20" class="tab-pane-row">
							<el-col :xs="24" :sm="12" class="mb16">
								<el-form-item label="路由路径" prop="path">
									<el-input v-model="state.ruleForm.path" placeholder="如 /system/menu" clearable />
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="12" class="mb16">
								<el-form-item label="路由名称" prop="routeName">
									<el-input v-model="state.ruleForm.routeName" placeholder="路由 name 值，如 systemMenu" clearable />
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="12" class="mb16">
								<el-form-item label="组件路径" prop="component">
									<el-input v-model="state.ruleForm.component" placeholder="如 system/menu/index" clearable />
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="12" class="mb16">
								<el-form-item label="重定向">
									<el-input v-model="state.ruleForm.redirect" placeholder="目录可配置默认子页跳转" clearable />
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="12" class="mb16">
								<el-form-item label="菜单图标">
									<IconSelector placeholder="请选择菜单图标" v-model="state.ruleForm.icon" />
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="12" class="mb16">
								<el-form-item label="链接地址" prop="externalUrl">
									<el-input
										v-model="state.ruleForm.externalUrl"
										placeholder="外链/内嵌 URL（https://...）"
										clearable
										:disabled="!state.ruleForm.isExternal && !state.ruleForm.isIframe"
									/>
								</el-form-item>
							</el-col>
							<!-- 开关行 -->
							<el-col :xs="12" :sm="6" class="mb16">
								<el-form-item label="页面缓存" label-width="80px">
									<el-switch v-model="state.ruleForm.keepAlive" inline-prompt active-text="是" inactive-text="否" />
								</el-form-item>
							</el-col>
							<el-col :xs="12" :sm="6" class="mb16">
								<el-form-item label="是否固定" label-width="80px">
									<el-switch v-model="state.ruleForm.affix" inline-prompt active-text="是" inactive-text="否" />
								</el-form-item>
							</el-col>
							<el-col :xs="12" :sm="6" class="mb16">
								<el-form-item label="是否外链" label-width="80px">
									<el-switch v-model="state.ruleForm.isExternal" inline-prompt active-text="是" inactive-text="否" @change="onExternalChange" :disabled="state.ruleForm.isIframe" />
								</el-form-item>
							</el-col>
							<el-col :xs="12" :sm="6" class="mb16">
								<el-form-item label="是否内嵌" label-width="80px">
									<el-switch v-model="state.ruleForm.isIframe" inline-prompt active-text="是" inactive-text="否" @change="onSelectIframeChange" :disabled="state.ruleForm.isExternal" />
								</el-form-item>
							</el-col>
						</el-row>
					</el-tab-pane>

					<!-- ========== Tab 3: 权限配置 ========== -->
					<el-tab-pane label="权限配置" name="permission">
						<el-row :gutter="20" class="tab-pane-row">
							<el-col :span="24" class="mb16">
								<el-form-item label="权限组" label-width="70px">
									<el-select
										v-model="state.ruleForm.selectedPermissionGroups"
										multiple
										clearable
										collapse-tags
										collapse-tags-tooltip
										placeholder="不选则展示全量权限组"
										class="w100"
										@change="onPermissionGroupsChange"
									>
										<el-option
											v-for="group in state.allPermissionGroups"
											:key="group.name"
											:label="group.displayName"
											:value="group.name"
										/>
									</el-select>
								</el-form-item>
							</el-col>
							<el-col :span="24" class="mb16">
								<el-form-item label="权限选择" prop="selectedPermissionNames" label-width="70px">
									<div class="permission-selector">
										<div class="permission-selector__toolbar">
											<el-tag type="primary" size="small" effect="light">已选 {{ state.ruleForm.selectedPermissionNames.length }} 项</el-tag>
											<el-button text type="danger" size="small" icon="ele-Delete" @click="clearPermissionSelection">清空</el-button>
										</div>
										<el-tree
											ref="permissionTreeRef"
											:data="permissionTreeData"
											node-key="name"
											show-checkbox
											default-expand-all
											:props="permissionTreeProps"
											class="permission-selector__tree"
											@check="syncSelectedPermissions"
										/>
									</div>
								</el-form-item>
							</el-col>
							<el-col :span="24" class="mb16">
								<el-form-item label="备注" label-width="70px">
									<el-input v-model="state.ruleForm.remark" type="textarea" :rows="3" placeholder="备注信息" />
								</el-form-item>
							</el-col>
						</el-row>
					</el-tab-pane>
				</el-tabs>
			</el-form>
			<template #footer>
				<span class="dialog-footer">
					<el-button icon="ele-CircleClose" size="default" @click="onCancel">取 消</el-button>
					<el-button type="primary" icon="ele-Select" size="default" :loading="state.submitting" @click="onSubmit">{{ state.dialog.submitTxt }}</el-button>
				</span>
			</template>
		</el-dialog>
	</div>
</template>

<script setup lang="ts" name="systemMenuDialog">
import { computed, defineAsyncComponent, nextTick, reactive, ref } from 'vue';
import type { ElTree, FormInstance, FormRules } from 'element-plus';
import { ElMessage } from 'element-plus';
import { useMenuApi } from '/@/api/apis';
import type {
	CreateMenuDto,
	MenuDetailDto,
	MenuPermissionDefinitionDto,
	MenuPermissionGroupDto,
	MenuTreeItemDto,
	UpdateMenuDto,
} from '/@/api/models/menu';
import { i18n } from '/@/i18n/index';

// 定义子组件向父组件传值/事件
const emit = defineEmits(['refresh']);

// 引入组件
const IconSelector = defineAsyncComponent(() => import('/@/components/iconSelector/index.vue'));

const menuApi = useMenuApi();
const permissionTreeRef = ref<InstanceType<typeof ElTree>>();

type DialogMode = 'add' | 'edit';

type MenuTreeOption = MenuTreeItemDto & {
	displayTitle: string;
	disabled?: boolean;
	children: MenuTreeOption[];
};

type MenuFormState = {
	id?: string;
	parentId?: string;
	name: string;
	title: string;
	routeName: string;
	path: string;
	component: string;
	redirect: string;
	icon: string;
	type: 1 | 2 | 3;
	sort: number;
	visible: boolean;
	keepAlive: boolean;
	affix: boolean;
	isExternal: boolean;
	externalUrl: string;
	isIframe: boolean;
	status: boolean;
	authorizationMode: 1 | 2 | 3;
	selectedPermissionNames: string[];
	selectedPermissionGroups: string[];
	remark: string;
	buttonCode: string;
	concurrencyStamp: string;
};

type PermissionTreeNode = {
	name: string;
	displayName: string;
	children?: PermissionTreeNode[];
	disabled?: boolean;
};

const createDefaultForm = (): MenuFormState => ({
	id: undefined,
	parentId: undefined,
	name: '',
	title: '',
	routeName: '',
	path: '',
	component: '',
	redirect: '',
	icon: '',
	type: 2,
	sort: 0,
	visible: true,
	keepAlive: true,
	affix: false,
	isExternal: false,
	externalUrl: '',
	isIframe: false,
	status: true,
	authorizationMode: 1,
	selectedPermissionNames: [],
	selectedPermissionGroups: [],
	remark: '',
	buttonCode: '',
	concurrencyStamp: '',
});

// 定义变量内容
const menuDialogFormRef = ref<FormInstance>();
const state = reactive({
	ruleForm: createDefaultForm(),
	menuData: [] as MenuTreeItemDto[],
	permissionGroups: [] as MenuPermissionGroupDto[],
	allPermissionGroups: [] as { name: string; displayName: string }[],
	loading: false,
	submitting: false,
	activeTab: 'basic' as 'basic' | 'route' | 'permission',
	dialog: {
		isShowDialog: false,
		type: 'add' as DialogMode,
		title: '',
		submitTxt: '',
	},
});

const rules = computed<FormRules>(() => ({
	name: [{ required: true, message: '请输入菜单编码', trigger: 'blur' }],
	title: [{ required: true, message: '请输入菜单名称', trigger: 'blur' }],
	path: [
		{
			validator: (_rule, value, callback) => {
				if (state.ruleForm.type !== 3 && !value) {
					callback(new Error('请输入路由路径'));
					return;
				}
				callback();
			},
			trigger: 'blur',
		},
	],
	component: [
		{
			validator: (_rule, value, callback) => {
				if (state.ruleForm.type !== 3 && !state.ruleForm.isExternal && !state.ruleForm.isIframe && !value) {
					callback(new Error('请输入组件路径'));
					return;
				}
				callback();
			},
			trigger: 'blur',
		},
	],
	buttonCode: [
		{
			validator: (_rule, value, callback) => {
				if (state.ruleForm.type === 3 && !value) {
					callback(new Error('按钮类型必须填写按钮编码'));
					return;
				}
				callback();
			},
			trigger: 'blur',
		},
	],
	selectedPermissionNames: [
		{
			validator: (_rule, _value, callback) => {
				if (state.ruleForm.authorizationMode !== 1 && state.ruleForm.selectedPermissionNames.length === 0) {
					callback(new Error('权限模式菜单至少绑定一个权限名'));
					return;
				}
				callback();
			},
			trigger: 'blur',
		},
	],
	externalUrl: [
		{
			validator: (_rule, value, callback) => {
				if ((state.ruleForm.isExternal || state.ruleForm.isIframe) && !value) {
					callback(new Error('外链或内嵌菜单必须填写链接地址'));
					return;
				}
				callback();
			},
			trigger: 'blur',
		},
	],
}));

const parentOptions = computed<MenuTreeOption[]>(() => {
	const excludedIds = new Set<string>();
	if (state.dialog.type === 'edit' && state.ruleForm.id) {
		collectDescendantIds(state.menuData, state.ruleForm.id, excludedIds);
	}
	return buildTreeOptions(state.menuData, excludedIds);
});

const permissionTreeProps = {
	label: 'displayName',
	children: 'children',
};

const permissionTreeData = computed<PermissionTreeNode[]>(() => {
	return state.permissionGroups.map((group) => {
		const nodeMap = new Map<string, PermissionTreeNode>();
		const roots: PermissionTreeNode[] = [];

		for (const permission of group.permissions) {
			nodeMap.set(permission.name, {
				name: permission.name,
				displayName: permission.displayName || permission.name,
				children: [],
			});
		}

		for (const permission of group.permissions) {
			const node = nodeMap.get(permission.name)!;
			if (permission.parentName && nodeMap.has(permission.parentName)) {
				nodeMap.get(permission.parentName)!.children!.push(node);
			} else {
				roots.push(node);
			}
		}

		return {
			name: `group:${group.name}`,
			displayName: group.displayName || group.name,
			disabled: true,
			children: roots,
		};
	});
});

const translateTitle = (title: string) => {
	return title?.startsWith('message.') ? i18n.global.t(title) : title;
};

const collectDescendantIds = (items: MenuTreeItemDto[], id: string, result: Set<string>) => {
	for (const item of items) {
		if (item.id === id) {
			collectAllIds(item, result);
			return true;
		}
		if (collectDescendantIds(item.children ?? [], id, result)) {
			return true;
		}
	}
	return false;
};

const collectAllIds = (item: MenuTreeItemDto, result: Set<string>) => {
	result.add(item.id);
	for (const child of item.children ?? []) {
		collectAllIds(child, result);
	}
};

const buildTreeOptions = (items: MenuTreeItemDto[], excludedIds: Set<string>): MenuTreeOption[] => {
	return items
		.filter((item) => item.type !== 3)
		.filter((item) => !excludedIds.has(item.id))
		.map((item) => ({
			...item,
			displayTitle: `${translateTitle(item.title)} (${item.name})`,
			children: buildTreeOptions(item.children ?? [], excludedIds),
		}));
};

const resetForm = () => {
	state.ruleForm = createDefaultForm();
};

const loadMenuTree = async () => {
	const data = await menuApi.getMenuTree();
	state.menuData = data.items ?? [];
};

const loadPermissionGroups = async (params?: { menuId?: string; parentId?: string }) => {
	const data = await menuApi.getMenuPermissionGroups(params);
	state.permissionGroups = data.items ?? [];
};

const loadAllPermissionGroups = async () => {
	// 加载全量权限组（不传 menuId/parentId），用于权限组多选下拉
	const data = await menuApi.getMenuPermissionGroups();
	state.allPermissionGroups = (data.items ?? []).map((g) => ({ name: g.name, displayName: g.displayName }));
};

const fillForm = (detail: MenuDetailDto) => {
	state.ruleForm = {
		id: detail.id,
		parentId: detail.parentId ?? undefined,
		name: detail.name,
		title: detail.title,
		routeName: detail.routeName ?? '',
		path: detail.path ?? '',
		component: detail.component ?? '',
		redirect: detail.redirect ?? '',
		icon: detail.icon ?? '',
		type: detail.type,
		sort: detail.sort,
		visible: detail.visible,
		keepAlive: detail.keepAlive,
		affix: detail.affix,
		isExternal: detail.isExternal,
		externalUrl: detail.externalUrl ?? '',
		isIframe: detail.isIframe,
		status: detail.status,
		authorizationMode: detail.authorizationMode,
		selectedPermissionNames: [...(detail.permissionNames ?? [])],
		selectedPermissionGroups: detail.permissionGroups
			? detail.permissionGroups.split(',').map((s) => s.trim()).filter(Boolean)
			: [],
		remark: detail.remark ?? '',
		buttonCode: detail.buttonCode ?? '',
		concurrencyStamp: detail.concurrencyStamp,
	};
};

const syncPermissionTreeState = () => {
	nextTick(() => {
		permissionTreeRef.value?.setCheckedKeys(state.ruleForm.selectedPermissionNames);
	});
};

// 打开弹窗
const openDialog = async (type: DialogMode, row?: MenuTreeItemDto) => {
	state.loading = true;
	state.activeTab = 'basic';
	try {
		state.dialog.type = type;
		state.dialog.title = type === 'edit' ? '修改菜单' : '新增菜单';
		state.dialog.submitTxt = type === 'edit' ? '修 改' : '新 增';
		await loadMenuTree();
		await loadAllPermissionGroups();
		resetForm();

		// Determine context for permission group filtering
		const permissionParams: { menuId?: string; parentId?: string } = {};
		if (type === 'edit' && row?.id) {
			const detail = await menuApi.getMenu(row.id);
			fillForm(detail);
			permissionParams.menuId = row.id;
			if (detail.parentId) {
				permissionParams.parentId = detail.parentId;
			}
		} else if (row?.id && row.type !== 3) {
			state.ruleForm.parentId = row.id;
			permissionParams.parentId = row.id;
		}

		await loadPermissionGroups(permissionParams);
		state.dialog.isShowDialog = true;
		syncPermissionTreeState();
	} finally {
		state.loading = false;
	}
};

// 关闭弹窗
const closeDialog = () => {
	state.dialog.isShowDialog = false;
};

const syncSelectedPermissions = () => {
	const checkedKeys = (permissionTreeRef.value?.getCheckedKeys(false) ?? []) as string[];
	const halfCheckedKeys = (permissionTreeRef.value?.getHalfCheckedKeys() ?? []) as string[];
	state.ruleForm.selectedPermissionNames = [...new Set([...checkedKeys, ...halfCheckedKeys].filter((item) => !String(item).startsWith('group:')))];
	menuDialogFormRef.value?.validateField('selectedPermissionNames').catch(() => undefined);
};

const clearPermissionSelection = () => {
	permissionTreeRef.value?.setCheckedKeys([]);
	state.ruleForm.selectedPermissionNames = [];
};

const onPermissionGroupsChange = async () => {
	// 权限组变更时，根据新选择的权限组重新加载权限树，并清空已选权限
	const groups = state.ruleForm.selectedPermissionGroups;
	if (groups.length > 0) {
		// 直接从 allPermissionGroups 过滤，不需要再请求接口
		const allowedNames = new Set(groups);
		const data = await menuApi.getMenuPermissionGroups();
		state.permissionGroups = (data.items ?? []).filter((g) => allowedNames.has(g.name));
	} else {
		// 未选权限组时展示全量
		const data = await menuApi.getMenuPermissionGroups();
		state.permissionGroups = data.items ?? [];
	}
	// 清空已选权限（权限组变了，之前选的可能不再适用）
	permissionTreeRef.value?.setCheckedKeys([]);
	state.ruleForm.selectedPermissionNames = [];
};

const normalizeOptional = (value: string) => {
	const normalized = value.trim();
	return normalized.length > 0 ? normalized : undefined;
};

const buildPayload = (): CreateMenuDto => {
	const permissionNames = [...state.ruleForm.selectedPermissionNames];
	return {
		parentId: state.ruleForm.parentId ?? null,
		name: state.ruleForm.name.trim(),
		title: state.ruleForm.title.trim(),
		routeName: normalizeOptional(state.ruleForm.routeName),
		path: state.ruleForm.type === 3 ? undefined : normalizeOptional(state.ruleForm.path),
		component: state.ruleForm.type === 3 || state.ruleForm.isExternal || state.ruleForm.isIframe ? undefined : normalizeOptional(state.ruleForm.component),
		redirect: state.ruleForm.type === 3 ? undefined : normalizeOptional(state.ruleForm.redirect),
		icon: state.ruleForm.type === 3 ? undefined : normalizeOptional(state.ruleForm.icon),
		type: state.ruleForm.type,
		sort: state.ruleForm.sort,
		visible: state.ruleForm.visible,
		keepAlive: state.ruleForm.type === 3 ? false : state.ruleForm.keepAlive,
		affix: state.ruleForm.type === 3 ? false : state.ruleForm.affix,
		isExternal: state.ruleForm.type === 3 ? false : state.ruleForm.isExternal,
		externalUrl: state.ruleForm.type === 3 ? undefined : normalizeOptional(state.ruleForm.externalUrl),
		isIframe: state.ruleForm.type === 3 ? false : state.ruleForm.isIframe,
		status: state.ruleForm.status,
		authorizationMode: state.ruleForm.authorizationMode,
		remark: normalizeOptional(state.ruleForm.remark),
		buttonCode: state.ruleForm.type === 3 ? normalizeOptional(state.ruleForm.buttonCode) : undefined,
		permissionGroups: state.ruleForm.selectedPermissionGroups.length > 0
			? state.ruleForm.selectedPermissionGroups.join(',')
			: undefined,
		permissionNames,
	};
};

const onSelectIframeChange = () => {
	if (state.ruleForm.isIframe) {
		state.ruleForm.isExternal = false;
	}
};

const onExternalChange = () => {
	if (state.ruleForm.isExternal) {
		state.ruleForm.isIframe = false;
	}
};

const onTypeChange = () => {
	if (state.ruleForm.type === 3) {
		state.ruleForm.routeName = '';
		state.ruleForm.path = '';
		state.ruleForm.component = '';
		state.ruleForm.redirect = '';
		state.ruleForm.icon = '';
		state.ruleForm.keepAlive = false;
		state.ruleForm.affix = false;
		state.ruleForm.isExternal = false;
		state.ruleForm.isIframe = false;
		state.ruleForm.externalUrl = '';
	} else if (!state.ruleForm.component && state.ruleForm.type === 1) {
		state.ruleForm.component = 'layout/routerView/parent';
	}

	if (state.ruleForm.type === 3 && !state.ruleForm.parentId) {
		state.ruleForm.authorizationMode = 2;
	}
};

// 取消
const onCancel = () => {
	closeDialog();
};

// 提交
const onSubmit = async () => {
	if (!menuDialogFormRef.value) {
		return;
	}

	await menuDialogFormRef.value.validate(async (valid) => {
		if (!valid) {
			return;
		}

		state.submitting = true;
		try {
			const payload = buildPayload();
			if (state.dialog.type === 'edit' && state.ruleForm.id) {
				const updatePayload: UpdateMenuDto = {
					...payload,
					concurrencyStamp: state.ruleForm.concurrencyStamp,
				};
				await menuApi.updateMenu(state.ruleForm.id, updatePayload);
				ElMessage.success('菜单修改成功');
			} else {
				await menuApi.createMenu(payload);
				ElMessage.success('菜单创建成功');
			}
			closeDialog();
			emit('refresh');
		} finally {
			state.submitting = false;
		}
	});
};

// 暴露变量
defineExpose({
	openDialog,
});
</script>

<style scoped lang="scss">
.menu-dialog-tabs {
	:deep(.el-tabs__content) {
		overflow: visible;
	}
}

.tab-pane-row {
	padding: 4px 2px 0;
	min-height: 260px;
}

.permission-selector {
	width: 100%;

	&__toolbar {
		display: flex;
		justify-content: space-between;
		align-items: center;
		margin-bottom: 8px;
	}

	&__tree {
		border: 1px solid var(--el-border-color);
		border-radius: var(--el-border-radius-base);
		padding: 10px;
		max-height: 280px;
		overflow-y: auto;
	}
}
</style>
