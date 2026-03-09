using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;
using Microsoft.AspNetCore.Builder;
using Volo.Abp.DependencyInjection;
using Censeq.Framework.Core;
using Microsoft.Extensions.Options;
using Volo.Abp.AspNetCore.Mvc;

namespace Censeq.Framework.Swashbuckle
{
    /// <summary>
    /// Swashbuckle模块入口
    /// </summary>
    [DependsOn(typeof(AbpAspNetCoreMvcModule))]
    public class CenseqSwashbuckleModule : CenseqAbpModule
    {
        /// <summary>
        /// 配置服务
        /// </summary>
        /// <param name="context"></param>
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.AddEmbedded<CenseqSwashbuckleModule>();
            });
            context.Services.AddCenseqSwaggerGen();
            context.Services.AddEndpointsApiExplorer();
        }

        /// <summary>
        /// 配置应用程序
        /// </summary>
        /// <param name="context"></param>
        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            var app = context.GetApplicationBuilder();
            var options = app.ApplicationServices.GetRequiredService<IOptions<SwaggerSettingsOptions>>().Value;
            if (options.SwaggerUI == 2)
            {
                app.UseCenseqSwaggerKnife4j();
            }
            else
            {
                app.UseCenseqSwagger();
            }
        }
    }
}
