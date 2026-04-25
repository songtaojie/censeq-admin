# Requirements Document

## Introduction

本文档定义了 Censeq 后台管理系统多租户管理功能的需求。系统基于 ABP Framework,采用共享数据库、共享表结构的多租户架构,通过 TenantId 字段实现租户隔离。系统需要支持平台管理员管理租户、配置租户权限范围,以及租户管理员管理本租户内的用户和角色。

## Glossary

- **Platform_Admin_System**: 平台管理后台系统,供平台账号(TenantId = null)使用
- **Tenant_Admin_System**: 租户管理后台系统,供租户账号(TenantId != null)使用
- **Tenant_Authorization_Manager**: 租户授权管理器,负责管理平台给租户开通的权限范围
- **Permission_Definition_Manager**: 权限定义管理器,负责管理系统中所有可用的权限项
- **Tenant_Manager**: 租户管理器,负责租户的增删改查操作
- **Identity_Manager**: 身份管理器,负责用户和角色的管理
- **Menu_Controller**: 菜单控制器,根据账号类型动态显示菜单
- **Permission_Validator**: 权限验证器,验证操作是否在授权范围内
- **Admin_User**: 管理员用户,包括平台管理员和租户管理员
- **Tenant_Scope**: 租户授权范围,平台授予租户的权限集合
- **Permission_Item**: 权限项,系统中定义的功能权限单元

## Requirements

### Requirement 1: 租户授权范围管理

**User Story:** 作为平台管理员,我希望能够为租户配置可用的权限范围,以便控制租户可以使用哪些系统功能

#### Acceptance Criteria

1. WHEN 平台管理员为租户配置权限范围时, THE Tenant_Authorization_Manager SHALL 保存租户与权限项的授权关系
2. WHEN 平台管理员查询租户授权范围时, THE Tenant_Authorization_Manager SHALL 返回该租户已授权的所有权限项列表
3. WHEN 平台管理员移除租户的某个权限时, THE Tenant_Authorization_Manager SHALL 删除该授权关系并验证租户内角色不再持有该权限
4. THE Tenant_Authorization_Manager SHALL 支持批量授权和批量移除权限操作
5. WHEN 租户被删除时, THE Tenant_Authorization_Manager SHALL 自动清理该租户的所有授权记录

### Requirement 2: 租户权限范围验证

**User Story:** 作为系统,我需要验证租户内的权限分配不超出平台授权范围,以确保权限控制的有效性

#### Acceptance Criteria

1. WHEN 租户管理员为角色分配权限时, THE Permission_Validator SHALL 验证该权限是否在租户授权范围内
2. IF 分配的权限不在租户授权范围内, THEN THE Permission_Validator SHALL 拒绝操作并返回错误信息
3. WHEN 平台管理员移除租户的某个权限时, THE Permission_Validator SHALL 自动移除租户内所有角色对该权限的授权
4. WHEN 租户用户执行需要权限的操作时, THE Permission_Validator SHALL 验证该权限在租户授权范围内且用户角色拥有该权限

### Requirement 3: 租户管理功能

**User Story:** 作为平台管理员,我需要管理租户的基本信息和状态,以便控制租户的生命周期

#### Acceptance Criteria

1. THE Tenant_Manager SHALL 支持创建租户并指定租户名称和编码
2. THE Tenant_Manager SHALL 支持更新租户的基本信息(名称、编码、状态)
3. THE Tenant_Manager SHALL 支持启用和禁用租户
4. WHEN 租户被禁用时, THE Tenant_Manager SHALL 阻止该租户的所有用户登录
5. THE Tenant_Manager SHALL 支持删除租户
6. WHEN 租户被删除时, THE Tenant_Manager SHALL 级联删除该租户的所有用户、角色和权限授权记录
7. THE Tenant_Manager SHALL 支持分页查询租户列表并按名称、编码、状态筛选


### Requirement 4: 租户管理员账号管理

**User Story:** 作为平台管理员,我需要管理租户的管理员账号,以便为租户提供初始访问权限和密码重置服务

#### Acceptance Criteria

1. WHEN 创建租户时, THE Tenant_Manager SHALL 自动创建该租户的默认管理员账号
2. THE Tenant_Manager SHALL 支持为租户创建额外的管理员账号
3. THE Tenant_Manager SHALL 支持重置租户管理员账号的密码
4. WHEN 重置密码时, THE Tenant_Manager SHALL 生成安全的临时密码并要求用户首次登录时修改
5. THE Tenant_Manager SHALL 支持查询租户的所有管理员账号列表

### Requirement 5: 用户管理的租户隔离

**User Story:** 作为管理员,我需要只看到和管理属于我管理范围内的用户,以确保数据隔离

#### Acceptance Criteria

1. WHEN 平台管理员访问用户管理页面时, THE Identity_Manager SHALL 只显示 TenantId 为 null 的平台用户
2. WHEN 租户管理员访问用户管理页面时, THE Identity_Manager SHALL 只显示当前租户(TenantId 匹配)的用户
3. THE Identity_Manager SHALL 在创建用户时自动设置正确的 TenantId 值
4. THE Identity_Manager SHALL 阻止跨租户的用户查询和修改操作
5. THE Identity_Manager SHALL 支持按用户名、邮箱、状态筛选用户并分页显示

### Requirement 6: 角色管理的租户隔离

**User Story:** 作为管理员,我需要只看到和管理属于我管理范围内的角色,以确保权限隔离

#### Acceptance Criteria

1. WHEN 平台管理员访问角色管理页面时, THE Identity_Manager SHALL 只显示 TenantId 为 null 的平台角色
2. WHEN 租户管理员访问角色管理页面时, THE Identity_Manager SHALL 只显示当前租户(TenantId 匹配)的角色
3. THE Identity_Manager SHALL 在创建角色时自动设置正确的 TenantId 值
4. THE Identity_Manager SHALL 阻止跨租户的角色查询和修改操作
5. THE Identity_Manager SHALL 支持为角色分配权限时只显示租户授权范围内的权限项

### Requirement 7: 多管理员支持

**User Story:** 作为租户,我需要能够设置多个管理员账号,以便团队协作管理租户

#### Acceptance Criteria

1. THE Identity_Manager SHALL 支持在租户内创建"管理员角色"
2. THE Identity_Manager SHALL 支持将多个用户分配到管理员角色
3. WHEN 用户被分配管理员角色时, THE Identity_Manager SHALL 授予该用户租户管理权限
4. THE Identity_Manager SHALL 支持移除用户的管理员角色
5. THE Identity_Manager SHALL 确保租户至少保留一个管理员账号

### Requirement 8: 权限定义管理

**User Story:** 作为平台管理员,我需要管理系统中所有可用的权限定义,以便控制系统功能的权限粒度

#### Acceptance Criteria

1. THE Permission_Definition_Manager SHALL 支持创建权限定义并指定权限名称、显示名称、所属分组
2. THE Permission_Definition_Manager SHALL 支持更新权限定义的显示名称和启用状态
3. THE Permission_Definition_Manager SHALL 支持删除权限定义
4. WHEN 权限定义被删除时, THE Permission_Definition_Manager SHALL 级联删除所有相关的授权记录
5. THE Permission_Definition_Manager SHALL 支持按分组查询权限定义
6. THE Permission_Definition_Manager SHALL 支持权限定义的层级结构(父子关系)
7. THE Permission_Definition_Manager SHALL 支持禁用权限定义而不删除历史授权记录

### Requirement 9: 权限分组管理

**User Story:** 作为平台管理员,我需要管理权限分组,以便组织和展示权限定义

#### Acceptance Criteria

1. THE Permission_Definition_Manager SHALL 支持创建权限分组并指定分组名称和显示名称
2. THE Permission_Definition_Manager SHALL 支持更新权限分组的显示名称
3. THE Permission_Definition_Manager SHALL 支持删除权限分组
4. WHEN 权限分组被删除时, THE Permission_Definition_Manager SHALL 验证该分组下没有权限定义
5. THE Permission_Definition_Manager SHALL 支持查询所有权限分组及其包含的权限定义数量

### Requirement 10: 菜单动态显示

**User Story:** 作为用户,我希望根据我的账号类型看到相应的菜单,以便快速访问我可用的功能

#### Acceptance Criteria

1. WHEN 平台账号(TenantId = null)登录时, THE Menu_Controller SHALL 显示平台管理菜单
2. WHEN 租户账号(TenantId != null)登录时, THE Menu_Controller SHALL 显示租户管理菜单
3. THE Menu_Controller SHALL 根据用户的权限过滤菜单项
4. WHEN 租户权限范围变更时, THE Menu_Controller SHALL 动态更新租户用户可见的菜单项
5. THE Menu_Controller SHALL 隐藏用户无权访问的菜单项

### Requirement 11: 路由权限控制

**User Story:** 作为系统,我需要在路由层面验证用户权限,以防止未授权访问

#### Acceptance Criteria

1. WHEN 用户访问需要权限的路由时, THE Permission_Validator SHALL 验证用户是否拥有所需权限
2. IF 用户无权访问, THEN THE Permission_Validator SHALL 重定向到无权限提示页面
3. WHEN 平台账号尝试访问租户功能路由时, THE Permission_Validator SHALL 拒绝访问
4. WHEN 租户账号尝试访问平台管理路由时, THE Permission_Validator SHALL 拒绝访问
5. THE Permission_Validator SHALL 在前端路由守卫中验证权限

### Requirement 12: 租户权限配置界面

**User Story:** 作为平台管理员,我需要一个直观的界面来配置租户的权限范围,以便高效管理租户授权

#### Acceptance Criteria

1. THE Platform_Admin_System SHALL 提供租户权限配置页面
2. THE Platform_Admin_System SHALL 以树形结构展示所有可用的权限定义(按分组和层级组织)
3. THE Platform_Admin_System SHALL 支持勾选/取消勾选权限项来授权/移除权限
4. THE Platform_Admin_System SHALL 显示租户当前已授权的权限项
5. WHEN 父权限被取消勾选时, THE Platform_Admin_System SHALL 自动取消勾选所有子权限
6. THE Platform_Admin_System SHALL 支持保存权限配置并实时生效

### Requirement 13: 平台用户管理界面

**User Story:** 作为平台管理员,我需要管理平台运营账号,以便为团队成员提供系统访问权限

#### Acceptance Criteria

1. THE Platform_Admin_System SHALL 提供平台用户管理页面
2. THE Platform_Admin_System SHALL 支持创建、编辑、删除平台用户
3. THE Platform_Admin_System SHALL 支持为平台用户分配平台角色
4. THE Platform_Admin_System SHALL 支持重置平台用户密码
5. THE Platform_Admin_System SHALL 支持启用/禁用平台用户
6. THE Platform_Admin_System SHALL 只显示和操作 TenantId 为 null 的用户

### Requirement 14: 租户用户管理界面

**User Story:** 作为租户管理员,我需要管理本租户的用户账号,以便为团队成员提供系统访问权限

#### Acceptance Criteria

1. THE Tenant_Admin_System SHALL 提供租户用户管理页面
2. THE Tenant_Admin_System SHALL 支持创建、编辑、删除租户用户
3. THE Tenant_Admin_System SHALL 支持为租户用户分配租户内的角色
4. THE Tenant_Admin_System SHALL 支持重置租户用户密码
5. THE Tenant_Admin_System SHALL 支持启用/禁用租户用户
6. THE Tenant_Admin_System SHALL 只显示和操作当前租户的用户

### Requirement 15: 租户角色权限分配界面

**User Story:** 作为租户管理员,我需要为角色分配权限,以便控制用户的功能访问范围

#### Acceptance Criteria

1. THE Tenant_Admin_System SHALL 提供角色权限分配页面
2. THE Tenant_Admin_System SHALL 以树形结构展示租户授权范围内的权限定义
3. THE Tenant_Admin_System SHALL 支持勾选/取消勾选权限项来为角色授权/移除权限
4. THE Tenant_Admin_System SHALL 显示角色当前已分配的权限项
5. WHEN 父权限被取消勾选时, THE Tenant_Admin_System SHALL 自动取消勾选所有子权限
6. THE Tenant_Admin_System SHALL 只显示租户授权范围内的权限项,不显示未授权的权限

### Requirement 16: 租户列表管理界面

**User Story:** 作为平台管理员,我需要查看和管理所有租户,以便监控和维护租户状态

#### Acceptance Criteria

1. THE Platform_Admin_System SHALL 提供租户列表页面
2. THE Platform_Admin_System SHALL 显示租户的名称、编码、状态、创建时间等信息
3. THE Platform_Admin_System SHALL 支持按名称、编码、状态筛选租户
4. THE Platform_Admin_System SHALL 支持分页显示租户列表
5. THE Platform_Admin_System SHALL 提供快捷操作按钮(编辑、配置权限、重置管理员密码、启用/禁用、删除)
6. THE Platform_Admin_System SHALL 在删除租户前显示确认对话框

### Requirement 17: 权限定义管理界面

**User Story:** 作为平台管理员,我需要管理系统的权限定义,以便维护系统的权限体系

#### Acceptance Criteria

1. THE Platform_Admin_System SHALL 提供权限定义管理页面
2. THE Platform_Admin_System SHALL 以树形结构展示权限定义(按分组和层级组织)
3. THE Platform_Admin_System SHALL 支持创建、编辑、删除权限定义
4. THE Platform_Admin_System SHALL 支持创建、编辑、删除权限分组
5. THE Platform_Admin_System SHALL 支持拖拽调整权限定义的层级关系
6. THE Platform_Admin_System SHALL 显示每个权限定义的使用情况(被多少租户授权、被多少角色使用)
7. THE Platform_Admin_System SHALL 在删除权限定义前显示影响范围并要求确认

### Requirement 18: 系统初始化数据

**User Story:** 作为系统,我需要在首次部署时初始化必要的数据,以便系统能够正常运行

#### Acceptance Criteria

1. WHEN 系统首次启动时, THE Identity_Manager SHALL 创建默认的平台管理员账号
2. WHEN 系统首次启动时, THE Permission_Definition_Manager SHALL 从代码中同步所有权限定义和分组
3. THE Identity_Manager SHALL 为默认平台管理员账号分配所有平台管理权限
4. THE Permission_Definition_Manager SHALL 支持在系统升级时自动同步新增的权限定义
5. THE Permission_Definition_Manager SHALL 保留用户自定义的权限显示名称不被系统同步覆盖
