using AutoMapper;
using Censeq.FileManagement.Files;

namespace Censeq.FileManagement;

/// <summary>
/// 文件管理应用层对象映射配置。
/// </summary>
public class CenseqFileManagementApplicationAutoMapperProfile : Profile
{
    /// <summary>
    /// 建立文件记录、存储提供器实体到对应 DTO 的映射关系。
    /// </summary>
    public CenseqFileManagementApplicationAutoMapperProfile()
    {
        CreateMap<FileRecord, FileRecordDto>();
        CreateMap<FileProvider, FileProviderDto>();
    }
}
