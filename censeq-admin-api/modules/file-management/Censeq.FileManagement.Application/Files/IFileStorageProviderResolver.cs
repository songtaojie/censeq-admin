namespace Censeq.FileManagement.Files;

public interface IFileStorageProviderResolver
{
    IFileStorageProvider Resolve();

    IFileStorageProvider Resolve(FileRecord file);
}
