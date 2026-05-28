using AutoMapper;
using Censeq.FileManagement.Files;

namespace Censeq.FileManagement;

public class CenseqFileManagementApplicationAutoMapperProfile : Profile
{
    public CenseqFileManagementApplicationAutoMapperProfile()
    {
        CreateMap<FileRecord, FileRecordDto>();
        CreateMap<FileProvider, FileProviderDto>();
    }
}
