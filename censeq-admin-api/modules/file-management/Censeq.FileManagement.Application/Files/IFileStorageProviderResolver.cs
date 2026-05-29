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
    /// 按文件记录中保存的提供器信息解析存储提供器。
    /// </summary>
    IFileStorageProvider Resolve(FileRecord file);
}
