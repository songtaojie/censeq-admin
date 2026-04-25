using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Censeq.Identity;

/// <summary>
/// 组织单元应用服务接口
/// </summary>
public interface IOrganizationUnitAppService : IApplicationService
{
    /// <summary>
    /// 根据标识获取组织单元
    /// </summary>
    Task<OrganizationUnitDto> GetAsync(Guid id);

    /// <summary>
    /// 获取所有组织单元列表
    /// </summary>
    Task<ListResultDto<OrganizationUnitDto>> GetAllListAsync();

    /// <summary>
    /// 创建组织单元
    /// </summary>
    Task<OrganizationUnitDto> CreateAsync(OrganizationUnitCreateDto input);

    /// <summary>
    /// 更新组织单元
    /// </summary>
    Task<OrganizationUnitDto> UpdateAsync(Guid id, OrganizationUnitUpdateDto input);

    /// <summary>
    /// 删除组织单元
    /// </summary>
    Task DeleteAsync(Guid id);
}
