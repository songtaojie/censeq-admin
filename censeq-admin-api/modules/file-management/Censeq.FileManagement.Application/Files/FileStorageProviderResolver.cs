using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;

namespace Censeq.FileManagement.Files;

/// <summary>
/// 文件存储提供器解析器，根据配置或文件记录选择本地存储或 OSS 存储实现。
/// </summary>
public class FileStorageProviderResolver : IFileStorageProviderResolver, ITransientDependency
{
    private readonly FileStorageOptions _options;
    private readonly IFileProviderSelector _fileProviderSelector;
    private readonly IEnumerable<IFileStorageProvider> _providers;

    public FileStorageProviderResolver(
        IOptions<FileStorageOptions> options,
        IFileProviderSelector fileProviderSelector,
        IEnumerable<IFileStorageProvider> providers)
    {
        _options = options.Value;
        _fileProviderSelector = fileProviderSelector;
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
    /// 解析上传时使用的文件存储提供器，数据库存在可用 OSS 配置时优先使用 OSS。
    /// </summary>
    public async Task<IFileStorageProvider> ResolveForUploadAsync()
    {
        if (_options.Provider.Equals(FileStorageProviderNames.Oss, StringComparison.OrdinalIgnoreCase))
        {
            return ResolveByName(FileStorageProviderNames.Oss);
        }

        var defaultOssProvider = await _fileProviderSelector.GetDefaultAsync();
        if (defaultOssProvider != null)
        {
            return ResolveByName(FileStorageProviderNames.Oss);
        }

        return Resolve();
    }

    /// <summary>
    /// 按文件记录中的提供器信息解析文件存储提供器。
    /// </summary>
    public IFileStorageProvider Resolve(FileRecord file)
    {
        var providerName = ResolveProviderName(file);
        return ResolveByName(providerName, fallbackUnknownProviderToOss: true);
    }

    private IFileStorageProvider ResolveByName(string? providerName, bool fallbackUnknownProviderToOss = false)
    {
        var name = providerName.IsNullOrWhiteSpace()
            ? FileStorageProviderNames.Local
            : providerName;

        var provider = _providers.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        if (provider != null)
        {
            return provider;
        }

        if (fallbackUnknownProviderToOss && !name.Equals(FileStorageProviderNames.Local, StringComparison.OrdinalIgnoreCase))
        {
            var ossProvider = _providers.FirstOrDefault(x => x.Name.Equals(FileStorageProviderNames.Oss, StringComparison.OrdinalIgnoreCase));
            if (ossProvider != null)
            {
                return ossProvider;
            }
        }

        throw new InvalidOperationException($"File storage provider '{name}' is not registered.");
    }

    private static string ResolveProviderName(FileRecord file)
    {
        if (!file.StorageProvider.IsNullOrWhiteSpace())
        {
            return file.StorageProvider!;
        }

        if (file.Provider.IsNullOrWhiteSpace())
        {
            return FileStorageProviderNames.Local;
        }

        return file.Provider;
    }
}
