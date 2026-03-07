using JetBrains.Annotations;
using Volo.Abp.Data;

namespace Censeq.Abp.Users;
/// <summary>
/// �û�����
/// </summary>
public class UserData : IUserData
{
    /// <summary>
    /// ����id
    /// </summary>
    public Guid Id { get; set; }


    /// <summary>
    /// �⻧id
    /// </summary>
    public Guid? TenantId { get; set; }

    /// <summary>
    /// �û��ǳ�
    /// </summary>
    public string UserName { get; set; }

    /// <summary>
    /// �û�����
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// �û���
    /// </summary>
    public string? Surname { get; set; }

    /// <summary>
    /// �Ƿ񼤻�
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// ����
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// �����Ƿ�ȷ��
    /// </summary>
    public bool EmailConfirmed { get; set; }

    /// <summary>
    /// �ֻ�����
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// �ֻ����Ƿ�ȷ��
    /// </summary>
    public bool PhoneNumberConfirmed { get; set; }

    /// <summary>
    /// ��������
    /// </summary>
    public ExtraPropertyDictionary ExtraProperties { get; }

    /// <summary>
    /// Ĭ�Ϲ��캯��
    /// </summary>
    public UserData()
    {
        UserName = string.Empty;
        Email = string.Empty;
        ExtraProperties = [];
    }

    /// <summary>
    /// �û����캯��
    /// </summary>
    /// <param name="userData"></param>
    public UserData(IUserData userData)
    {
        Id = userData.Id;
        UserName = userData.UserName;
        Email = userData.Email;
        Name = userData.Name;
        Surname = userData.Surname;
        IsActive = userData.IsActive;
        EmailConfirmed = userData.EmailConfirmed;
        PhoneNumber = userData.PhoneNumber;
        PhoneNumberConfirmed = userData.PhoneNumberConfirmed;
        TenantId = userData.TenantId;
        ExtraProperties = userData.ExtraProperties;
    }

    /// <summary>
    /// �û�����
    /// </summary>
    /// <param name="id"></param>
    /// <param name="userName"></param>
    /// <param name="email"></param>
    /// <param name="name"></param>
    /// <param name="surname"></param>
    /// <param name="emailConfirmed"></param>
    /// <param name="phoneNumber"></param>
    /// <param name="phoneNumberConfirmed"></param>
    /// <param name="tenantId"></param>
    /// <param name="isActive"></param>
    /// <param name="extraProperties"></param>
    public UserData(
        Guid id,
        [NotNull] string userName,
        [NotNull] string email,
        [CanBeNull] string? name = null,
        [CanBeNull] string? surname = null,
        bool emailConfirmed = false,
        [CanBeNull] string? phoneNumber = null,
        bool phoneNumberConfirmed = false,
        Guid? tenantId = null,
        bool isActive = true,
        ExtraPropertyDictionary? extraProperties = null)
    {
        Id = id;
        UserName = userName;
        Email = email;
        Name = name;
        Surname = surname;
        IsActive = isActive;
        EmailConfirmed = emailConfirmed;
        PhoneNumber = phoneNumber;
        PhoneNumberConfirmed = phoneNumberConfirmed;
        TenantId = tenantId;
        ExtraProperties = extraProperties ?? [];
    }
}
