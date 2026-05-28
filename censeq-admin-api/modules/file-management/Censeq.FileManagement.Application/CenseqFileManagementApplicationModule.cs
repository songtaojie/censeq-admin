using Censeq.FileManagement.Files;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OnceMi.AspNetCore.OSS;
using Volo.Abp.Application;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;

namespace Censeq.FileManagement;

[DependsOn(
    typeof(AbpDddApplicationModule),
    typeof(AbpAutoMapperModule),
    typeof(CenseqFileManagementApplicationContractsModule),
    typeof(CenseqFileManagementDomainModule)
)]
public class CenseqFileManagementApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();
        context.Services.Configure<FileStorageOptions>(configuration.GetSection(FileStorageOptions.SectionName));
        context.Services.AddHttpClient<OssFileStorageProvider>();

        var fileStorageOptions = configuration
            .GetSection(FileStorageOptions.SectionName)
            .Get<FileStorageOptions>() ?? new FileStorageOptions();
        if (fileStorageOptions.Oss.Enabled)
        {
            context.Services.AddOSSService(Enum.GetName(fileStorageOptions.Oss.Provider), $"{FileStorageOptions.SectionName}:Oss");
        }

        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<CenseqFileManagementApplicationModule>();
        });
    }
}
