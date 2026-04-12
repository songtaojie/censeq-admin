using System.Linq;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace Censeq.Admin.Menus;

public class MenuManager : DomainService
{
    private readonly IRepository<Menu, Guid> _menuRepository;

    public MenuManager(IRepository<Menu, Guid> menuRepository)
    {
        _menuRepository = menuRepository;
    }

    public virtual async Task ValidateAsync([NotNull] Menu menu, Guid? excludedId = null, CancellationToken cancellationToken = default)
    {
        await ValidateParentAsync(menu, cancellationToken);
        await ValidateNameAsync(menu, excludedId, cancellationToken);
        await ValidatePathAsync(menu, excludedId, cancellationToken);
        ValidateComponent(menu);
        ValidateButton(menu);
    }

    private async Task ValidateParentAsync(Menu menu, CancellationToken cancellationToken)
    {
        if (!menu.ParentId.HasValue)
        {
            return;
        }

        var parent = await _menuRepository.FindAsync(menu.ParentId.Value, cancellationToken: cancellationToken);
        if (parent == null)
        {
            throw new AbpException("The specified parent menu does not exist.");
        }

        if (parent.Type == MenuType.Button)
        {
            throw new AbpException("Button nodes can not contain child menus.");
        }

        if (parent.TenantId != menu.TenantId)
        {
            throw new AbpException("Menu parent must belong to the same tenant scope.");
        }
    }

    private async Task ValidateNameAsync(Menu menu, Guid? excludedId, CancellationToken cancellationToken)
    {
        var queryable = await _menuRepository.GetQueryableAsync();
        var exists = await AsyncExecuter.AnyAsync(
            queryable.Where(x => x.TenantId == menu.TenantId && x.ParentId == menu.ParentId && x.Name == menu.Name)
                .Where(x => !excludedId.HasValue || x.Id != excludedId.Value),
            cancellationToken);

        if (exists)
        {
            throw new AbpException("A menu with the same name already exists under the same parent.");
        }
    }

    private async Task ValidatePathAsync(Menu menu, Guid? excludedId, CancellationToken cancellationToken)
    {
        if (menu.Type == MenuType.Button || string.IsNullOrWhiteSpace(menu.Path))
        {
            return;
        }

        var queryable = await _menuRepository.GetQueryableAsync();
        var exists = await AsyncExecuter.AnyAsync(
            queryable.Where(x => x.TenantId == menu.TenantId && x.Path == menu.Path)
                .Where(x => !excludedId.HasValue || x.Id != excludedId.Value),
            cancellationToken);

        if (exists)
        {
            throw new AbpException("A menu with the same path already exists in the current tenant scope.");
        }
    }

    private static void ValidateComponent(Menu menu)
    {
        if (string.IsNullOrWhiteSpace(menu.Component))
        {
            return;
        }

        if (menu.Component.Contains("..", StringComparison.Ordinal) || menu.Component.Contains('\\'))
        {
            throw new AbpException("Component path can only use logical frontend module paths.");
        }

        if (menu.Component.EndsWith(".vue", StringComparison.OrdinalIgnoreCase) || menu.Component.EndsWith(".tsx", StringComparison.OrdinalIgnoreCase))
        {
            throw new AbpException("Component path should not contain file extensions.");
        }
    }

    private static void ValidateButton(Menu menu)
    {
        if (menu.Type != MenuType.Button)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(menu.ButtonCode))
        {
            throw new AbpException("Button menus must define a button code.");
        }
    }
}