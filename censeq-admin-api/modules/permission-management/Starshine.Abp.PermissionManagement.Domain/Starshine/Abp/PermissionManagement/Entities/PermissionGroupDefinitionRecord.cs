using Censeq.Abp.Domain.Entities;
using Volo.Abp;
using Volo.Abp.Data;

namespace Censeq.Abp.PermissionManagement.Entities;

/// <summary>
/// Ȩ�޶����¼
/// </summary>
public class PermissionGroupDefinitionRecord : BasicAggregateRoot<Guid>, IHasExtraProperties
{
    /// <summary>
    /// ����
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// ��ʾ����
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// ��������
    /// </summary>
    public ExtraPropertyDictionary ExtraProperties { get; protected set; } = [];

    /// <summary>
    /// Ȩ�޶����¼
    /// </summary>
    public PermissionGroupDefinitionRecord()
    {
        this.SetDefaultsForExtraProperties();
    }

    /// <summary>
    /// Ȩ�޶����¼
    /// </summary>
    /// <param name="id">����</param>
    /// <param name="name">����</param>
    /// <param name="displayName">��ʾ����</param>
    public PermissionGroupDefinitionRecord(Guid id,string name,string? displayName): base(id)
    {
        Name = Check.NotNullOrWhiteSpace(name, nameof(name), PermissionGroupDefinitionRecordConsts.MaxNameLength);
        DisplayName = Check.NotNullOrWhiteSpace(displayName, nameof(displayName), PermissionGroupDefinitionRecordConsts.MaxDisplayNameLength); ;
        this.SetDefaultsForExtraProperties();
    }

    /// <summary>
    /// ����ͬ������
    /// </summary>
    /// <param name="otherRecord"></param>
    /// <returns></returns>
    public bool HasSameData(PermissionGroupDefinitionRecord otherRecord)
    {
        if (Name != otherRecord.Name || DisplayName != otherRecord.DisplayName)
        {
            return false;
        }
        if (!this.HasSameExtraProperties(otherRecord))
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// �޲�����
    /// </summary>
    /// <param name="otherRecord"></param>
    public void Patch(PermissionGroupDefinitionRecord otherRecord)
    {
        if (Name != otherRecord.Name)
        {
            Name = otherRecord.Name;
        }

        if (DisplayName != otherRecord.DisplayName)
        {
            DisplayName = otherRecord.DisplayName;
        }

        if (!this.HasSameExtraProperties(otherRecord))
        {
            ExtraProperties.Clear();
            foreach (var property in otherRecord.ExtraProperties)
            {
                ExtraProperties.Add(property.Key, property.Value);
            }
        }
    }
}
