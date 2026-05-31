using System.Collections;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Application.Dtos;

namespace Censeq.Admin.SystemMonitor;

public class SystemMonitorAppService : AdminAppService, ISystemMonitorAppService
{
    private readonly IHostEnvironment _hostEnvironment;
    private readonly IDistributedCache _distributedCache;
    private readonly IServiceProvider _serviceProvider;

    public SystemMonitorAppService(
        IHostEnvironment hostEnvironment,
        IDistributedCache distributedCache,
        IServiceProvider serviceProvider)
    {
        _hostEnvironment = hostEnvironment;
        _distributedCache = distributedCache;
        _serviceProvider = serviceProvider;
    }

    public Task<SystemBaseInfoDto> GetServerBaseAsync()
    {
        var uptime = TimeSpan.FromMilliseconds(Environment.TickCount64);

        return Task.FromResult(new SystemBaseInfoDto
        {
            HostName = Environment.MachineName,
            SystemOs = RuntimeInformation.OSDescription,
            OsArchitecture = $"{RuntimeInformation.OSArchitecture} / {RuntimeInformation.ProcessArchitecture}",
            ProcessorCount = $"{Environment.ProcessorCount} 核",
            SysRunTime = FormatDuration(uptime),
            RemoteIp = null,
            LocalIp = GetLocalIpAddress(),
            FrameworkDescription = RuntimeInformation.FrameworkDescription,
            Environment = _hostEnvironment.EnvironmentName,
            Wwwroot = _hostEnvironment.ContentRootPath,
            Stage = _hostEnvironment.IsStaging() ? "Stage 环境" : "非 Stage 环境"
        });
    }

    public async Task<SystemUsageInfoDto> GetServerUsageAsync()
    {
        var process = Process.GetCurrentProcess();
        var startCpu = process.TotalProcessorTime;
        var startTime = Stopwatch.GetTimestamp();

        await Task.Delay(350);

        process.Refresh();
        var cpuUsedMs = (process.TotalProcessorTime - startCpu).TotalMilliseconds;
        var elapsedMs = Stopwatch.GetElapsedTime(startTime).TotalMilliseconds;
        var cpuRate = elapsedMs <= 0 ? 0 : Math.Clamp(cpuUsedMs / (elapsedMs * Environment.ProcessorCount) * 100, 0, 100);

        var memory = GetMemoryInfo(process);
        var runTime = DateTime.Now - process.StartTime;

        return new SystemUsageInfoDto
        {
            FreeRam = FormatBytes(memory.FreeBytes),
            UsedRam = FormatBytes(memory.UsedBytes),
            TotalRam = FormatBytes(memory.TotalBytes),
            RamRate = $"{memory.UsagePercent:0.##}%",
            CpuRate = $"{cpuRate:0.##}%",
            StartTime = process.StartTime.ToString("yyyy-MM-dd HH:mm:ss"),
            RunTime = FormatDuration(runTime)
        };
    }

    public Task<ListResultDto<SystemDiskInfoDto>> GetServerDisksAsync()
    {
        var disks = DriveInfo.GetDrives()
            .Where(drive => drive.IsReady)
            .Select(drive =>
            {
                var usedBytes = drive.TotalSize - drive.AvailableFreeSpace;
                var usedPercent = drive.TotalSize <= 0 ? 0 : (int)Math.Round(usedBytes * 100d / drive.TotalSize);

                return new SystemDiskInfoDto
                {
                    DiskName = drive.Name,
                    DiskType = drive.DriveType.ToString(),
                    TotalSize = ToGb(drive.TotalSize),
                    Used = ToGb(usedBytes),
                    AvailableFreeSpace = ToGb(drive.AvailableFreeSpace),
                    UsedPercent = Math.Clamp(usedPercent, 0, 100)
                };
            })
            .ToList();

        return Task.FromResult(new ListResultDto<SystemDiskInfoDto>(disks));
    }

    public Task<ListResultDto<AssemblyInfoDto>> GetAssemblyListAsync()
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(assembly => !assembly.IsDynamic)
            .Select(assembly => assembly.GetName())
            .Where(name => !string.IsNullOrWhiteSpace(name.Name))
            .Where(name => !name.Name!.StartsWith("Censeq", StringComparison.OrdinalIgnoreCase))
            .OrderBy(name => name.Name)
            .Select(name => new AssemblyInfoDto
            {
                Name = name.Name!,
                Version = name.Version?.ToString() ?? string.Empty
            })
            .ToList();

        return Task.FromResult(new ListResultDto<AssemblyInfoDto>(assemblies));
    }

    public Task<ListResultDto<string>> GetCacheKeysAsync()
    {
        var keys = GetMemoryCacheKeys()
            .Select(static key => key?.ToString())
            .Where(static key => !string.IsNullOrWhiteSpace(key))
            .Select(static key => key!)
            .Distinct(StringComparer.Ordinal)
            .OrderBy(static key => key)
            .ToList();

        return Task.FromResult(new ListResultDto<string>(keys));
    }

    public async Task<object?> GetCacheValueAsync(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return null;
        }

        var decodedKey = Uri.UnescapeDataString(key);
        var rawValue = TryGetMemoryCacheValue(decodedKey);
        if (rawValue != null)
        {
            return rawValue;
        }

        var bytes = await _distributedCache.GetAsync(decodedKey);
        if (bytes == null)
        {
            return null;
        }

        return TryDecodeBytes(bytes);
    }

    public async Task DeleteCacheAsync(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return;
        }

        var decodedKey = Uri.UnescapeDataString(key);
        if (_serviceProvider.GetService<IMemoryCache>() is { } memoryCache)
        {
            memoryCache.Remove(decodedKey);
        }

        await _distributedCache.RemoveAsync(decodedKey);
    }

    public async Task ClearCacheAsync()
    {
        foreach (var key in GetMemoryCacheKeys().ToList())
        {
            if (key == null)
            {
                continue;
            }

            if (_serviceProvider.GetService<IMemoryCache>() is { } memoryCache)
            {
                memoryCache.Remove(key);
            }

            await _distributedCache.RemoveAsync(key.ToString()!);
        }
    }

    private IEnumerable<object?> GetMemoryCacheKeys()
    {
        if (_serviceProvider.GetService<IMemoryCache>() is not MemoryCache memoryCache)
        {
            return [];
        }

        var coherentState = typeof(MemoryCache).GetField("_coherentState", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(memoryCache);
        if (coherentState == null)
        {
            return [];
        }

        var stringEntries = coherentState.GetType().GetField("_stringEntries", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(coherentState) as IDictionary;
        var nonStringEntries = coherentState.GetType().GetField("_nonStringEntries", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(coherentState) as IDictionary;

        return (stringEntries?.Keys.Cast<object?>() ?? [])
            .Concat(nonStringEntries?.Keys.Cast<object?>() ?? []);
    }

    private object? TryGetMemoryCacheValue(string key)
    {
        if (_serviceProvider.GetService<IMemoryCache>() is not { } memoryCache)
        {
            return null;
        }

        return memoryCache.TryGetValue(key, out var value) ? NormalizeCacheValue(value) : null;
    }

    private static object? NormalizeCacheValue(object? value)
    {
        if (value is null)
        {
            return null;
        }

        if (value is byte[] bytes)
        {
            return TryDecodeBytes(bytes);
        }

        if (value is string or ValueType)
        {
            return value;
        }

        return value;
    }

    private static object TryDecodeBytes(byte[] bytes)
    {
        try
        {
            return Encoding.UTF8.GetString(bytes);
        }
        catch
        {
            return Convert.ToBase64String(bytes);
        }
    }

    private static (long TotalBytes, long UsedBytes, long FreeBytes, double UsagePercent) GetMemoryInfo(Process process)
    {
        var totalBytes = GetTotalPhysicalMemoryBytes();
        var usedBytes = process.WorkingSet64;

        if (totalBytes <= 0)
        {
            totalBytes = GC.GetGCMemoryInfo().TotalAvailableMemoryBytes;
        }

        var freeBytes = Math.Max(totalBytes - usedBytes, 0);
        var usagePercent = totalBytes <= 0 ? 0 : usedBytes * 100d / totalBytes;

        return (totalBytes, usedBytes, freeBytes, usagePercent);
    }

    private static long GetTotalPhysicalMemoryBytes()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            var memoryStatus = new MemoryStatusEx();
            if (GlobalMemoryStatusEx(memoryStatus))
            {
                return (long)memoryStatus.ullTotalPhys;
            }
        }

        return 0;
    }

    private static string? GetLocalIpAddress()
    {
        return NetworkInterface.GetAllNetworkInterfaces()
            .Where(static item => item.OperationalStatus == OperationalStatus.Up)
            .SelectMany(static item => item.GetIPProperties().UnicastAddresses)
            .Where(static item => item.Address.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(item.Address))
            .Select(static item => item.Address.ToString())
            .FirstOrDefault();
    }

    private static string FormatBytes(long bytes)
    {
        string[] units = ["B", "KB", "MB", "GB", "TB"];
        double size = bytes;
        var unit = 0;
        while (size >= 1024 && unit < units.Length - 1)
        {
            size /= 1024;
            unit++;
        }

        return $"{size:0.##} {units[unit]}";
    }

    private static double ToGb(long bytes)
    {
        return Math.Round(bytes / 1024d / 1024d / 1024d, 2);
    }

    private static string FormatDuration(TimeSpan timeSpan)
    {
        if (timeSpan.TotalDays >= 1)
        {
            return $"{(int)timeSpan.TotalDays} 天 {timeSpan.Hours} 小时 {timeSpan.Minutes} 分钟";
        }

        if (timeSpan.TotalHours >= 1)
        {
            return $"{(int)timeSpan.TotalHours} 小时 {timeSpan.Minutes} 分钟";
        }

        return $"{Math.Max(0, (int)timeSpan.TotalMinutes)} 分钟 {timeSpan.Seconds} 秒";
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GlobalMemoryStatusEx([In, Out] MemoryStatusEx lpBuffer);

    [StructLayout(LayoutKind.Sequential)]
    private sealed class MemoryStatusEx
    {
        public uint dwLength = (uint)Marshal.SizeOf<MemoryStatusEx>();
        public uint dwMemoryLoad;
        public ulong ullTotalPhys;
        public ulong ullAvailPhys;
        public ulong ullTotalPageFile;
        public ulong ullAvailPageFile;
        public ulong ullTotalVirtual;
        public ulong ullAvailVirtual;
        public ulong ullAvailExtendedVirtual;
    }
}
