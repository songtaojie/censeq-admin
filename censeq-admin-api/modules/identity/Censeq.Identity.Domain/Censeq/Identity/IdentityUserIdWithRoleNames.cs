using System;
using System.Collections.Generic;

namespace Censeq.Identity;

public class IdentityUserIdWithRoleNames
{
    public Guid Id { get; set; }

    public string[] RoleNames { get; set; }
}