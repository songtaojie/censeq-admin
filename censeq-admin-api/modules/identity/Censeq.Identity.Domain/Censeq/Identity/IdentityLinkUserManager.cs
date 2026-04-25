using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;
using Volo.Abp.MultiTenancy;

namespace Censeq.Identity;

/// <summary>
/// 身份关联用户管理器
/// </summary>
public class IdentityLinkUserManager : DomainService
{
    /// <summary>
    /// I身份关联用户仓储
    /// </summary>
    protected IIdentityLinkUserRepository IdentityLinkUserRepository { get; }

    /// <summary>
    /// 身份用户管理器
    /// </summary>
    protected IdentityUserManager UserManager { get; }

    /// <summary>
    /// new ICurrent租户
    /// </summary>
    protected new ICurrentTenant CurrentTenant { get; }

    public IdentityLinkUserManager(IIdentityLinkUserRepository identityLinkUserRepository, IdentityUserManager userManager, ICurrentTenant currentTenant)
    {
        IdentityLinkUserRepository = identityLinkUserRepository;
        UserManager = userManager;
        CurrentTenant = currentTenant;
    }

    /// <summary>
    /// Task<List<Identity关联User>>
    /// </summary>
    public async Task<List<IdentityLinkUser>> GetListAsync(IdentityLinkUserInfo linkUserInfo, bool includeIndirect = false, CancellationToken cancellationToken = default)
    {
        using (CurrentTenant.Change(null))
        {
            var users = await IdentityLinkUserRepository.GetListAsync(linkUserInfo, cancellationToken: cancellationToken);
            if (includeIndirect == false)
            {
                return users;
            }

            var userInfos = new List<IdentityLinkUserInfo>()
                {
                    linkUserInfo
                };

            var allUsers = new List<IdentityLinkUser>();
            allUsers.AddRange(users);

            do
            {
                var nextUsers = new List<IdentityLinkUserInfo>();
                foreach (var user in users)
                {
                    if (userInfos.Any(x => x.TenantId != user.SourceTenantId || x.UserId != user.SourceUserId))
                    {
                        nextUsers.Add(new IdentityLinkUserInfo(user.SourceUserId, user.SourceTenantId));
                    }

                    if (userInfos.Any(x => x.TenantId != user.TargetTenantId || x.UserId != user.TargetUserId))
                    {
                        nextUsers.Add(new IdentityLinkUserInfo(user.TargetUserId, user.TargetTenantId));
                    }
                }

                users = new List<IdentityLinkUser>();
                foreach (var next in nextUsers)
                {
                    users.AddRange(await IdentityLinkUserRepository.GetListAsync(next, userInfos, cancellationToken));
                }

                userInfos.AddRange(nextUsers);
                allUsers.AddRange(users);
            } while (users.Any());

            return allUsers;
        }
    }

    public virtual async Task LinkAsync(IdentityLinkUserInfo sourceLinkUser, IdentityLinkUserInfo targetLinkUser, CancellationToken cancellationToken = default)
    {
        using (CurrentTenant.Change(null))
        {
            if (sourceLinkUser.UserId == targetLinkUser.UserId && sourceLinkUser.TenantId == targetLinkUser.TenantId)
            {
                return;
            }

            if (await IsLinkedAsync(sourceLinkUser, targetLinkUser, cancellationToken: cancellationToken))
            {
                return;
            }

            var userLink = new IdentityLinkUser(
                GuidGenerator.Create(),
                sourceLinkUser,
                targetLinkUser);
            await IdentityLinkUserRepository.InsertAsync(userLink, true, cancellationToken);
        }
    }

    /// <summary>
    /// Task<bool>
    /// </summary>
    public virtual async Task<bool> IsLinkedAsync(IdentityLinkUserInfo sourceLinkUser, IdentityLinkUserInfo targetLinkUser, bool includeIndirect = false, CancellationToken cancellationToken = default)
    {
        using (CurrentTenant.Change(null))
        {
            if (includeIndirect)
            {
                return (await GetListAsync(sourceLinkUser, true, cancellationToken: cancellationToken))
                    .Any(x => x.SourceTenantId == targetLinkUser.TenantId && x.SourceUserId == targetLinkUser.UserId ||
                              x.TargetTenantId == targetLinkUser.TenantId && x.TargetUserId == targetLinkUser.UserId);
            }
            return await IdentityLinkUserRepository.FindAsync(sourceLinkUser, targetLinkUser, cancellationToken) != null;
        }
    }

    public virtual async Task UnlinkAsync(IdentityLinkUserInfo sourceLinkUser, IdentityLinkUserInfo targetLinkUser, CancellationToken cancellationToken = default)
    {
        using (CurrentTenant.Change(null))
        {
            if (!await IsLinkedAsync(sourceLinkUser, targetLinkUser, cancellationToken: cancellationToken))
            {
                return;
            }

            var linkedUser = await IdentityLinkUserRepository.FindAsync(sourceLinkUser, targetLinkUser, cancellationToken);
            if (linkedUser != null)
            {
                await IdentityLinkUserRepository.DeleteAsync(linkedUser, cancellationToken: cancellationToken);
            }
        }
    }

    /// <summary>
    /// Task<string>
    /// </summary>
    public virtual async Task<string> GenerateLinkTokenAsync(IdentityLinkUserInfo targetLinkUser, string tokenPurpose, CancellationToken cancellationToken = default)
    {
        using (CurrentTenant.Change(targetLinkUser.TenantId))
        {
            var user = await UserManager.GetByIdAsync(targetLinkUser.UserId);
            return await UserManager.GenerateUserTokenAsync(
                user,
                LinkUserTokenProviderConsts.LinkUserTokenProviderName,
                tokenPurpose);
        }
    }

    /// <summary>
    /// Task<bool>
    /// </summary>
    public virtual async Task<bool> VerifyLinkTokenAsync(IdentityLinkUserInfo targetLinkUser, string token, string tokenPurpose, CancellationToken cancellationToken = default)
    {
        using (CurrentTenant.Change(targetLinkUser.TenantId))
        {
            var user = await UserManager.GetByIdAsync(targetLinkUser.UserId);
            return await UserManager.VerifyUserTokenAsync(
                user,
                LinkUserTokenProviderConsts.LinkUserTokenProviderName,
                tokenPurpose,
                token);
        }
    }
}
