using Censeq.TenantManagement.Entities;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Caching;
using Volo.Abp.Domain;
using Volo.Abp.Domain.Services;
using Volo.Abp.EventBus.Local;
using Volo.Abp.MultiTenancy;

namespace Censeq.TenantManagement;

public class TenantManager : DomainService, ITenantManager
{
    protected ITenantRepository TenantRepository { get; }
    protected ITenantNormalizer TenantNormalizer { get; }
    protected ILocalEventBus LocalEventBus { get; }

    public TenantManager(
        ITenantRepository tenantRepository,
        ITenantNormalizer tenantNormalizer,
        ILocalEventBus localEventBus)
    {
        TenantRepository = tenantRepository;
        TenantNormalizer = tenantNormalizer;
        LocalEventBus = localEventBus;
    }

    public virtual async Task<Tenant> CreateAsync(string name, string? code)
    {
        Check.NotNull(name, nameof(name));

        var normalizedName = TenantNormalizer.NormalizeName(name) ?? name;
        await ValidateNameAsync(normalizedName);

        var normalizedCode = Tenant.NormalizeCode(code);
        if (normalizedCode == null)
        {
            throw new BusinessException("Censeq.TenantManagement:TenantCodeRequired");
        }

        await ValidateCodeAsync(normalizedCode);
        return new Tenant(GuidGenerator.Create(), name, normalizedName, normalizedCode);
    }

    public virtual async Task ChangeNameAsync(Tenant tenant, string name)
    {
        Check.NotNull(tenant, nameof(tenant));
        Check.NotNull(name, nameof(name));

        var normalizedName = TenantNormalizer.NormalizeName(name) ?? name;

        await ValidateNameAsync(normalizedName, tenant.Id);
        await LocalEventBus.PublishAsync(new TenantChangedEvent(tenant.Id, tenant.NormalizedName));
        tenant.SetName(name);
        tenant.SetNormalizedName(normalizedName);
    }

    public virtual async Task ChangeCodeAsync(Tenant tenant, string? code)
    {
        Check.NotNull(tenant, nameof(tenant));

        var normalizedCode = Tenant.NormalizeCode(code);
        if (normalizedCode == tenant.Code)
        {
            return;
        }

        if (normalizedCode != null)
        {
            await ValidateCodeAsync(normalizedCode, tenant.Id);
        }

        tenant.SetCodePublic(normalizedCode);
    }

    protected virtual async Task ValidateCodeAsync(string normalizedCode, Guid? expectedTenantId = null)
    {
        Check.NotNullOrWhiteSpace(normalizedCode, nameof(normalizedCode));
        var existing = await TenantRepository.FindByCodeAsync(normalizedCode);
        if (existing != null && existing.Id != expectedTenantId)
        {
            throw new BusinessException("Censeq.TenantManagement:DuplicateTenantCode").WithData("Code", normalizedCode);
        }
    }

    protected virtual async Task ValidateNameAsync(string? normalizeName, Guid? expectedId = null)
    {
        Check.NotNullOrWhiteSpace(normalizeName, nameof(normalizeName));
        var tenant = await TenantRepository.FindByNameAsync(normalizeName!);
        if (tenant != null && tenant.Id != expectedId)
        {
            throw new BusinessException("Censeq.TenantManagement:DuplicateTenantName").WithData("Name", normalizeName);
        }
    }
}
