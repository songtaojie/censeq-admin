# 需求文档

## 简介

本文档定义了 Censeq 多租户管理系统中租户表扩展字段功能的需求。在现有租户实体（包含 Name、Code、IsActive 字段）的基础上，新增以下字段：域名（Host）、图标/Logo（Icon）、版权信息（Copyright）、ICP 备案号（IcpLicense）、ICP 地址（IcpUrl）、备注（Remarks）、最大允许用户数（MaxUserCount）。这些字段将贯穿领域实体、数据库迁移、应用层 DTO、API 接口及前端管理界面的完整实现。

## 词汇表

- **Tenant_Manager**：租户管理器，负责租户实体的增删改查及扩展字段的持久化
- **Tenant_Entity**：租户实体，对应数据库中的 AbpTenants 表（含扩展字段）
- **Tenant_Extended_Fields**：租户扩展字段集合，包括 Host、Icon、Copyright、IcpLicense、IcpUrl、Remarks、MaxUserCount
- **Platform_Admin_System**：平台管理后台，供平台账号（TenantId = null）使用
- **TenantDto**：租户数据传输对象，用于 API 响应
- **TenantCreateOrUpdateDtoBase**：租户创建/更新 DTO 基类，包含可写字段
- **Host**：租户绑定的域名（HTTP Host 头值），用于基于域名的租户解析路由及品牌标识，格式为合法域名字符串（可含端口，如 `example.com` 或 `example.com:8080`）
- **Icon**：租户图标/Logo 的存储路径或 URL，用于前端品牌展示
- **Copyright**：租户的版权声明文本，显示在前端页脚
- **IcpLicense**：ICP 备案号，中国大陆网站合规要求的备案编号
- **IcpUrl**：ICP 备案信息查询地址，指向工信部备案查询页面的 URL
- **Remarks**：平台管理员对租户的内部备注，不对租户用户展示
- **MaxUserCount**：租户允许创建的最大用户数量配额，0 表示不限制

## 需求

### 需求 1：租户实体扩展字段持久化

**用户故事：** 作为平台管理员，我希望租户记录能够存储域名、图标、版权、ICP 备案等扩展信息，以便在系统中统一管理租户的品牌和合规数据

#### 验收标准

1. THE Tenant_Entity SHALL 包含 Host 字段，类型为可空字符串，最大长度 256 字符
2. THE Tenant_Entity SHALL 包含 Icon 字段，类型为可空字符串，最大长度 512 字符
3. THE Tenant_Entity SHALL 包含 Copyright 字段，类型为可空字符串，最大长度 256 字符
4. THE Tenant_Entity SHALL 包含 IcpLicense 字段，类型为可空字符串，最大长度 64 字符
5. THE Tenant_Entity SHALL 包含 IcpUrl 字段，类型为可空字符串，最大长度 512 字符
6. THE Tenant_Entity SHALL 包含 Remarks 字段，类型为可空字符串，最大长度 1024 字符
7. THE Tenant_Entity SHALL 包含 MaxUserCount 字段，类型为非空整数，默认值为 0（表示不限制），最小值为 0
8. WHEN 租户被创建时，THE Tenant_Manager SHALL 将所有扩展字段的初始值设置为 null（MaxUserCount 设置为 0）
9. WHEN 租户扩展字段被更新时，THE Tenant_Manager SHALL 将变更持久化到数据库

### 需求 2：租户扩展字段的输入验证

**用户故事：** 作为系统，我需要对租户扩展字段的输入进行格式和长度验证，以确保数据质量和存储安全

#### 验收标准

1. WHEN Host 字段被提交时，THE Tenant_Manager SHALL 验证其长度不超过 256 字符
2. WHEN Icon 字段被提交时，THE Tenant_Manager SHALL 验证其长度不超过 512 字符
3. WHEN Copyright 字段被提交时，THE Tenant_Manager SHALL 验证其长度不超过 256 字符
4. WHEN IcpLicense 字段被提交时，THE Tenant_Manager SHALL 验证其长度不超过 64 字符
5. WHEN IcpUrl 字段被提交时，THE Tenant_Manager SHALL 验证其长度不超过 512 字符
6. WHEN Remarks 字段被提交时，THE Tenant_Manager SHALL 验证其长度不超过 1024 字符
7. WHEN MaxUserCount 字段被提交时，THE Tenant_Manager SHALL 验证其值大于或等于 0
8. IF MaxUserCount 字段值小于 0，THEN THE Tenant_Manager SHALL 拒绝操作并返回验证错误信息

### 需求 3：租户 API 扩展字段支持

**用户故事：** 作为平台管理员，我希望通过 API 能够读取和写入租户的扩展字段，以便前端界面和第三方集成可以使用这些数据

#### 验收标准

1. THE TenantDto SHALL 包含所有 Tenant_Extended_Fields 字段，供 GET 接口返回
2. THE TenantCreateOrUpdateDtoBase SHALL 包含所有 Tenant_Extended_Fields 字段，供创建和更新接口接收
3. WHEN 调用 GET /api/tenant-management/tenants/{id} 时，THE Tenant_Manager SHALL 在响应中返回所有扩展字段的当前值
4. WHEN 调用 GET /api/tenant-management/tenants 时，THE Tenant_Manager SHALL 在列表响应的每条记录中包含所有扩展字段
5. WHEN 调用 POST /api/tenant-management/tenants 时，THE Tenant_Manager SHALL 接受并持久化请求体中的扩展字段值
6. WHEN 调用 PUT /api/tenant-management/tenants/{id} 时，THE Tenant_Manager SHALL 接受并持久化请求体中的扩展字段变更
7. WHEN 扩展字段在请求体中未提供时，THE Tenant_Manager SHALL 将对应字段保持为 null（MaxUserCount 保持为 0）

### 需求 4：用户数量配额控制

**用户故事：** 作为平台管理员，我希望能够为每个租户设置最大用户数量上限，以便实现按配额的资源管控

#### 验收标准

1. WHEN MaxUserCount 为 0 时，THE Tenant_Manager SHALL 对该租户的用户创建数量不施加限制
2. WHEN MaxUserCount 大于 0 时，THE Tenant_Manager SHALL 在创建新用户前验证当前租户的用户数量未超过 MaxUserCount
3. IF 租户当前用户数量已达到 MaxUserCount，THEN THE Tenant_Manager SHALL 拒绝创建新用户并返回配额超限错误信息
4. WHEN 查询租户详情时，THE Tenant_Manager SHALL 返回 MaxUserCount 字段值
5. WHEN 平台管理员更新 MaxUserCount 时，THE Tenant_Manager SHALL 立即生效，后续用户创建操作使用新的配额值

### 需求 5：租户扩展字段的前端管理界面

**用户故事：** 作为平台管理员，我希望在租户创建和编辑表单中能够填写和修改扩展字段，以便通过界面管理租户的完整信息

#### 验收标准

1. THE Platform_Admin_System SHALL 在租户创建表单中展示所有 Tenant_Extended_Fields 的输入控件
2. THE Platform_Admin_System SHALL 在租户编辑表单中展示所有 Tenant_Extended_Fields 的当前值并允许修改
3. THE Platform_Admin_System SHALL 在租户列表页面的详情列中展示 Host 字段
4. WHEN 用户提交租户表单时，THE Platform_Admin_System SHALL 将所有扩展字段值包含在请求体中发送至 API
5. IF 表单验证失败（如字段超长），THEN THE Platform_Admin_System SHALL 在对应输入控件旁显示错误提示信息
6. THE Platform_Admin_System SHALL 对 MaxUserCount 字段使用数字输入控件，并限制输入值不小于 0
7. THE Platform_Admin_System SHALL 对 IcpUrl 字段提供 URL 格式的输入提示

### 需求 6：租户扩展字段的数据库迁移

**用户故事：** 作为开发人员，我需要通过数据库迁移脚本将扩展字段添加到现有租户表，以便在不丢失现有数据的情况下完成升级

#### 验收标准

1. THE Tenant_Manager SHALL 通过 EF Core 迁移将所有扩展字段添加到 AbpTenants 表
2. WHEN 迁移执行时，THE Tenant_Manager SHALL 为现有租户记录的所有可空扩展字段设置 NULL 默认值
3. WHEN 迁移执行时，THE Tenant_Manager SHALL 为现有租户记录的 MaxUserCount 字段设置默认值 0
4. THE Tenant_Manager SHALL 确保迁移可回滚，回滚后 AbpTenants 表恢复到迁移前的结构
