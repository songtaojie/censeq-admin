using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.SignalR;

namespace Censeq.Framework.SignalR.Extensions;

/// <summary>
/// SignalR 端点映射扩展。
/// </summary>
public static class SignalRApplicationBuilderExtensions
{
    private static readonly MethodInfo MapHubMethod = typeof(HubEndpointRouteBuilderExtensions)
        .GetMethods(BindingFlags.Public | BindingFlags.Static)
        .Single(x =>
            x.Name == nameof(HubEndpointRouteBuilderExtensions.MapHub) &&
            x.IsGenericMethodDefinition &&
            x.GetParameters().Length == 2 &&
            x.GetParameters()[1].ParameterType == typeof(string));

    /// <summary>
    /// 自动发现当前程序集内带有 <see cref="CenseqHubRouteAttribute"/> 的 Hub 并映射到端点路由。
    /// </summary>
    public static IEndpointRouteBuilder MapCenseqHubs(this IEndpointRouteBuilder endpoints)
    {
        var hubTypes = typeof(SignalRApplicationBuilderExtensions).Assembly
            .GetTypes()
            .Where(x => !x.IsAbstract && typeof(Hub).IsAssignableFrom(x))
            .Select(x => new
            {
                HubType = x,
                Route = x.GetCustomAttribute<CenseqHubRouteAttribute>()?.Route
            })
            .Where(x => !string.IsNullOrWhiteSpace(x.Route));

        foreach (var hub in hubTypes)
        {
            MapHubMethod.MakeGenericMethod(hub.HubType).Invoke(null, [endpoints, hub.Route!]);
        }

        return endpoints;
    }
}
