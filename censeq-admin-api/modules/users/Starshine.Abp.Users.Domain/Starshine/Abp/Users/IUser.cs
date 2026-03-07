using Volo.Abp.Data;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace Censeq.Abp.Users;

/// <summary>
/// �û�����
/// </summary>
public interface IUser : IAggregateRoot<Guid>, IMultiTenant, IHasExtraProperties
{
    /// <summary>
    /// �û���
    /// </summary>
    string UserName { get; }

    /// <summary>
    /// ����
    /// </summary>
    string Email { get; }

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
