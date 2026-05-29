using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;

namespace Censeq.FileManagement.Files;

/// <summary>
/// 文件存储提供器解析器，根据配置或文件记录选择本地存储或 OSS 存储实现。
/// </summary>
public class FileStorageProviderResolver : IFileStorageProviderResolver, ITransientDependency
{
    private readonly FileStorageOptions _options;
    private readonly IEnumerable<IFileStorageProvider> _providers;

    public FileStorageProviderResolver(
        IOptions<FileStorageOptions> options,
        IEnumerable<IFileStorageProvider> providers)
    {
        _options = options.Value;
        _providers = providers;
    }

    /// <summary>
    /// 按当前配置解析默认文件存储提供器。
    /// </summary>
    public IFileStorageProvider Resolve()
    {
        return ResolveByName(_options.Provider);
    }

    /// <summary>
    /// 按文件记录中的提供器信息解析文件存储提供器。
    /// </summary>
    public IFileStorageProvider Resolve(FileRecord file)
    {
        return ResolveByName(file.Provider.IsNullOrWhiteSpace()
            ? FileStorageProviderNames.Local
            : file.Provider);
    }

    private IFileStorageProvider ResolveByName(string? providerName)
    {
        var name = providerName.IsNullOrWhiteSpace()
            ? FileStorageProviderNames.Local
            : providerName;

        var provider = _providers.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        if (provider == null)
        {
            throw new InvalidOperationException($"File storage provider '{name}' is not registered.");
        }

        return provider;
    }
}
