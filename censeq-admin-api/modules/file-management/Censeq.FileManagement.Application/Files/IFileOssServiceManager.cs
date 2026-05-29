using OnceMi.AspNetCore.OSS;

namespace Censeq.FileManagement.Files;

/// <summary>
/// OSS 服务实例管理器，按文件存储提供器配置创建并缓存 OSS 客户端。
/// </summary>
public interface IFileOssServiceManager
{
    /// <summary>
    /// 获取指定文件存储提供器对应的 OSS 服务实例。
    /// </summary>
    Task<IOSSService> GetAsync(FileProvider provider);
}
