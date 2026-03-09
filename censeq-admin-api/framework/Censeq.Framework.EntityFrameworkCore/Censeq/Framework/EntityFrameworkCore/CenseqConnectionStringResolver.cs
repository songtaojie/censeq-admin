using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;

namespace Censeq.Framework.EntityFrameworkCore
{
    [ExposeServices(typeof(IConnectionStringResolver))]
    [Dependency(ReplaceServices = true)]
    public class CenseqConnectionStringResolver(IOptionsMonitor<AbpDbConnectionOptions> options) : DefaultConnectionStringResolver(options)
    {
        /// <summary>
        /// 解析连接字符串
        /// </summary>
        /// <param name="connectionStringName"></param>
        /// <returns></returns>
        public override async Task<string> ResolveAsync(string? connectionStringName = null)
        {
            var connectionString = await base.ResolveAsync(connectionStringName);
            return ConnectionStringParser.ParseConnectionString(connectionString);
        }

        /// <summary>
        /// 解析连接字符串
        /// </summary>
        /// <param name="connectionStringName"></param>
        /// <returns></returns>
        [Obsolete(message: "请使用ResolveAsync")]
        public override string Resolve(string? connectionStringName = null)
        {
            var connectionString = base.Resolve(connectionStringName);
            return ConnectionStringParser.ParseConnectionString(connectionString);
        }
    }
}
