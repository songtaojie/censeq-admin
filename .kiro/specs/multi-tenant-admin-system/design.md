# Design Document

## Overview

本文档描述 Censeq 多租户后台管理系统的技术设计。系统基于 ABP Framework，采用共享数据库多租户架构，通过 TenantId 字段实现数据隔离。

## Architecture

### System Layers

前端 SPA 分为平台管理 UI 和租管 UI 两套菜单，共用同一个前端项目，根据登录账号的 TenantId 是否为 null 来决定显示哪套菜单。后端基于 ABP Framework DDD 架构，现有模块：Identity、Permission Management、Tenant Management，新增 Tenant Authorization 模块处理租户授权范围。

### Multi-Tenancy Strategy

- 共享数据库、共享表结构
- TenantId = null 表示平台数据
- TenantId = {guid} 表示租户数据
- ABP CurrentTenant 中间件自动注入租户上下文

## Components

### 1. 新增实体：TenantPermissionGrant

平台给租户开通权限项的授权记录，独立于 PermissionGrant（角色/用户权限授予）。

表名: CenseqTenantPermissionGrants

`csharp
public class TenantPermissionGrant : BasicAggregateRoot<Guid>
{
    public Guid TenantId { get; set; }
    public string PermissionName { get; set; }
    public DateTime CreationTime { get; set; }
    public Guid? CreatorId { get; set; }
}
`

索引：(TenantId, PermissionName) 唯一索引

### 2. 扩展实体：Tenant

在现有 Tenant 实体基础上新增 IsActive 字段：

`csharp
public class Tenant : FullAuditedAggregateRoot<Guid>
{
    // 现有字段...
    public bool IsActive { get; set; } = true;
}
`

### 3. 领域服务：ITenantAuthorizationManager

`csharp
public interface ITenantAuthorizationManager
{
    Task GrantPermissionsAsync(Guid tenantId, IEnumerable<string> permissionNames);
    Task RevokePermissionsAsync(Guid tenantId, IEnumerable<string> permissionNames);
    Task<List<string>> GetGrantedPermissionsAsync(Guid tenantId);
    Task<bool> IsGrantedAsync(Guid tenantId, string permissionName);
}
`

### 4. 领域服务：ITenantPermissionValidator

`csharp
public interface ITenantPermissionValidator
{
    // 验证权限列表是否都在租户授权范围内，不在则抛出异常
    Task ValidateAsync(Guid tenantId, IEnumerable<string> permissionNames);
}
`

### 5. TenantAppService 扩展

在现有 TenantAppService 基础上新增以下方法：

- SetActiveAsync(Guid id, bool isActive) - 启用/禁用租户
- ResetAdminPasswordAsync(Guid id, ResetTenantAdminPasswordDto input) - 重置租管密码
- GetAdminUsersAsync(Guid id) - 获取租管账号列表
- GetGrantedPermissionsAsync(Guid id) - 获取租户已授权权限列表
- UpdateGrantedPermissionsAsync(Guid id, UpdateTenantPermissionsDto input) - 更新租户授权权限

## Data Models

### 现有表（无需修改结构）

| 表名 | 说明 | 租户隔离 |
|------|------|---------|
| AbpUsers | 用户表 | TenantId |
| AbpRoles | 角色表 | TenantId |
| AbpPermissionGrants | 权限授予（角色/用户） | TenantId |
| AbpPermissionDefinitionRecords | 权限定义 | 无（平台级） |
| AbpPermissionGroups | 权限分组 | 无（平台级） |
| AbpTenants | 租户表 | 无 |

### 新增表

| 表名 | 说明 |
|------|------|
| CenseqTenantPermissionGrants | 租户授权范围（平台给租户开通的权限） |

### Tenant 表扩展

新增字段：IsActive (bit, NOT NULL, DEFAULT 1)

### 权限定义

平台管理权限组（Platform）：

- Platform.Tenants - 租户管理查看
- Platform.Tenants.Create - 创建租户
- Platform.Tenants.Update - 更新租户
- Platform.Tenants.Delete - 删除租户
- Platform.Tenants.ManagePermissions - 配置租户权限范围
- Platform.Tenants.ResetAdminPassword - 重置租管密码
- Platform.Tenants.SetActive - 启用/禁用租户
- Platform.Users - 平台用户管理查看
- Platform.Users.Create / Update / Delete / ResetPassword
- Platform.Roles - 平台角色管理查看
- Platform.Roles.Create / Update / Delete / ManagePermissions
- Platform.PermissionDefinitions - 权限定义管理查看
- Platform.PermissionDefinitions.Create / Update / Delete

租管权限组（TenantAdmin）：

- TenantAdmin.Users - 租户用户管理查看
- TenantAdmin.Users.Create / Update / Delete / ResetPassword
- TenantAdmin.Roles - 租户角色管理查看
- TenantAdmin.Roles.Create / Update / Delete / ManagePermissions

## API Design

### 租户管理 API

`
GET    /api/tenant-management/tenants                          租户列表（分页、筛选）
POST   /api/tenant-management/tenants                          创建租户
GET    /api/tenant-management/tenants/{id}                     获取租户详情
PUT    /api/tenant-management/tenants/{id}                     更新租户
DELETE /api/tenant-management/tenants/{id}                     删除租户
PUT    /api/tenant-management/tenants/{id}/active              启用/禁用租户
POST   /api/tenant-management/tenants/{id}/reset-admin-password 重置租管密码
GET    /api/tenant-management/tenants/{id}/admin-users         获取租管账号列表
GET    /api/tenant-management/tenants/{id}/permissions         获取租户授权权限
PUT    /api/tenant-management/tenants/{id}/permissions         更新租户授权权限
`

### 权限管理 API（扩展）

`
GET    /api/permission-management/permissions                  获取权限列表（支持租户范围过滤）
GET    /api/permission-management/permission-definitions       权限定义列表
POST   /api/permission-management/permission-definitions       创建权限定义
PUT    /api/permission-management/permission-definitions/{id}  更新权限定义
DELETE /api/permission-management/permission-definitions/{id}  删除权限定义
GET    /api/permission-management/permission-groups            权限分组列表
POST   /api/permission-management/permission-groups            创建权限分组
PUT    /api/permission-management/permission-groups/{id}       更新权限分组
DELETE /api/permission-management/permission-groups/{id}       删除权限分组
`

## Frontend Architecture

### 路由设计

`
/login                              登录页（统一入口）
/platform/                          平台管理（TenantId=null 账号可访问）
  dashboard                         平台概览
  tenants                           租户列表
  tenants/:id/permissions           租户权限配置
  users                             平台用户管理
  roles                             平台角色管理
  permission-definitions            权限定义管理
/tenant/                            租管后台（TenantId!=null 账号可访问）
  dashboard                         租户概览
  users                             租户用户管理
  roles                             租户角色管理（含权限分配）
/403                                无权限提示页
`

### 路由守卫逻辑

1. 未登录  跳转 /login
2. 访问 /platform/* 且 tenantId != null  跳转 /403
3. 访问 /tenant/* 且 tenantId == null  跳转 /403
4. 路由 meta.permission 不满足  跳转 /403

### 菜单动态控制

登录后根据 tenantId 决定菜单：
- tenantId == null  显示平台菜单（租户管理、平台用户、平台角色、权限定义）
- tenantId != null  显示租管菜单（租户用户、租户角色），并根据租户授权范围过滤

### 权限树组件（复用）

租户权限配置页和角色权限分配页共用 PermissionTree 组件：

Props:
- availablePermissions: PermissionGroup[] - 可选权限项（平台传全量，租管传租户授权范围）
- grantedPermissions: string[] - 已选中的权限项
- readonly?: boolean - 是否只读
- onChange?: (permissions: string[]) => void - 变更回调

行为：父权限取消勾选时自动取消所有子权限；子权限全部勾选时自动勾选父权限。

## Correctness Properties

### Property 1: 租户权限范围不可超越

对于任意租户 T 和权限 P，若 P 不在 TenantPermissionGrants(T) 中，则 PermissionGrants 中不存在 TenantId=T 且 Name=P 的记录。

验证方式：为角色分配权限时，后端 TenantPermissionValidator 验证，不在范围内则抛出 BusinessException。

### Property 2: 租户数据隔离

在租户上下文 T 中执行的任意查询，返回的所有记录满足 TenantId = T。

验证方式：ABP IDataFilter 自动过滤，单元测试验证跨租户查询返回空集合。

### Property 3: 租户禁用后无法登录

若 Tenant.IsActive = false，则该租户下所有用户的登录请求返回 401。

验证方式：登录流程中检查租户 IsActive 状态，集成测试验证禁用后登录失败。

### Property 4: 权限级联清理

平台移除租户权限 P 时，TenantPermissionGrants 和 PermissionGrants 中该租户的 P 记录均被删除。

验证方式：领域事件触发级联清理，集成测试验证移除后角色不再拥有该权限。

### Property 5: 至少保留一个租管账号

对于任意租户，其管理员角色下的用户数量 >= 1。

验证方式：删除用户前检查是否为最后一个管理员，若是则拒绝删除并返回错误。