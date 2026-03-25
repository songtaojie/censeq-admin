using System;

namespace Censeq.PermissionManagement;

public class IsGrantedRequest
{
    public Guid UserId { get; set; }
    public string[]? PermissionNames { get; set; }
}
