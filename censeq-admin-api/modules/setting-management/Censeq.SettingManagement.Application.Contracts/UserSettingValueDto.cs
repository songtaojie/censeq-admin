using System;

namespace Censeq.SettingManagement;

public class UserSettingValueDto
{
    public Guid UserId { get; set; }

    public string? Value { get; set; }
}