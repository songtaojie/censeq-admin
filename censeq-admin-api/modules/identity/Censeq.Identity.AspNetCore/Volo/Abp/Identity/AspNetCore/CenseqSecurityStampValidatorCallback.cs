using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;

namespace Censeq.Identity.AspNetCore;

/// <summary>
/// Censeq 安全戳验证器回调
/// </summary>
public class CenseqSecurityStampValidatorCallback
{
    /// <summary>
    /// 实现 SecurityStampValidator 的 OnRefreshingPrincipal 事件的回调
    /// https://github.com/IdentityServer/IdentityServer4/blob/main/src/AspNetIdentity/src/SecurityStampValidatorCallback.cs
    /// </summary>
    public class SecurityStampValidatorCallback
    {
        /// <summary>
        /// 保留登录时捕获的声明，这些声明不是由 ASP.NET Identity 创建的
        /// 需要保留 idp、auth_time、amr 等声明
        /// </summary>
        /// <param name="context">安全戳刷新主体上下文</param>
        /// <param name="refreshingPrincipalOptions">刷新主体选项</param>
        /// <returns>异步任务</returns>
        public static Task UpdatePrincipal(SecurityStampRefreshingPrincipalContext context, CenseqRefreshingPrincipalOptions refreshingPrincipalOptions)
        {
            if (context.NewPrincipal == null || context.CurrentPrincipal == null)
            {
                return Task.CompletedTask;
            }

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
