using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Volo.Abp.Users;

namespace Censeq.Identity.EntityFrameworkCore.Modeling
{
    internal static class CenseqUsersDbContextModelCreatingExtensions
    {
        public static void ConfigureCenseqUser<TUser>(this EntityTypeBuilder<TUser> b)
            where TUser : class, IUser
        {
            b.Property(u => u.TenantId);
            b.Property(u => u.UserName).IsRequired().HasMaxLength(AbpUserConsts.MaxUserNameLength);
            b.Property(u => u.Email).IsRequired().HasMaxLength(AbpUserConsts.MaxEmailLength);
            b.Property(u => u.Name).HasMaxLength(AbpUserConsts.MaxNameLength);
            b.Property(u => u.Surname).HasMaxLength(AbpUserConsts.MaxSurnameLength);
            b.Property(u => u.EmailConfirmed).HasDefaultValue(false);
            b.Property(u => u.PhoneNumber)
                .HasMaxLength(AbpUserConsts.MaxPhoneNumberLength)
                .IsRequired(false);
            b.Property(u => u.PhoneNumberConfirmed).HasDefaultValue(false);
            b.Property(u => u.IsActive);
        }
    }

}
