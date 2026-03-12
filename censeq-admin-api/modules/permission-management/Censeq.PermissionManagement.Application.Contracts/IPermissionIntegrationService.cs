using System.Collections.Generic;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Censeq.PermissionManagement.Integration;

/// <summary>
/// Ȩ�޼��ɷ���
/// </summary>
public interface IPermissionIntegrationService : IApplicationService
{
    /// <summary>
    /// �Ƿ���Ȩ
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    Task<ListResultDto<IsGrantedResponse>> IsGrantedAsync(List<IsGrantedRequest> input);
}
