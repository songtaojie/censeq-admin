using Censeq.PermissionManagement;

namespace Censeq.Admin.Permissions
{
    public interface ICenseqPermissionAppService : IPermissionAppService
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
