using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Volo.Abp;
using Censeq.Identity.Entities;
using Censeq.Identity.Localization;

namespace Censeq.Identity
{
    public class CenseqIdentityUserValidator : IUserValidator<IdentityUser>
    {
        /// <summary>
        /// IStringLocalizer<IdentityResource>
        /// </summary>
        protected IStringLocalizer<IdentityResource> Localizer { get; }

        public CenseqIdentityUserValidator(IStringLocalizer<IdentityResource> localizer)
        {
            Localizer = localizer;
        }

        /// <summary>
        /// Task<IdentityResult>
        /// </summary>
        public virtual async Task<IdentityResult> ValidateAsync(UserManager<IdentityUser> manager, IdentityUser user)
        {
            var describer = new IdentityErrorDescriber();

            Check.NotNull(manager, nameof(manager));
            Check.NotNull(user, nameof(user));

            var errors = new List<IdentityError>();

            var userName = await manager.GetUserNameAsync(user);
            if (userName == null)
            {
                errors.Add(describer.InvalidUserName(null));
            }
            else
            {
                var owner = await manager.FindByEmailAsync(userName);
                if (owner != null && !string.Equals(await manager.GetUserIdAsync(owner), await manager.GetUserIdAsync(user)))
                {
                    errors.Add(new IdentityError
                    {
                        Code = "InvalidUserName",
                        Description = Localizer["InvalidUserName", userName]
                    });
                }
            }
            
            var email = await manager.GetEmailAsync(user);
            if (email == null)
            {
                errors.Add(describer.InvalidEmail(null));
            }
            else
            {
                var owner = await manager.FindByNameAsync(email);
                if (owner != null && !string.Equals(await manager.GetUserIdAsync(owner), await manager.GetUserIdAsync(user)))
                {
                    errors.Add(new IdentityError
                    {
                        Code = "InvalidEmail",
                        Description = Localizer["Censeq.Identity:InvalidEmail", email]
                    });
                }
            }

            return errors.Count > 0 ? IdentityResult.Failed(errors.ToArray()) : IdentityResult.Success;
        }
    }
}
