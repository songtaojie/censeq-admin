using Volo.Abp.Application.Dtos;

namespace Censeq.Identity;

/// <summary>
/// 获取身份声明类型列表输入参数
/// </summary>
public class GetIdentityClaimTypesInput : PagedAndSortedResultRequestDto
{
    /// <summary>
    /// 过滤条件
    /// </summary>
    public string? Filter { get; set; }
}