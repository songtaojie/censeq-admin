namespace Censeq.FileManagement.Files;

/// <summary>
/// 文件存储提供器解析器。
/// </summary>
public interface IFileStorageProviderResolver
{
    /// <summary>
    /// 按当前配置解析默认文件存储提供器。
    /// </summary>
    IFileStorageProvider Resolve();

    /// <summary>
    /// 解析上传时使用的文件存储提供器，数据库存在可用 OSS 配置时优先使用 OSS。
    /// </summary>
    Task<IFileStorageProvider> ResolveForUploadAsync();

    /// <summary>
    /// 按文件记录中保存的提供器信息解析存储提供器。
    /// </summary>
    IFileStorageProvider Resolve(FileRecord file);
}
