# 实现任务列表

## 任务

- [ ] 1. 扩展 TenantConsts 常量定义
  - 在 `TenantConsts.cs` 中新增 6 个字段长度常量：`MaxHostLength`（256）、`MaxIconLength`（512）、`MaxCopyrightLength`（256）、`MaxIcpLicenseLength`（64）、`MaxIcpUrlLength`（512）、`MaxRemarksLength`（1024）
  - 文件路径：`censeq-admin-api/modules/tenant-management/Censeq.TenantManagement.Domain.Shared/Censeq/TenantManagement/TenantConsts.cs`
  - 需求：需求 1（验收标准 1-6）、需求 2（验收标准 1-6）

- [ ] 2. 扩展 Tenant 实体
  - 在 `Tenant.cs` 中新增 7 个属性：`Host`、`Icon`、`Copyright`、`IcpLicense`、`IcpUrl`、`Remarks`（可空字符串）、`MaxUserCount`（int，默认 0）
  - 新增对应的 setter 方法：`SetHost`、`SetIcon`、`SetCopyright`、`SetIcpLicense`、`SetIcpUrl`、`SetRemarks`、`SetMaxUserCount`，遵循现有 `SetName`/`SetCode` 模式，使用 `Check.Length` 做长度校验，`SetMaxUserCount` 校验值 >= 0
  - 构造函数中初始化 `MaxUserCount = 0`
  - 文件路径：`censeq-admin-api/modules/tenant-management/Censeq.TenantManagement.Domain/Entities/Tenant.cs`
  - 需求：需求 1（验收标准 1-9）、需求 2（验收标准 1-8）

- [ ] 3. 扩展 ITenantManager 接口并实现配额校验
  - 在 `ITenantManager.cs` 中新增 `Task CheckUserQuotaAsync(Guid tenantId)` 方法签名
  - 在 `TenantManager.cs` 中实现该方法：注入 `IIdentityUserRepository`，查询当前租户用户数，若 `MaxUserCount > 0` 且用户数 >= `MaxUserCount` 则抛出 `BusinessException("Censeq.TenantManagement:TenantUserQuotaExceeded")`，携带 `MaxUserCount` 和 `CurrentCount` 数据
  - 文件路径：`censeq-admin-api/modules/tenant-management/Censeq.TenantManagement.Domain/ITenantManager.cs`、`TenantManager.cs`
  - 需求：需求 4（验收标准 1-5）

- [ ] 4. 扩展 TenantCreateOrUpdateDtoBase 和 TenantDto
  - 在 `TenantCreateOrUpdateDtoBase.cs` 中新增 7 个属性，使用 `[DynamicStringLength]` 特性引用 `TenantConsts` 常量，`MaxUserCount` 使用 `[Range(0, int.MaxValue)]`
  - 在 `TenantDto.cs` 中新增相同的 7 个属性（无验证特性）
  - 文件路径：`censeq-admin-api/modules/tenant-management/Censeq.TenantManagement.Application.Contracts/TenantCreateOrUpdateDtoBase.cs`、`TenantDto.cs`
  - 需求：需求 3（验收标准 1-2）

- [ ] 5. 更新 TenantAppService 写入扩展字段
  - 在 `TenantAppService.CreateAsync` 中，`TenantManager.CreateAsync` 之后调用 7 个 setter 方法写入扩展字段
  - 在 `TenantAppService.UpdateAsync` 中，`ChangeCodeAsync` 之后同样调用 7 个 setter 方法
  - 文件路径：`censeq-admin-api/modules/tenant-management/Censeq.TenantManagement.Application/TenantAppService.cs`
  - 需求：需求 3（验收标准 5-7）

- [ ] 6. 更新 AutoMapper Profile
  - 在 `CenseqTenantManagementApplicationAutoMapperProfile.cs` 中，为 `Tenant → TenantDto` 映射新增 7 个 `ForMember` 配置
  - 文件路径：`censeq-admin-api/modules/tenant-management/Censeq.TenantManagement.Application/CenseqTenantManagementApplicationAutoMapperProfile.cs`
  - 需求：需求 3（验收标准 3-4）

- [ ] 7. 更新 EF Core 模型配置
  - 在 `CenseqTenantManagementDbContextModelCreatingExtensions.cs` 的 `Tenant` 配置块中追加 7 个字段的列约束：可空字段使用 `IsRequired(false).HasMaxLength(...)`，`MaxUserCount` 使用 `IsRequired().HasDefaultValue(0)`
  - 文件路径：`censeq-admin-api/modules/tenant-management/Censeq.TenantManagement.EntityFrameworkCore/EntityFrameworkCore/CenseqTenantManagementDbContextModelCreatingExtensions.cs`
  - 需求：需求 6（验收标准 1）

- [ ] 8. 生成数据库迁移
  - 在 `Censeq.Admin.EntityFrameworkCore` 项目中执行 EF Core CLI 命令生成迁移 `AddTenantExtendedFields`
  - 验证迁移文件中 `Up` 方法包含 7 个新列的 `AddColumn` 语句，可空字段默认 NULL，`MaxUserCount` 默认 0
  - 验证 `Down` 方法包含对应的 `DropColumn` 语句（支持回滚）
  - 需求：需求 6（验收标准 1-4）

- [ ] 9. 在用户创建流程中注入配额校验
  - 在 `IdentityUserAppService.cs` 中注入 `ITenantManager`，在 `CreateAsync` 方法创建用户逻辑之前，当 `CurrentTenant.Id` 有值时调用 `TenantManager.CheckUserQuotaAsync(CurrentTenant.Id.Value)`
  - 文件路径：`censeq-admin-api/modules/identity/Censeq.Identity.Application/Censeq/Identity/IdentityUserAppService.cs`
  - 需求：需求 4（验收标准 2-3、5）

- [ ] 10. 新增本地化错误文本
  - 在租户管理模块的本地化资源文件中新增错误码 `TenantUserQuotaExceeded` 的中英文文本
  - 中文示例：`租户用户数量已达到上限（最大 {MaxUserCount} 人，当前 {CurrentCount} 人）`
  - 需求：需求 4（验收标准 3）

- [ ] 11. 更新前端租户表单
  - 在租户创建/编辑表单组件中新增 7 个字段的输入控件：`Host`（a-input）、`Icon`（a-input）、`Copyright`（a-input）、`IcpLicense`（a-input）、`IcpUrl`（a-input，URL 提示）、`Remarks`（a-textarea）、`MaxUserCount`（a-input-number，min=0）
  - 表单提交时将扩展字段值包含在请求体中
  - 需求：需求 5（验收标准 1-2、4-7）

- [ ] 12. 更新前端租户列表页
  - 在租户列表表格中新增 `Host` 列
  - 需求：需求 5（验收标准 3）
