# 需求文档

## 简介

本功能为基于 ABP 框架的 Censeq.Admin 后台管理系统实现动态菜单管理能力。菜单数据持久化到数据库，支持多级树形结构，与 ABP 权限系统深度集成，实现按用户权限动态渲染菜单。后端提供完整的菜单 CRUD 管理接口，前端提供可视化菜单配置页面，支持多租户隔离。

## 词汇表

- **Menu（菜单）**：系统导航菜单项，存储于数据库，包含名称、路由、图标、排序、父级等属性。
- **MenuTree（菜单树）**：以树形结构组织的菜单集合，根节点的 ParentId 为 null。
- **MenuPermission（菜单权限）**：菜单项与 ABP 权限名称的关联关系，用于控制菜单可见性。
- **MenuAppService（菜单应用服务）**：负责菜单 CRUD 及菜单树查询的应用层服务。
- **MenuManager（菜单领域服务）**：负责菜单业务规则校验（如唯一性、循环引用检测）的领域服务。
- **MenuRepository（菜单仓储）**：负责菜单实体持久化操作的仓储接口。
- **DynamicMenuProvider（动态菜单提供者）**：前端运行时根据当前用户权限过滤并返回可见菜单树的服务。
- **Tenant（租户）**：多租户系统中的独立业务单元，每个租户拥有独立的菜单配置。
- **PermissionName（权限名称）**：ABP 权限系统中权限的唯一标识字符串，格式为 `GroupName.PermissionName`。
- **SortOrder（排序序号）**：同级菜单中用于控制显示顺序的整数值，值越小越靠前。

---

## 需求

### 需求 1：菜单实体与数据持久化

**用户故事：** 作为系统管理员，我希望菜单数据存储在数据库中，以便动态管理菜单而无需修改代码或重启服务。

#### 验收标准

1. THE Menu_Entity SHALL 包含以下字段：Id（Guid）、TenantId（Guid?）、ParentId（Guid?）、Name（string，菜单唯一标识）、DisplayName（string，显示名称）、Path（string?，前端路由路径）、Icon（string?，图标标识）、SortOrder（int，排序序号）、IsEnabled（bool，是否启用）、PermissionName（string?，关联权限名称）、Component（string?，前端组件路径）、IsExternal（bool，是否外链）、Redirect（string?，重定向路径）。
2. THE Menu_Entity SHALL 实现 `IMultiTenant` 接口，使每条菜单记录携带 TenantId。
3. THE Menu_Entity SHALL 实现 `IHasCreationTime`、`IHasModificationTime` 审计接口，记录创建和修改时间。
4. THE MenuRepository SHALL 将菜单数据持久化到数据库表 `censeq_menus`，使用 snake_case 命名规范。
5. WHEN 同一租户下创建菜单时，THE MenuManager SHALL 校验同级（相同 ParentId）菜单的 Name 唯一性，IF 存在重复 Name，THEN THE MenuManager SHALL 抛出业务异常。
6. WHEN 设置菜单的 ParentId 时，THE MenuManager SHALL 检测是否形成循环引用，IF 检测到循环引用，THEN THE MenuManager SHALL 抛出业务异常。
7. THE Menu_Entity SHALL 支持最多 5 层嵌套深度，IF 超过 5 层，THEN THE MenuManager SHALL 抛出业务异常。

---

### 需求 2：菜单 CRUD 管理接口

**用户故事：** 作为系统管理员，我希望通过 RESTful API 对菜单进行增删改查操作，以便管理系统的导航结构。

#### 验收标准

1. THE MenuAppService SHALL 提供 `CreateAsync(CreateMenuInput)` 方法，创建新菜单并返回 `MenuDto`。
2. THE MenuAppService SHALL 提供 `UpdateAsync(Guid id, UpdateMenuInput)` 方法，更新指定菜单并返回 `MenuDto`。
3. THE MenuAppService SHALL 提供 `DeleteAsync(Guid id)` 方法，删除指定菜单。
4. THE MenuAppService SHALL 提供 `GetAsync(Guid id)` 方法，返回指定菜单的 `MenuDto`。
5. THE MenuAppService SHALL 提供 `GetListAsync(GetMenuListInput)` 方法，支持按 ParentId、IsEnabled 过滤，返回分页菜单列表。
6. THE MenuAppService SHALL 提供 `GetTreeAsync()` 方法，返回当前租户完整菜单树（`List<MenuTreeNodeDto>`），树节点包含 Children 集合。
7. WHEN 删除含有子菜单的菜单项时，THE MenuAppService SHALL 级联删除所有子孙菜单，OR 拒绝删除并返回错误提示（由配置决定，默认拒绝）。
8. WHEN 调用菜单管理接口时，THE MenuAppService SHALL 要求调用方持有 `Admin.Menus.Default` 权限，IF 权限不足，THEN THE MenuAppService SHALL 返回 403 响应。
9. WHEN 调用创建或更新接口时，THE MenuAppService SHALL 对 DisplayName（最大长度 128）、Name（最大长度 64，仅允许字母、数字、连字符、下划线）、Path（最大长度 256）进行输入校验，IF 校验失败，THEN THE MenuAppService SHALL 返回包含字段错误信息的 400 响应。

---

### 需求 3：菜单权限集成

**用户故事：** 作为系统管理员，我希望为每个菜单项绑定一个 ABP 权限，以便根据用户权限控制菜单的可见性。

#### 验收标准

1. THE Menu_Entity SHALL 通过 PermissionName 字段存储与 ABP 权限系统关联的权限名称，PermissionName 为可选字段（null 表示无权限限制，所有已登录用户可见）。
2. WHEN 创建或更新菜单时，IF PermissionName 不为 null，THEN THE MenuAppService SHALL 验证该 PermissionName 在 ABP 权限定义中存在，IF 不存在，THEN THE MenuAppService SHALL 返回包含错误描述的 400 响应。
3. THE DynamicMenuProvider SHALL 提供 `GetCurrentUserMenuTreeAsync()` 方法，返回当前登录用户有权访问的菜单树。
4. WHEN 构建用户菜单树时，THE DynamicMenuProvider SHALL 过滤掉 IsEnabled 为 false 的菜单项。
5. WHEN 构建用户菜单树时，THE DynamicMenuProvider SHALL 过滤掉当前用户不具备对应 PermissionName 权限的菜单项。
6. WHEN 父菜单因权限被过滤时，THE DynamicMenuProvider SHALL 同时过滤其所有子孙菜单，即使子菜单的权限满足条件。
7. WHERE 菜单的 PermissionName 为 null，THE DynamicMenuProvider SHALL 对所有已认证用户显示该菜单项。
8. THE DynamicMenuProvider SHALL 对同一用户的菜单树查询结果进行缓存，缓存时间为 5 分钟，WHEN 菜单数据发生变更时，THE MenuAppService SHALL 主动清除相关缓存。

---

### 需求 4：多租户菜单隔离

**用户故事：** 作为 SaaS 平台运营者，我希望不同租户拥有独立的菜单配置，以便为不同客户提供差异化的导航体验。

#### 验收标准

1. THE MenuRepository SHALL 在所有查询中自动应用 TenantId 过滤，确保租户间数据完全隔离。
2. WHEN 宿主（Host）管理员访问菜单管理接口时，THE MenuAppService SHALL 仅操作 TenantId 为 null 的宿主菜单数据。
3. WHEN 租户管理员访问菜单管理接口时，THE MenuAppService SHALL 仅操作当前租户的菜单数据。
4. THE MenuAppService SHALL 提供 `GetTreeAsync()` 接口，WHILE 在租户上下文中调用时，THE MenuAppService SHALL 仅返回该租户的菜单树。
5. WHEN 宿主切换到租户上下文时，THE DynamicMenuProvider SHALL 返回对应租户的菜单树，而非宿主菜单树。

---

### 需求 5：前端菜单管理页面

**用户故事：** 作为系统管理员，我希望通过可视化界面管理菜单树，以便直观地配置系统导航结构。

#### 验收标准

1. THE MenuManagementPage SHALL 以树形表格展示当前租户的完整菜单结构，支持展开/折叠节点。
2. THE MenuManagementPage SHALL 提供新增菜单按钮，点击后弹出表单对话框，支持填写 DisplayName、Name、Path、Icon、SortOrder、PermissionName、Component、IsExternal、Redirect、IsEnabled 字段。
3. THE MenuManagementPage SHALL 提供编辑菜单按钮，点击后弹出预填充当前数据的表单对话框。
4. THE MenuManagementPage SHALL 提供删除菜单按钮，点击后弹出确认对话框，确认后调用删除接口。
5. THE MenuManagementPage SHALL 在 PermissionName 输入框中提供权限名称的下拉搜索选择，数据来源为 ABP 权限定义列表。
6. WHEN 表单提交失败时，THE MenuManagementPage SHALL 在对应字段下方显示后端返回的错误信息。
7. THE MenuManagementPage SHALL 支持通过拖拽调整同级菜单的 SortOrder，拖拽完成后自动调用批量更新排序接口。
8. THE MenuManagementPage SHALL 仅对持有 `Admin.Menus.Default` 权限的用户显示，前端路由守卫应拦截无权限访问。

---

### 需求 6：前端运行时动态菜单渲染

**用户故事：** 作为系统用户，我希望登录后看到的导航菜单仅包含我有权访问的菜单项，以便快速找到功能入口。

#### 验收标准

1. WHEN 用户登录成功后，THE Frontend_App SHALL 调用 `DynamicMenuProvider.GetCurrentUserMenuTreeAsync()` 接口获取当前用户的菜单树。
2. THE Frontend_App SHALL 将获取到的菜单树渲染为侧边栏导航，支持多级菜单的展开/折叠交互。
3. WHEN 菜单项的 IsExternal 为 true 时，THE Frontend_App SHALL 在新标签页中打开 Path 对应的外部链接。
4. WHEN 菜单项的 Redirect 不为 null 时，THE Frontend_App SHALL 在路由匹配到该菜单时自动重定向到 Redirect 路径。
5. WHEN 用户权限发生变更（如角色调整）后，THE Frontend_App SHALL 在下次页面刷新或重新登录时重新获取菜单树。
6. WHEN 获取菜单树接口返回空列表时，THE Frontend_App SHALL 显示"暂无可用菜单"提示，而非空白侧边栏。
7. THE Frontend_App SHALL 根据当前路由高亮对应的菜单项，WHEN 路由变化时，THE Frontend_App SHALL 自动更新高亮状态。

---

### 需求 7：菜单批量排序接口

**用户故事：** 作为系统管理员，我希望通过一次接口调用批量更新菜单排序，以便支持前端拖拽排序功能。

#### 验收标准

1. THE MenuAppService SHALL 提供 `SortAsync(List<MenuSortItemDto>)` 方法，接受包含菜单 Id 和新 SortOrder 的列表，批量更新排序。
2. WHEN 调用 `SortAsync` 时，THE MenuAppService SHALL 在单个事务中完成所有排序更新，IF 任意一条更新失败，THEN THE MenuAppService SHALL 回滚整个事务并返回错误。
3. WHEN 调用 `SortAsync` 时，THE MenuAppService SHALL 验证列表中所有菜单 Id 均属于当前租户，IF 存在跨租户 Id，THEN THE MenuAppService SHALL 返回 403 响应。
4. THE MenuAppService SHALL 要求调用 `SortAsync` 的用户持有 `Admin.Menus.Update` 权限。

---

### 需求 8：菜单权限定义注册

**用户故事：** 作为开发者，我希望菜单管理功能的操作权限在 ABP 权限系统中有明确定义，以便通过角色管理界面进行授权配置。

#### 验收标准

1. THE MenuPermissionDefinitionProvider SHALL 在 ABP 权限系统中注册以下权限：
   - `Admin.Menus`（权限组）
   - `Admin.Menus.Default`（查看菜单列表）
   - `Admin.Menus.Create`（创建菜单）
   - `Admin.Menus.Update`（更新菜单）
   - `Admin.Menus.Delete`（删除菜单）
2. THE MenuPermissionDefinitionProvider SHALL 将上述权限注册到 `Admin` 权限组下，与现有权限保持一致的命名规范。
3. THE MenuPermissionDefinitionProvider SHALL 为每个权限提供中文 DisplayName，以便在权限管理界面正确显示。
4. WHEN ABP 权限系统初始化时，THE MenuPermissionDefinitionProvider SHALL 自动注册上述权限，无需手动干预。
