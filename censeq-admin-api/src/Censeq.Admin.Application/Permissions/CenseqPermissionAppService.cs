using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SimpleStateChecking;

namespace Censeq.Admin.Permissions
{
    public class StarshinePermissionAppService: PermissionAppService,IStarshinePermissionAppService
    {
        public StarshinePermissionAppService(
            IPermissionManager permissionManager,
            IPermissionDefinitionManager permissionDefinitionManager,
            IOptions<PermissionManagementOptions> options,
            ISimpleStateCheckerManager<PermissionDefinition> simpleStateCheckerManager):base(permissionManager,
            permissionDefinitionManager,
            options, 
            simpleStateCheckerManager)
        {
           
        }

        public async Task AddAsync(string providerName, string providerKey, UpdatePermissionsDto input)
        {
            await CheckProviderPolicy(providerName);

            foreach (var permissionDto in input.Permissions)
            {
                await PermissionManager.SetAsync(permissionDto.Name, providerName, providerKey, permissionDto.IsGranted);
            }
        }
    }
}
