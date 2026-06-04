using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenIddict.Abstractions;
using Volo.Abp.Data;
using Volo.Abp.Guids;
using Volo.Abp.Uow;

namespace Censeq.OpenIddict.Scopes;

/// <summary>
/// OpenIddict 作用域存储，适配 OpenIddict 存储契约。
/// </summary>
public class CenseqOpenIddictScopeStore : CenseqOpenIddictStoreBase<IOpenIddictScopeRepository>, IOpenIddictScopeStore<OpenIddictScopeModel>
{
    /// <summary>
    /// 初始化 CenseqOpenIddictScopeStore 实例。
    /// </summary>
    /// <param name="repository">仓储。</param>
    /// <param name="unitOfWorkManager">工作单元管理器。</param>
    /// <param name="guidGenerator">GUID生成器。</param>
    /// <param name="identifierConverter">dentifier转换器。</param>
    /// <param name="concurrencyExceptionHandler">并发异常处理器。</param>
    /// <param name="storeOptions">存储配置项。</param>
    public CenseqOpenIddictScopeStore(
        IOpenIddictScopeRepository repository,
        IUnitOfWorkManager unitOfWorkManager,
        IGuidGenerator guidGenerator,
        CenseqOpenIddictIdentifierConverter identifierConverter,
        IOpenIddictDbConcurrencyExceptionHandler concurrencyExceptionHandler,
        IOptions<CenseqOpenIddictStoreOptions> storeOptions)
        : base(repository, unitOfWorkManager, guidGenerator, identifierConverter, concurrencyExceptionHandler, storeOptions)
    {

    }

   /// <summary>
   /// 获取数据数量。
   /// </summary>
   /// <param name="cancellationToken">取消令牌。</param>
   /// <returns>数据数量。</returns>
   public virtual async ValueTask<long> CountAsync(CancellationToken cancellationToken)
   {
       return await Repository.GetCountAsync(cancellationToken);
   }

   /// <summary>
   /// 获取数据数量。
   /// </summary>
   /// <param name="query">查询。</param>
   /// <param name="cancellationToken">取消令牌。</param>
   /// <returns>数据数量。</returns>
   public virtual ValueTask<long> CountAsync<TResult>(Func<IQueryable<OpenIddictScopeModel>, IQueryable<TResult>> query, CancellationToken cancellationToken)
   {
       throw new NotSupportedException();
   }

   /// <summary>
   /// 创建数据。
   /// </summary>
   /// <param name="scope">OpenIddict 作用域。</param>
   /// <param name="cancellationToken">取消令牌。</param>
   /// <returns>创建后的数据。</returns>
   public virtual async ValueTask CreateAsync(OpenIddictScopeModel scope, CancellationToken cancellationToken)
   {
       Check.NotNull(scope, nameof(scope));

       await Repository.InsertAsync(scope.ToEntity(), autoSave: true, cancellationToken: cancellationToken);

       scope = (await Repository.FindAsync(scope.Id, cancellationToken: cancellationToken)).ToModel();
   }

   /// <summary>
   /// 删除数据。
   /// </summary>
   /// <param name="scope">OpenIddict 作用域。</param>
   /// <param name="cancellationToken">取消令牌。</param>
   /// <returns>表示异步操作的任务。</returns>
   public virtual async ValueTask DeleteAsync(OpenIddictScopeModel scope, CancellationToken cancellationToken)
   {
       Check.NotNull(scope, nameof(scope));

       try
       {
           await Repository.DeleteAsync(scope.Id, autoSave: true, cancellationToken: cancellationToken);
       }
       catch (AbpDbConcurrencyException e)
       {
           Logger.LogException(e);
           await ConcurrencyExceptionHandler.HandleAsync(e);
           throw new OpenIddictExceptions.ConcurrencyException(e.Message, e.InnerException);
       }
   }

   /// <summary>
   /// 根据标识查找数据。
   /// </summary>
   /// <param name="identifier">标识。</param>
   /// <param name="cancellationToken">取消令牌。</param>
   /// <returns>匹配的数据。</returns>
   public virtual async ValueTask<OpenIddictScopeModel> FindByIdAsync(string identifier, CancellationToken cancellationToken)
   {
       Check.NotNullOrEmpty(identifier, nameof(identifier));

       return (await Repository.FindByIdAsync(ConvertIdentifierFromString(identifier), cancellationToken)).ToModel();
   }

    /// <summary>
    /// 根据名称查找数据。
    /// </summary>
    /// <param name="name">name。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>匹配的数据。</returns>
    public virtual async ValueTask<OpenIddictScopeModel> FindByNameAsync(string name, CancellationToken cancellationToken)
    {
        Check.NotNullOrEmpty(name, nameof(name));

        return (await Repository.FindByNameAsync(name, cancellationToken)).ToModel();
    }

    /// <summary>
    /// 根据名称查找数据。
    /// </summary>
    /// <param name="names">名称。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>匹配的数据。</returns>
    public virtual async IAsyncEnumerable<OpenIddictScopeModel> FindByNamesAsync(ImmutableArray<string> names, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        Check.NotNull(names, nameof(names));

        foreach (var name in names)
        {
            Check.NotNullOrEmpty(name, nameof(name));
        }

        var scopes = await Repository.FindByNamesAsync(names.ToArray(), cancellationToken);
        foreach (var scope in scopes)
        {
            yield return scope.ToModel();
        }
    }

    /// <summary>
    /// 根据资源查找数据。
    /// </summary>
    /// <param name="resource">资源。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>匹配的数据。</returns>
    public virtual async IAsyncEnumerable<OpenIddictScopeModel> FindByResourceAsync(string resource, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        Check.NotNullOrEmpty(resource, nameof(resource));

        var scopes = await Repository.FindByResourceAsync(resource, cancellationToken);
        foreach (var scope in scopes)
        {
            var resources = await GetResourcesAsync(scope.ToModel(), cancellationToken);
            if (resources.Contains(resource, StringComparer.Ordinal))
            {
                yield return scope.ToModel();
            }
        }
    }

    /// <summary>
    /// 获取指定标识的数据。
    /// </summary>
    /// <param name="query">查询。</param>
    /// <param name="state">状态。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>查询结果。</returns>
    public virtual ValueTask<TResult> GetAsync<TState, TResult>(Func<IQueryable<OpenIddictScopeModel>, TState, IQueryable<TResult>> query, TState state, CancellationToken cancellationToken)
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// 获取描述。
    /// </summary>
    /// <param name="scope">OpenIddict 作用域。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>异步操作结果。</returns>
    public virtual  ValueTask<string> GetDescriptionAsync(OpenIddictScopeModel scope, CancellationToken cancellationToken)
    {
        Check.NotNull(scope, nameof(scope));

        return new ValueTask<string>(scope.Description);
    }

    /// <summary>
    /// 获取描述。
    /// </summary>
    /// <param name="scope">OpenIddict 作用域。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>异步操作结果。</returns>
    public virtual ValueTask<ImmutableDictionary<CultureInfo, string>> GetDescriptionsAsync(OpenIddictScopeModel scope, CancellationToken cancellationToken)
    {
        Check.NotNull(scope, nameof(scope));

        if (string.IsNullOrEmpty(scope.Descriptions))
        {
            return new ValueTask<ImmutableDictionary<CultureInfo, string>>(ImmutableDictionary.Create<CultureInfo, string>());
        }

        using (var document = JsonDocument.Parse(scope.Descriptions))
        {
            var builder = ImmutableDictionary.CreateBuilder<CultureInfo, string>();

            foreach (var property in document.RootElement.EnumerateObject())
            {
                var value = property.Value.GetString();
                if (string.IsNullOrEmpty(value))
                {
                    continue;
                }

                builder[CultureInfo.GetCultureInfo(property.Name)] = value;
            }

            return new ValueTask<ImmutableDictionary<CultureInfo, string>>(builder.ToImmutable());
        }
    }

    /// <summary>
    /// 获取显示名称。
    /// </summary>
    /// <param name="scope">OpenIddict 作用域。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>异步操作结果。</returns>
    public virtual ValueTask<string> GetDisplayNameAsync(OpenIddictScopeModel scope, CancellationToken cancellationToken)
    {
        Check.NotNull(scope, nameof(scope));

        return new ValueTask<string>(scope.DisplayName);
    }

    /// <summary>
    /// 获取显示名称。
    /// </summary>
    /// <param name="scope">OpenIddict 作用域。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>异步操作结果。</returns>
    public virtual ValueTask<ImmutableDictionary<CultureInfo, string>> GetDisplayNamesAsync(OpenIddictScopeModel scope, CancellationToken cancellationToken)
    {
        Check.NotNull(scope, nameof(scope));

        if (string.IsNullOrEmpty(scope.DisplayNames))
        {
            return new ValueTask<ImmutableDictionary<CultureInfo, string>>(ImmutableDictionary.Create<CultureInfo, string>());
        }

        using (var document = JsonDocument.Parse(scope.DisplayNames))
        {
            var builder = ImmutableDictionary.CreateBuilder<CultureInfo, string>();

            foreach (var property in document.RootElement.EnumerateObject())
            {
                var value = property.Value.GetString();
                if (string.IsNullOrEmpty(value))
                {
                    continue;
                }

                builder[CultureInfo.GetCultureInfo(property.Name)] = value;
            }

            return new ValueTask<ImmutableDictionary<CultureInfo, string>>(builder.ToImmutable());
        }
    }

    /// <summary>
    /// 获取标识。
    /// </summary>
    /// <param name="scope">OpenIddict 作用域。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>异步操作结果。</returns>
    public virtual ValueTask<string> GetIdAsync(OpenIddictScopeModel scope, CancellationToken cancellationToken)
    {
        Check.NotNull(scope, nameof(scope));

        return new ValueTask<string>(ConvertIdentifierToString(scope.Id));
    }

    /// <summary>
    /// 获取名称。
    /// </summary>
    /// <param name="scope">OpenIddict 作用域。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>异步操作结果。</returns>
    public virtual ValueTask<string> GetNameAsync(OpenIddictScopeModel scope, CancellationToken cancellationToken)
    {
        Check.NotNull(scope, nameof(scope));

        return new ValueTask<string>(scope.Name);
    }

    /// <summary>
    /// 获取属性。
    /// </summary>
    /// <param name="scope">OpenIddict 作用域。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>异步操作结果。</returns>
    public virtual ValueTask<ImmutableDictionary<string, JsonElement>> GetPropertiesAsync(OpenIddictScopeModel scope, CancellationToken cancellationToken)
    {
        Check.NotNull(scope, nameof(scope));

        if (string.IsNullOrEmpty(scope.Properties))
        {
            return new ValueTask<ImmutableDictionary<string, JsonElement>>(ImmutableDictionary.Create<string, JsonElement>());
        }

        using (var document = JsonDocument.Parse(scope.Properties))
        {
            var builder = ImmutableDictionary.CreateBuilder<string, JsonElement>();

            foreach (var property in document.RootElement.EnumerateObject())
            {
                builder[property.Name] = property.Value.Clone();
            }

            return new ValueTask<ImmutableDictionary<string, JsonElement>>(builder.ToImmutable());
        }
    }

    /// <summary>
    /// 获取资源。
    /// </summary>
    /// <param name="scope">OpenIddict 作用域。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>异步操作结果。</returns>
    public virtual ValueTask<ImmutableArray<string>> GetResourcesAsync(OpenIddictScopeModel scope, CancellationToken cancellationToken)
    {
        Check.NotNull(scope, nameof(scope));

        if (string.IsNullOrEmpty(scope.Resources))
        {
            return new ValueTask<ImmutableArray<string>>(ImmutableArray.Create<string>());
        }

        using (var document = JsonDocument.Parse(scope.Resources))
        {
            var builder = ImmutableArray.CreateBuilder<string>(document.RootElement.GetArrayLength());

            foreach (var element in document.RootElement.EnumerateArray())
            {
                var value = element.GetString();
                if (string.IsNullOrEmpty(value))
                {
                    continue;
                }

                builder.Add(value);
            }

            return new ValueTask<ImmutableArray<string>>(builder.ToImmutable());
        }
    }

    /// <summary>
    /// 异步创建实例。
    /// </summary>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>异步操作结果。</returns>
    public virtual ValueTask<OpenIddictScopeModel> InstantiateAsync(CancellationToken cancellationToken)
    {
        return new ValueTask<OpenIddictScopeModel>(new OpenIddictScopeModel
        {
            Id = GuidGenerator.Create()
        });
    }

    /// <summary>
    /// 列出数据。
    /// </summary>
    /// <param name="count">count。</param>
    /// <param name="offset">offset。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>异步操作结果。</returns>
    public virtual async IAsyncEnumerable<OpenIddictScopeModel> ListAsync(int? count, int? offset, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var scopes = await Repository.ListAsync(count, offset, cancellationToken);
        foreach (var scope in scopes)
        {
            yield return scope.ToModel();
        }
    }

    /// <summary>
    /// 列出数据。
    /// </summary>
    /// <param name="query">查询。</param>
    /// <param name="state">状态。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>异步操作结果。</returns>
    public IAsyncEnumerable<TResult> ListAsync<TState, TResult>(Func<IQueryable<OpenIddictScopeModel>, TState, IQueryable<TResult>> query, TState state, CancellationToken cancellationToken)
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// 设置描述。
    /// </summary>
    /// <param name="scope">OpenIddict 作用域。</param>
    /// <param name="description">描述。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>表示异步操作的任务。</returns>
    public virtual ValueTask SetDescriptionAsync(OpenIddictScopeModel scope, string description, CancellationToken cancellationToken)
    {
        Check.NotNull(scope, nameof(scope));

        scope.Description = description;

        return default;
    }

    /// <summary>
    /// 设置描述。
    /// </summary>
    /// <param name="scope">OpenIddict 作用域。</param>
    /// <param name="descriptions">descriptions。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>表示异步操作的任务。</returns>
    public virtual ValueTask SetDescriptionsAsync(OpenIddictScopeModel scope, ImmutableDictionary<CultureInfo, string> descriptions, CancellationToken cancellationToken)
    {
        Check.NotNull(scope, nameof(scope));

        if (descriptions is null || descriptions.IsEmpty)
        {
            scope.Descriptions = null;
            return default;
        }

        scope.Descriptions =WriteStream(writer =>
        {
            writer.WriteStartObject();
            foreach (var description in descriptions)
            {
                writer.WritePropertyName(description.Key.Name);
                writer.WriteStringValue(description.Value);
            }
            writer.WriteEndObject();
        });

        return default;
    }

    /// <summary>
    /// 设置显示名称。
    /// </summary>
    /// <param name="scope">OpenIddict 作用域。</param>
    /// <param name="name">name。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>表示异步操作的任务。</returns>
    public virtual ValueTask SetDisplayNameAsync(OpenIddictScopeModel scope, string name, CancellationToken cancellationToken)
    {
        Check.NotNull(scope, nameof(scope));

        scope.DisplayName = name;

        return default;
    }

    /// <summary>
    /// 设置显示名称。
    /// </summary>
    /// <param name="scope">OpenIddict 作用域。</param>
    /// <param name="names">名称。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>表示异步操作的任务。</returns>
    public virtual ValueTask SetDisplayNamesAsync(OpenIddictScopeModel scope, ImmutableDictionary<CultureInfo, string> names, CancellationToken cancellationToken)
    {
        Check.NotNull(scope, nameof(scope));

        if (names is null || names.IsEmpty)
        {
            scope.DisplayNames = null;
            return default;
        }

        scope.DisplayNames =WriteStream(writer =>
        {
            writer.WriteStartObject();
            foreach (var name in names)
            {
                writer.WritePropertyName(name.Key.Name);
                writer.WriteStringValue(name.Value);
            }
            writer.WriteEndObject();
        });

        return default;
    }

    /// <summary>
    /// 设置名称。
    /// </summary>
    /// <param name="scope">OpenIddict 作用域。</param>
    /// <param name="name">name。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>表示异步操作的任务。</returns>
    public virtual ValueTask SetNameAsync(OpenIddictScopeModel scope, string name, CancellationToken cancellationToken)
    {
        Check.NotNull(scope, nameof(scope));

        scope.Name = name;

        return default;
    }

    /// <summary>
    /// 设置属性。
    /// </summary>
    /// <param name="scope">OpenIddict 作用域。</param>
    /// <param name="properties">properties。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>表示异步操作的任务。</returns>
    public virtual ValueTask SetPropertiesAsync(OpenIddictScopeModel scope, ImmutableDictionary<string, JsonElement> properties, CancellationToken cancellationToken)
    {
        Check.NotNull(scope, nameof(scope));

        if (properties is null || properties.IsEmpty)
        {
            scope.Properties = null;
            return default;
        }

        scope.Properties =WriteStream(writer =>
        {
            writer.WriteStartObject();
            foreach (var property in properties)
            {
                writer.WritePropertyName(property.Key);
                property.Value.WriteTo(writer);
            }
            writer.WriteEndObject();
        });

        return default;
    }

    /// <summary>
    /// 设置资源。
    /// </summary>
    /// <param name="scope">OpenIddict 作用域。</param>
    /// <param name="resources">resources。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>表示异步操作的任务。</returns>
    public virtual ValueTask SetResourcesAsync(OpenIddictScopeModel scope, ImmutableArray<string> resources, CancellationToken cancellationToken)
    {
        Check.NotNull(scope, nameof(scope));

        if (resources.IsDefaultOrEmpty)
        {
            scope.Resources = null;
            return default;
        }

        scope.Resources = WriteStream(writer =>
        {
            writer.WriteStartArray();
            foreach (var resource in resources)
            {
                writer.WriteStringValue(resource);
            }
            writer.WriteEndArray();
        });

        return default;
    }

    /// <summary>
    /// 更新数据。
    /// </summary>
    /// <param name="scope">OpenIddict 作用域。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>更新后的数据。</returns>
    public virtual async ValueTask UpdateAsync(OpenIddictScopeModel scope, CancellationToken cancellationToken)
    {
        Check.NotNull(scope, nameof(scope));

        var entity = await Repository.GetAsync(scope.Id, cancellationToken: cancellationToken);

        try
        {
            await Repository.UpdateAsync(scope.ToEntity(entity), autoSave: true, cancellationToken: cancellationToken);
        }
        catch (AbpDbConcurrencyException e)
        {
            Logger.LogException(e);
            await ConcurrencyExceptionHandler.HandleAsync(e);
            throw new OpenIddictExceptions.ConcurrencyException(e.Message, e.InnerException);
        }

        scope = (await Repository.FindAsync(entity.Id, cancellationToken: cancellationToken)).ToModel();
    }
}
