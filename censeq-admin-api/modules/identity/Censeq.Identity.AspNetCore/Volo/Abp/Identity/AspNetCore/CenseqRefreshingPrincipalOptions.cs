using System.Collections.Generic;
using Volo.Abp.Security.Claims;

namespace Censeq.Identity.AspNetCore;

public class CenseqRefreshingPrincipalOptions
{
    public List<string> CurrentPrincipalKeepClaimTypes { get; set; }

    public CenseqRefreshingPrincipalOptions()
    {
        CurrentPrincipalKeepClaimTypes = new List<string>
        {
            AbpClaimTypes.SessionId
        };
    }
}
