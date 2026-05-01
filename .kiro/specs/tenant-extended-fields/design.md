# 设计文档

## 概述

本文档描述租户表扩展字段功能的技术设计。在现有 `Tenant` 实体基础上新增 7 个字段：`Host`、`Icon`、`Copyright`、`IcpLicense`、`IcpUrl`、`Remarks`、`MaxUserCount`，并在领域层、应用层、数据库层和前端层完整实现相关处理逻辑。

## 架构

本功能遵循项目现有的 ABP Framework 分层架构，变更涉及以下层次：

```
Domain.Shared  →  常量定义（TenantConsts 新增字段长度常量）
Domain         →  实体扩展（Tenant 新增属性和 setter 方法）
               →  领域服务扩展（ITenantManager / TenantManager 新增配额校验）
Application.Contracts  →  DTO 扩展（TenantDto / TenantCreateOrUpdateDtoBase）
Application    →  应用服务扩展（TenantAppService 映射新字段）
               →  AutoMapper Profile 更新
EntityFrameworkCore    →  EF Core 模型配置（新增列约束）
               →  数据库迁移（Migration）
前端 (Vue)     →  租户创建/编辑表单新增字段控件
```

## 组件与接口

### 1. TenantConsts（Domain.Shared 层）

在 `TenantConsts.cs` 中新增扩展字段的长度常量：

```csharp
public static int MaxHostLength { get; set; } = 256;
public static int MaxIconLength { get; set; } = 512;
public static int MaxCopyrightLength { get; set; } = 256;
public static int MaxIcpLicenseLength { get; set; } = 64;
public static int MaxIcpUrlLength { get; set; } = 512;
public static int MaxRemarksLength { get; set; } = 1024;
```

`MaxUserCount` 为整数类型，无需长度常量。

---

### 2. Tenant 实体（Domain 层）

在 `Tenant.cs` 中新增属性和对应的 setter 方法：

```csharp
public virtual string? Host { get; protected set; }
public virtual string? Icon { get; protected set; }
public virtual string? Copyright { get; protected set; }
public virtual string? IcpLicense { get; protected set; }
public virtual string? IcpUrl { get; protected set; }
public virtual string? Remarks { get; protected set; }
public virtual int MaxUserCount { get; protected set; }
```

新增 setter 方法（遵循现有 `SetName` / `SetCode` 模式）：

```csharp
public virtual void SetHost(string? host)
public virtual void SetIcon(string? icon)
public virtual void SetCopyright(string? copyright)
public virtual void SetIcpLicense(string? icpLicense)
public virtual void SetIcpUrl(string? icpUrl)
public virtual void SetRemarks(string? remarks)
public virtual void SetMaxUserCount(int maxUserCount)
```

构造函数中初始化 `MaxUserCount = 0`，其余可空字段默认为 `null`。

---

### 3. ITenantManager 接口（Domain 层）

新增配额校验方法：

```csharp
/// <summary>
/// 校验当前租户用户数量是否未超过配额，超限时抛出 BusinessException
/// </summary>
Task CheckUserQuotaAsync(Guid tenantId);
```

---

### 4. TenantManager 实现（Domain 层）

实现 `CheckUserQuotaAsync`，依赖 `IIdentityUserRepository` 查询当前租户用户数：

```csharp
public virtual async Task CheckUserQuotaAsync(Guid tenantId)
{
    var tenant = await TenantRepository.GetAsync(tenantId);
    if (tenant.MaxUserCount <= 0) return; // 0 表示不限制

    var currentCount = await UserRepository.GetCountAsync(tenantId: tenantId);
    if (currentCount >= tenant.MaxUserCount)
    {
        throw new BusinessException("Censeq.TenantManagement:TenantUserQuotaExceeded")
            .WithData("MaxUserCount", tenant.MaxUserCount)
            .WithData("CurrentCount", currentCount);
    }
}
```

---

### 5. TenantCreateOrUpdateDtoBase（Application.Contracts 层）

新增扩展字段属性，使用 `DynamicStringLength` 特性引用常量：

```csharp
[DynamicStringLength(typeof(TenantConsts), nameof(TenantConsts.MaxHostLength))]
public string? Host { get; set; }

[DynamicStringLength(typeof(TenantConsts), nameof(TenantConsts.MaxIconLength))]
public string? Icon { get; set; }

[DynamicStringLength(typeof(TenantConsts), nameof(TenantConsts.MaxCopyrightLength))]
public string? Copyright { get; set; }

[DynamicStringLength(typeof(TenantConsts), nameof(TenantConsts.MaxIcpLicenseLength))]
public string? IcpLicense { get; set; }

[DynamicStringLength(typeof(TenantConsts), nameof(TenantConsts.MaxIcpUrlLength))]
public string? IcpUrl { get; set; }

[DynamicStringLength(typeof(TenantConsts), nameof(TenantConsts.MaxRemarksLength))]
public string? Remarks { get; set; }

[Range(0, int.MaxValue)]
public int MaxUserCount { get; set; } = 0;
```

---

### 6. TenantDto（Application.Contracts 层）

新增与 DTO 基类相同的扩展字段属性（只读，无验证特性）：

```csharp
public string? Host { get; set; }
public string? Icon { get; set; }
public string? Copyright { get; set; }
public string? IcpLicense { get; set; }
public string? IcpUrl { get; set; }
public string? Remarks { get; set; }
public int MaxUserCount { get; set; }
```

---

### 7. TenantAppService（Application 层）

**CreateAsync** — 在 `TenantManager.CreateAsync` 之后，调用实体 setter 写入扩展字段：

```csharp
tenant.SetHost(input.Host);
tenant.SetIcon(input.Icon);
tenant.SetCopyright(input.Copyright);
tenant.SetIcpLicense(input.IcpLicense);
tenant.SetIcpUrl(input.IcpUrl);
tenant.SetRemarks(input.Remarks);
tenant.SetMaxUserCount(input.MaxUserCount);
```

**UpdateAsync** — 在现有 `ChangeNameAsync` / `ChangeCodeAsync` 之后追加相同的 setter 调用。

---

### 8. AutoMapper Profile（Application 层）

更新 `CenseqTenantManagementApplicationAutoMapperProfile`，将新字段加入映射：

```csharp
CreateMap<Tenant, TenantDto>()
    .ForMember(d => d.IsActive, opts => opts.MapFrom(s => s.IsActive))
    .ForMember(d => d.Host, opts => opts.MapFrom(s => s.Host))
    .ForMember(d => d.Icon, opts => opts.MapFrom(s => s.Icon))
    .ForMember(d => d.Copyright, opts => opts.MapFrom(s => s.Copyright))
    .ForMember(d => d.IcpLicense, opts => opts.MapFrom(s => s.IcpLicense))
    .ForMember(d => d.IcpUrl, opts => opts.MapFrom(s => s.IcpUrl))
    .ForMember(d => d.Remarks, opts => opts.MapFrom(s => s.Remarks))
    .ForMember(d => d.MaxUserCount, opts => opts.MapFrom(s => s.MaxUserCount))
    .MapExtraProperties();
```

---

### 9. EF Core 模型配置（EntityFrameworkCore 层）

在 `CenseqTenantManagementDbContextModelCreatingExtensions.cs` 的 `Tenant` 实体配置块中追加：

```csharp
b.Property(t => t.Host).IsRequired(false).HasMaxLength(TenantConsts.MaxHostLength);
b.Property(t => t.Icon).IsRequired(false).HasMaxLength(TenantConsts.MaxIconLength);
b.Property(t => t.Copyright).IsRequired(false).HasMaxLength(TenantConsts.MaxCopyrightLength);
b.Property(t => t.IcpLicense).IsRequired(false).HasMaxLength(TenantConsts.MaxIcpLicenseLength);
b.Property(t => t.IcpUrl).IsRequired(false).HasMaxLength(TenantConsts.MaxIcpUrlLength);
b.Property(t => t.Remarks).IsRequired(false).HasMaxLength(TenantConsts.MaxRemarksLength);
b.Property(t => t.MaxUserCount).IsRequired().HasDefaultValue(0);
```

---

### 10. 数据库迁移

在 `Censeq.Admin.EntityFrameworkCore` 项目中通过 EF Core CLI 生成新迁移：

```bash
dotnet ef migrations add AddTenantExtendedFields \
  --project src/Censeq.Admin.EntityFrameworkCore \
  --startup-project src/Censeq.Admin.HttpApi.Host
```

迁移将生成 `ALTER TABLE` 语句，为现有记录的可空字段设置 `NULL` 默认值，`MaxUserCount` 设置默认值 `0`。

---

### 11. 用户配额拦截（Identity 层）

在 `IdentityUserAppService.CreateAsync` 方法中，于创建用户逻辑之前注入 `ITenantManager` 并调用配额校验：

```csharp
// 注入 ITenantRepository 用于获取当前租户配额
if (CurrentTenant.Id.HasValue)
{
    await TenantManager.CheckUserQuotaAsync(CurrentTenant.Id.Value);
}
```

错误码 `Censeq.TenantManagement:TenantUserQuotaExceeded` 需在本地化资源文件中添加对应文本。

---

### 12. 前端表单（Vue 前端）

在租户创建/编辑表单组件中新增以下控件：

| 字段 | 控件类型 | 说明 |
|------|----------|------|
| Host | `<a-input>` | 文本输入，placeholder 示例：`example.com` |
| Icon | `<a-input>` | 文本输入，存储图标 URL 或路径 |
| Copyright | `<a-input>` | 文本输入，版权声明 |
| IcpLicense | `<a-input>` | 文本输入，ICP 备案号 |
| IcpUrl | `<a-input>` | 文本输入，URL 格式提示 |
| Remarks | `<a-textarea>` | 多行文本，内部备注 |
| MaxUserCount | `<a-input-number>` | 数字输入，最小值 0，0 表示不限制 |

租户列表页在表格中新增 `Host` 列展示。

## 数据模型

### Tenant 表（AbpTenants）新增列

| 列名 | 类型 | 可空 | 默认值 | 最大长度 |
|------|------|------|--------|----------|
| Host | nvarchar | 是 | NULL | 256 |
| Icon | nvarchar | 是 | NULL | 512 |
| Copyright | nvarchar | 是 | NULL | 256 |
| IcpLicense | nvarchar | 是 | NULL | 64 |
| IcpUrl | nvarchar | 是 | NULL | 512 |
| Remarks | nvarchar | 是 | NULL | 1024 |
| MaxUserCount | int | 否 | 0 | — |

## 正确性属性

以下属性用于属性测试（Property-Based Testing）验证：

### P1：字段长度约束
对任意租户创建/更新请求，若任一字段超过最大长度，系统必须拒绝并返回验证错误，不得持久化数据。

### P2：MaxUserCount 非负约束
对任意 MaxUserCount 输入值，若值小于 0，系统必须拒绝并返回验证错误。

### P3：配额为 0 时不限制
对 MaxUserCount = 0 的租户，无论当前用户数量为多少，用户创建操作不得因配额原因被拒绝。

### P4：配额超限拒绝
对 MaxUserCount = N（N > 0）的租户，当当前用户数量 ≥ N 时，任何新建用户请求必须被拒绝并返回 `TenantUserQuotaExceeded` 错误。

### P5：配额未超限允许
对 MaxUserCount = N（N > 0）的租户，当当前用户数量 < N 时，新建用户请求不得因配额原因被拒绝。

### P6：扩展字段读写一致性
对任意合法的扩展字段值，经过创建或更新操作后，通过 GET 接口读取的值必须与写入值完全一致。
