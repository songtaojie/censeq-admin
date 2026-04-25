using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Censeq.Identity;

/// <summary>
/// 身份声明类型应用服务接口
/// </summary>
public interface IIdentityClaimTypeAppService
    : ICrudAppService<
        IdentityClaimTypeDto,
        Guid,
        GetIdentityClaimTypesInput,
        IdentityClaimTypeCreateDto,
        IdentityClaimTypeUpdateDto>
{
    /// <summary>
    /// 获取所有声明类型列表
    /// </summary>
    Task<ListResultDto<IdentityClaimTypeDto>> GetAllListAsync();
}