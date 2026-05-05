namespace Censeq.SettingManagement;

public class SettingDefinitionDto
{
    public Guid Id { get; set; }

    /// <summary>配置编码（唯一 key）</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>配置名称</summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>描述 / 备注</summary>
    public string? Description { get; set; }

    /// <summary>默认值</summary>
    public string? DefaultValue { get; set; }

    /// <summary>当前全局值（Global provider）</summary>
    public string? CurrentValue { get; set; }

    /// <summary>客户端可见</summary>
    public bool IsVisibleToClients { get; set; }

    /// <summary>是否系统内置（代码定义，不允许删除）</summary>
    public bool IsSystem { get; set; }
}
