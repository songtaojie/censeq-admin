using System;

namespace Censeq.Abp.Identity;

/// <summary>
/// ���ݽ�ɫ���û�����
/// </summary>
/// <remarks>
/// </remarks>
/// <param name="role"></param>
/// <param name="userCount"></param>
public class IdentityRoleWithUserCount(IdentityRole role, long userCount)
{
    /// <summary>
    /// ��ɫ
    /// </summary>
    public IdentityRole Role { get; set; } = role;

    /// <summary>
    /// �û�����
    /// </summary>
    public long UserCount { get; set; } = userCount;
}
