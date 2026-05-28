using Microsoft.Extensions.Options;
using OnceMi.AspNetCore.OSS;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace Censeq.FileManagement.Files;

public class FileProviderSelector : IFileProviderSelector, ITransientDependency
{
    private readonly IRepository<FileProvider, Guid> _repository;
    private readonly FileStorageOptions _options;

    public FileProviderSelector(
        IRepository<FileProvider, Guid> repository,
        IOptions<FileStorageOptions> options)
    {
        _repository = repository;
        _options = options.Value;
    }

    public async Task<FileProvider?> GetDefaultAsync()
    {
        var queryable = await _repository.GetQueryableAsync();
        var provider = queryable
            .Where(x => x.IsEnable)
            .OrderByDescending(x => x.IsDefault)
            .ThenBy(x => x.OrderNo)
            .FirstOrDefault();

        return provider ?? CreateFromOptions();
    }

    public async Task<FileProvider?> FindAsync(string? provider, string? bucketName)
    {
        if (provider.IsNullOrWhiteSpace() || bucketName.IsNullOrWhiteSpace())
        {
            return CreateFromOptions();
        }

        var queryable = await _repository.GetQueryableAsync();
        return queryable.FirstOrDefault(x =>
            x.IsEnable &&
            x.Provider == provider &&
            x.BucketName == bucketName) ?? CreateFromOptions();
    }

    private FileProvider? CreateFromOptions()
    {
        if (!_options.Oss.Enabled || _options.Oss.Bucket.IsNullOrWhiteSpace())
        {
            return null;
        }

        return new FileProvider(Guid.Empty, null)
        {
            Provider = Enum.GetName(_options.Oss.Provider) ?? OSSProvider.Minio.ToString(),
            BucketName = _options.Oss.Bucket,
            AccessKey = _options.Oss.AccessKey,
            SecretKey = _options.Oss.SecretKey,
            Region = _options.Oss.Region,
            Endpoint = _options.Oss.Endpoint,
            CustomDomain = _options.Oss.CustomHost,
            IsEnableHttps = _options.Oss.IsEnableHttps,
            IsEnableCache = _options.Oss.IsEnableCache,
            IsEnable = true,
            IsDefault = true
        };
    }
}
