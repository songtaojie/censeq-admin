using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;

namespace Censeq.Identity.AspNetCore;

public class CenseqSecurityStampValidatorCallback
{
    /// <summary>
    /// Implements callback for SecurityStampValidator's OnRefreshingPrincipal event.
    /// https://github.com/IdentityServer/IdentityServer4/blob/main/src/AspNetIdentity/src/SecurityStampValidatorCallback.cs
    /// </summary>
    public class SecurityStampValidatorCallback
    {
        /// <summary>
        /// Maintains the claims captured at login time that are not being created by ASP.NET Identity.
        /// This is needed to preserve claims such as idp, auth_time, amr.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="refreshingPrincipalOptions">The CenseqRefreshingPrincipalOptions.</param>
        /// <returns></returns>
        public static Task UpdatePrincipal(SecurityStampRefreshingPrincipalContext context, CenseqRefreshingPrincipalOptions refreshingPrincipalOptions)
        {
            var newClaimTypes = context.NewPrincipal.Claims.Select(x => x.Type).ToArray();
            var currentClaimsToKeep = context.CurrentPrincipal.Claims.Where(x => !newClaimTypes.Contains(x.Type)).ToArray();

            var id = context.NewPrincipal.Identities.First();
            id.AddClaims(currentClaimsToKeep);

            if (refreshingPrincipalOptions.CurrentPrincipalKeepClaimTypes.Any())
            {
                foreach (var claimType in refreshingPrincipalOptions.CurrentPrincipalKeepClaimTypes)
                {
                    var sessionIdClaim = context.CurrentPrincipal.Claims.FirstOrDefault(x => x.Type == claimType);
                    if (sessionIdClaim != null)
                    {
                        id.AddOrReplace(sessionIdClaim);
                    }
                }
            }

            return Task.CompletedTask;
        }
    }
}
