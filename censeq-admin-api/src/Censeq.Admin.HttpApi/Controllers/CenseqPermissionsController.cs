using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.PermissionManagement;

namespace Censeq.Admin.Controllers
{
    public class StarshinePermissionsController : PermissionsController
    {
        public StarshinePermissionsController(IPermissionAppService permissionAppService):base(permissionAppService)
        {
        }

        [HttpPost]
        public virtual Task AddAsync(string providerName, string providerKey, UpdatePermissionsDto input)
        {
            return PermissionAppService.UpdateAsync(providerName, providerKey, input);
        }
    }
}
