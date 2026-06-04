using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using OpenIddict.Abstractions;
using Volo.Abp.DependencyInjection;

namespace Censeq.OpenIddict;

/// <summary>
/// OpenIddict 请求辅助器。
/// </summary>
public class CenseqOpenIddictRequestHelper : ITransientDependency
{
    /// <summary>
    /// 从返回地址获取数据。
    /// </summary>
    /// <param name="returnUrl">returnURL。</param>
    /// <returns>异步操作结果。</returns>
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
