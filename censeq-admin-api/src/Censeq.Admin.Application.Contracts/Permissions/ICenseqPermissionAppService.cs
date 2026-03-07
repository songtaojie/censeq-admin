using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.PermissionManagement;

namespace Censeq.Admin.Permissions
{
    public interface IStarshinePermissionAppService : IPermissionAppService
    {
        /// <summary>
        /// 添加权限
        /// </summary>
        /// <param name="providerName"></param>
        /// <param name="providerKey"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        Task AddAsync(string providerName, string providerKey, UpdatePermissionsDto input);
    }
}
