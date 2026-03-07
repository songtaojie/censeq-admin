using Censeq.Abp.Application.Dtos;
using Censeq.Abp.Application.Services;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace Censeq.Abp.PermissionManagement.Integration;

/// <summary>
/// Ȩ�޼��ɷ���
/// </summary>
[IntegrationService]
public class PermissionIntegrationService : ApplicationService, IPermissionIntegrationService
{
    /// <summary>
    /// Ȩ�޲�����
    /// </summary>
    protected IPermissionFinder PermissionFinder { get; }

    /// <summary>
    /// Ȩ�޼��ɷ���
    /// </summary>
    /// <param name="permissionFinder">Ȩ�޲�����</param>
    public PermissionIntegrationService(IPermissionFinder permissionFinder,IAbpLazyServiceProvider abpLazyServiceProvider):base(abpLazyServiceProvider)
    {
        PermissionFinder = permissionFinder;
    }

    /// <summary>
    /// ������
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public virtual async Task<ListResultDto<IsGrantedResponse>> IsGrantedAsync(List<IsGrantedRequest> input)
    {
        return new ListResultDto<IsGrantedResponse>(await PermissionFinder.IsGrantedAsync(input));
    }
}
