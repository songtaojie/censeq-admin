namespace Censeq.Framework.SignalR.Extensions;

/// <summary>
/// 标记 Hub 对外暴露的路由地址，供统一端点映射时自动发现。
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class CenseqHubRouteAttribute : Attribute
{
    /// <summary>
    /// 创建 Hub 路由标记。
    /// </summary>
    public CenseqHubRouteAttribute(string route)
    {
        Route = route;
    }

    /// <summary>
    /// Hub 对外暴露的路由地址。
    /// </summary>
    public string Route { get; }
}
