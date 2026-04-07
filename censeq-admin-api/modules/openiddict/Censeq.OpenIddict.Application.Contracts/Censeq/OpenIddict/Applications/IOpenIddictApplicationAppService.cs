using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Censeq.OpenIddict.Applications;

public interface IOpenIddictApplicationAppService :
    ICrudAppService<
        OpenIddictApplicationDto,
        Guid,
        GetOpenIddictApplicationsInput,
        OpenIddictApplicationCreateDto,
        OpenIddictApplicationUpdateDto>
{
    /// <summary>
    /// 生成新的客户端密钥
    /// </summary>
    Task<string> GenerateClientSecretAsync(Guid id);

    /// <summary>
    /// 验证客户端ID是否已存在
    /// </summary>
    Task<bool> CheckClientIdExistsAsync(string clientId, Guid? excludeId = null);
}
