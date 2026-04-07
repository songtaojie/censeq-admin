using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OpenIddict.Abstractions;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Guids;

namespace Censeq.OpenIddict.Applications;

public class OpenIddictApplicationAppService :
    CrudAppService<
        OpenIddictApplication,
        OpenIddictApplicationDto,
        Guid,
        GetOpenIddictApplicationsInput,
        OpenIddictApplicationCreateDto,
        OpenIddictApplicationUpdateDto>,
    IOpenIddictApplicationAppService
{
    protected IOpenIddictApplicationManager ApplicationManager { get; }
    protected IOpenIddictApplicationRepository ApplicationRepository { get; }

    public OpenIddictApplicationAppService(
        IOpenIddictApplicationRepository repository,
        IOpenIddictApplicationManager applicationManager)
        : base(repository)
    {
        ApplicationManager = applicationManager;
        ApplicationRepository = repository;

        CreatePolicyName = OpenIddictPermissions.Applications.Create;
        UpdatePolicyName = OpenIddictPermissions.Applications.Update;
        DeletePolicyName = OpenIddictPermissions.Applications.Delete;
        GetPolicyName = OpenIddictPermissions.Applications.Default;
        GetListPolicyName = OpenIddictPermissions.Applications.Default;
    }

    public override async Task<OpenIddictApplicationDto> GetAsync(Guid id)
    {
        var application = await ApplicationRepository.GetAsync(id);
            return MapToDto(application);
    }

    public override async Task<PagedResultDto<OpenIddictApplicationDto>> GetListAsync(GetOpenIddictApplicationsInput input)
    {
        await CheckGetListPolicyAsync();

        var count = await ApplicationRepository.GetCountAsync(input.Filter, input.ClientType);

        var list = await ApplicationRepository.GetListAsync(
            input.Sorting,
            input.MaxResultCount,
            input.SkipCount,
            input.Filter,
            input.ClientType);

        var dtos = new List<OpenIddictApplicationDto>();
        foreach (var item in list)
        {
                dtos.Add(MapToDto(item));
        }

        return new PagedResultDto<OpenIddictApplicationDto>(count, dtos);
    }

    public override async Task<OpenIddictApplicationDto> CreateAsync(OpenIddictApplicationCreateDto input)
    {
        await CheckCreatePolicyAsync();

        // 检查ClientId是否已存在
        var existing = await ApplicationManager.FindByClientIdAsync(input.ClientId);
        if (existing != null)
        {
            throw new UserFriendlyException($"客户端ID '{input.ClientId}' 已存在");
        }

        var descriptor = new CenseqApplicationDescriptor
        {
            ApplicationType = input.ApplicationType,
            ClientId = input.ClientId,
            ClientSecret = input.ClientSecret,
            ClientType = input.ClientType,
            ConsentType = input.ConsentType,
            DisplayName = input.DisplayName,
            ClientUri = input.ClientUri,
            LogoUri = input.LogoUri
        };

        // 添加回调地址
        foreach (var uri in input.RedirectUris.Where(u => !string.IsNullOrWhiteSpace(u)))
        {
            descriptor.RedirectUris.Add(new Uri(uri));
        }

        foreach (var uri in input.PostLogoutRedirectUris.Where(u => !string.IsNullOrWhiteSpace(u)))
        {
            descriptor.PostLogoutRedirectUris.Add(new Uri(uri));
        }

        // 添加权限
        foreach (var permission in input.Permissions.Where(p => !string.IsNullOrWhiteSpace(p)))
        {
            descriptor.Permissions.Add(permission);
        }

        // 添加要求
        foreach (var requirement in input.Requirements.Where(r => !string.IsNullOrWhiteSpace(r)))
        {
            descriptor.Requirements.Add(requirement);
        }

        await ApplicationManager.CreateAsync(descriptor);
        var application = await ApplicationRepository.FindByClientIdAsync(input.ClientId);

            return MapToDto(application);
    }

    public override async Task<OpenIddictApplicationDto> UpdateAsync(Guid id, OpenIddictApplicationUpdateDto input)
    {
        await CheckUpdatePolicyAsync();

        var application = await ApplicationRepository.GetAsync(id);

        var descriptor = new CenseqApplicationDescriptor();
        await ApplicationManager.PopulateAsync(descriptor, application);

        // 更新属性
        descriptor.DisplayName = input.DisplayName;
        descriptor.ConsentType = input.ConsentType;
        descriptor.ClientUri = input.ClientUri;
        descriptor.LogoUri = input.LogoUri;

        // 更新密钥（如果提供了）
        if (!string.IsNullOrEmpty(input.ClientSecret))
        {
            descriptor.ClientSecret = input.ClientSecret;
        }

        // 更新回调地址
        descriptor.RedirectUris.Clear();
        foreach (var uri in input.RedirectUris.Where(u => !string.IsNullOrWhiteSpace(u)))
        {
            descriptor.RedirectUris.Add(new Uri(uri));
        }

        descriptor.PostLogoutRedirectUris.Clear();
        foreach (var uri in input.PostLogoutRedirectUris.Where(u => !string.IsNullOrWhiteSpace(u)))
        {
            descriptor.PostLogoutRedirectUris.Add(new Uri(uri));
        }

        // 更新权限
        descriptor.Permissions.Clear();
        foreach (var permission in input.Permissions.Where(p => !string.IsNullOrWhiteSpace(p)))
        {
            descriptor.Permissions.Add(permission);
        }

        // 更新要求
        descriptor.Requirements.Clear();
        foreach (var requirement in input.Requirements.Where(r => !string.IsNullOrWhiteSpace(r)))
        {
            descriptor.Requirements.Add(requirement);
        }

        await ApplicationManager.UpdateAsync(application, descriptor);
        application = await ApplicationRepository.GetAsync(id);

            return MapToDto(application);
    }

    public override async Task DeleteAsync(Guid id)
    {
        await CheckDeletePolicyAsync();

        var application = await ApplicationRepository.GetAsync(id);
        await ApplicationManager.DeleteAsync(application);
    }

    public virtual async Task<string> GenerateClientSecretAsync(Guid id)
    {
        await CheckUpdatePolicyAsync();

        var application = await ApplicationRepository.GetAsync(id);

        // 生成随机密钥
        var secret = Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N");

        var descriptor = new CenseqApplicationDescriptor();
        await ApplicationManager.PopulateAsync(descriptor, application);
        descriptor.ClientSecret = secret;

        await ApplicationManager.UpdateAsync(application, descriptor);

        return secret;
    }

    public virtual async Task<bool> CheckClientIdExistsAsync(string clientId, Guid? excludeId = null)
    {
        var existing = await ApplicationRepository.FindByClientIdAsync(clientId);
        if (existing == null) return false;

        if (excludeId.HasValue && existing.Id == excludeId.Value) return false;

        return true;
    }

        protected virtual OpenIddictApplicationDto MapToDto(OpenIddictApplication application)
    {
        var dto = new OpenIddictApplicationDto
        {
            Id = application.Id,
            ClientId = application.ClientId,
            ClientType = application.ClientType,
            ApplicationType = application.ApplicationType,
            ConsentType = application.ConsentType,
            DisplayName = application.DisplayName,
            ClientUri = application.ClientUri,
            LogoUri = application.LogoUri,
            RedirectUris = string.IsNullOrEmpty(application.RedirectUris)
                ? new List<string>()
                : System.Text.Json.JsonSerializer.Deserialize<List<string>>(application.RedirectUris) ?? new List<string>(),
            PostLogoutRedirectUris = string.IsNullOrEmpty(application.PostLogoutRedirectUris)
                ? new List<string>()
                : System.Text.Json.JsonSerializer.Deserialize<List<string>>(application.PostLogoutRedirectUris) ?? new List<string>(),
            Permissions = string.IsNullOrEmpty(application.Permissions)
                ? new List<string>()
                : System.Text.Json.JsonSerializer.Deserialize<List<string>>(application.Permissions) ?? new List<string>(),
            Requirements = string.IsNullOrEmpty(application.Requirements)
                ? new List<string>()
                : System.Text.Json.JsonSerializer.Deserialize<List<string>>(application.Requirements) ?? new List<string>(),
            CreationTime = application.CreationTime,
            CreatorId = application.CreatorId,
            LastModificationTime = application.LastModificationTime,
            LastModifierId = application.LastModifierId
        };

        return dto;
    }
}
