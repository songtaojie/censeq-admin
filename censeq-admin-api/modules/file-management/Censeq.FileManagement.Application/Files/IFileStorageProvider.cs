namespace Censeq.FileManagement.Files;

/// <summary>
/// 文件物理存储提供器抽象。
/// </summary>
public interface IFileStorageProvider
{
    /// <summary>
    /// 存储提供器名称。
    /// </summary>
    string Name { get; }

    /// <summary>
    /// 保存文件流并返回存储后的路径、访问地址和提供器信息。
    /// </summary>
    Task<StoredFileInfo> SaveAsync(SaveFileStorageInput input);

    /// <summary>
    /// 根据文件记录创建下载结果。
    /// </summary>
    Task<FileDownloadInfo> GetDownloadAsync(FileRecord file);

    /// <summary>
    /// 删除文件记录对应的物理文件。
    /// </summary>
    Task DeleteAsync(FileRecord file);
}
