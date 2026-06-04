using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Censeq.Identity;

namespace Censeq.Account;

/// <summary>
/// 个人资料应用服务接口。
/// </summary>
public interface IProfileAppService : IApplicationService
{
    /// <summary>
    /// 异步获取个人资料。
    /// </summary>
    /// <returns>个人资料。</returns>
    Task<ProfileDto> GetAsync();

    /// <summary>
    /// 异步更新个人资料。
    /// </summary>
    /// <param name="input">输入数据。</param>
    /// <returns>个人资料。</returns>
    Task<ProfileDto> UpdateAsync(UpdateProfileDto input);

    /// <summary>
    /// 异步修改密码。
    /// </summary>
    /// <param name="input">输入数据。</param>
    /// <returns>表示异步操作的任务。</returns>
    Task ChangePasswordAsync(ChangePasswordInput input);
}
