using OnceMi.AspNetCore.OSS;

namespace Censeq.FileManagement.Files;

public interface IFileOssServiceManager
{
    Task<IOSSService> GetAsync(FileProvider provider);
}
