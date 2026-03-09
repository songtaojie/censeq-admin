using Censeq.Framework.Core;
using Volo.Abp;
using Volo.Abp.AspNetCore;
using Volo.Abp.Modularity;

namespace Censeq.Framework.AspNetCore
{
    /// <summary>
    /// StarshineAbpAspNetCore模块入口
    /// </summary>
    [DependsOn(
     typeof(AbpAspNetCoreModule)
     )]
    public class StarshineAspNetCoreModule : StarshineAbpModule
    {

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddStarshineCors();
        }


        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            var app = context.GetApplicationBuilder();
            app?.UseStarshineCors();
        }
    }
}
