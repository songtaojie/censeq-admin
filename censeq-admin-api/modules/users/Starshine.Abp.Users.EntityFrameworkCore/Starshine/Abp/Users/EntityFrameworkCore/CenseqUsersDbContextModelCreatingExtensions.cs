using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Censeq.Abp.Users.EntityFrameworkCore;

/// <summary>
/// 用户模型创建扩展
/// </summary>
public static class CenseqUsersDbContextModelCreatingExtensions
{
    /// <summary>
    /// 配置用户实体
    /// </summary>
    /// <typeparam name="TUser"></typeparam>
    /// <param name="b"></param>
    public static void ConfigureCenseqUser<TUser>(this EntityTypeBuilder<TUser> b)
        where TUser : class, IUser
    {
        b.Property(u => u.TenantId);
        b.Property(u => u.UserName).IsRequired().HasMaxLength(CenseqUserConsts.MaxUserNameLength);
        b.Property(u => u.Email).IsRequired().HasMaxLength(CenseqUserConsts.MaxEmailLength);
        b.Property(u => u.Name).HasMaxLength(CenseqUserConsts.MaxNameLength);
        b.Property(u => u.Surname).HasMaxLength(CenseqUserConsts.MaxSurnameLength);
        b.Property(u => u.EmailConfirmed).HasDefaultValue(false);
        b.Property(u => u.PhoneNumber).HasMaxLength(CenseqUserConsts.MaxPhoneNumberLength);
        b.Property(u => u.PhoneNumberConfirmed).HasDefaultValue(false);
        b.Property(u => u.IsActive);
    }
}
