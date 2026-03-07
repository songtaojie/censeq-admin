using System.Threading.Tasks;

namespace Censeq.Abp.PermissionManagement;

/// <summary>
/// ��̬Ȩ�ޱ���
/// </summary>
public interface IStaticPermissionSaver
{
    /// <summary>
    /// ����
    /// </summary>
    /// <returns></returns>
    Task SaveAsync();
}