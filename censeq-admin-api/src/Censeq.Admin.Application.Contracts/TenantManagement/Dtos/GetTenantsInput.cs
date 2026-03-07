using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Censeq.Admin.TenantManagement.Dtos
{
    public class GetTenantsInput : PagedAndSortedResultRequestDto
    {
        public string? Filter { get; set; }
    }

}
