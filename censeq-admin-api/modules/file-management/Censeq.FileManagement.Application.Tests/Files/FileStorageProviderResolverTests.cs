using Censeq.FileManagement.Files;
using Microsoft.Extensions.Options;
using Shouldly;
using Xunit;

namespace Censeq.FileManagement.Application.Tests.Files;

public class FileStorageProviderResolverTests
{
    [Fact]
    public void Resolve_ShouldUseStorageProvider_WhenRecordStoresPhysicalProvider()
    {
        var resolver = CreateResolver();
        var file = new FileRecord(Guid.NewGuid(), null)
        {
            Provider = "Minio",
            StorageProvider = FileStorageProviderNames.Oss,
            BucketName = "avatars"
        };

        resolver.Resolve(file).Name.ShouldBe(FileStorageProviderNames.Oss);
    }

    [Fact]
    public void Resolve_ShouldTreatLegacyOssVendorProviderAsOss_WhenStorageProviderIsMissing()
    {
        var resolver = CreateResolver();
        var file = new FileRecord(Guid.NewGuid(), null)
        {
            Provider = "Minio",
            BucketName = "avatars"
        };

        resolver.Resolve(file).Name.ShouldBe(FileStorageProviderNames.Oss);
    }

    [Fact]
    public async Task ResolveForUploadAsync_ShouldUseOss_WhenDatabaseDefaultProviderExists()
    {
        var resolver = CreateResolver(
            options: new FileStorageOptions { Provider = FileStorageProviderNames.Local },
            selector: new StubFileProviderSelector(new FileProvider(Guid.NewGuid(), null)
            {
                Provider = "Minio",
                BucketName = "avatars",
                IsEnable = true,
                IsDefault = true
            }));

        var provider = await resolver.ResolveForUploadAsync();

        provider.Name.ShouldBe(FileStorageProviderNames.Oss);
    }

    private static FileStorageProviderResolver CreateResolver(
        FileStorageOptions? options = null,
        IFileProviderSelector? selector = null)
    {
        return new FileStorageProviderResolver(
            Options.Create(options ?? new FileStorageOptions { Provider = FileStorageProviderNames.Oss }),
            selector ?? new StubFileProviderSelector(null),
            new IFileStorageProvider[]
            {
                new StubFileStorageProvider(FileStorageProviderNames.Local),
                new StubFileStorageProvider(FileStorageProviderNames.Oss)
            });
    }

    private sealed class StubFileStorageProvider(string name) : IFileStorageProvider
    {
        public string Name { get; } = name;

        public Task<StoredFileInfo> SaveAsync(SaveFileStorageInput input)
        {
            throw new NotSupportedException();
        }

        public Task<FileDownloadInfo> GetDownloadAsync(FileRecord file)
        {
            throw new NotSupportedException();
        }

        public Task DeleteAsync(FileRecord file)
        {
            throw new NotSupportedException();
        }
    }

    private sealed class StubFileProviderSelector(FileProvider? defaultProvider) : IFileProviderSelector
    {
        public Task<FileProvider?> GetDefaultAsync()
        {
            return Task.FromResult(defaultProvider);
        }

        public Task<FileProvider?> FindAsync(string? provider, string? bucketName)
        {
            throw new NotSupportedException();
        }
    }
}
