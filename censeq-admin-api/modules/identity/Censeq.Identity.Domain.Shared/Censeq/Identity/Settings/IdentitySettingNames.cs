namespace Censeq.Identity.Settings;

/// <summary>
/// 身份模块设置名称常量
/// </summary>
public static class IdentitySettingNames
{
    private const string Prefix = "Abp.Identity";

    /// <summary>
    /// 密码相关设置
    /// </summary>
    public static class Password
    {
        private const string PasswordPrefix = Prefix + ".Password";

        /// <summary>
        /// 密码要求的最小长度
        /// </summary>
        public const string RequiredLength = PasswordPrefix + ".RequiredLength";

        /// <summary>
        /// 密码要求的唯一字符数
        /// </summary>
        public const string RequiredUniqueChars = PasswordPrefix + ".RequiredUniqueChars";

        /// <summary>
        /// 是否要求密码包含非字母数字字符
        /// </summary>
        public const string RequireNonAlphanumeric = PasswordPrefix + ".RequireNonAlphanumeric";

        /// <summary>
        /// 是否要求密码包含小写字母
        /// </summary>
        public const string RequireLowercase = PasswordPrefix + ".RequireLowercase";

        /// <summary>
        /// 是否要求密码包含大写字母
        /// </summary>
        public const string RequireUppercase = PasswordPrefix + ".RequireUppercase";

        /// <summary>
        /// 是否要求密码包含数字
        /// </summary>
        public const string RequireDigit = PasswordPrefix + ".RequireDigit";

        /// <summary>
        /// 是否强制用户定期更改密码
        /// </summary>
        public const string ForceUsersToPeriodicallyChangePassword = PasswordPrefix + ".ForceUsersToPeriodicallyChangePassword";

        /// <summary>
        /// 密码更改周期天数
        /// </summary>
        public const string PasswordChangePeriodDays = PasswordPrefix + ".PasswordChangePeriodDays";
    }

    /// <summary>
    /// 账户锁定相关设置
    /// </summary>
    public static class Lockout
    {
        private const string LockoutPrefix = Prefix + ".Lockout";

        /// <summary>
        /// 是否允许新用户被锁定
        /// </summary>
        public const string AllowedForNewUsers = LockoutPrefix + ".AllowedForNewUsers";

        /// <summary>
        /// 锁定持续时间
        /// </summary>
        public const string LockoutDuration = LockoutPrefix + ".LockoutDuration";

        /// <summary>
        /// 最大失败访问尝试次数
        /// </summary>
        public const string MaxFailedAccessAttempts = LockoutPrefix + ".MaxFailedAccessAttempts";
    }

    /// <summary>
    /// 登录相关设置
    /// </summary>
    public static class SignIn
    {
        private const string SignInPrefix = Prefix + ".SignIn";

        /// <summary>
        /// 是否要求确认邮箱
        /// </summary>
        public const string RequireConfirmedEmail = SignInPrefix + ".RequireConfirmedEmail";

        /// <summary>
        /// 是否启用电话号码确认
        /// </summary>
        public const string EnablePhoneNumberConfirmation = SignInPrefix + ".EnablePhoneNumberConfirmation";

        /// <summary>
        /// 是否要求确认电话号码
        /// </summary>
        public const string RequireConfirmedPhoneNumber = SignInPrefix + ".RequireConfirmedPhoneNumber";
    }

    /// <summary>
    /// 用户相关设置
    /// </summary>
    public static class User
    {
        private const string UserPrefix = Prefix + ".User";

        /// <summary>
        /// 是否允许更新用户名
        /// </summary>
        public const string IsUserNameUpdateEnabled = UserPrefix + ".IsUserNameUpdateEnabled";

        /// <summary>
        /// 是否允许更新邮箱
        /// </summary>
        public const string IsEmailUpdateEnabled = UserPrefix + ".IsEmailUpdateEnabled";
    }

    /// <summary>
    /// 组织单元相关设置
    /// </summary>
    public static class OrganizationUnit
    {
        private const string OrganizationUnitPrefix = Prefix + ".OrganizationUnit";

        /// <summary>
        /// 用户最大成员数
        /// </summary>
        public const string MaxUserMembershipCount = OrganizationUnitPrefix + ".MaxUserMembershipCount";
    }
}
