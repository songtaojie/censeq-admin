namespace Censeq.FileManagement.Files;

/// <summary>
/// 文件记录实体字段长度限制。
/// </summary>
public static class FileRecordConsts
{
    public const int MaxNameLength = 256;
    public const int MaxPathLength = 512;
    public const int MaxContentTypeLength = 128;
    public const int MaxHashLength = 128;
    public const int MaxCategoryLength = 64;
    public const int MaxProviderLength = 64;
    public const int MaxStorageProviderLength = 32;
    public const int MaxBucketNameLength = 128;
}
