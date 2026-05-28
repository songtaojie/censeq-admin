using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OnceMi.AspNetCore.OSS;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace Censeq.FileManagement.Files;

public class FileOssServiceManager : IFileOssServiceManager, ISingletonDependency
{
    private readonly Dictionary<string, IOSSService> _cache = new(StringComparer.OrdinalIgnoreCase);
    private readonly object _lockObject = new();

    public Task<IOSSService> GetAsync(FileProvider provider)
    {
        if (provider.Provider.IsNullOrWhiteSpace() ||
            provider.BucketName.IsNullOrWhiteSpace() ||
            provider.AccessKey.IsNullOrWhiteSpace() ||
            provider.SecretKey.IsNullOrWhiteSpace())
        {
            throw new UserFriendlyException("OSS 提供器配置不完整");
        }

        lock (_lockObject)
        {
            if (_cache.TryGetValue(provider.ConfigKey, out var cached))
            {
                return Task.FromResult(cached);
            }

            var service = Create(provider);
            _cache[provider.ConfigKey] = service;
            return Task.FromResult(service);
        }
    }

    private static IOSSService Create(FileProvider provider)
    {
        var sectionName = $"FileProvider_{provider.Id:N}";
        var data = new Dictionary<string, string?>
        {
            [$"{sectionName}:Provider"] = provider.Provider,
            [$"{sectionName}:Endpoint"] = provider.Endpoint,
            [$"{sectionName}:AccessKey"] = provider.AccessKey,
            [$"{sectionName}:SecretKey"] = provider.SecretKey,
            [$"{sectionName}:Region"] = provider.Region,
            [$"{sectionName}:IsEnableHttps"] = provider.IsEnableHttps.ToString(),
            [$"{sectionName}:IsEnableCache"] = provider.IsEnableCache.ToString()
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(data)
            .Build();

        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddLogging();
        services.AddOSSService(provider.Provider, sectionName);

        var serviceProvider = services.BuildServiceProvider();
        var factory = serviceProvider.GetRequiredService<IOSSServiceFactory>();
        return factory.Create(provider.Provider);
    }
}
