using System.Collections.Generic;

namespace Censeq.PermissionManagement;

public class IsGrantedResponse
{
    public Guid UserId { get; set; }
    public Dictionary<string, bool> Permissions { get; set; } = [];
}
