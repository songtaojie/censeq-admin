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
using Censeq.OpenIddict.Authorizations;
using Volo.Abp.Uow;

namespace Censeq.OpenIddict.Tokens;

/// <summary>
/// OpenIddict 令牌存储，适配 OpenIddict 存储契约。
/// </summary>
public class CenseqOpenIddictTokenStore : CenseqOpenIddictStoreBase<IOpenIddictTokenRepository>, IOpenIddictTokenStore<OpenIddictTokenModel>
{
    /// <summary>
    /// OpenIddict 应用程序仓储。
    /// </summary>
    protected IOpenIddictApplicationRepository ApplicationRepository { get; }
    /// <summary>
    /// 授权仓储。
    /// </summary>
    protected IOpenIddictAuthorizationRepository AuthorizationRepository { get; }

    /// <summary>
    /// 初始化 CenseqOpenIddictTokenStore 实例。
    /// </summary>
    /// <param name="repository">仓储。</param>
    /// <param name="unitOfWorkManager">工作单元管理器。</param>
    /// <param name="guidGenerator">GUID生成器。</param>
    /// <param name="applicationRepository">应用程序仓储。</param>
    /// <param name="authorizationRepository">授权仓储。</param>
    /// <param name="identifierConverter">dentifier转换器。</param>
    /// <param name="concurrencyExceptionHandler">并发异常处理器。</param>
    /// <param name="storeOptions">存储配置项。</param>
    public CenseqOpenIddictTokenStore(
        IOpenIddictTokenRepository repository,
        IUnitOfWorkManager unitOfWorkManager,
        IGuidGenerator guidGenerator,
        IOpenIddictApplicationRepository applicationRepository,
        IOpenIddictAuthorizationRepository authorizationRepository,
        CenseqOpenIddictIdentifierConverter identifierConverter,
        IOpenIddictDbConcurrencyExceptionHandler concurrencyExceptionHandler,
        IOptions<CenseqOpenIddictStoreOptions> storeOptions)
        : base(repository, unitOfWorkManager, guidGenerator, identifierConverter, concurrencyExceptionHandler, storeOptions)
    {
        ApplicationRepository = applicationRepository;
        AuthorizationRepository = authorizationRepository;
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
    public virtual ValueTask<long> CountAsync<TResult>(Func<IQueryable<OpenIddictTokenModel>, IQueryable<TResult>> query, CancellationToken cancellationToken)
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// 创建数据。
    /// </summary>
    /// <param name="token">OpenIddict 令牌。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>创建后的数据。</returns>
    public virtual async ValueTask CreateAsync(OpenIddictTokenModel token, CancellationToken cancellationToken)
    {
        Check.NotNull(token, nameof(token));

        await Repository.InsertAsync(token.ToEntity(), autoSave: true, cancellationToken: cancellationToken);

        token = (await Repository.FindAsync(token.Id, cancellationToken: cancellationToken)).ToModel();
    }

    /// <summary>
    /// 删除数据。
    /// </summary>
    /// <param name="token">OpenIddict 令牌。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>表示异步操作的任务。</returns>
    public virtual async ValueTask DeleteAsync(OpenIddictTokenModel token, CancellationToken cancellationToken)
    {
        Check.NotNull(token, nameof(token));

        try
        {
            await Repository.DeleteAsync(token.ToEntity(), autoSave: true, cancellationToken: cancellationToken);
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
    public virtual async IAsyncEnumerable<OpenIddictTokenModel> FindAsync(string subject, string client, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        Check.NotNullOrEmpty(subject, nameof(subject));
        Check.NotNullOrEmpty(client, nameof(client));

        var tokens = await Repository.FindAsync(subject, ConvertIdentifierFromString(client), cancellationToken);
        foreach (var token in tokens)
        {
            yield return token.ToModel();
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
    public virtual async IAsyncEnumerable<OpenIddictTokenModel> FindAsync(string subject, string client, string status, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        Check.NotNullOrEmpty(subject, nameof(subject));
        Check.NotNullOrEmpty(client, nameof(client));
        Check.NotNullOrEmpty(status, nameof(status));

        var tokens = await Repository.FindAsync(subject, ConvertIdentifierFromString(client), status, cancellationToken);
        foreach (var token in tokens)
        {
            yield return token.ToModel();
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
    public virtual async IAsyncEnumerable<OpenIddictTokenModel> FindAsync(string subject, string client, string status, string type, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        Check.NotNullOrEmpty(subject, nameof(subject));
        Check.NotNullOrEmpty(client, nameof(client));
        Check.NotNullOrEmpty(status, nameof(status));
        Check.NotNullOrEmpty(type, nameof(type));

        var tokens = await Repository.FindAsync(subject, ConvertIdentifierFromString(client), status, type, cancellationToken);
        foreach (var token in tokens)
        {
            yield return token.ToModel();
        }
    }

    /// <summary>
    /// 根据应用程序标识查找数据。
    /// </summary>
    /// <param name="identifier">标识。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>匹配的数据。</returns>
    public virtual async IAsyncEnumerable<OpenIddictTokenModel> FindByApplicationIdAsync(string identifier, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        Check.NotNullOrEmpty(identifier, nameof(identifier));

        var tokens = await Repository.FindByApplicationIdAsync(ConvertIdentifierFromString(identifier), cancellationToken);
        foreach (var token in tokens)
        {
            yield return token.ToModel();
        }
    }

    /// <summary>
    /// 根据授权标识查找数据。
    /// </summary>
    /// <param name="identifier">标识。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>匹配的数据。</returns>
    public virtual async IAsyncEnumerable<OpenIddictTokenModel> FindByAuthorizationIdAsync(string identifier, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        Check.NotNullOrEmpty(identifier, nameof(identifier));

        var tokens = await Repository.FindByAuthorizationIdAsync(ConvertIdentifierFromString(identifier), cancellationToken);
        foreach (var token in tokens)
        {
            yield return token.ToModel();
        }
    }

    /// <summary>
    /// 根据标识查找数据。
    /// </summary>
    /// <param name="identifier">标识。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>匹配的数据。</returns>
    public virtual async ValueTask<OpenIddictTokenModel> FindByIdAsync(string identifier, CancellationToken cancellationToken)
    {
        Check.NotNullOrEmpty(identifier, nameof(identifier));

        return (await Repository.FindByIdAsync(ConvertIdentifierFromString(identifier), cancellationToken)).ToModel();
    }

    /// <summary>
    /// 根据引用标识查找数据。
    /// </summary>
    /// <param name="identifier">标识。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>匹配的数据。</returns>
    public virtual async ValueTask<OpenIddictTokenModel> FindByReferenceIdAsync(string identifier, CancellationToken cancellationToken)
    {
        Check.NotNullOrEmpty(identifier, nameof(identifier));

        return (await Repository.FindByReferenceIdAsync(identifier, cancellationToken)).ToModel();
    }

    /// <summary>
    /// 根据主体查找数据。
    /// </summary>
    /// <param name="subject">subject。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>匹配的数据。</returns>
    public virtual async IAsyncEnumerable<OpenIddictTokenModel> FindBySubjectAsync(string subject, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        Check.NotNullOrEmpty(subject, nameof(subject));

        var tokens = await Repository.FindBySubjectAsync(subject, cancellationToken);
        foreach (var token in tokens)
        {
            yield return token.ToModel();
        }
    }

    /// <summary>
    /// 获取应用程序标识。
    /// </summary>
    /// <param name="token">OpenIddict 令牌。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>异步操作结果。</returns>
    public virtual ValueTask<string> GetApplicationIdAsync(OpenIddictTokenModel token, CancellationToken cancellationToken)
    {
        Check.NotNull(token, nameof(token));

        return new ValueTask<string>(token.ApplicationId.HasValue
            ? ConvertIdentifierToString(token.ApplicationId.Value)
            : null);
    }

    /// <summary>
    /// 获取指定标识的数据。
    /// </summary>
    /// <param name="query">查询。</param>
    /// <param name="state">状态。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>查询结果。</returns>
    public virtual ValueTask<TResult> GetAsync<TState, TResult>(Func<IQueryable<OpenIddictTokenModel>, TState, IQueryable<TResult>> query, TState state, CancellationToken cancellationToken)
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// 获取授权标识。
    /// </summary>
    /// <param name="token">OpenIddict 令牌。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>异步操作结果。</returns>
    public virtual ValueTask<string> GetAuthorizationIdAsync(OpenIddictTokenModel token, CancellationToken cancellationToken)
    {
        Check.NotNull(token, nameof(token));

        return new ValueTask<string>(token.AuthorizationId.HasValue
            ? ConvertIdentifierToString(token.AuthorizationId.Value)
            : null);
    }

    /// <summary>
    /// 获取创建时间。
    /// </summary>
    /// <param name="token">OpenIddict 令牌。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>异步操作结果。</returns>
    public virtual ValueTask<DateTimeOffset?> GetCreationDateAsync(OpenIddictTokenModel token, CancellationToken cancellationToken)
    {
        Check.NotNull(token, nameof(token));

        if (token.CreationDate is null)
        {
            return new ValueTask<DateTimeOffset?>(result: null);
        }

        return new ValueTask<DateTimeOffset?>(DateTime.SpecifyKind(token.CreationDate.Value, DateTimeKind.Utc));
    }

    /// <summary>
    /// 获取过期时间。
    /// </summary>
    /// <param name="token">OpenIddict 令牌。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>异步操作结果。</returns>
    public virtual ValueTask<DateTimeOffset?> GetExpirationDateAsync(OpenIddictTokenModel token, CancellationToken cancellationToken)
    {
        Check.NotNull(token, nameof(token));

        if (token.ExpirationDate is null)
        {
            return new ValueTask<DateTimeOffset?>(result: null);
        }

        return new ValueTask<DateTimeOffset?>(DateTime.SpecifyKind(token.ExpirationDate.Value, DateTimeKind.Utc));
    }

    /// <summary>
    /// 获取标识。
    /// </summary>
    /// <param name="token">OpenIddict 令牌。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>异步操作结果。</returns>
    public virtual ValueTask<string> GetIdAsync(OpenIddictTokenModel token, CancellationToken cancellationToken)
    {
        Check.NotNull(token, nameof(token));

        return new ValueTask<string>(ConvertIdentifierToString(token.Id));
    }

    /// <summary>
    /// 获取载荷。
    /// </summary>
    /// <param name="token">OpenIddict 令牌。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>异步操作结果。</returns>
    public virtual ValueTask<string> GetPayloadAsync(OpenIddictTokenModel token, CancellationToken cancellationToken)
    {
        Check.NotNull(token, nameof(token));

        return new ValueTask<string>(token.Payload);
    }

    /// <summary>
    /// 获取属性。
    /// </summary>
    /// <param name="token">OpenIddict 令牌。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>异步操作结果。</returns>
    public virtual ValueTask<ImmutableDictionary<string, JsonElement>> GetPropertiesAsync(OpenIddictTokenModel token, CancellationToken cancellationToken)
    {
        Check.NotNull(token, nameof(token));

        if (string.IsNullOrEmpty(token.Properties))
        {
            return new ValueTask<ImmutableDictionary<string, JsonElement>>(ImmutableDictionary.Create<string, JsonElement>());
        }

        using (var document = JsonDocument.Parse(token.Properties))
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
    /// 获取兑换时间。
    /// </summary>
    /// <param name="token">OpenIddict 令牌。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>异步操作结果。</returns>
    public virtual ValueTask<DateTimeOffset?> GetRedemptionDateAsync(OpenIddictTokenModel token, CancellationToken cancellationToken)
    {
        Check.NotNull(token, nameof(token));

        if (token.RedemptionDate is null)
        {
            return new ValueTask<DateTimeOffset?>(result: null);
        }

        return new ValueTask<DateTimeOffset?>(DateTime.SpecifyKind(token.RedemptionDate.Value, DateTimeKind.Utc));
    }

    /// <summary>
    /// 获取引用标识。
    /// </summary>
    /// <param name="token">OpenIddict 令牌。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>异步操作结果。</returns>
    public virtual ValueTask<string> GetReferenceIdAsync(OpenIddictTokenModel token, CancellationToken cancellationToken)
    {
        Check.NotNull(token, nameof(token));

        return new ValueTask<string>(token.ReferenceId);
    }

    /// <summary>
    /// 获取状态。
    /// </summary>
    /// <param name="token">OpenIddict 令牌。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>异步操作结果。</returns>
    public virtual ValueTask<string> GetStatusAsync(OpenIddictTokenModel token, CancellationToken cancellationToken)
    {
        Check.NotNull(token, nameof(token));

        return new ValueTask<string>(token.Status);
    }

    /// <summary>
    /// 获取主体。
    /// </summary>
    /// <param name="token">OpenIddict 令牌。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>异步操作结果。</returns>
    public virtual ValueTask<string> GetSubjectAsync(OpenIddictTokenModel token, CancellationToken cancellationToken)
    {
        Check.NotNull(token, nameof(token));

        return new ValueTask<string>(token.Subject);
    }

    /// <summary>
    /// 获取类型。
    /// </summary>
    /// <param name="token">OpenIddict 令牌。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>异步操作结果。</returns>
    public virtual ValueTask<string> GetTypeAsync(OpenIddictTokenModel token, CancellationToken cancellationToken)
    {
        Check.NotNull(token, nameof(token));

        return new ValueTask<string>(token.Type);
    }

    /// <summary>
    /// 异步创建实例。
    /// </summary>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>异步操作结果。</returns>
    public virtual ValueTask<OpenIddictTokenModel> InstantiateAsync(CancellationToken cancellationToken)
    {
        return new ValueTask<OpenIddictTokenModel>(new OpenIddictTokenModel
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
    public virtual async IAsyncEnumerable<OpenIddictTokenModel> ListAsync(int? count, int? offset, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var tokens = await Repository.ListAsync(count, offset, cancellationToken);
        foreach (var token in tokens)
        {
            yield return token.ToModel();
        }
    }

    /// <summary>
    /// 列出数据。
    /// </summary>
    /// <param name="query">查询。</param>
    /// <param name="state">状态。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>异步操作结果。</returns>
    public virtual IAsyncEnumerable<TResult> ListAsync<TState, TResult>(Func<IQueryable<OpenIddictTokenModel>, TState, IQueryable<TResult>> query, TState state, CancellationToken cancellationToken)
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// 撤销数据。
    /// </summary>
    /// <param name="identifier">标识。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>异步操作结果。</returns>
    public virtual async ValueTask<long> RevokeByAuthorizationIdAsync(string identifier, CancellationToken cancellationToken)
    {
        return await Repository.RevokeByAuthorizationIdAsync(ConvertIdentifierFromString(identifier), cancellationToken);
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
    /// <param name="token">OpenIddict 令牌。</param>
    /// <param name="identifier">标识。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>表示异步操作的任务。</returns>
    public virtual async ValueTask SetApplicationIdAsync(OpenIddictTokenModel token, string identifier, CancellationToken cancellationToken)
    {
        Check.NotNull(token, nameof(token));

        if (!string.IsNullOrEmpty(identifier))
        {
            var application = await ApplicationRepository.GetAsync(ConvertIdentifierFromString(identifier), cancellationToken: cancellationToken);
            token.ApplicationId = application.Id;
        }
        else
        {
            token.ApplicationId = null;
        }
    }

    /// <summary>
    /// 设置授权标识。
    /// </summary>
    /// <param name="token">OpenIddict 令牌。</param>
    /// <param name="identifier">标识。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>表示异步操作的任务。</returns>
    public virtual async ValueTask SetAuthorizationIdAsync(OpenIddictTokenModel token, string identifier, CancellationToken cancellationToken)
    {
        Check.NotNull(token, nameof(token));

        if (!string.IsNullOrEmpty(identifier))
        {
            var authorization = await AuthorizationRepository.GetAsync(ConvertIdentifierFromString(identifier), cancellationToken: cancellationToken);
            token.AuthorizationId = authorization.Id;
        }
        else
        {
            token.AuthorizationId = null;
        }
    }

    /// <summary>
    /// 设置创建时间。
    /// </summary>
    /// <param name="token">OpenIddict 令牌。</param>
    /// <param name="date">date。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>表示异步操作的任务。</returns>
    public virtual ValueTask SetCreationDateAsync(OpenIddictTokenModel token, DateTimeOffset? date, CancellationToken cancellationToken)
    {
        Check.NotNull(token, nameof(token));

        token.CreationDate = date?.UtcDateTime;

        return default;
    }

    /// <summary>
    /// 设置过期时间。
    /// </summary>
    /// <param name="token">OpenIddict 令牌。</param>
    /// <param name="date">date。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>表示异步操作的任务。</returns>
    public virtual ValueTask SetExpirationDateAsync(OpenIddictTokenModel token, DateTimeOffset? date, CancellationToken cancellationToken)
    {
        Check.NotNull(token, nameof(token));

        token.ExpirationDate = date?.UtcDateTime;

        return default;
    }

    /// <summary>
    /// 设置载荷。
    /// </summary>
    /// <param name="token">OpenIddict 令牌。</param>
    /// <param name="payload">payload。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>表示异步操作的任务。</returns>
    public virtual ValueTask SetPayloadAsync(OpenIddictTokenModel token, string payload, CancellationToken cancellationToken)
    {
        Check.NotNull(token, nameof(token));

        token.Payload = payload;

        return default;
    }

    /// <summary>
    /// 设置属性。
    /// </summary>
    /// <param name="token">OpenIddict 令牌。</param>
    /// <param name="properties">properties。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>表示异步操作的任务。</returns>
    public virtual ValueTask SetPropertiesAsync(OpenIddictTokenModel token, ImmutableDictionary<string, JsonElement> properties, CancellationToken cancellationToken)
    {
        Check.NotNull(token, nameof(token));

        if (properties is null || properties.IsEmpty)
        {
            token.Properties = null;
            return default;
        }

        token.Properties  = WriteStream(writer =>
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
    /// 设置兑换时间。
    /// </summary>
    /// <param name="token">OpenIddict 令牌。</param>
    /// <param name="date">date。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>表示异步操作的任务。</returns>
    public virtual ValueTask SetRedemptionDateAsync(OpenIddictTokenModel token, DateTimeOffset? date, CancellationToken cancellationToken)
    {
        Check.NotNull(token, nameof(token));

        token.RedemptionDate = date?.UtcDateTime;

        return default;
    }

    /// <summary>
    /// 设置引用标识。
    /// </summary>
    /// <param name="token">OpenIddict 令牌。</param>
    /// <param name="identifier">标识。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>表示异步操作的任务。</returns>
    public virtual ValueTask SetReferenceIdAsync(OpenIddictTokenModel token, string identifier, CancellationToken cancellationToken)
    {
        Check.NotNull(token, nameof(token));

        token.ReferenceId = identifier;

        return default;
    }

    /// <summary>
    /// 设置状态。
    /// </summary>
    /// <param name="token">OpenIddict 令牌。</param>
    /// <param name="status">status。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>表示异步操作的任务。</returns>
    public virtual ValueTask SetStatusAsync(OpenIddictTokenModel token, string status, CancellationToken cancellationToken)
    {
        Check.NotNull(token, nameof(token));

        token.Status = status;

        return default;
    }

    /// <summary>
    /// 设置主体。
    /// </summary>
    /// <param name="token">OpenIddict 令牌。</param>
    /// <param name="subject">subject。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>表示异步操作的任务。</returns>
    public virtual ValueTask SetSubjectAsync(OpenIddictTokenModel token, string subject, CancellationToken cancellationToken)
    {
        Check.NotNull(token, nameof(token));

        token.Subject = subject;

        return default;
    }

    /// <summary>
    /// 设置类型。
    /// </summary>
    /// <param name="token">OpenIddict 令牌。</param>
    /// <param name="type">type。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>表示异步操作的任务。</returns>
    public virtual ValueTask SetTypeAsync(OpenIddictTokenModel token, string type, CancellationToken cancellationToken)
    {
        Check.NotNull(token, nameof(token));

        token.Type = type;

        return default;
    }

    /// <summary>
    /// 更新数据。
    /// </summary>
    /// <param name="token">OpenIddict 令牌。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>更新后的数据。</returns>
    public virtual async ValueTask UpdateAsync(OpenIddictTokenModel token, CancellationToken cancellationToken)
    {
        Check.NotNull(token, nameof(token));

        var entity = await Repository.GetAsync(token.Id, cancellationToken: cancellationToken);

        try
        {
            await Repository.UpdateAsync(token.ToEntity(entity), autoSave: true, cancellationToken: cancellationToken);
        }
        catch (AbpDbConcurrencyException e)
        {
            Logger.LogException(e);
            await ConcurrencyExceptionHandler.HandleAsync(e);
            throw new OpenIddictExceptions.ConcurrencyException(e.Message, e.InnerException);
        }

        token = (await Repository.FindAsync(entity.Id, cancellationToken: cancellationToken)).ToModel();
    }
}
