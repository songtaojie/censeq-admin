namespace Censeq.OpenIddict.Authorizations;

/// <summary>
/// OpenIddict 授权扩展方法。
/// </summary>
public static class OpenIddictAuthorizationExtensions
{
    /// <summary>
    /// 转换为实体。
    /// </summary>
    /// <param name="model">模型。</param>
    /// <returns>操作结果。</returns>
    public static OpenIddictAuthorization ToEntity(this OpenIddictAuthorizationModel model)
    {
        Check.NotNull(model, nameof(model));

        var entity = new OpenIddictAuthorization(model.Id)
        {
            ApplicationId = model.ApplicationId,
            CreationDate = model.CreationDate,
            Properties = model.Properties,
            Scopes = model.Scopes,
            Status = model.Status,
            Subject = model.Subject,
            Type = model.Type
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
    public static OpenIddictAuthorization ToEntity(this OpenIddictAuthorizationModel model, OpenIddictAuthorization entity)
    {
        Check.NotNull(model, nameof(model));
        Check.NotNull(entity, nameof(entity));

        entity.ApplicationId = model.ApplicationId;
        entity.CreationDate = model.CreationDate;
        entity.Properties = model.Properties;
        entity.Scopes = model.Scopes;
        entity.Status = model.Status;
        entity.Subject = model.Subject;
        entity.Type = model.Type;

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
    public static OpenIddictAuthorizationModel ToModel(this OpenIddictAuthorization entity)
    {
        if(entity == null)
        {
            return null;
        }

        var model = new OpenIddictAuthorizationModel
        {
            Id = entity.Id,
            ApplicationId = entity.ApplicationId,
            CreationDate = entity.CreationDate,
            Properties = entity.Properties,
            Scopes = entity.Scopes,
            Status = entity.Status,
            Subject = entity.Subject,
            Type = entity.Type
        };

        foreach (var extraProperty in entity.ExtraProperties)
        {
            model.ExtraProperties.Remove(extraProperty.Key);
            model.ExtraProperties.Add(extraProperty.Key, extraProperty.Value);
        }

        return model;
    }
}
