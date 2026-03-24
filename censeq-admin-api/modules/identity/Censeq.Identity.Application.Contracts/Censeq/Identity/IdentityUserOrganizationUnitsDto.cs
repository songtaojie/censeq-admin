using System;
using System.Collections.Generic;

namespace Censeq.Identity;

public class IdentityUserOrganizationUnitsDto
{
    public List<Guid> OrganizationUnitIds { get; set; } = new();
}
