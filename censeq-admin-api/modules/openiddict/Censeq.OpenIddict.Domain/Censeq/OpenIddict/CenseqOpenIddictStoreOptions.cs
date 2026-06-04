using System.Data;

namespace Censeq.OpenIddict;

/// <summary>
/// OpenIddict存储配置项，用于配置相关行为。
/// </summary>
public class CenseqOpenIddictStoreOptions
{
    /// <summary>
    /// 清理隔离级别。
    /// </summary>
    public IsolationLevel? PruneIsolationLevel { get; set; }

    /// <summary>
    /// 删除隔离级别。
    /// </summary>
    public IsolationLevel? DeleteIsolationLevel { get; set; }

    /// <summary>
    /// 初始化 CenseqOpenIddictStoreOptions 实例。
    /// </summary>
    public CenseqOpenIddictStoreOptions()
    {
        PruneIsolationLevel = IsolationLevel.RepeatableRead;
        DeleteIsolationLevel = IsolationLevel.Serializable;
    }
}
