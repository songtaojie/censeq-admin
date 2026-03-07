namespace Censeq.Abp.Users;

/// <summary>
/// �û�����
/// </summary>
public class StarshineUserConsts
{
    /// <summary>
    /// �û������ȣ�Ĭ��ֵ: 256
    /// </summary>
    public static int MaxUserNameLength { get; set; } = 64;

    /// <summary>
    /// Default value: 64
    /// </summary>
    public static int MaxNameLength { get; set; } = 64;

    /// <summary>
    /// �ճ��ȣ�Ĭ��ֵ: 64
    /// </summary>
    public static int MaxSurnameLength { get; set; } = 64;

    /// <summary>
    ///���䳤�ȣ�Ĭ��ֵ: 128
    /// </summary>
    public static int MaxEmailLength { get; set; } = 128;

    /// <summary>
    /// �ֻ����볤�ȣ�Ĭ��ֵ: 20
    /// </summary>
    public static int MaxPhoneNumberLength { get; set; } = 20;

    /// <summary>
    /// �û�ʱ�䴫�����ģ��
    /// </summary>
    public static string UserEventName { get; set; } = "Censeq.Abp.Users.User";
}
