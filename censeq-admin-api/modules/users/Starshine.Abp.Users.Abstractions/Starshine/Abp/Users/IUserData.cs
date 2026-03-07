using Volo.Abp.Data;

namespace Censeq.Abp.Users;

/// <summary>
/// �û�����
/// </summary>
public interface IUserData : IHasExtraProperties
{
    /// <summary>
    /// �û�id
    /// </summary>
    Guid Id { get; }

    /// <summary>
    /// �⻧id
    /// </summary>
    Guid? TenantId { get; }

    /// <summary>
    /// �û���
    /// </summary>
    string UserName { get; }

    /// <summary>
    /// �û�����
    /// </summary>
    string? Name { get; }

    /// <summary>
    /// �û���
    /// </summary>
    string? Surname { get; }

    /// <summary>
    /// �Ƿ񼤻�
    /// </summary>
    bool IsActive { get; }

    /// <summary>
    /// ����
    /// </summary>
    string Email { get; }

    /// <summary>
    /// �����Ƿ�ȷ��
    /// </summary>
    bool EmailConfirmed { get; }

    /// <summary>
    /// �ֻ�����
    /// </summary>
    string? PhoneNumber { get; }

    /// <summary>
    /// �ֻ����Ƿ�ȷ��
    /// </summary>
    bool PhoneNumberConfirmed { get; }
}
