using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;

namespace Censeq.FileManagement.Files;

public class LocalFileStorageProvider : IFileStorageProvider, ITransientDependency
{
    private readonly IWebHostEnvironment _environment;

    public LocalFileStorageProvider(IWebHostEnvironment environment, IOptions<FileStorageOptions> options)
    {
        _environment = environment;
    }

    public string Name => FileStorageProviderNames.Local;

    public async Task<StoredFileInfo> SaveAsync(SaveFileStorageInput input)
    {
        var fullDirectory = GetFullPath(input.RelativeDirectory);
        Directory.CreateDirectory(fullDirectory);

        var fullPath = GetFullPath(input.RelativePath);
        await using (var stream = File.Create(fullPath))
        {
            await input.Stream.CopyToAsync(stream);
        }

        var relativePath = ToUrlPath(input.RelativePath);
        return new StoredFileInfo
        {
            RelativePath = relativePath,
            Url = "/" + relativePath,
            Provider = Name
        };
    }

    public Task<FileDownloadInfo> GetDownloadAsync(FileRecord file)
    {
        var fullPath = GetFullPath(file.RelativePath);
        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException("文件不存在", fullPath);
        }

        var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        return Task.FromResult(new FileDownloadInfo
        {
            Result = new FileStreamResult(stream, file.ContentType)
            {
                FileDownloadName = file.OriginalName
            }
        });
    }

    public Task DeleteAsync(FileRecord file)
    {
        var fullPath = GetFullPath(file.RelativePath);
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }

        return Task.CompletedTask;
    }

    private string GetFullPath(string relativePath)
    {
        var root = _environment.WebRootPath;
        if (root.IsNullOrWhiteSpace())
        {
            root = Path.Combine(_environment.ContentRootPath, "wwwroot");
        }

        return Path.Combine(root, relativePath.Replace('/', Path.DirectorySeparatorChar));
    }

    private static string ToUrlPath(string path)
    {
        return path.Replace('\\', '/').TrimStart('/');
    }
}
