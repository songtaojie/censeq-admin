namespace Censeq.FileManagement.Files;

public interface IFileStorageProvider
{
    string Name { get; }

    Task<StoredFileInfo> SaveAsync(SaveFileStorageInput input);

    Task<FileDownloadInfo> GetDownloadAsync(FileRecord file);

    Task DeleteAsync(FileRecord file);
}
