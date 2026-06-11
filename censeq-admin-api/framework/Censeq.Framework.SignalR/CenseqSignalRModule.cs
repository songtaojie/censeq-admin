using Censeq.Framework.SignalR.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AspNetCore;
using Volo.Abp.Caching;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;

namespace Censeq.Framework.SignalR;

/// <summary>
/// SignalR 框架模块，负责注册 Hub 基础能力、在线用户注册表和实时通知发送服务。
/// </summary>
[DependsOn(
    typeof(AbpAspNetCoreModule),
    typeof(AbpCachingModule),
    typeof(AbpMultiTenancyModule)
)]
public class CenseqSignalRModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();
        var options = configuration.GetSection("SignalR").Get<Options.CenseqSignalROptions>() ?? new();

        context.Services.AddSignalR(signalROptions =>
        {
            signalROptions.EnableDetailedErrors = options.EnableDetailedErrors;
            signalROptions.KeepAliveInterval = TimeSpan.FromSeconds(options.KeepAliveSeconds);
            signalROptions.ClientTimeoutInterval = TimeSpan.FromSeconds(options.ClientTimeoutSeconds);
        });

        context.Services.AddSingleton<IUserIdProvider, AbpUserIdProvider>();
        context.Services.AddSingleton<IOnlineUserRegistry, OnlineUserRegistry>();
        context.Services.AddTransient<INotificationSender, NotificationSender>();
    }
}
