using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Censeq.OpenIddict;

/// <summary>
/// OpenIddict 异步枚举扩展方法。
/// </summary>
public static class CenseqOpenIddictAsyncEnumerableExtensions
{
    /// <summary>
    /// 将异步序列转换为列表。
    /// </summary>
    /// <param name="source">source。</param>
    /// <returns>异步操作结果。</returns>
    public static Task<List<T>> ToListAsync<T>(this IAsyncEnumerable<T> source)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        return ExecuteAsync();

        async Task<List<T>> ExecuteAsync()
        {
            var list = new List<T>();

            await foreach (var element in source)
            {
                list.Add(element);
            }

            return list;
        }
    }
}
