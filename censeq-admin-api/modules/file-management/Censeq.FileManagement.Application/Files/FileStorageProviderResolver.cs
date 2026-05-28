using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;

namespace Censeq.FileManagement.Files;

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

    public IFileStorageProvider Resolve()
    {
        return ResolveByName(_options.Provider);
    }

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
