using Censeq.Abp.Application.Dtos;
using Censeq.Abp.Application.Services;
using Volo.Abp;

namespace Censeq.Abp.PermissionManagement.Integration;

/// <summary>
/// Ȩ�޼��ɷ���
/// </summary>
[IntegrationService]
public interface IPermissionIntegrationService : IApplicationService
{
    /// <summary>
    /// �Ƿ���Ȩ
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    Task<ListResultDto<IsGrantedResponse>> IsGrantedAsync(List<IsGrantedRequest> input);
}
