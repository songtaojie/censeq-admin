namespace Censeq.Abp.Identity;

/// <summary>
/// ���ݻỰ����
/// </summary>
public class IdentitySessionConsts
{
    /// <summary>
    /// ���ỰID����
    /// </summary>
    public static int MaxSessionIdLength { get; set; } = 128;

    /// <summary>
    /// ����豸����
    /// </summary>
    public static int MaxDeviceLength { get; set; } = 64;

    /// <summary>
    /// ����豸��Ϣ����
    /// </summary>
    public static int MaxDeviceInfoLength { get; set; } = 64;

    /// <summary>
    /// ���ͻ���ID����
    /// </summary>
    public static int MaxClientIdLength { get; set; } = 64;

    /// <summary>
    /// ���IP��ַ����
    /// </summary>
    public static int MaxIpAddressesLength { get; set; } = 256;
}
