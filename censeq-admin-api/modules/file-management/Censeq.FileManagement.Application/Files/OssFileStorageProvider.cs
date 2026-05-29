using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace Censeq.FileManagement.Files;

/// <summary>
/// OSS 文件存储提供器，负责对象存储上传、预签名下载和对象删除。
/// </summary>
public class OssFileStorageProvider : IFileStorageProvider, ITransientDependency
{
    private readonly IFileProviderSelector _providerSelector;
    private readonly IFileOssServiceManager _ossServiceManager;
    private readonly HttpClient _httpClient;

    public OssFileStorageProvider(
        IFileProviderSelector providerSelector,
        IFileOssServiceManager ossServiceManager,
        HttpClient httpClient)
    {
        _providerSelector = providerSelector;
        _ossServiceManager = ossServiceManager;
        _httpClient = httpClient;
    }

    /// <summary>
    /// 存储提供器名称。
    /// </summary>
    public string Name => FileStorageProviderNames.Oss;

    /// <summary>
    /// 保存文件到默认 OSS 提供器并返回访问地址。
    /// </summary>
    public async Task<StoredFileInfo> SaveAsync(SaveFileStorageInput input)
    {
        var provider = await GetDefaultProviderAsync();
        var ossService = await _ossServiceManager.GetAsync(provider);

        var objectName = input.RelativePath.TrimStart('/').Replace('\\', '/');
        await ossService.PutObjectAsync(provider.BucketName, objectName, input.Stream);

        return new StoredFileInfo
        {
            RelativePath = objectName,
            Url = GenerateUrl(provider, objectName),
            Provider = provider.Provider,
            StorageProvider = Name,
            BucketName = provider.BucketName
        };
    }

    /// <summary>
    /// 使用文件记录对应的 OSS 提供器创建下载结果。
    /// </summary>
    public async Task<FileDownloadInfo> GetDownloadAsync(FileRecord file)
    {
        var provider = await GetFileProviderAsync(file);
        var ossService = await _ossServiceManager.GetAsync(provider);

        var objectName = file.RelativePath.TrimStart('/').Replace('\\', '/');
        var url = await ossService.PresignedGetObjectAsync(provider.BucketName, objectName, 5);
        var stream = await _httpClient.GetStreamAsync(url);

        return new FileDownloadInfo
        {
            Result = new FileStreamResult(stream, file.ContentType)
            {
                FileDownloadName = file.OriginalName
            }
        };
    }

    /// <summary>
    /// 删除文件记录对应的 OSS 对象。
    /// </summary>
    public async Task DeleteAsync(FileRecord file)
    {
        var provider = await GetFileProviderAsync(file);
        var ossService = await _ossServiceManager.GetAsync(provider);

        var objectName = file.RelativePath.TrimStart('/').Replace('\\', '/');
        await ossService.RemoveObjectAsync(provider.BucketName, objectName);
    }

    private async Task<FileProvider> GetDefaultProviderAsync()
    {
        return await _providerSelector.GetDefaultAsync()
            ?? throw new UserFriendlyException("未配置可用的 OSS 文件存储提供器");
    }

    private async Task<FileProvider> GetFileProviderAsync(FileRecord file)
    {
        return await _providerSelector.FindAsync(file.Provider, file.BucketName)
            ?? throw new UserFriendlyException($"未找到文件对应的 OSS 提供器：{file.Provider}-{file.BucketName}");
    }

    private static string GenerateUrl(FileProvider provider, string objectName)
    {
        if (!provider.CustomDomain.IsNullOrWhiteSpace())
        {
            return $"{provider.CustomDomain.TrimEnd('/')}/{objectName}";
        }

        var protocol = provider.IsEnableHttps ? "https" : "http";

        return provider.Provider.ToUpperInvariant() switch
        {
            "ALIYUN" => $"{protocol}://{provider.BucketName}.{provider.Endpoint}/{objectName}",
            "QCLOUD" => $"{protocol}://{provider.BucketName}-{provider.Endpoint}.cos.{provider.Region}.myqcloud.com/{objectName}",
            "MINIO" => $"{protocol}://{provider.Endpoint?.TrimEnd('/')}/{provider.BucketName}/{objectName}",
            _ => throw new UserFriendlyException($"暂不支持的 OSS 提供商：{provider.Provider}")
        };
    }
}
