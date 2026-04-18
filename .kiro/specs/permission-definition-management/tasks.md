# 实现计划：权限定义管理（Permission Definition Management）

## 概述

在现有 `Censeq.PermissionManagement` 模块基础上扩展，新增权限定义维护能力。后端采用 .NET / ABP Framework，前端采用 Vue 3 + Element Plus。实现遵循最小侵入原则：复用现有实体和 Repository，只新增必要的 DTO、接口、Service、Controller 和前端组件。

## 任务

- [x] 1. 后端 Application.Contracts 层：新增 DTO 和 Service 接口
  - [x] 1.1 创建 `PermissionGroupDefinitionDto.cs`
    - 位置：`censeq-admin-api/modules/permission-management/src/Censeq.PermissionManagement.Application.Contracts/Censeq/PermissionManagement/Definition/`
    - 包含字段：`Guid Id`、`string Name`、`string DisplayName`
    - _需求：1.1, 3.1, 3.6_

  - [x] 1.2 创建 `PermissionDefinitionDto.cs`
    - 位置：同上目录
    - 包含字段：`Guid Id`、`string GroupName`、`string Name`、`string? ParentName`、`string DisplayName`、`bool IsEnabled`
    - _需求：2.1, 4.1, 4.6_

  - [x] 1.3 创建 `UpdatePermissionGroupDefinitionDto.cs`
    - 位置：同上目录
    - 包含字段：`string DisplayName`，添加 `[Required]` 和 `[MaxLength(256)]` 数据注解
    - _需求：3.1, 3.3, 3.4_

  - [x] 1.4 创建 `UpdatePermissionDefinitionDto.cs`
    - 位置：同上目录
    - 包含字段：`string DisplayName`，添加 `[Required]` 和 `[MaxLength(256)]` 数据注解
    - _需求：4.1, 4.3, 4.4_

  - [x] 1.5 创建 `IPermissionDefinitionAppService.cs` 接口
    - 位置：同上目录
    - 继承 `IApplicationService`
    - 声明四个方法：`GetGroupsAsync()`、`UpdateGroupAsync(groupName, input)`、`GetPermissionsAsync(groupName)`、`UpdatePermissionAsync(name, input)`
    - _需求：1.1, 2.1, 3.1, 4.1_

- [x] 2. 后端 Application 层：实现 PermissionDefinitionAppService
  - [x] 2.1 创建 `PermissionDefinitionAppService.cs`
    - 位置：`censeq-admin-api/modules/permission-management/src/Censeq.PermissionManagement.Application/`
    - 继承 `ApplicationService`，实现 `IPermissionDefinitionAppService`
    - 添加 `[Authorize("PermissionManagement.DefinitionManagement")]` 特性
    - 注入 `IPermissionGroupDefinitionRecordRepository` 和 `IPermissionDefinitionRecordRepository`
    - _需求：6.1_

  - [x] 2.2 实现 `GetGroupsAsync()` 方法
    - 调用 `_groupRepo.GetListAsync()`，映射为 `PermissionGroupDefinitionDto` 列表，按 `Name` 排序返回
    - _需求：1.1_

  - [x] 2.3 实现 `UpdateGroupAsync(groupName, input)` 方法
    - 通过 `FindAsync(g => g.Name == groupName)` 查找记录，不存在时抛出 `EntityNotFoundException`（触发 404）
    - 更新 `DisplayName`（使用 `Trim()`），调用 `UpdateAsync`，返回更新后的 DTO
    - _需求：3.1, 3.3, 3.4, 3.5, 3.6, 5.1_

  - [x] 2.4 实现 `GetPermissionsAsync(groupName)` 方法
    - 先验证 groupName 对应的权限组存在，不存在时抛出 `EntityNotFoundException`（触发 404）
    - 调用 `_permissionRepo.GetListAsync(p => p.GroupName == groupName)`，映射为 DTO 列表，按 `Name` 排序返回
    - _需求：2.1, 2.3_

  - [x] 2.5 实现 `UpdatePermissionAsync(name, input)` 方法
    - 通过 `FindByNameAsync(name)` 查找记录，不存在时抛出 `EntityNotFoundException`（触发 404）
    - 更新 `DisplayName`（使用 `Trim()`），调用 `UpdateAsync`，返回更新后的 DTO
    - _需求：4.1, 4.3, 4.4, 4.5, 4.6, 5.2_

  - [ ]* 2.6 为 AppService 编写单元测试
    - 测试 `GetGroupsAsync` 返回排序后的列表
    - 测试 `UpdateGroupAsync` 在 groupName 不存在时抛出 `EntityNotFoundException`
    - 测试 `UpdatePermissionAsync` 在 name 不存在时抛出 `EntityNotFoundException`
    - 测试 displayName 空白字符串经 Trim 后触发验证失败
    - _需求：3.3, 3.5, 4.4, 4.5_

- [x] 3. 后端 HttpApi 层：新增 PermissionDefinitionController
  - [x] 3.1 创建 `PermissionDefinitionController.cs`
    - 位置：`censeq-admin-api/modules/permission-management/src/Censeq.PermissionManagement.HttpApi/`
    - 继承 `AbpControllerBase`，实现 `IPermissionDefinitionAppService`
    - 添加路由特性：`[Route("api/permission-management/definition")]`
    - 添加 `[RemoteService]` 和 `[Area]` 特性，复用现有模块常量
    - 注入并委托 `IPermissionDefinitionAppService`
    - _需求：1.1, 2.1, 3.1, 4.1_

  - [x] 3.2 为四个端点配置 HTTP 方法和路由
    - `GET groups` → `GetGroupsAsync()`
    - `PUT groups/{groupName}` → `UpdateGroupAsync(groupName, input)`
    - `GET groups/{groupName}/permissions` → `GetPermissionsAsync(groupName)`
    - `PUT permissions/{name}` → `UpdatePermissionAsync(name, input)`
    - _需求：1.1, 2.1, 3.1, 4.1_

- [x] 4. 检查点 — 后端编译与接口验证
  - 确保后端项目编译通过，无编译错误
  - 确认四个 API 端点路由正确注册（可通过 Swagger 或单元测试验证）
  - 如有问题，请向用户说明后再继续

- [x] 5. 前端 API 层：新增类型定义和 API 服务
  - [x] 5.1 创建 `censeq-admin-web/src/api/models/permission/definition.ts`
    - 定义 `PermissionGroupDefinitionDto`、`PermissionDefinitionDto`、`UpdatePermissionGroupDefinitionDto`、`UpdatePermissionDefinitionDto` 四个 TypeScript 接口
    - _需求：1.1, 2.1, 3.1, 4.1_

  - [x] 5.2 创建 `censeq-admin-web/src/api/apis/permission-management/permission-definition.service.ts`
    - 使用 `useBaseApi('permission-management')` 创建 api 实例
    - 实现 `usePermissionDefinitionApi()` 组合函数，包含 `getGroups`、`updateGroup`、`getPermissions`、`updatePermission` 四个方法
    - groupName 和 name 参数使用 `encodeURIComponent` 编码
    - _需求：1.1, 2.1, 3.1, 4.1_

  - [x] 5.3 在 `censeq-admin-web/src/api/apis/index.ts` 中新增导出
    - 添加 `export * from './permission-management/permission-definition.service';`
    - _需求：1.1_

- [x] 6. 前端组件层：实现页面组件
  - [x] 6.1 创建 `EditDisplayNameDialog.vue`
    - 位置：`censeq-admin-web/src/views/system/permission-definition/components/`
    - Props：`visible: boolean`、`title: string`、`currentValue: string`
    - Emits：`confirm(newDisplayName: string)`、`cancel()`
    - 包含 `el-dialog` + `el-form` + `el-input`（maxlength=256，required 校验）
    - 提交前执行表单校验，空字符串或仅空白字符不允许提交
    - _需求：3.2, 3.3, 3.4, 4.2, 4.3, 4.4_

  - [x] 6.2 创建 `GroupTable.vue`
    - 位置：同上目录
    - 展示权限组列表，列：名称（只读）、显示名称、操作（编辑按钮）
    - 点击行高亮并 emit `select(group: PermissionGroupDefinitionDto)`
    - 点击编辑按钮打开 `EditDisplayNameDialog`，提交后调用 `updateGroup` 并刷新列表
    - 列表加载中显示 `v-loading` 加载状态
    - 接口报错时使用 `ElMessage.error` 显示错误提示
    - _需求：1.2, 1.3, 1.4, 3.2, 3.6, 5.1, 5.3_

  - [x] 6.3 创建 `PermissionTable.vue`
    - 位置：同上目录
    - Props：`groupName: string`（当前选中的权限组名称）
    - 展示权限项列表，列：名称（只读）、显示名称、父权限（只读）、是否启用（只读 `el-tag`）、操作（编辑按钮）
    - 点击编辑按钮打开 `EditDisplayNameDialog`，提交后调用 `updatePermission` 并刷新列表
    - 列表加载中显示 `v-loading` 加载状态
    - _需求：2.2, 2.4, 4.2, 4.6, 5.2, 5.3_

  - [x] 6.4 创建主页面 `index.vue`
    - 位置：`censeq-admin-web/src/views/system/permission-definition/`
    - 采用左右分栏布局（`el-row` + `el-col`，左 8 右 16）
    - 左侧嵌入 `GroupTable`，监听 `select` 事件更新 `selectedGroupName`
    - 右侧嵌入 `PermissionTable`，传入 `selectedGroupName`
    - 页面挂载时触发权限组列表加载
    - _需求：1.2, 2.2, 5.3_

- [x] 7. 前端路由注册
  - [x] 7.1 在 `censeq-admin-web/src/router/route.ts` 的 `/system` children 中新增路由条目
    - path: `/system/permission-definition`
    - name: `systemPermissionDefinition`
    - component: 懒加载 `/@/views/system/permission-definition/index.vue`
    - meta: `title: '权限定义管理'`、`isKeepAlive: true`、`roles: ['admin']`、`icon: 'ele-Key'`
    - _需求：6.2_

- [x] 8. 最终检查点 — 确保所有任务完成
  - 确认前端项目编译通过，无 TypeScript 类型错误
  - 确认路由可正常访问，页面渲染无控制台报错
  - 确认权限组列表、权限项列表、编辑弹窗功能均可正常使用
  - 如有问题，请向用户说明后再继续

## 备注

- 标有 `*` 的子任务为可选任务，可在 MVP 阶段跳过
- 每个任务均引用了具体的需求条款，便于追溯
- 后端遵循最小侵入原则，不修改任何现有文件的核心逻辑
- 前端组件通过 Props/Emits 解耦，`EditDisplayNameDialog` 可同时服务于权限组和权限项的编辑场景
