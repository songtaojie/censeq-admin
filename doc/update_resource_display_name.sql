-- =============================================================
-- 批量更新 censeq_localization_resource 表的 display_name 字段
-- 数据库: PostgreSQL (censeq_admin)
-- 使用方法: 在 pgAdmin / DBeaver / psql 中执行本脚本
-- 安全性: 所有 UPDATE 均加 WHERE (display_name IS NULL OR display_name = '')
--          不会覆盖已手动设置的值
-- =============================================================

-- 先查看当前表中所有资源（可选）
-- SELECT id, name, display_name, default_culture_name FROM censeq_localization_resource ORDER BY name;

-- ─────────────────────────────────────────────────────────────
-- 1. 项目自定义资源
-- ─────────────────────────────────────────────────────────────

-- 管理后台主资源
UPDATE censeq_localization_resource
SET display_name = '管理后台'
WHERE name = 'CenseqAdmin'
  AND (display_name IS NULL OR display_name = '');

-- 账户管理（登录/注册/找回密码等）
UPDATE censeq_localization_resource
SET display_name = '账户管理'
WHERE name = 'Account'
  AND (display_name IS NULL OR display_name = '');

-- 审计日志
UPDATE censeq_localization_resource
SET display_name = '审计日志'
WHERE name = 'CenseqAuditLogging'
  AND (display_name IS NULL OR display_name = '');

-- 功能管理
UPDATE censeq_localization_resource
SET display_name = '功能管理'
WHERE name = 'CenseqFeatureManagement'
  AND (display_name IS NULL OR display_name = '');

-- 身份认证（用户/角色/组织等）
UPDATE censeq_localization_resource
SET display_name = '身份认证'
WHERE name = 'Identity'
  AND (display_name IS NULL OR display_name = '');

-- 本地化管理
UPDATE censeq_localization_resource
SET display_name = '本地化管理'
WHERE name = 'CenseqLocalizationManagement'
  AND (display_name IS NULL OR display_name = '');

-- 授权认证（OpenIddict）
UPDATE censeq_localization_resource
SET display_name = '授权认证'
WHERE name = 'CenseqOpenIddict'
  AND (display_name IS NULL OR display_name = '');

-- 权限管理
UPDATE censeq_localization_resource
SET display_name = '权限管理'
WHERE name = 'CenseqPermissionManagement'
  AND (display_name IS NULL OR display_name = '');

-- 系统配置
UPDATE censeq_localization_resource
SET display_name = '系统配置'
WHERE name = 'CenseqSettingManagement'
  AND (display_name IS NULL OR display_name = '');

-- 租户管理
UPDATE censeq_localization_resource
SET display_name = '租户管理'
WHERE name = 'CenseqTenantManagement'
  AND (display_name IS NULL OR display_name = '');

-- ─────────────────────────────────────────────────────────────
-- 2. ABP 框架内置资源（由 Volo.Abp.* NuGet 包自动注册）
--    若不存在于表中，这些语句会执行 0 行，不影响结果
-- ─────────────────────────────────────────────────────────────

-- ABP 数据验证提示
UPDATE censeq_localization_resource
SET display_name = '数据验证'
WHERE name = 'AbpValidation'
  AND (display_name IS NULL OR display_name = '');

-- ABP 界面提示
UPDATE censeq_localization_resource
SET display_name = '界面提示'
WHERE name = 'AbpUi'
  AND (display_name IS NULL OR display_name = '');

-- ABP 本地化框架
UPDATE censeq_localization_resource
SET display_name = '本地化框架'
WHERE name = 'AbpLocalization'
  AND (display_name IS NULL OR display_name = '');

-- ABP 对象扩展
UPDATE censeq_localization_resource
SET display_name = '对象扩展'
WHERE name = 'AbpObjectExtending'
  AND (display_name IS NULL OR display_name = '');

-- ABP 全局异常处理
UPDATE censeq_localization_resource
SET display_name = '全局异常'
WHERE name = 'AbpExceptionHandling'
  AND (display_name IS NULL OR display_name = '');

-- ─────────────────────────────────────────────────────────────
-- 执行后验证（检查还有哪些资源的 display_name 仍为空）
-- ─────────────────────────────────────────────────────────────
SELECT name, display_name, default_culture_name
FROM censeq_localization_resource
ORDER BY name;
