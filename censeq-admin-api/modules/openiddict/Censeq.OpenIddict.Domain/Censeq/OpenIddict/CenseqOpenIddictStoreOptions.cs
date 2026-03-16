using System.Data;

namespace Censeq.OpenIddict;

public class CenseqOpenIddictStoreOptions
{
    public IsolationLevel? PruneIsolationLevel { get; set; }

    public IsolationLevel? DeleteIsolationLevel { get; set; }

    public CenseqOpenIddictStoreOptions()
    {
        PruneIsolationLevel = IsolationLevel.RepeatableRead;
        DeleteIsolationLevel = IsolationLevel.Serializable;
    }
}
