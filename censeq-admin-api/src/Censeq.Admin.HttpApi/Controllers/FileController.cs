using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Censeq.Admin.Files;
using Censeq.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Users;

namespace Censeq.Admin.Controllers;

/// <summary>
/// 文件上传、下载和头像维护接口。
/// </summary>
[Authorize]
[Area("Admin")]
[Route("api/admin/files")]
public class FileController : AbpControllerBase
{
    private static readonly HashSet<string> ImageExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp"
    };

    private const long MaxFileSize = 20 * 1024 * 1024;

    private readonly IRepository<FileRecord, Guid> _fileRepository;
    private readonly IdentityUserManager _userManager;
    private readonly IWebHostEnvironment _environment;
    private readonly IGuidGenerator _guidGenerator;
    private readonly ICurrentTenant _currentTenant;
    private readonly ICurrentUser _currentUser;
    private readonly IObjectMapper _objectMapper;

    public FileController(
        IRepository<FileRecord, Guid> fileRepository,
        IdentityUserManager userManager,
        IWebHostEnvironment environment,
        IGuidGenerator guidGenerator,
        ICurrentTenant currentTenant,
        ICurrentUser currentUser,
        IObjectMapper objectMapper)
    {
        _fileRepository = fileRepository;
        _userManager = userManager;
        _environment = environment;
        _guidGenerator = guidGenerator;
        _currentTenant = currentTenant;
        _currentUser = currentUser;
        _objectMapper = objectMapper;
    }

    /// <summary>
    /// 分页获取文件上传记录。
    /// </summary>
    [HttpGet]
    public virtual async Task<PagedResultDto<FileRecordDto>> GetListAsync([FromQuery] GetFileRecordsInput input)
    {
        var queryable = await _fileRepository.GetQueryableAsync();
        var query = queryable
            .WhereIf(!input.Filter.IsNullOrWhiteSpace(),
                x => x.OriginalName.Contains(input.Filter!) || x.FileName.Contains(input.Filter!))
            .WhereIf(!input.Category.IsNullOrWhiteSpace(), x => x.Category == input.Category);

        var totalCount = query.Count();
        var items = query
            .OrderByDescending(x => x.CreationTime)
            .Skip(input.SkipCount)
            .Take(input.MaxResultCount)
            .ToList();

        return new PagedResultDto<FileRecordDto>(
            totalCount,
            _objectMapper.Map<List<FileRecord>, List<FileRecordDto>>(items)
        );
    }

    /// <summary>
    /// 上传通用文件，并写入文件记录。
    /// </summary>
    [HttpPost]
    [Consumes("multipart/form-data")]
    public virtual async Task<FileRecordDto> UploadAsync([FromForm] UploadFileRequest input)
    {
        return await SaveFileAsync(input.File, input.Category, input.IsPublic, input.AllowImageOnly);
    }

    /// <summary>
    /// 上传当前用户头像，并同步更新用户扩展属性 AvatarUrl。
    /// </summary>
    [HttpPost("avatar")]
    [Consumes("multipart/form-data")]
    public virtual async Task<FileRecordDto> UploadAvatarAsync([FromForm] IFormFile file)
    {
        var dto = await SaveFileAsync(file, "avatar", true, true);

        var user = await _userManager.GetByIdAsync(_currentUser.GetId());
        var oldAvatarUrl = user.GetProperty<string>("AvatarUrl", null);
        user.SetProperty("AvatarUrl", dto.Url);
        CheckIdentityResult(await _userManager.UpdateAsync(user));

        await TryDeleteOldAvatarAsync(oldAvatarUrl, dto.Url);
        return dto;
    }

    /// <summary>
    /// 下载指定文件。
    /// </summary>
    [HttpGet("{id:guid}/download")]
    public virtual async Task<IActionResult> DownloadAsync(Guid id)
    {
        var file = await _fileRepository.GetAsync(id);
        var fullPath = GetFullPath(file.RelativePath);
        if (!System.IO.File.Exists(fullPath))
        {
            throw new UserFriendlyException("文件不存在");
        }

        return PhysicalFile(fullPath, file.ContentType, file.OriginalName);
    }

    /// <summary>
    /// 删除文件记录及其物理文件。
    /// </summary>
    [HttpDelete("{id:guid}")]
    public virtual async Task DeleteAsync(Guid id)
    {
        var file = await _fileRepository.GetAsync(id);
        await _fileRepository.DeleteAsync(file);
        DeletePhysicalFile(file.RelativePath);
    }

    private async Task<FileRecordDto> SaveFileAsync(IFormFile? file, string? category, bool isPublic, bool allowImageOnly)
    {
        if (file == null || file.Length <= 0)
        {
            throw new UserFriendlyException("请选择要上传的文件");
        }

        if (file.Length > MaxFileSize)
        {
            throw new UserFriendlyException("文件大小不能超过 20MB");
        }

        if (file.FileName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
        {
            throw new UserFriendlyException("文件名包含非法字符");
        }

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (extension.IsNullOrWhiteSpace())
        {
            throw new UserFriendlyException("文件缺少扩展名");
        }

        if (allowImageOnly && !ImageExtensions.Contains(extension))
        {
            throw new UserFriendlyException("仅支持 jpg、png、gif、bmp、webp 图片");
        }

        var id = _guidGenerator.Create();
        var safeCategory = NormalizeCategory(category);
        var datePath = Clock.Now.ToString("yyyy/MM/dd");
        var relativeDirectory = Path.Combine("uploads", safeCategory, datePath);
        var fileName = $"{id:N}{extension}";
        var relativePath = Path.Combine(relativeDirectory, fileName);
        var fullDirectory = GetFullPath(relativeDirectory);
        Directory.CreateDirectory(fullDirectory);

        var fullPath = GetFullPath(relativePath);
        await using (var stream = System.IO.File.Create(fullPath))
        {
            await file.CopyToAsync(stream);
        }

        var record = new FileRecord(id, _currentTenant.Id)
        {
            OwnerUserId = _currentUser.Id,
            OriginalName = Path.GetFileName(file.FileName),
            FileName = Path.GetFileNameWithoutExtension(file.FileName),
            Extension = extension,
            ContentType = file.ContentType.IsNullOrWhiteSpace() ? "application/octet-stream" : file.ContentType,
            RelativePath = ToUrlPath(relativePath),
            Url = "/" + ToUrlPath(relativePath),
            Size = file.Length,
            Hash = await ComputeHashAsync(fullPath),
            Category = safeCategory,
            IsPublic = isPublic
        };

        await _fileRepository.InsertAsync(record, autoSave: true);
        return _objectMapper.Map<FileRecord, FileRecordDto>(record);
    }

    private async Task TryDeleteOldAvatarAsync(string? oldAvatarUrl, string newAvatarUrl)
    {
        if (oldAvatarUrl.IsNullOrWhiteSpace() || oldAvatarUrl == newAvatarUrl)
        {
            return;
        }

        var queryable = await _fileRepository.GetQueryableAsync();
        var oldFile = queryable.FirstOrDefault(x => x.Url == oldAvatarUrl && x.Category == "avatar");
        if (oldFile == null)
        {
            return;
        }

        await _fileRepository.DeleteAsync(oldFile);
        DeletePhysicalFile(oldFile.RelativePath);
    }

    private string GetFullPath(string relativePath)
    {
        var root = _environment.WebRootPath;
        if (root.IsNullOrWhiteSpace())
        {
            root = Path.Combine(_environment.ContentRootPath, "wwwroot");
        }

        return Path.Combine(root, relativePath.Replace('/', Path.DirectorySeparatorChar));
    }

    private static string NormalizeCategory(string? category)
    {
        var value = category.IsNullOrWhiteSpace() ? "common" : category.Trim().ToLowerInvariant();
        var chars = value.Where(c => char.IsLetterOrDigit(c) || c is '-' or '_').ToArray();
        return chars.Length == 0 ? "common" : new string(chars);
    }

    private static string ToUrlPath(string path)
    {
        return path.Replace('\\', '/').TrimStart('/');
    }

    private static async Task<string> ComputeHashAsync(string fullPath)
    {
        await using var stream = System.IO.File.OpenRead(fullPath);
        var hash = await SHA256.HashDataAsync(stream);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    private void DeletePhysicalFile(string relativePath)
    {
        var fullPath = GetFullPath(relativePath);
        if (System.IO.File.Exists(fullPath))
        {
            System.IO.File.Delete(fullPath);
        }
    }

    private static void CheckIdentityResult(IdentityResult result)
    {
        if (!result.Succeeded)
        {
            throw new UserFriendlyException(string.Join("; ", result.Errors.Select(x => x.Description)));
        }
    }
}

/// <summary>
/// 通用文件上传请求。
/// </summary>
public class UploadFileRequest
{
    /// <summary>
    /// 待上传文件。
    /// </summary>
    public IFormFile? File { get; set; }

    /// <summary>
    /// 业务分类，未传时默认为 common。
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// 是否作为公开文件保存。
    /// </summary>
    public bool IsPublic { get; set; }

    /// <summary>
    /// 是否限制为图片类型。
    /// </summary>
    public bool AllowImageOnly { get; set; }
}
