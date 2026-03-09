using Censeq.Framework.Core;
using Volo.Abp;
using Volo.Abp.AspNetCore;
using Volo.Abp.Modularity;

namespace Censeq.Framework.AspNetCore
{
    /// <summary>
    /// CenseqAbpAspNetCore模块入口
    /// </summary>
    [DependsOn(
     typeof(AbpAspNetCoreModule)
     )]
    public class CenseqAspNetCoreModule : CenseqAbpModule
    {

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddCenseqCors();
        }


        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            var app = context.GetApplicationBuilder();
            app?.UseCenseqCors();
        }
    }
}
