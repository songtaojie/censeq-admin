using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
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
using Censeq.OpenIddict.Applications;
using Censeq.OpenIddict.Tokens;
using Volo.Abp.Uow;

namespace Censeq.OpenIddict.Authorizations;

/// <summary>
/// OpenIddict 授权存储，适配 OpenIddict 存储契约。
/// </summary>
public class CenseqOpenIddictAuthorizationStore : CenseqOpenIddictStoreBase<IOpenIddictAuthorizationRepository>, IOpenIddictAuthorizationStore<OpenIddictAuthorizationModel>
{
    /// <summary>
    /// OpenIddict 应用程序仓储。
    /// </summary>
    protected IOpenIddictApplicationRepository ApplicationRepository { get; }
    /// <summary>
    /// 令牌仓储。
    /// </summary>
    protected IOpenIddictTokenRepository TokenRepository { get; }

    /// <summary>
    /// 初始化 CenseqOpenIddictAuthorizationStore 实例。
    /// </summary>
    /// <param name="repository">仓储。</param>
    /// <param name="unitOfWorkManager">工作单元管理器。</param>
    /// <param name="guidGenerator">GUID生成器。</param>
    /// <param name="applicationRepository">应用程序仓储。</param>
    /// <param name="tokenRepository">令牌仓储。</param>
    /// <param name="identifierConverter">dentifier转换器。</param>
    /// <param name="concurrencyExceptionHandler">并发异常处理器。</param>
    /// <param name="storeOptions">存储配置项。</param>
    public CenseqOpenIddictAuthorizationStore(
        IOpenIddictAuthorizationRepository repository,
        IUnitOfWorkManager unitOfWorkManager,
        IGuidGenerator guidGenerator,
        IOpenIddictApplicationRepository applicationRepository,
        IOpenIddictTokenRepository tokenRepository,
        CenseqOpenIddictIdentifierConverter identifierConverter,
        IOpenIddictDbConcurrencyExceptionHandler concurrencyExceptionHandler,
        IOptions<CenseqOpenIddictStoreOptions> storeOptions)
        : base(repository, unitOfWorkManager, guidGenerator, identifierConverter, concurrencyExceptionHandler, storeOptions)
    {
        ApplicationRepository = applicationRepository;
        TokenRepository = tokenRepository;
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
    public virtual ValueTask<long> CountAsync<TResult>(Func<IQueryable<OpenIddictAuthorizationModel>, IQueryable<TResult>> query, CancellationToken cancellationToken)
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// 创建数据。
    /// </summary>
    /// <param name="authorization">OpenIddict 授权。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>创建后的数据。</returns>
    public virtual async ValueTask CreateAsync(OpenIddictAuthorizationModel authorization, CancellationToken cancellationToken)
    {
        Check.NotNull(authorization, nameof(authorization));

        await Repository.InsertAsync(authorization.ToEntity(), autoSave: true, cancellationToken: cancellationToken);

        authorization = (await Repository.FindAsync(authorization.Id, cancellationToken: cancellationToken)).ToModel();
    }

    /// <summary>
    /// 删除数据。
    /// </summary>
    /// <param name="authorization">OpenIddict 授权。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>表示异步操作的任务。</returns>
    public virtual async ValueTask DeleteAsync(OpenIddictAuthorizationModel authorization, CancellationToken cancellationToken)
    {
        Check.NotNull(authorization, nameof(authorization));

        try
        {
            using (var uow = UnitOfWorkManager.Begin(requiresNew: true, isTransactional: true, isolationLevel: StoreOptions.Value.DeleteIsolationLevel))
            {
                await TokenRepository.DeleteManyByAuthorizationIdAsync(authorization.Id, cancellationToken: cancellationToken);

                await Repository.DeleteAsync(authorization.Id, cancellationToken: cancellationToken);

                await uow.CompleteAsync(cancellationToken);
            }
        }
        catch (AbpDbConcurrencyException e)
        {
            Logger.LogException(e);
            await ConcurrencyExceptionHandler.HandleAsync(e);
            throw new OpenIddictExceptions.ConcurrencyException(e.Message, e.InnerException);
        }
    }

    /// <summary>
    /// 异步查找数据。
    /// </summary>
    /// <param name="subject">subject。</param>
    /// <param name="client">客户端标识。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>匹配的数据。</returns>
    public virtual async IAsyncEnumerable<OpenIddictAuthorizationModel> FindAsync(string subject, string client, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        Check.NotNullOrEmpty(subject, nameof(subject));
        Check.NotNullOrEmpty(client, nameof(client));

        var authorizations = await Repository.FindAsync(subject, ConvertIdentifierFromString(client), cancellationToken);
        foreach (var authorization in authorizations)
        {
            yield return authorization.ToModel();
        }
    }

    /// <summary>
    /// 异步查找数据。
    /// </summary>
    /// <param name="subject">subject。</param>
    /// <param name="client">客户端标识。</param>
    /// <param name="status">status。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>匹配的数据。</returns>
    public virtual async IAsyncEnumerable<OpenIddictAuthorizationModel> FindAsync(string subject, string client, string status, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        Check.NotNullOrEmpty(subject, nameof(subject));
        Check.NotNullOrEmpty(client, nameof(client));
        Check.NotNullOrEmpty(status, nameof(status));

        var authorizations = await Repository.FindAsync(subject, ConvertIdentifierFromString(client), status, cancellationToken);
        foreach (var authorization in authorizations)
        {
            yield return authorization.ToModel();
        }
    }

    /// <summary>
    /// 异步查找数据。
    /// </summary>
    /// <param name="subject">subject。</param>
    /// <param name="client">客户端标识。</param>
    /// <param name="status">status。</param>
    /// <param name="type">type。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>匹配的数据。</returns>
    public virtual async IAsyncEnumerable<OpenIddictAuthorizationModel> FindAsync(string subject, string client, string status, string type, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        Check.NotNullOrEmpty(subject, nameof(subject));
        Check.NotNullOrEmpty(client, nameof(client));
        Check.NotNullOrEmpty(status, nameof(status));
        Check.NotNullOrEmpty(type, nameof(type));

        var authorizations = await Repository.FindAsync(subject, ConvertIdentifierFromString(client), status, type, cancellationToken);
        foreach (var authorization in authorizations)
        {
            yield return authorization.ToModel();
        }
    }

    /// <summary>
    /// 异步查找数据。
    /// </summary>
    /// <param name="subject">subject。</param>
    /// <param name="client">客户端标识。</param>
    /// <param name="status">status。</param>
    /// <param name="type">type。</param>
    /// <param name="scopes">作用域。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>匹配的数据。</returns>
    public virtual async IAsyncEnumerable<OpenIddictAuthorizationModel> FindAsync(string subject, string client, string status, string type, ImmutableArray<string> scopes, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        Check.NotNullOrEmpty(subject, nameof(subject));
        Check.NotNullOrEmpty(client, nameof(client));
        Check.NotNullOrEmpty(status, nameof(status));
        Check.NotNullOrEmpty(type, nameof(type));

        var authorizations = await Repository.FindAsync(subject, ConvertIdentifierFromString(client), status, type, cancellationToken);

        foreach (var authorization in authorizations)
        {
            if (new HashSet<string>(await GetScopesAsync(authorization.ToModel(), cancellationToken), StringComparer.Ordinal).IsSupersetOf(scopes))
            {
                yield return authorization.ToModel();
            }
        }
    }

    /// <summary>
    /// 根据应用程序标识查找数据。
    /// </summary>
    /// <param name="identifier">标识。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>匹配的数据。</returns>
    public virtual async IAsyncEnumerable<OpenIddictAuthorizationModel> FindByApplicationIdAsync(string identifier, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        Check.NotNullOrEmpty(identifier, nameof(identifier));

        var authorizations = await Repository.FindByApplicationIdAsync(ConvertIdentifierFromString(identifier), cancellationToken);
        foreach (var authorization in authorizations)
        {
            yield return authorization.ToModel();
        }
    }

    /// <summary>
    /// 根据标识查找数据。
    /// </summary>
    /// <param name="identifier">标识。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>匹配的数据。</returns>
    public virtual async ValueTask<OpenIddictAuthorizationModel> FindByIdAsync(string identifier, CancellationToken cancellationToken)
    {
        Check.NotNullOrEmpty(identifier, nameof(identifier));

        return (await Repository.FindByIdAsync(ConvertIdentifierFromString(identifier), cancellationToken)).ToModel();
    }

    /// <summary>
    /// 根据主体查找数据。
    /// </summary>
    /// <param name="subject">subject。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>匹配的数据。</returns>
    public virtual async IAsyncEnumerable<OpenIddictAuthorizationModel> FindBySubjectAsync(string subject, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        Check.NotNullOrEmpty(subject, nameof(subject));

        var authorizations = await Repository.FindBySubjectAsync(subject, cancellationToken);
        foreach (var authorization in authorizations)
        {
            yield return authorization.ToModel();
        }
    }

    /// <summary>
    /// 获取应用程序标识。
    /// </summary>
    /// <param name="authorization">OpenIddict 授权。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>操作结果。</returns>
    public virtual ValueTask <string> GetApplicationIdAsync(OpenIddictAuthorizationModel authorization, CancellationToken cancellationToken)
    {
        Check.NotNull(authorization, nameof(authorization));

        return new ValueTask<string>(authorization.ApplicationId.HasValue
            ? ConvertIdentifierToString(authorization.ApplicationId.Value)
            : null);
    }

    /// <summary>
    /// 获取指定标识的数据。
    /// </summary>
    /// <param name="query">查询。</param>
    /// <param name="state">状态。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>查询结果。</returns>
    public virtual ValueTask<TResult> GetAsync<TState, TResult>(Func<IQueryable<OpenIddictAuthorizationModel>, TState, IQueryable<TResult>> query, TState state, CancellationToken cancellationToken)
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// 获取创建时间。
    /// </summary>
    /// <param name="authorization">OpenIddict 授权。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>操作结果。</returns>
    public virtual ValueTask <DateTimeOffset?> GetCreationDateAsync(OpenIddictAuthorizationModel authorization, CancellationToken cancellationToken)
    {
        Check.NotNull(authorization, nameof(authorization));

        return authorization.CreationDate is null
            ? new ValueTask<DateTimeOffset?>(result: null)
            : new ValueTask<DateTimeOffset?>(DateTime.SpecifyKind(authorization.CreationDate.Value, DateTimeKind.Utc));
    }

    /// <summary>
    /// 获取标识。
    /// </summary>
    /// <param name="authorization">OpenIddict 授权。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>操作结果。</returns>
    public virtual ValueTask <string> GetIdAsync(OpenIddictAuthorizationModel authorization, CancellationToken cancellationToken)
    {
        Check.NotNull(authorization, nameof(authorization));

        return new ValueTask<string>(ConvertIdentifierToString(authorization.Id));
    }

    /// <summary>
    /// 获取属性。
    /// </summary>
    /// <param name="authorization">OpenIddict 授权。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>操作结果。</returns>
    public virtual ValueTask <ImmutableDictionary<string, JsonElement>> GetPropertiesAsync(OpenIddictAuthorizationModel authorization, CancellationToken cancellationToken)
    {
        Check.NotNull(authorization, nameof(authorization));

        if (string.IsNullOrEmpty(authorization.Properties))
        {
            return new ValueTask<ImmutableDictionary<string, JsonElement>>(ImmutableDictionary.Create<string, JsonElement>());
        }

        using (var document = JsonDocument.Parse(authorization.Properties))
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
    /// 获取作用域。
    /// </summary>
    /// <param name="authorization">OpenIddict 授权。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>操作结果。</returns>
    public virtual ValueTask <ImmutableArray<string>> GetScopesAsync(OpenIddictAuthorizationModel authorization, CancellationToken cancellationToken)
    {
        Check.NotNull(authorization, nameof(authorization));

        if (string.IsNullOrEmpty(authorization.Scopes))
        {
            return new ValueTask<ImmutableArray<string>>(ImmutableArray.Create<string>());
        }

        using (var document = JsonDocument.Parse(authorization.Scopes))
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
    /// 获取状态。
    /// </summary>
    /// <param name="authorization">OpenIddict 授权。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>操作结果。</returns>
    public virtual ValueTask <string> GetStatusAsync(OpenIddictAuthorizationModel authorization, CancellationToken cancellationToken)
    {
        Check.NotNull(authorization, nameof(authorization));

        return new ValueTask<string>(authorization.Status);
    }

    /// <summary>
    /// 获取主体。
    /// </summary>
    /// <param name="authorization">OpenIddict 授权。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>操作结果。</returns>
    public virtual ValueTask <string> GetSubjectAsync(OpenIddictAuthorizationModel authorization, CancellationToken cancellationToken)
    {
        Check.NotNull(authorization, nameof(authorization));

        return new ValueTask<string>(authorization.Subject);
    }

    /// <summary>
    /// 获取类型。
    /// </summary>
    /// <param name="authorization">OpenIddict 授权。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>操作结果。</returns>
    public virtual ValueTask <string> GetTypeAsync(OpenIddictAuthorizationModel authorization, CancellationToken cancellationToken)
    {
        Check.NotNull(authorization, nameof(authorization));

        return new ValueTask<string>(authorization.Type);
    }

    /// <summary>
    /// 异步创建实例。
    /// </summary>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>操作结果。</returns>
    public virtual ValueTask <OpenIddictAuthorizationModel> InstantiateAsync(CancellationToken cancellationToken)
    {
        return new ValueTask<OpenIddictAuthorizationModel>(new OpenIddictAuthorizationModel
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
    public virtual async IAsyncEnumerable<OpenIddictAuthorizationModel> ListAsync(int? count, int? offset, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var authorizations = await Repository.ListAsync(count, offset, cancellationToken);
        foreach (var authorization in authorizations)
        {
            yield return authorization.ToModel();
        }
    }

    /// <summary>
    /// 列出数据。
    /// </summary>
    /// <param name="query">查询。</param>
    /// <param name="state">状态。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>异步操作结果。</returns>
    public virtual IAsyncEnumerable<TResult> ListAsync<TState, TResult>(Func<IQueryable<OpenIddictAuthorizationModel>, TState, IQueryable<TResult>> query, TState state, CancellationToken cancellationToken)
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// 清理过期或无效数据。
    /// </summary>
    /// <param name="threshold">threshold。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>异步操作结果。</returns>
    public virtual async ValueTask<long> PruneAsync(DateTimeOffset threshold, CancellationToken cancellationToken)
    {
        using (var uow = UnitOfWorkManager.Begin(requiresNew: true, isTransactional: true, isolationLevel: StoreOptions.Value.PruneIsolationLevel))
        {
            var date = threshold.UtcDateTime;
            var count = await Repository.PruneAsync(date, cancellationToken: cancellationToken);
            await uow.CompleteAsync(cancellationToken);
            return count;
        }
    }

    /// <summary>
    /// 设置应用程序标识。
    /// </summary>
    /// <param name="authorization">OpenIddict 授权。</param>
    /// <param name="identifier">标识。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>表示异步操作的任务。</returns>
    public virtual async ValueTask SetApplicationIdAsync(OpenIddictAuthorizationModel authorization, string identifier, CancellationToken cancellationToken)
    {
        Check.NotNull(authorization, nameof(authorization));

        if (!string.IsNullOrEmpty(identifier))
        {
            var application = await ApplicationRepository.GetAsync(ConvertIdentifierFromString(identifier), cancellationToken: cancellationToken);
            authorization.ApplicationId = application.Id;
        }
        else
        {
            authorization.ApplicationId = null;
        }
    }

    /// <summary>
    /// 设置创建时间。
    /// </summary>
    /// <param name="authorization">OpenIddict 授权。</param>
    /// <param name="date">date。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>表示异步操作的任务。</returns>
    public virtual ValueTask SetCreationDateAsync(OpenIddictAuthorizationModel authorization, DateTimeOffset? date, CancellationToken cancellationToken)
    {
        Check.NotNull(authorization, nameof(authorization));

        authorization.CreationDate = date?.UtcDateTime;

        return default;
    }

    /// <summary>
    /// 设置属性。
    /// </summary>
    /// <param name="authorization">OpenIddict 授权。</param>
    /// <param name="properties">properties。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>表示异步操作的任务。</returns>
    public virtual ValueTask SetPropertiesAsync(OpenIddictAuthorizationModel authorization, ImmutableDictionary<string, JsonElement> properties, CancellationToken cancellationToken)
    {
        if (properties is null || properties.IsEmpty)
        {
            authorization.Properties = null;
            return default;
        }

        authorization.Properties = WriteStream(writer =>
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
    /// 设置作用域。
    /// </summary>
    /// <param name="authorization">OpenIddict 授权。</param>
    /// <param name="scopes">作用域。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>表示异步操作的任务。</returns>
    public virtual ValueTask SetScopesAsync(OpenIddictAuthorizationModel authorization, ImmutableArray<string> scopes, CancellationToken cancellationToken)
    {
        Check.NotNull(authorization, nameof(authorization));

        if (scopes.IsDefaultOrEmpty)
        {
            authorization.Scopes = null;
            return default;
        }

        authorization.Scopes = WriteStream(writer =>
        {
            writer.WriteStartArray();
            foreach (var scope in scopes)
            {
                writer.WriteStringValue(scope);
            }
            writer.WriteEndArray();
        });

        return default;
    }

    /// <summary>
    /// 设置状态。
    /// </summary>
    /// <param name="authorization">OpenIddict 授权。</param>
    /// <param name="status">status。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>表示异步操作的任务。</returns>
    public virtual ValueTask SetStatusAsync(OpenIddictAuthorizationModel authorization, string status, CancellationToken cancellationToken)
    {
        Check.NotNull(authorization, nameof(authorization));

        authorization.Status = status;

        return default;
    }

    /// <summary>
    /// 设置主体。
    /// </summary>
    /// <param name="authorization">OpenIddict 授权。</param>
    /// <param name="subject">subject。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>表示异步操作的任务。</returns>
    public virtual ValueTask SetSubjectAsync(OpenIddictAuthorizationModel authorization, string subject, CancellationToken cancellationToken)
    {
        Check.NotNull(authorization, nameof(authorization));

        authorization.Subject = subject;

        return default;
    }

    /// <summary>
    /// 设置类型。
    /// </summary>
    /// <param name="authorization">OpenIddict 授权。</param>
    /// <param name="type">type。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>表示异步操作的任务。</returns>
    public virtual ValueTask SetTypeAsync(OpenIddictAuthorizationModel authorization, string type, CancellationToken cancellationToken)
    {
        Check.NotNull(authorization, nameof(authorization));

        authorization.Type = type;

        return default;
    }

    /// <summary>
    /// 更新数据。
    /// </summary>
    /// <param name="authorization">OpenIddict 授权。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>更新后的数据。</returns>
    public virtual async ValueTask UpdateAsync(OpenIddictAuthorizationModel authorization, CancellationToken cancellationToken)
    {
        Check.NotNull(authorization, nameof(authorization));

        var entity = await Repository.GetAsync(authorization.Id, cancellationToken: cancellationToken);

        try
        {
            await Repository.UpdateAsync(authorization.ToEntity(entity), autoSave: true, cancellationToken: cancellationToken);
        }
        catch (AbpDbConcurrencyException e)
        {
            Logger.LogException(e);
            await ConcurrencyExceptionHandler.HandleAsync(e);
            throw new OpenIddictExceptions.ConcurrencyException(e.Message, e.InnerException);
        }

        authorization = (await Repository.FindAsync(entity.Id, cancellationToken: cancellationToken)).ToModel();
    }
}
