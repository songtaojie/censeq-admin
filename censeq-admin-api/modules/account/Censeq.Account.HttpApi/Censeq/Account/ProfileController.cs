using System.Threading.Tasks;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;

namespace Censeq.Account;

/// <summary>
/// 个人资料控制器，提供对应的 HTTP API。
/// </summary>
[RemoteService(Name = AccountRemoteServiceConsts.RemoteServiceName)]
[Area(AccountRemoteServiceConsts.ModuleName)]
[ControllerName("Profile")]
[Route("/api/account/my-profile")]
public class ProfileController : AbpControllerBase, IProfileAppService
{
    /// <summary>
    /// 个人资料应用服务。
    /// </summary>
    protected IProfileAppService ProfileAppService { get; }

    /// <summary>
    /// 初始化 ProfileController 实例。
    /// </summary>
    /// <param name="profileAppService">个人资料应用服务。</param>
    public ProfileController(IProfileAppService profileAppService)
    {
        ProfileAppService = profileAppService;
    }

    /// <summary>
    /// 异步获取个人资料。
    /// </summary>
    /// <returns>个人资料。</returns>
    [HttpGet]
    public virtual Task<ProfileDto> GetAsync()
    {
        return ProfileAppService.GetAsync();
    }

    /// <summary>
    /// 异步更新个人资料。
    /// </summary>
    /// <param name="input">输入数据。</param>
    /// <returns>个人资料。</returns>
    [HttpPut]
    public virtual Task<ProfileDto> UpdateAsync(UpdateProfileDto input)
    {
        return ProfileAppService.UpdateAsync(input);
    }

    /// <summary>
    /// 异步修改密码。
    /// </summary>
    /// <param name="input">输入数据。</param>
    /// <returns>表示异步操作的任务。</returns>
    [HttpPost]
    [Route("change-password")]
    public virtual Task ChangePasswordAsync(ChangePasswordInput input)
    {
        return ProfileAppService.ChangePasswordAsync(input);
    }
}
