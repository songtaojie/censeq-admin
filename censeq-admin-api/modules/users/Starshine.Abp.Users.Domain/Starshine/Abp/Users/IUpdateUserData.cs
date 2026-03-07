using JetBrains.Annotations;

namespace Censeq.Abp.Users;

/// <summary>
/// �����û�
/// </summary>
public interface IUpdateUserData
{
    /// <summary>
    /// �����û�
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    bool Update([NotNull] IUserData user);
}
