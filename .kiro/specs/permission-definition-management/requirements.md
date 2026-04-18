# 需求文档

## 简介

权限定义管理功能为系统管理员提供一个独立的管理页面，用于查看和维护权限组（PermissionGroupDefinitionRecord）及权限项（PermissionDefinitionRecord）的可编辑字段（如显示名称）。

权限定义由代码在应用启动时自动同步写入数据库，运行时不允许新增或删除。本功能仅支持对已存在记录的可维护字段进行编辑，以满足多语言显示名称定制、业务语义调整等运营需求。

本功能与现有的权限授权接口（`GET/PUT /api/permission-management/permissions`）完全独立，不影响授权逻辑。

---

## 词汇表

- **PermissionDefinitionManagement_API**：本功能新增的后端应用服务及 HTTP API，路由前缀为 `/api/permission-management/definition`
- **PermissionDefinitionManagement_Page**：本功能新增的前端独立管理页面，路由为 `/system/permission-definition`
- **PermissionGroupDefinitionRecord**：权限组定义记录实体，包含 `Name`（唯一标识，只读）和 `DisplayName`（可编辑显示名称，最大 256 字符）
- **PermissionDefinitionRecord**：权限项定义记录实体，包含 `GroupName`（所属组，只读）、`Name`（唯一标识，只读）、`ParentName`（父权限，只读）、`DisplayName`（可编辑显示名称，最大 256 字符）、`IsEnabled`（只读，由代码控制）等字段
- **管理员**：具有权限定义管理操作权限的系统用户

---

## 需求

### 需求 1：查看权限组列表

**用户故事：** 作为管理员，我希望查看所有权限组的列表，以便了解系统中已注册的权限分组。

#### 验收标准

1. THE PermissionDefinitionManagement_API SHALL 提供 `GET /api/permission-management/definition/groups` 接口，返回所有 PermissionGroupDefinitionRecord 的列表，每条记录包含 `id`、`name`、`displayName` 字段。
2. WHEN 管理员访问 PermissionDefinitionManagement_Page，THE PermissionDefinitionManagement_Page SHALL 调用权限组列表接口并以表格形式展示所有权限组，列包含"名称（Name）"和"显示名称（DisplayName）"。
3. WHILE 权限组列表数据加载中，THE PermissionDefinitionManagement_Page SHALL 显示加载状态指示器。
4. IF 权限组列表接口返回错误，THEN THE PermissionDefinitionManagement_Page SHALL 显示错误提示信息。

---

### 需求 2：查看权限组下的权限项列表

**用户故事：** 作为管理员，我希望查看某个权限组下的所有权限项，以便了解该组包含的具体权限。

#### 验收标准

1. THE PermissionDefinitionManagement_API SHALL 提供 `GET /api/permission-management/definition/groups/{groupName}/permissions` 接口，返回指定权限组下所有 PermissionDefinitionRecord 的列表，每条记录包含 `id`、`name`、`groupName`、`parentName`、`displayName`、`isEnabled` 字段。
2. WHEN 管理员在 PermissionDefinitionManagement_Page 选择某个权限组，THE PermissionDefinitionManagement_Page SHALL 调用对应权限项列表接口并以表格形式展示该组下的权限项，列包含"名称（Name）"、"显示名称（DisplayName）"、"父权限（ParentName）"和"是否启用（IsEnabled）"。
3. IF 指定的 `groupName` 不存在，THEN THE PermissionDefinitionManagement_API SHALL 返回 HTTP 404 状态码及描述性错误信息。
4. WHILE 权限项列表数据加载中，THE PermissionDefinitionManagement_Page SHALL 显示加载状态指示器。

---

### 需求 3：编辑权限组显示名称

**用户故事：** 作为管理员，我希望修改权限组的显示名称，以便将系统内部名称替换为更符合业务语义的描述。

#### 验收标准

1. THE PermissionDefinitionManagement_API SHALL 提供 `PUT /api/permission-management/definition/groups/{groupName}` 接口，接受包含 `displayName` 字段的请求体，并将对应 PermissionGroupDefinitionRecord 的 `DisplayName` 更新为提交值。
2. WHEN 管理员在 PermissionDefinitionManagement_Page 提交权限组编辑表单，THE PermissionDefinitionManagement_Page SHALL 调用编辑接口并在成功后刷新权限组列表。
3. IF 提交的 `displayName` 为空字符串或仅包含空白字符，THEN THE PermissionDefinitionManagement_API SHALL 返回 HTTP 400 状态码及描述性验证错误信息。
4. IF 提交的 `displayName` 超过 256 字符，THEN THE PermissionDefinitionManagement_API SHALL 返回 HTTP 400 状态码及描述性验证错误信息。
5. IF 指定的 `groupName` 不存在，THEN THE PermissionDefinitionManagement_API SHALL 返回 HTTP 404 状态码及描述性错误信息。
6. WHEN 权限组编辑成功，THE PermissionDefinitionManagement_API SHALL 返回更新后的 PermissionGroupDefinitionRecord 数据。

---

### 需求 4：编辑权限项显示名称

**用户故事：** 作为管理员，我希望修改权限项的显示名称，以便将系统内部权限名称替换为更符合业务语义的描述。

#### 验收标准

1. THE PermissionDefinitionManagement_API SHALL 提供 `PUT /api/permission-management/definition/permissions/{name}` 接口，接受包含 `displayName` 字段的请求体，并将对应 PermissionDefinitionRecord 的 `DisplayName` 更新为提交值。
2. WHEN 管理员在 PermissionDefinitionManagement_Page 提交权限项编辑表单，THE PermissionDefinitionManagement_Page SHALL 调用编辑接口并在成功后刷新当前权限组的权限项列表。
3. IF 提交的 `displayName` 为空字符串或仅包含空白字符，THEN THE PermissionDefinitionManagement_API SHALL 返回 HTTP 400 状态码及描述性验证错误信息。
4. IF 提交的 `displayName` 超过 256 字符，THEN THE PermissionDefinitionManagement_API SHALL 返回 HTTP 400 状态码及描述性验证错误信息。
5. IF 指定的 `name` 不存在，THEN THE PermissionDefinitionManagement_API SHALL 返回 HTTP 404 状态码及描述性错误信息。
6. WHEN 权限项编辑成功，THE PermissionDefinitionManagement_API SHALL 返回更新后的 PermissionDefinitionRecord 数据。

---

### 需求 5：只读字段保护

**用户故事：** 作为系统，我希望确保权限定义的结构性字段不被运行时修改，以便保持权限系统与代码定义的一致性。

#### 验收标准

1. THE PermissionDefinitionManagement_API SHALL 仅允许修改 PermissionGroupDefinitionRecord 的 `DisplayName` 字段，`Name` 字段不得通过本功能接口修改。
2. THE PermissionDefinitionManagement_API SHALL 仅允许修改 PermissionDefinitionRecord 的 `DisplayName` 字段，`GroupName`、`Name`、`ParentName`、`IsEnabled`、`MultiTenancySide`、`Providers`、`StateCheckers` 字段不得通过本功能接口修改。
3. THE PermissionDefinitionManagement_Page SHALL 以只读方式展示 `Name`、`GroupName`、`ParentName`、`IsEnabled` 等结构性字段，不提供对这些字段的编辑入口。

---

### 需求 6：访问权限控制

**用户故事：** 作为系统，我希望只有具备相应权限的管理员才能访问权限定义管理功能，以便保护系统配置安全。

#### 验收标准

1. THE PermissionDefinitionManagement_API SHALL 要求调用方具备 `PermissionManagement.DefinitionManagement` 策略权限，未授权请求返回 HTTP 403 状态码。
2. WHEN 未授权用户访问 PermissionDefinitionManagement_Page，THE PermissionDefinitionManagement_Page SHALL 拒绝访问并跳转至无权限提示页面。
