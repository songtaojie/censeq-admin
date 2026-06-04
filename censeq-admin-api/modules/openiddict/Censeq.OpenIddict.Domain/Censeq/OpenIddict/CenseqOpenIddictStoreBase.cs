using System;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.Uow;

namespace Censeq.OpenIddict;

/// <summary>
/// OpenIddict存储基类。
/// </summary>
public abstract class CenseqOpenIddictStoreBase<TRepository>
    where TRepository : IRepository
{
    /// <summary>
    /// 日志记录器。
    /// </summary>
    public ILogger<CenseqOpenIddictStoreBase<TRepository>> Logger { get; set; }

    /// <summary>
    /// 仓储。
    /// </summary>
    protected TRepository Repository { get; }
    /// <summary>
    /// 工作单元管理器。
    /// </summary>
    protected IUnitOfWorkManager UnitOfWorkManager { get; }
    /// <summary>
    /// GUID 生成器。
    /// </summary>
    protected IGuidGenerator GuidGenerator { get; }
    /// <summary>
    /// 标识转换器。
    /// </summary>
    protected CenseqOpenIddictIdentifierConverter IdentifierConverter { get; }
    /// <summary>
    /// 并发异常处理器。
    /// </summary>
    protected IOpenIddictDbConcurrencyExceptionHandler ConcurrencyExceptionHandler { get; }
    /// <summary>
    /// 存储配置项。
    /// </summary>
    protected IOptions<CenseqOpenIddictStoreOptions> StoreOptions { get; }

    /// <summary>
    /// 初始化 CenseqOpenIddictStoreBase 实例。
    /// </summary>
    /// <param name="repository">仓储。</param>
    /// <param name="unitOfWorkManager">工作单元管理器。</param>
    /// <param name="guidGenerator">GUID生成器。</param>
    /// <param name="identifierConverter">dentifier转换器。</param>
    /// <param name="concurrencyExceptionHandler">并发异常处理器。</param>
    /// <param name="storeOptions">存储配置项。</param>
    protected CenseqOpenIddictStoreBase(TRepository repository, IUnitOfWorkManager unitOfWorkManager, IGuidGenerator guidGenerator, CenseqOpenIddictIdentifierConverter identifierConverter, IOpenIddictDbConcurrencyExceptionHandler concurrencyExceptionHandler, IOptions<CenseqOpenIddictStoreOptions> storeOptions)
    {
        Repository = repository;
        UnitOfWorkManager = unitOfWorkManager;
        GuidGenerator = guidGenerator;
        IdentifierConverter = identifierConverter;
        ConcurrencyExceptionHandler = concurrencyExceptionHandler;
        StoreOptions = storeOptions;

        Logger = NullLogger<CenseqOpenIddictStoreBase<TRepository>>.Instance;
    }

    /// <summary>
    /// 从字符串转换标识。
    /// </summary>
    /// <param name="identifier">标识。</param>
    /// <returns>操作结果。</returns>
    protected virtual Guid ConvertIdentifierFromString(string identifier)
    {
        return IdentifierConverter.FromString(identifier);
    }

    /// <summary>
    /// 将标识转换为字符串。
    /// </summary>
    /// <param name="identifier">标识。</param>
    /// <returns>操作结果。</returns>
    protected virtual string ConvertIdentifierToString(Guid identifier)
    {
        return IdentifierConverter.ToString(identifier);
    }

    /// <summary>
    /// 写入流。
    /// </summary>
    /// <param name="action">操作。</param>
    /// <returns>操作结果。</returns>
    protected virtual string WriteStream(Action<Utf8JsonWriter> action)
    {
        using (var stream = new MemoryStream())
        {
            using (var writer = new Utf8JsonWriter(stream, new JsonWriterOptions
                   {
                       Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                       Indented = false
                   }))
            {
                action(writer);
                writer.Flush();
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }
    }

    /// <summary>
    /// 异步写入流。
    /// </summary>
    /// <param name="func">func。</param>
    /// <returns>异步操作结果。</returns>
    protected virtual async Task<string> WriteStreamAsync(Func<Utf8JsonWriter, Task> func)
    {
        using (var stream = new MemoryStream())
        {
            using (var writer = new Utf8JsonWriter(stream, new JsonWriterOptions
                   {
                       Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                       Indented = false
                   }))
            {
                await func(writer);
                await writer.FlushAsync();
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }
    }
}
