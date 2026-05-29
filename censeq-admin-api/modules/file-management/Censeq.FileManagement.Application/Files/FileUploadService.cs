using System.Security.Cryptography;
using Censeq.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Users;

namespace Censeq.FileManagement.Files;

/// <summary>
/// 文件上传应用服务，负责校验文件、保存物理文件、维护文件记录和用户头像地址。
/// </summary>
public class FileUploadService : IFileUploadService, ITransientDependency
{
    private readonly IRepository<FileRecord, Guid> _fileRepository;
    private readonly IdentityUserManager _userManager;
    private readonly IGuidGenerator _guidGenerator;
    private readonly ICurrentTenant _currentTenant;
    private readonly ICurrentUser _currentUser;
    private readonly IObjectMapper _objectMapper;
    private readonly IFileStorageProviderResolver _storageProviderResolver;
    private readonly FileStorageOptions _options;

    public FileUploadService(
        IRepository<FileRecord, Guid> fileRepository,
        IdentityUserManager userManager,
        IGuidGenerator guidGenerator,
        ICurrentTenant currentTenant,
        ICurrentUser currentUser,
        IObjectMapper objectMapper,
        IFileStorageProviderResolver storageProviderResolver,
        IOptions<FileStorageOptions> options)
    {
        _fileRepository = fileRepository;
        _userManager = userManager;
        _guidGenerator = guidGenerator;
        _currentTenant = currentTenant;
        _currentUser = currentUser;
        _objectMapper = objectMapper;
        _storageProviderResolver = storageProviderResolver;
        _options = options.Value;
    }

    /// <summary>
    /// 分页查询文件上传记录。
    /// </summary>
    public virtual async Task<PagedResultDto<FileRecordDto>> GetListAsync(GetFileRecordsInput input)
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

        return new PagedResultDto<FileRecordDto>(totalCount, items.Select(MapToDto).ToList());
    }

    /// <summary>
    /// 上传通用文件并保存文件记录。
    /// </summary>
    public virtual async Task<FileRecordDto> UploadAsync(IFormFile? file, string? category, bool isPublic, bool allowImageOnly)
    {
        return await SaveFileAsync(file, category, isPublic, allowImageOnly);
    }

    /// <summary>
    /// 上传当前用户头像并同步更新用户扩展属性 AvatarUrl。
    /// </summary>
    public virtual async Task<FileRecordDto> UploadAvatarAsync(IFormFile? file)
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
    public virtual async Task<FileStreamResult> DownloadAsync(Guid id)
    {
        var file = await _fileRepository.GetAsync(id);
        return (await _storageProviderResolver.Resolve(file).GetDownloadAsync(file)).Result;
    }

    /// <summary>
    /// 删除文件记录及对应的物理文件。
    /// </summary>
    public virtual async Task DeleteAsync(Guid id)
    {
        var file = await _fileRepository.GetAsync(id);
        await _fileRepository.DeleteAsync(file);
        await _storageProviderResolver.Resolve(file).DeleteAsync(file);
    }

    private async Task<FileRecordDto> SaveFileAsync(IFormFile? file, string? category, bool isPublic, bool allowImageOnly)
    {
        ValidateFile(file, allowImageOnly);

        var id = _guidGenerator.Create();
        var safeCategory = NormalizeCategory(category);
        var datePath = DateTime.Now.ToString("yyyy/MM/dd");
        var relativeDirectory = Path.Combine(_options.Local.BasePath, safeCategory, datePath);
        var extension = Path.GetExtension(file!.FileName).ToLowerInvariant();
        var storedName = $"{id:N}{extension}";
        var relativePath = Path.Combine(relativeDirectory, storedName);
        var provider = await _storageProviderResolver.ResolveForUploadAsync();
        var contentType = GetContentType(file);

        var hash = await ComputeHashAsync(file);
        await using var stream = file.OpenReadStream();
        var stored = await provider.SaveAsync(new SaveFileStorageInput
        {
            Stream = stream,
            OriginalName = Path.GetFileName(file.FileName),
            StoredName = storedName,
            Extension = extension,
            ContentType = contentType,
            Category = safeCategory,
            RelativeDirectory = relativeDirectory,
            RelativePath = relativePath
        });

        var record = new FileRecord(id, _currentTenant.Id)
        {
            OwnerUserId = _currentUser.Id,
            OriginalName = Path.GetFileName(file.FileName),
            FileName = Path.GetFileNameWithoutExtension(file.FileName),
            Extension = extension,
            ContentType = contentType,
            RelativePath = stored.RelativePath,
            Url = stored.Url,
            Size = file.Length,
            Hash = hash,
            Category = safeCategory,
            IsPublic = isPublic,
            Provider = stored.Provider,
            StorageProvider = stored.StorageProvider,
            BucketName = stored.BucketName
        };

        await _fileRepository.InsertAsync(record, autoSave: true);
        return MapToDto(record);
    }

    private void ValidateFile(IFormFile? file, bool allowImageOnly)
    {
        if (file == null || file.Length <= 0)
        {
            throw new UserFriendlyException("请选择要上传的文件");
        }

        if (file.Length > _options.MaxFileSize)
        {
            throw new UserFriendlyException($"文件大小不能超过 {_options.MaxFileSize / 1024 / 1024}MB");
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

        if (allowImageOnly && !_options.ImageExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
        {
            throw new UserFriendlyException($"仅支持 {string.Join('、', _options.ImageExtensions)} 图片");
        }
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
        await _storageProviderResolver.Resolve(oldFile).DeleteAsync(oldFile);
    }

    private static string NormalizeCategory(string? category)
    {
        var value = category.IsNullOrWhiteSpace() ? "common" : category.Trim().ToLowerInvariant();
        var chars = value.Where(c => char.IsLetterOrDigit(c) || c is '-' or '_').ToArray();
        return chars.Length == 0 ? "common" : new string(chars);
    }

    private static async Task<string> ComputeHashAsync(IFormFile file)
    {
        await using var stream = file.OpenReadStream();
        var hash = await SHA256.HashDataAsync(stream);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    private static string GetContentType(IFormFile file)
    {
        return file.ContentType.IsNullOrWhiteSpace() ? "application/octet-stream" : file.ContentType;
    }

    private static void CheckIdentityResult(IdentityResult result)
    {
        if (!result.Succeeded)
        {
            throw new UserFriendlyException(string.Join("; ", result.Errors.Select(x => x.Description)));
        }
    }

    private FileRecordDto MapToDto(FileRecord record)
    {
        var dto = _objectMapper.Map<FileRecord, FileRecordDto>(record);
        dto.Provider = record.Provider.IsNullOrWhiteSpace()
            ? FileStorageProviderNames.Local
            : record.Provider;
        dto.StorageProvider = ResolveStorageProviderName(record);
        dto.BucketName = record.BucketName;
        return dto;
    }

    private static string ResolveStorageProviderName(FileRecord record)
    {
        if (!record.StorageProvider.IsNullOrWhiteSpace())
        {
            return record.StorageProvider!;
        }

        if (record.Provider.Equals(FileStorageProviderNames.Local, StringComparison.OrdinalIgnoreCase))
        {
            return FileStorageProviderNames.Local;
        }

        return record.BucketName.IsNullOrWhiteSpace()
            ? FileStorageProviderNames.Local
            : FileStorageProviderNames.Oss;
    }
}
