using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Censeq.Identity;

public interface IIdentityClaimTypeAppService
    : ICrudAppService<
        IdentityClaimTypeDto,
        Guid,
        GetIdentityClaimTypesInput,
        IdentityClaimTypeCreateDto,
        IdentityClaimTypeUpdateDto>
{
    Task<ListResultDto<IdentityClaimTypeDto>> GetAllListAsync();
}