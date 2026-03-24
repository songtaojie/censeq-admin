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
            // CORS 须在 UseRouting 之后注册，见各 Host 模块（如 CenseqHttpApiHostModule）。
        }
    }
}
