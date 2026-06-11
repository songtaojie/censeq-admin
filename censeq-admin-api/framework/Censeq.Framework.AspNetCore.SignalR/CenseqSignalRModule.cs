using Censeq.Framework.AspNetCore.SignalR.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AspNetCore.SignalR;
using Volo.Abp.Caching;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;

namespace Censeq.Framework.AspNetCore.SignalR;

/// <summary>
/// SignalR 框架模块，负责配置 Hub 参数、注册在线用户连接表和实时通知发送服务。
/// </summary>
[DependsOn(
    typeof(AbpAspNetCoreSignalRModule),
    typeof(AbpCachingModule),
    typeof(AbpMultiTenancyModule)
)]
public class CenseqSignalRModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();
        var options = configuration.GetSection("SignalR").Get<Options.CenseqSignalROptions>() ?? new();

        Configure<HubOptions>(signalROptions =>
        {
            signalROptions.EnableDetailedErrors = options.EnableDetailedErrors;
            signalROptions.KeepAliveInterval = TimeSpan.FromSeconds(options.KeepAliveSeconds);
            signalROptions.ClientTimeoutInterval = TimeSpan.FromSeconds(options.ClientTimeoutSeconds);
        });

        context.Services.AddSingleton<IOnlineUserRegistry, OnlineUserRegistry>();
        context.Services.AddTransient<INotificationSender, NotificationSender>();
    }
}