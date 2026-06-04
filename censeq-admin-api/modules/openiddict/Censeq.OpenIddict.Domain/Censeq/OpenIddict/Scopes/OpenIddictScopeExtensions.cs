namespace Censeq.OpenIddict.Scopes;

/// <summary>
/// OpenIddict 作用域扩展方法。
/// </summary>
public static class OpenIddictScopeExtensions
{
    /// <summary>
    /// 转换为实体。
    /// </summary>
    /// <param name="model">模型。</param>
    /// <returns>操作结果。</returns>
    public static OpenIddictScope ToEntity(this OpenIddictScopeModel model)
    {
        Check.NotNull(model, nameof(model));

        var entity = new OpenIddictScope(model.Id)
        {
            Description = model.Description,
            Descriptions = model.Descriptions,
            DisplayName = model.DisplayName,
            DisplayNames = model.DisplayNames,
            Name = model.Name,
            Properties = model.Properties,
            Resources = model.Resources
        };

        foreach (var extraProperty in model.ExtraProperties)
        {
            entity.ExtraProperties.Remove(extraProperty.Key);
            entity.ExtraProperties.Add(extraProperty.Key, extraProperty.Value);
        }

        return entity;
    }

    /// <summary>
    /// 转换为实体。
    /// </summary>
    /// <param name="model">模型。</param>
    /// <param name="entity">实体。</param>
    /// <returns>操作结果。</returns>
    public static OpenIddictScope ToEntity(this OpenIddictScopeModel model, OpenIddictScope entity)
    {
        Check.NotNull(model, nameof(model));
        Check.NotNull(entity, nameof(entity));

        entity.Description = model.Description;
        entity.Descriptions = model.Descriptions;
        entity.DisplayName = model.DisplayName;
        entity.DisplayNames = model.DisplayNames;
        entity.Name = model.Name;
        entity.Properties = model.Properties;
        entity.Resources = model.Resources;

        foreach (var extraProperty in model.ExtraProperties)
        {
            entity.ExtraProperties.Remove(extraProperty.Key);
            entity.ExtraProperties.Add(extraProperty.Key, extraProperty.Value);
        }

        return entity;
    }

    /// <summary>
    /// 转换为模型。
    /// </summary>
    /// <param name="entity">实体。</param>
    /// <returns>操作结果。</returns>
    public static OpenIddictScopeModel ToModel(this OpenIddictScope entity)
    {
        if(entity == null)
        {
            return null;
        }

        var model = new OpenIddictScopeModel
        {
            Id = entity.Id,
            Description = entity.Description,
            Descriptions = entity.Descriptions,
            DisplayName = entity.DisplayName,
            DisplayNames = entity.DisplayNames,
            Name = entity.Name,
            Properties = entity.Properties,
            Resources = entity.Resources
        };


        foreach (var extraProperty in entity.ExtraProperties)
        {
            model.ExtraProperties.Remove(extraProperty.Key);
            model.ExtraProperties.Add(extraProperty.Key, extraProperty.Value);
        }

        return model;
    }
}
