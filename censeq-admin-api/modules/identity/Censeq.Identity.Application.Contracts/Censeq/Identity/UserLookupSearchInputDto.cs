using Volo.Abp.Application.Dtos;

namespace Censeq.Identity;

/// <summary>
/// 用户查找搜索输入参数数据传输对象
/// </summary>
public class UserLookupSearchInputDto : ExtensiblePagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }
}
