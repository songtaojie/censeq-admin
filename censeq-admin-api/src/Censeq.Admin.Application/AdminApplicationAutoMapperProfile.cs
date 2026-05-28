using AutoMapper;
using Censeq.Admin.Files;

namespace Censeq.Admin;

public class AdminApplicationAutoMapperProfile : Profile
{
    public AdminApplicationAutoMapperProfile()
    {
        CreateMap<FileRecord, FileRecordDto>();
    }
}
