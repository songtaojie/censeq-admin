using System.IO;
using Microsoft.AspNetCore.Mvc;

namespace Censeq.FileManagement.Files;

public class SaveFileStorageInput
{
    public required Stream Stream { get; init; }

    public required string OriginalName { get; init; }

    public required string StoredName { get; init; }

    public required string Extension { get; init; }

    public required string ContentType { get; init; }

    public required string Category { get; init; }

    public required string RelativeDirectory { get; init; }

    public required string RelativePath { get; init; }
}

public class StoredFileInfo
{
    public required string RelativePath { get; init; }

    public required string Url { get; init; }

    public required string Provider { get; init; }

    public string? BucketName { get; init; }
}

public class FileDownloadInfo
{
    public required FileStreamResult Result { get; init; }
}
