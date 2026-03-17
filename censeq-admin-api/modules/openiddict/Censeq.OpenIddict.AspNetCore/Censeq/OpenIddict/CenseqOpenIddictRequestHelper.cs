using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using OpenIddict.Abstractions;
using Volo.Abp.DependencyInjection;

namespace Censeq.OpenIddict;

public class CenseqOpenIddictRequestHelper : ITransientDependency
{
    public virtual Task<OpenIddictRequest> GetFromReturnUrlAsync(string returnUrl)
    {
        if (!returnUrl.IsNullOrWhiteSpace())
        {
            var qm = returnUrl.IndexOf("?", StringComparison.Ordinal);
            if (qm > 0)
            {
                return Task.FromResult(new OpenIddictRequest(returnUrl.Substring(qm + 1)
                    .Split("&")
                    .Select(x => x.Split("="))
                    .Where(p => p.Length == 2)
                    .Select(p => new KeyValuePair<string, string?>(p[0], WebUtility.UrlDecode(p[1])))));
            }
        }

        return Task.FromResult(new OpenIddictRequest(Array.Empty<KeyValuePair<string, string?>>()));
    }
}
