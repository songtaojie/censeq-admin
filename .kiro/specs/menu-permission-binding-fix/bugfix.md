# Bugfix 需求文档：菜单与权限绑定关系修复

## 简介

当前菜单管理系统中，菜单节点与权限的绑定关系存在设计缺陷。核心问题是：权限（Permission）在概念上应当归属于操作行为（即 Button 类型节点），而非页面容器（Directory/Menu 类型节点）。现有实现允许对所有类型的菜单节点绑定任意权限，且权限选择范围不受菜单所属业务域约束，导致配置混乱、可见性逻辑不清晰。

本次修复涉及三个相互关联的缺陷：
1. 维护菜单权限时可选到全部系统权限，缺乏业务域约束
2. Directory 和 Menu 类型节点不应允许绑定权限，权限应仅挂载在 Button 节点上
3. Directory/Menu 节点的可见性应由其子 Button 节点的权限授予情况推导，而非自身绑定权限

---

## Bug 分析

### Current Behavior（缺陷行为）

**1.1** WHEN 管理员为任意菜单节点（包括 Directory 和 Menu 类型）维护权限时，THEN 系统在权限选择器中展示所有权限组的全部权限，不受该菜单节点的 `PermissionGroups` 字段约束（当 `PermissionGroups` 为 null 时返回全量权限）

**1.2** WHEN 管理员创建或更新 Directory 类型节点时，THEN 系统允许为其设置 `AuthorizationMode` 为非 Anonymous 值并绑定权限名称，不做类型限制

**1.3** WHEN 管理员创建或更新 Menu 类型节点时，THEN 系统允许为其设置 `AuthorizationMode` 为非 Anonymous 值并绑定权限名称，不做类型限制

**1.4** WHEN 数据种子（`MenuDataSeedContributor`）初始化菜单数据时，THEN 系统为 `platformUser`（平台用户管理）等 Menu 类型节点直接绑定权限（如 `CenseqIdentity.Users`），而非通过其子 Button 节点承载权限

**1.5** WHEN 运行时构建用户菜单路由时，THEN 系统通过 Directory/Menu 节点自身的 `AuthorizationMode` 和绑定权限来决定该节点是否可见，而非通过子 Button 节点的权限授予情况推导

### Expected Behavior（期望行为）

**2.1** WHEN 管理员为 Button 类型节点维护权限时，THEN 系统 SHALL 仅展示与该节点所属菜单业务域相关的权限组（由父级 Menu 节点的 `PermissionGroups` 字段确定），当无法确定业务域时展示全量权限

**2.2** WHEN 管理员创建或更新 Directory 类型节点时，THEN 系统 SHALL 强制将 `AuthorizationMode` 设为 Anonymous，并拒绝为其绑定任何权限名称

**2.3** WHEN 管理员创建或更新 Menu 类型节点时，THEN 系统 SHALL 强制将 `AuthorizationMode` 设为 Anonymous，并拒绝为其绑定任何权限名称

**2.4** WHEN 管理员为 Button 类型节点绑定权限时，THEN 系统 SHALL 允许绑定权限，且权限选择范围由其父级 Menu 节点的 `PermissionGroups` 字段约束

**2.5** WHEN 运行时构建用户菜单路由时，THEN 系统 SHALL 根据 Directory/Menu 节点下是否存在至少一个已授权的 Button 子节点来决定该节点是否可见（即：有任意子 Button 被授权则父节点可见）

**2.6** WHEN 数据种子初始化菜单数据时，THEN 系统 SHALL 为每个需要权限控制的 Menu 节点（如平台用户管理）创建对应的 Button 子节点，并将权限绑定在 Button 节点上，Menu 节点本身设为 Anonymous

### Unchanged Behavior（回归防护）

**3.1** WHEN Button 类型节点被授权时，THEN 系统 SHALL CONTINUE TO 将其 `ButtonCode` 收集到 `authBtnList` 中返回给前端，用于前端按钮级权限控制

**3.2** WHEN Directory/Menu 节点下不存在任何子节点（或所有子节点均未授权）时，THEN 系统 SHALL CONTINUE TO 从路由结果中过滤掉该节点，不向前端返回空容器节点

**3.3** WHEN 菜单节点的 `AuthorizationMode` 为 Anonymous 时，THEN 系统 SHALL CONTINUE TO 对所有已认证用户显示该节点，不做权限检查

**3.4** WHEN 租户用户访问菜单时，THEN 系统 SHALL CONTINUE TO 仅加载 Tenant scope 的菜单数据，平台与租户菜单隔离不受影响

**3.5** WHEN 管理员调用 `GetPermissionGroupsAsync` 接口时，THEN 系统 SHALL CONTINUE TO 支持通过 `menuId` 参数查询已有节点的权限组范围，接口签名不变

**3.6** WHEN 管理员对菜单进行 CRUD 操作时，THEN 系统 SHALL CONTINUE TO 要求持有对应的 `CenseqAdmin.Menus.*` 权限，鉴权逻辑不受影响

---

## Bug Condition 推导

### Bug Condition 函数

```pascal
FUNCTION isBugCondition(node)
  INPUT: node of type Menu
  OUTPUT: boolean

  // 触发缺陷的条件：非 Button 类型节点绑定了权限，或权限选择不受业务域约束
  RETURN (node.Type = Directory OR node.Type = Menu)
         AND (node.AuthorizationMode ≠ Anonymous OR node.PermissionNames.Count > 0)
END FUNCTION
```

### Fix Checking 属性

```pascal
// Property: Fix Checking — 非 Button 节点不得绑定权限
FOR ALL node WHERE isBugCondition(node) DO
  result ← CreateOrUpdate'(node)
  ASSERT result.AuthorizationMode = Anonymous
  ASSERT result.PermissionNames.Count = 0
END FOR
```

### Preservation Checking 属性

```pascal
// Property: Preservation Checking — Button 节点权限绑定行为不变
FOR ALL node WHERE node.Type = Button DO
  ASSERT CreateOrUpdate'(node).PermissionNames = CreateOrUpdate(node).PermissionNames
END FOR

// Property: Preservation Checking — 运行时路由过滤逻辑对无子节点情况不变
FOR ALL node WHERE node.Type ≠ Button AND childRoutes(node).Count = 0 AND NOT granted(node) DO
  ASSERT BuildRoutes'(node) excludes node
END FOR
```
