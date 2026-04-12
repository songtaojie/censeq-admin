<template>
	<div class="system-menu-dialog-container">
		<el-dialog :title="state.dialog.title" v-model="state.dialog.isShowDialog" width="900px" destroy-on-close>
			<el-form ref="menuDialogFormRef" :model="state.ruleForm" :rules="rules" size="default" label-width="100px" v-loading="state.loading">
				<el-row :gutter="35">
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item label="上级菜单" prop="parentId">
							<el-tree-select
								v-model="state.ruleForm.parentId"
								:data="parentOptions"
								:props="{ label: 'displayTitle', value: 'id', children: 'children', disabled: 'disabled' }"
								check-strictly
								clearable
								default-expand-all
								class="w100"
								placeholder="请选择上级菜单"
							/>
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="菜单类型" prop="type">
							<el-radio-group v-model="state.ruleForm.type" @change="onTypeChange">
								<el-radio :label="1">目录</el-radio>
								<el-radio :label="2">菜单</el-radio>
								<el-radio :label="3">按钮</el-radio>
							</el-radio-group>
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="授权方式" prop="authorizationMode">
							<el-select v-model="state.ruleForm.authorizationMode" class="w100">
								<el-option label="匿名访问" :value="1" />
								<el-option label="命中任一权限" :value="2" />
								<el-option label="命中全部权限" :value="3" />
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="菜单名称" prop="title">
							<el-input v-model="state.ruleForm.title" placeholder="支持 i18n key 或直接标题" clearable></el-input>
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="菜单编码" prop="name">
							<el-input v-model="state.ruleForm.name" placeholder="唯一编码，例如 systemMenu" clearable></el-input>
						</el-form-item>
					</el-col>
					<template v-if="state.ruleForm.type !== 3">
						<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
							<el-form-item label="路由名称" prop="routeName">
								<el-input v-model="state.ruleForm.routeName" placeholder="路由中的 name 值" clearable></el-input>
							</el-form-item>
						</el-col>
						<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
							<el-form-item label="路由路径" prop="path">
								<el-input v-model="state.ruleForm.path" placeholder="例如 /system/menu" clearable></el-input>
							</el-form-item>
						</el-col>
						<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
							<el-form-item label="重定向">
								<el-input v-model="state.ruleForm.redirect" placeholder="目录可配置默认跳转" clearable></el-input>
							</el-form-item>
						</el-col>
						<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
							<el-form-item label="菜单图标">
								<IconSelector placeholder="请输入菜单图标" v-model="state.ruleForm.icon" />
							</el-form-item>
						</el-col>
						<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
							<el-form-item label="组件路径" prop="component">
								<el-input v-model="state.ruleForm.component" placeholder="例如 system/menu/index 或 layout/routerView/parent" clearable></el-input>
							</el-form-item>
						</el-col>
						<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
							<el-form-item label="链接地址" prop="externalUrl">
								<el-input
									v-model="state.ruleForm.externalUrl"
									placeholder="外链/内嵌时链接地址（http:xxx.com）"
									clearable
									:disabled="!state.ruleForm.isExternal && !state.ruleForm.isIframe"
								>
								</el-input>
							</el-form-item>
						</el-col>
						<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
							<el-form-item label="菜单排序" prop="sort">
								<el-input-number v-model="state.ruleForm.sort" controls-position="right" placeholder="请输入排序" class="w100" />
							</el-form-item>
						</el-col>
					</template>
					<template v-else>
						<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
							<el-form-item label="按钮编码" prop="buttonCode">
								<el-input v-model="state.ruleForm.buttonCode" placeholder="例如 system.menu.create" clearable></el-input>
							</el-form-item>
						</el-col>
						<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
							<el-form-item label="菜单排序" prop="sort">
								<el-input-number v-model="state.ruleForm.sort" controls-position="right" placeholder="请输入排序" class="w100" />
							</el-form-item>
						</el-col>
					</template>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="是否显示">
							<el-switch v-model="state.ruleForm.visible" />
						</el-form-item>
					</el-col>
					<template v-if="state.ruleForm.type !== 3">
						<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
							<el-form-item label="页面缓存">
								<el-switch v-model="state.ruleForm.keepAlive" />
							</el-form-item>
						</el-col>
						<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
							<el-form-item label="是否固定">
								<el-switch v-model="state.ruleForm.affix" />
							</el-form-item>
						</el-col>
						<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
							<el-form-item label="是否外链">
								<el-switch v-model="state.ruleForm.isExternal" @change="onExternalChange" :disabled="state.ruleForm.isIframe" />
							</el-form-item>
						</el-col>
						<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
							<el-form-item label="是否内嵌">
								<el-switch v-model="state.ruleForm.isIframe" @change="onSelectIframeChange" :disabled="state.ruleForm.isExternal" />
							</el-form-item>
						</el-col>
					</template>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="启用状态">
							<el-switch v-model="state.ruleForm.status" />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item label="权限选择" prop="selectedPermissionNames">
							<div class="permission-selector">
								<div class="permission-selector__toolbar">
									<el-tag type="info">已选 {{ state.ruleForm.selectedPermissionNames.length }} 项</el-tag>
									<el-button text type="primary" @click="clearPermissionSelection">清空</el-button>
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
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item label="备注">
							<el-input v-model="state.ruleForm.remark" type="textarea" :rows="3" placeholder="备注信息" />
							</el-form-item>
						</el-col>
				</el-row>
			</el-form>
			<template #footer>
				<span class="dialog-footer">
					<el-button @click="onCancel" size="default">取 消</el-button>
					<el-button type="primary" @click="onSubmit" size="default" :loading="state.submitting">{{ state.dialog.submitTxt }}</el-button>
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
	loading: false,
	submitting: false,
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

const loadPermissionGroups = async () => {
	const data = await menuApi.getMenuPermissionGroups();
	state.permissionGroups = data.items ?? [];
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
	try {
		state.dialog.type = type;
		state.dialog.title = type === 'edit' ? '修改菜单' : '新增菜单';
		state.dialog.submitTxt = type === 'edit' ? '修 改' : '新 增';
		await loadMenuTree();
		await loadPermissionGroups();
		resetForm();
		if (type === 'edit' && row?.id) {
			const detail = await menuApi.getMenu(row.id);
			fillForm(detail);
		} else if (row?.id && row.type !== 3) {
			state.ruleForm.parentId = row.id;
		}
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
