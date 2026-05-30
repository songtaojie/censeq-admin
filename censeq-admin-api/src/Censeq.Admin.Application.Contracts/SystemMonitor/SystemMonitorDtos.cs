namespace Censeq.Admin.SystemMonitor;

public class SystemBaseInfoDto
{
    public string HostName { get; set; } = string.Empty;
    public string SystemOs { get; set; } = string.Empty;
    public string OsArchitecture { get; set; } = string.Empty;
    public string ProcessorCount { get; set; } = string.Empty;
    public string SysRunTime { get; set; } = string.Empty;
    public string? RemoteIp { get; set; }
    public string? LocalIp { get; set; }
    public string FrameworkDescription { get; set; } = string.Empty;
    public string Environment { get; set; } = string.Empty;
    public string? Wwwroot { get; set; }
    public string Stage { get; set; } = string.Empty;
}

public class SystemUsageInfoDto
{
    public string FreeRam { get; set; } = string.Empty;
    public string UsedRam { get; set; } = string.Empty;
    public string TotalRam { get; set; } = string.Empty;
    public string RamRate { get; set; } = string.Empty;
    public string CpuRate { get; set; } = string.Empty;
    public string StartTime { get; set; } = string.Empty;
    public string RunTime { get; set; } = string.Empty;
}

public class SystemDiskInfoDto
{
    public string DiskName { get; set; } = string.Empty;
    public string DiskType { get; set; } = string.Empty;
    public double TotalSize { get; set; }
    public double Used { get; set; }
    public double AvailableFreeSpace { get; set; }
    public int UsedPercent { get; set; }
}

public class AssemblyInfoDto
{
    public string Name { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
}
