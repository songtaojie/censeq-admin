using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Users;

namespace Censeq.Identity;

/// <summary>
/// 用户查找应用服务接口
/// </summary>
[Obsolete("Use IIdentityUserIntegrationService for module-to-module (or service-to-service) communication.")]
public interface IIdentityUserLookupAppService : IApplicationService
{
    /// <summary>
    /// 根据 ID 查找用户
    /// </summary>
    /// <param name="id">用户 ID</param>
    /// <returns>用户数据</returns>
    Task<UserData> FindByIdAsync(Guid id);

    /// <summary>
    /// 根据用户名查找用户
    /// </summary>
    /// <param name="userName">用户名</param>
    /// <returns>用户数据</returns>
    Task<UserData> FindByUserNameAsync(string userName);

    /// <summary>
    /// 搜索用户
    /// </summary>
    /// <param name="input">搜索输入参数</param>
    /// <returns>用户列表结果</returns>
    Task<ListResultDto<UserData>> SearchAsync(UserLookupSearchInputDto input);

    /// <summary>
    /// 获取用户数量
    /// </summary>
    /// <param name="input">查询输入参数</param>
    /// <returns>用户数量</returns>
    Task<long> GetCountAsync(UserLookupCountInputDto input);
}
