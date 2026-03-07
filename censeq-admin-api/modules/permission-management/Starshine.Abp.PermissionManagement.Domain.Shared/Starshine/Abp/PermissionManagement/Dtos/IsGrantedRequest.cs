using System;

namespace Censeq.Abp.PermissionManagement;

/// <summary>
/// �Ƿ���������
/// </summary>
public class IsGrantedRequest
{
    /// <summary>
    /// ��Ȩ�û�id
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Ȩ������
    /// </summary>
    public string[]? PermissionNames { get; set; }
}
