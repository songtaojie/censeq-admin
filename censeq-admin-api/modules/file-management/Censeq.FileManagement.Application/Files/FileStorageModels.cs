using System.IO;
using Microsoft.AspNetCore.Mvc;

namespace Censeq.FileManagement.Files;

/// <summary>
/// 保存文件到物理存储时使用的输入模型。
/// </summary>
public class SaveFileStorageInput
{
    /// <summary>
    /// 待保存的文件流。
    /// </summary>
    public required Stream Stream { get; init; }

    /// <summary>
    /// 用户上传时的原始文件名。
    /// </summary>
    public required string OriginalName { get; init; }

    /// <summary>
    /// 生成后的存储文件名。
    /// </summary>
    public required string StoredName { get; init; }

    /// <summary>
    /// 文件扩展名，包含前导点。
    /// </summary>
    public required string Extension { get; init; }

    /// <summary>
    /// 文件 MIME 类型。
    /// </summary>
    public required string ContentType { get; init; }

    /// <summary>
    /// 文件业务分类。
    /// </summary>
    public required string Category { get; init; }

    /// <summary>
    /// 文件所在的相对目录。
    /// </summary>
    public required string RelativeDirectory { get; init; }

    /// <summary>
    /// 文件完整相对路径。
    /// </summary>
    public required string RelativePath { get; init; }
}

/// <summary>
/// 文件保存后的存储结果。
/// </summary>
public class StoredFileInfo
{
    /// <summary>
    /// 存储提供器返回的文件相对路径。
    /// </summary>
    public required string RelativePath { get; init; }

    /// <summary>
    /// 可供客户端访问的文件地址。
    /// </summary>
    public required string Url { get; init; }

    /// <summary>
    /// 实际使用的存储提供器名称。
    /// </summary>
    public required string Provider { get; init; }

    /// <summary>
    /// OSS Bucket 名称。
    /// </summary>
    public string? BucketName { get; init; }
}

/// <summary>
/// 文件下载结果包装模型。
/// </summary>
public class FileDownloadInfo
{
    /// <summary>
    /// ASP.NET Core 文件流下载结果。
    /// </summary>
    public required FileStreamResult Result { get; init; }
}
