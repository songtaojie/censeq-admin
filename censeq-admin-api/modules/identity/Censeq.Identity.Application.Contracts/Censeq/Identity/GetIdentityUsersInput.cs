using Volo.Abp.Application.Dtos;

namespace Censeq.Identity;

/// <summary>
/// 获取身份用户列表输入参数
/// </summary>
public class GetIdentityUsersInput : ExtensiblePagedAndSortedResultRequestDto
{
    /// <summary>
    /// 过滤条件
    /// </summary>
    public string? Filter { get; set; }
}
