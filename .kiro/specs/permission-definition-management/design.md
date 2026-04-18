# 技术设计文档

## 概述

本功能在现有 `Censeq.PermissionManagement` 模块基础上扩展，新增权限定义维护能力。设计原则：**最小侵入**——复用现有实体、Repository 和模块结构，只新增必要的 DTO、接口、Service 和 Controller，不修改任何现有文件的核心逻辑。

---

## 架构概览

```
┌─────────────────────────────────────────────────────────┐
│  前端 Vue 3 + Element Plus                               │
│  /system/permission-definition                           │
│  ┌──────────────────┐  ┌──────────────────────────────┐ │
│  │  权限组列表表格   │  │  权限项列表表格（右侧/下方）  │ │
│  │  + 行内编辑按钮   │  │  + 行内编辑按钮              │ │
│  └──────────────────┘  └──────────────────────────────┘ │
└────────────────────────┬────────────────────────────────┘
                         │ HTTP
┌────────────────────────▼────────────────────────────────┐
│  后端 ASP.NET Core / ABP Framework                       │
│  PermissionDefinitionController                          │
│  GET  /api/permission-management/definition/groups       │
│  PUT  /api/permission-management/definition/groups/{n}   │
│  GET  .../definition/groups/{n}/permissions              │
│  PUT  .../definition/permissions/{n}                     │
└────────────────────────┬────────────────────────────────┘
                         │
┌────────────────────────▼────────────────────────────────┐
│  PermissionDefinitionAppService                          │
│  复用现有 Repository：                                    │
│  IPermissionGroupDefinitionRecordRepository              │
│  IPermissionDefinitionRecordRepository                   │
└─────────────────────────────────────────────────────────┘
```

---

## 后端设计

### 1. 新增 DTO（Application.Contracts 层）

位置：`Censeq.PermissionManagement.Application.Contracts/Censeq/PermissionManagement/Definition/`

#### `PermissionGroupDefinitionDto.cs`
```csharp
namespace Censeq.PermissionManagement;

/// <summary>权限组定义 DTO</summary>
public class PermissionGroupDefinitionDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
}
```

#### `PermissionDefinitionDto.cs`
```csharp
namespace Censeq.PermissionManagement;

/// <summary>权限项定义 DTO</summary>
public class PermissionDefinitionDto
{
    public Guid Id { get; set; }
    public string GroupName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? ParentName { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public bool IsEnabled { get; set; }
}
```

#### `UpdatePermissionGroupDefinitionDto.cs`
```csharp
using System.ComponentModel.DataAnnotations;

namespace Censeq.PermissionManagement;

/// <summary>更新权限组显示名称请求 DTO</summary>
public class UpdatePermissionGroupDefinitionDto
{
    [Required]
    [MaxLength(256)] // PermissionGroupDefinitionRecordConsts.MaxDisplayNameLength
    public string DisplayName { get; set; } = string.Empty;
}
```

#### `UpdatePermissionDefinitionDto.cs`
```csharp
using System.ComponentModel.DataAnnotations;

namespace Censeq.PermissionManagement;

/// <summary>更新权限项显示名称请求 DTO</summary>
public class UpdatePermissionDefinitionDto
{
    [Required]
    [MaxLength(256)] // PermissionDefinitionRecordConsts.MaxDisplayNameLength
    public string DisplayName { get; set; } = string.Empty;
}
```

---

### 2. 新增 Service 接口（Application.Contracts 层）

位置：`Censeq.PermissionManagement.Application.Contracts/Censeq/PermissionManagement/Definition/IPermissionDefinitionAppService.cs`

```csharp
using Volo.Abp.Application.Services;

namespace Censeq.PermissionManagement;

/// <summary>权限定义管理应用服务接口</summary>
public interface IPermissionDefinitionAppService : IApplicationService
{
    /// <summary>获取所有权限组列表</summary>
    Task<List<PermissionGroupDefinitionDto>> GetGroupsAsync();

    /// <summary>更新权限组显示名称</summary>
    Task<PermissionGroupDefinitionDto> UpdateGroupAsync(string groupName, UpdatePermissionGroupDefinitionDto input);

    /// <summary>获取指定权限组下的权限项列表</summary>
    Task<List<PermissionDefinitionDto>> GetPermissionsAsync(string groupName);

    /// <summary>更新权限项显示名称</summary>
    Task<PermissionDefinitionDto> UpdatePermissionAsync(string name, UpdatePermissionDefinitionDto input);
}
```

---

### 3. 新增 Service 实现（Application 层）

位置：`Censeq.PermissionManagement.Application/PermissionDefinitionAppService.cs`

```csharp
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Services;

namespace Censeq.PermissionManagement;

[Authorize("PermissionManagement.DefinitionManagement")]
public class PermissionDefinitionAppService : ApplicationService, IPermissionDefinitionAppService
{
    private readonly IPermissionGroupDefinitionRecordRepository _groupRepo;
    private readonly IPermissionDefinitionRecordRepository _permissionRepo;

    public PermissionDefinitionAppService(
        IPermissionGroupDefinitionRecordRepository groupRepo,
        IPermissionDefinitionRecordRepository permissionRepo)
    {
        _groupRepo = groupRepo;
        _permissionRepo = permissionRepo;
    }

    public async Task<List<PermissionGroupDefinitionDto>> GetGroupsAsync()
    {
        var groups = await _groupRepo.GetListAsync();
        return groups.Select(g => new PermissionGroupDefinitionDto
        {
            Id = g.Id,
            Name = g.Name,
            DisplayName = g.DisplayName
        }).OrderBy(g => g.Name).ToList();
    }

    public async Task<PermissionGroupDefinitionDto> UpdateGroupAsync(
        string groupName, UpdatePermissionGroupDefinitionDto input)
    {
        var group = await _groupRepo.FindAsync(g => g.Name == groupName)
            ?? throw new EntityNotFoundException(typeof(PermissionGroupDefinitionRecord), groupName);

        group.DisplayName = input.DisplayName.Trim();
        await _groupRepo.UpdateAsync(group);

        return new PermissionGroupDefinitionDto
        {
            Id = group.Id,
            Name = group.Name,
            DisplayName = group.DisplayName
        };
    }

    public async Task<List<PermissionDefinitionDto>> GetPermissionsAsync(string groupName)
    {
        // 验证 group 存在
        var groupExists = await _groupRepo.FindAsync(g => g.Name == groupName);
        if (groupExists == null)
            throw new EntityNotFoundException(typeof(PermissionGroupDefinitionRecord), groupName);

        var permissions = await _permissionRepo.GetListAsync(p => p.GroupName == groupName);
        return permissions.Select(p => new PermissionDefinitionDto
        {
            Id = p.Id,
            GroupName = p.GroupName,
            Name = p.Name,
            ParentName = p.ParentName,
            DisplayName = p.DisplayName,
            IsEnabled = p.IsEnabled
        }).OrderBy(p => p.Name).ToList();
    }

    public async Task<PermissionDefinitionDto> UpdatePermissionAsync(
        string name, UpdatePermissionDefinitionDto input)
    {
        var permission = await _permissionRepo.FindByNameAsync(name)
            ?? throw new EntityNotFoundException(typeof(PermissionDefinitionRecord), name);

        permission.DisplayName = input.DisplayName.Trim();
        await _permissionRepo.UpdateAsync(permission);

        return new PermissionDefinitionDto
        {
            Id = permission.Id,
            GroupName = permission.GroupName,
            Name = permission.Name,
            ParentName = permission.ParentName,
            DisplayName = permission.DisplayName,
            IsEnabled = permission.IsEnabled
        };
    }
}
```

---

### 4. 新增 Controller（HttpApi 层）

位置：`Censeq.PermissionManagement.HttpApi/PermissionDefinitionController.cs`

```csharp
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;

namespace Censeq.PermissionManagement;

[Controller]
[RemoteService(Name = PermissionManagementRemoteServiceConsts.RemoteServiceName)]
[Area(PermissionManagementRemoteServiceConsts.ModuleName)]
[Route("api/permission-management/definition")]
public class PermissionDefinitionController : AbpControllerBase, IPermissionDefinitionAppService
{
    private readonly IPermissionDefinitionAppService _appService;

    public PermissionDefinitionController(IPermissionDefinitionAppService appService)
    {
        _appService = appService;
    }

    [HttpGet("groups")]
    public Task<List<PermissionGroupDefinitionDto>> GetGroupsAsync()
        => _appService.GetGroupsAsync();

    [HttpPut("groups/{groupName}")]
    public Task<PermissionGroupDefinitionDto> UpdateGroupAsync(string groupName, UpdatePermissionGroupDefinitionDto input)
        => _appService.UpdateGroupAsync(groupName, input);

    [HttpGet("groups/{groupName}/permissions")]
    public Task<List<PermissionDefinitionDto>> GetPermissionsAsync(string groupName)
        => _appService.GetPermissionsAsync(groupName);

    [HttpPut("permissions/{name}")]
    public Task<PermissionDefinitionDto> UpdatePermissionAsync(string name, UpdatePermissionDefinitionDto input)
        => _appService.UpdatePermissionAsync(name, input);
}
```

---

### 5. 权限策略注册

在 `CenseqPermissionManagementDomainModule.cs` 或应用启动配置中注册策略：

```csharp
// 在 PermissionDefinitionProvider 中添加
context.Add(
    new PermissionGroupDefinition("PermissionManagement", L("Permission:PermissionManagement"))
        .AddPermission("PermissionManagement.DefinitionManagement", L("Permission:DefinitionManagement"))
);
```

---

## 前端设计

### 1. API 模型

位置：`censeq-admin-web/src/api/models/permission/definition.ts`

```typescript
export interface PermissionGroupDefinitionDto {
  id: string;
  name: string;
  displayName: string;
}

export interface PermissionDefinitionDto {
  id: string;
  groupName: string;
  name: string;
  parentName?: string;
  displayName: string;
  isEnabled: boolean;
}

export interface UpdatePermissionGroupDefinitionDto {
  displayName: string;
}

export interface UpdatePermissionDefinitionDto {
  displayName: string;
}
```

### 2. API Service

位置：`censeq-admin-web/src/api/apis/permission-management/permission-definition.service.ts`

```typescript
import type {
  PermissionGroupDefinitionDto,
  PermissionDefinitionDto,
  UpdatePermissionGroupDefinitionDto,
  UpdatePermissionDefinitionDto,
} from '/@/api/models/permission/definition';
import { useBaseApi } from '../base';

const api = useBaseApi('permission-management');
const BASE = 'api/permission-management/definition';

export function usePermissionDefinitionApi() {
  return {
    getGroups: (): Promise<PermissionGroupDefinitionDto[]> =>
      api.list<PermissionGroupDefinitionDto[]>(`${BASE}/groups`, {}),

    updateGroup: (groupName: string, input: UpdatePermissionGroupDefinitionDto): Promise<PermissionGroupDefinitionDto> =>
      api.update<PermissionGroupDefinitionDto>(`${BASE}/groups/${encodeURIComponent(groupName)}`, input),

    getPermissions: (groupName: string): Promise<PermissionDefinitionDto[]> =>
      api.list<PermissionDefinitionDto[]>(`${BASE}/groups/${encodeURIComponent(groupName)}/permissions`, {}),

    updatePermission: (name: string, input: UpdatePermissionDefinitionDto): Promise<PermissionDefinitionDto> =>
      api.update<PermissionDefinitionDto>(`${BASE}/permissions/${encodeURIComponent(name)}`, input),
  };
}
```

### 3. 页面组件结构

位置：`censeq-admin-web/src/views/system/permission-definition/`

```
permission-definition/
├── index.vue          # 主页面：左侧权限组列表 + 右侧权限项列表
└── components/
    ├── GroupTable.vue          # 权限组表格组件
    ├── PermissionTable.vue     # 权限项表格组件
    └── EditDisplayNameDialog.vue  # 通用编辑显示名称弹窗
```

#### 页面布局（index.vue）

采用左右分栏布局：
- 左侧（`el-col :span="8"`）：权限组列表，点击行高亮并加载右侧权限项
- 右侧（`el-col :span="16"`）：当前选中权限组的权限项列表

#### GroupTable.vue 关键逻辑

```
- 列：名称（只读）、显示名称、操作（编辑按钮）
- 点击行 → emit('select', group) → 父组件加载权限项
- 点击编辑 → 打开 EditDisplayNameDialog，提交后刷新列表
```

#### PermissionTable.vue 关键逻辑

```
- 列：名称（只读）、显示名称、父权限（只读）、是否启用（只读 Tag）、操作（编辑按钮）
- 点击编辑 → 打开 EditDisplayNameDialog，提交后刷新列表
```

#### EditDisplayNameDialog.vue

```
- Props: visible, title, currentValue
- Emits: confirm(newDisplayName), cancel
- 表单：单个 el-input，maxlength=256，required 校验
```

### 4. 路由注册

在 `censeq-admin-web/src/router/route.ts` 的 `/system` children 中新增：

```typescript
{
  path: '/system/permission-definition',
  name: 'systemPermissionDefinition',
  component: () => import('/@/views/system/permission-definition/index.vue'),
  meta: {
    title: '权限定义管理',
    isLink: '',
    isHide: false,
    isKeepAlive: true,
    isAffix: false,
    isIframe: false,
    roles: ['admin'],
    icon: 'ele-Key',
  },
},
```

### 5. API 导出

在 `censeq-admin-web/src/api/apis/index.ts` 新增：

```typescript
export * from './permission-management/permission-definition.service';
```

---

## 数据流

### 查看权限组
```
页面挂载 → getGroups() → GET /api/permission-management/definition/groups
→ 渲染 GroupTable
```

### 选择权限组查看权限项
```
点击权限组行 → getPermissions(groupName) → GET .../groups/{groupName}/permissions
→ 渲染 PermissionTable
```

### 编辑权限组显示名称
```
点击编辑 → 打开弹窗（预填当前 displayName）
→ 提交 → updateGroup(groupName, { displayName })
→ PUT .../groups/{groupName}
→ 成功 → 刷新 GroupTable → 关闭弹窗
```

### 编辑权限项显示名称
```
点击编辑 → 打开弹窗（预填当前 displayName）
→ 提交 → updatePermission(name, { displayName })
→ PUT .../permissions/{name}
→ 成功 → 刷新 PermissionTable → 关闭弹窗
```

---

## 错误处理

| 场景 | 后端响应 | 前端处理 |
|------|---------|---------|
| groupName / name 不存在 | HTTP 404 | ElMessage.error 提示 |
| displayName 为空或超长 | HTTP 400 + 验证信息 | 表单校验拦截，不发请求 |
| 未授权 | HTTP 403 | 路由守卫拦截，跳转无权限页 |
| 网络/服务器错误 | HTTP 5xx | ElMessage.error 通用提示 |

---

## 不在范围内

- 新增权限组或权限项（由代码启动时同步）
- 删除权限组或权限项
- 修改 `Name`、`GroupName`、`ParentName`、`IsEnabled`、`Providers` 等结构性字段
- 权限授予管理（由现有 `/api/permission-management/permissions` 接口处理）
