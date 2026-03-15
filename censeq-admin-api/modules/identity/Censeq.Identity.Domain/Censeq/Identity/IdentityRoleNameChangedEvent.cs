using System;

namespace Censeq.Identity;

[Obsolete("Use the distributed event (IdentityRoleNameChangedEto) instead.")]
public class IdentityRoleNameChangedEvent
{
    public IdentityRole IdentityRole { get; set; }
    public string OldName { get; set; }
}
