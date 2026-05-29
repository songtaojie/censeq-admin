using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;
using Volo.Abp.ObjectMapping;

namespace Censeq.FileManagement.Files;

/// <summary>
/// 文件存储提供器配置应用服务。
/// </summary>
public class FileProviderAppService : IFileProviderAppService, ITransientDependency
{
    private readonly IRepository<FileProvider, Guid> _repository;
    private readonly IGuidGenerator _guidGenerator;
    private readonly ICurrentTenant _currentTenant;
    private readonly IObjectMapper _objectMapper;

    public FileProviderAppService(
        IRepository<FileProvider, Guid> repository,
        IGuidGenerator guidGenerator,
        ICurrentTenant currentTenant,
        IObjectMapper objectMapper)
    {
        _repository = repository;
        _guidGenerator = guidGenerator;
        _currentTenant = currentTenant;
        _objectMapper = objectMapper;
    }

    /// <summary>
    /// 分页查询文件存储提供器配置。
    /// </summary>
    public async Task<PagedResultDto<FileProviderDto>> GetListAsync(GetFileProvidersInput input)
    {
        var queryable = await _repository.GetQueryableAsync();
        var query = queryable
            .WhereIf(!input.Filter.IsNullOrWhiteSpace(), x =>
                x.Provider.Contains(input.Filter!) ||
                x.BucketName.Contains(input.Filter!) ||
                (x.Remark != null && x.Remark.Contains(input.Filter!)))
            .WhereIf(!input.Provider.IsNullOrWhiteSpace(), x => x.Provider == input.Provider)
            .WhereIf(input.IsEnable.HasValue, x => x.IsEnable == input.IsEnable);

        var totalCount = query.Count();
        var items = query
            .OrderByDescending(x => x.IsDefault)
            .ThenBy(x => x.OrderNo)
            .Skip(input.SkipCount)
            .Take(input.MaxResultCount)
            .ToList();

        return new PagedResultDto<FileProviderDto>(totalCount, items.Select(MapToDto).ToList());
    }

    /// <summary>
    /// 获取指定文件存储提供器配置。
    /// </summary>
    public async Task<FileProviderDto> GetAsync(Guid id)
    {
        return MapToDto(await _repository.GetAsync(id));
    }

    /// <summary>
    /// 创建文件存储提供器配置。
    /// </summary>
    public async Task<FileProviderDto> CreateAsync(CreateUpdateFileProviderDto input)
    {
        Validate(input);

        var entity = new FileProvider(_guidGenerator.Create(), _currentTenant.Id);
        MapToEntity(input, entity);

        if (entity.IsDefault)
        {
            await ClearDefaultAsync(entity.Id);
        }

        await _repository.InsertAsync(entity, autoSave: true);
        return MapToDto(entity);
    }

    /// <summary>
    /// 更新文件存储提供器配置。
    /// </summary>
    public async Task<FileProviderDto> UpdateAsync(Guid id, CreateUpdateFileProviderDto input)
    {
        Validate(input);

        var entity = await _repository.GetAsync(id);
        MapToEntity(input, entity);

        if (entity.IsDefault)
        {
            await ClearDefaultAsync(entity.Id);
        }

        await _repository.UpdateAsync(entity, autoSave: true);
        return MapToDto(entity);
    }

    /// <summary>
    /// 删除文件存储提供器配置。
    /// </summary>
    public async Task DeleteAsync(Guid id)
    {
        await _repository.DeleteAsync(id);
    }

    /// <summary>
    /// 将指定文件存储提供器设置为默认配置。
    /// </summary>
    public async Task SetDefaultAsync(Guid id)
    {
        var entity = await _repository.GetAsync(id);
        entity.IsDefault = true;
        entity.IsEnable = true;

        await ClearDefaultAsync(id);
        await _repository.UpdateAsync(entity, autoSave: true);
    }

    private async Task ClearDefaultAsync(Guid exceptId)
    {
        var queryable = await _repository.GetQueryableAsync();
        var defaults = queryable.Where(x => x.Id != exceptId && x.IsDefault).ToList();
        foreach (var item in defaults)
        {
            item.IsDefault = false;
            await _repository.UpdateAsync(item, autoSave: false);
        }
    }

    private static void Validate(CreateUpdateFileProviderDto input)
    {
        if (input.Provider.IsNullOrWhiteSpace())
        {
            throw new UserFriendlyException("请选择 OSS 提供器");
        }

        if (input.BucketName.IsNullOrWhiteSpace())
        {
            throw new UserFriendlyException("请填写 BucketName");
        }
    }

    private static void MapToEntity(CreateUpdateFileProviderDto input, FileProvider entity)
    {
        entity.Provider = input.Provider.Trim();
        entity.BucketName = input.BucketName.Trim();
        entity.AccessKey = input.AccessKey?.Trim();
        if (!input.SecretKey.IsNullOrWhiteSpace())
        {
            entity.SecretKey = input.SecretKey.Trim();
        }

        entity.Region = input.Region?.Trim();
        entity.Endpoint = input.Endpoint?.Trim();
        entity.IsEnableHttps = input.IsEnableHttps;
        entity.IsEnableCache = input.IsEnableCache;
        entity.IsEnable = input.IsEnable;
        entity.IsDefault = input.IsDefault;
        entity.CustomDomain = input.CustomDomain?.Trim();
        entity.OrderNo = input.OrderNo;
        entity.Remark = input.Remark?.Trim();
    }

    private FileProviderDto MapToDto(FileProvider entity)
    {
        var dto = _objectMapper.Map<FileProvider, FileProviderDto>(entity);
        dto.DisplayName = entity.DisplayName;
        return dto;
    }
}
