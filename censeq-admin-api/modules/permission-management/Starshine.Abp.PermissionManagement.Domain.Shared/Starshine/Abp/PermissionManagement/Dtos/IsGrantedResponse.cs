using System;
using System.Collections.Generic;

namespace Censeq.Abp.PermissionManagement;

/// <summary>
/// �Ƿ���Ȩ��Ӧ
/// </summary>
public class IsGrantedResponse
{
    /// <summary>
    /// �û�id
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    ///��Ȩ���
    /// </summary>
    public Dictionary<string, bool> Permissions { get; set; } = [];
}
