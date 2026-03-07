using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Censeq.Abp.Identity.Consts;
using Censeq.Abp.Identity.Dtos;
using Censeq.Abp.Identity.Managers;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.ObjectExtending;

namespace Censeq.Abp.Identity;
/// <summary>
/// ��֤��ɫӦ�÷���
/// </summary>
/// <remarks>
/// 
/// </remarks>
/// <param name="roleManager"></param>
/// <param name="roleRepository"></param>
/// <param name="abpLazyServiceProvider"></param>
[Authorize(IdentityPermissionConsts.Roles.Default)]
public class IdentityRoleAppService(
    IdentityRoleManager roleManager,
    IIdentityRoleRepository roleRepository,
    IAbpLazyServiceProvider abpLazyServiceProvider) : IdentityAppServiceBase(abpLazyServiceProvider), IIdentityRoleAppService
{
    /// <summary>
    /// ��ɫ������
    /// </summary>
    protected IdentityRoleManager RoleManager { get; } = roleManager;

    /// <summary>
    /// ��ɫ�洢
    /// </summary>
    protected IIdentityRoleRepository RoleRepository { get; } = roleRepository;

    /// <summary>
    /// ��ȡ��֤��ɫ
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public virtual async Task<IdentityRoleDto> GetAsync(Guid id)
    {
        return (await RoleManager.GetByIdAsync(id)).ToIdentityRoleDto();
    }

    /// <summary>
    /// ��ȡ���н�ɫ
    /// </summary>
    /// <returns></returns>
    public virtual async Task<ListResultDto<IdentityRoleDto>> GetAllListAsync()
    {
        var list = await RoleRepository.GetListAsync();
        return new ListResultDto<IdentityRoleDto>(list.ConvertAll(DtoExtensions.ToIdentityRoleDto));
    }

    /// <summary>
    /// ��ȡ��֤��ɫ�б�
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public virtual async Task<PagedResultDto<IdentityRoleDto>> GetListAsync(GetIdentityRolesInputDto input)
    {
        var list = await RoleRepository.GetListAsync(input.Sorting, input.MaxResultCount, input.SkipCount, input.Filter);
        var totalCount = await RoleRepository.GetCountAsync(input.Filter);

        return new PagedResultDto<IdentityRoleDto>( totalCount, list.ConvertAll(DtoExtensions.ToIdentityRoleDto));
    }

    /// <summary>
    /// ������֤��ɫ
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [Authorize(IdentityPermissionConsts.Roles.Create)]
    public virtual async Task<IdentityRoleDto> CreateAsync(IdentityRoleCreateDto input)
    {
        var role = new IdentityRole(GuidGenerator.Create(),input.Name,CurrentTenant.Id)
        {
            IsDefault = input.IsDefault,
            IsPublic = input.IsPublic
        };

        input.MapExtraPropertiesTo(role);

        (await RoleManager.CreateAsync(role)).CheckErrors();
        if(CurrentUnitOfWork != null) await CurrentUnitOfWork.SaveChangesAsync();

        return role.ToIdentityRoleDto();
    }

    /// <summary>
    /// ������֤��ɫ
    /// </summary>
    /// <param name="id"></param>
    /// <param name="input"></param>
    /// <returns></returns>
    [Authorize(IdentityPermissionConsts.Roles.Update)]
    public virtual async Task<IdentityRoleDto> UpdateAsync(Guid id, IdentityRoleUpdateDto input)
    {
        var role = await RoleManager.GetByIdAsync(id);

        role.SetConcurrencyStampIfNotNull(input.ConcurrencyStamp);

        (await RoleManager.SetRoleNameAsync(role, input.Name)).CheckErrors();

        role.IsDefault = input.IsDefault;
        role.IsPublic = input.IsPublic;

        input.MapExtraPropertiesTo(role);

        (await RoleManager.UpdateAsync(role)).CheckErrors();
        if (CurrentUnitOfWork != null) await CurrentUnitOfWork.SaveChangesAsync();

        return role.ToIdentityRoleDto();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [Authorize(IdentityPermissionConsts.Roles.Delete)]
    public virtual async Task DeleteAsync(Guid id)
    {
        var role = await RoleManager.FindByIdAsync(id.ToString());
        if (role == null) return;

        (await RoleManager.DeleteAsync(role)).CheckErrors();
    }
}
