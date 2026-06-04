using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Censeq.OpenIddict.Scopes;

/// <summary>
/// OpenIddict 作用域应用服务接口。
/// </summary>
public interface IOpenIddictScopeAppService :
    ICrudAppService<
        OpenIddictScopeDto,
        Guid,
        GetOpenIddictScopesInput,
        OpenIddictScopeCreateDto,
        OpenIddictScopeUpdateDto>
{
    /// <summary>
    /// 验证作用域名称是否已存在
    /// </summary>
    Task<bool> CheckNameExistsAsync(string name, Guid? excludeId = null);
}
