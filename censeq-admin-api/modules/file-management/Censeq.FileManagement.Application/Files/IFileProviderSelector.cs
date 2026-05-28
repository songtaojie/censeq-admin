namespace Censeq.FileManagement.Files;

public interface IFileProviderSelector
{
    Task<FileProvider?> GetDefaultAsync();

    Task<FileProvider?> FindAsync(string? provider, string? bucketName);
}
