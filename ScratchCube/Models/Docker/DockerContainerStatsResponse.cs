using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using ScratchCube.ViewModels;

namespace ScratchCube.Models.Docker;

public partial class DockerContainerStatsResponse : ViewModelBase
{
    [ObservableProperty]
    private DateTime read;

    [ObservableProperty]
    private DateTime preRead;

    [ObservableProperty]
    private DockerPidsStats pidsStats = new();

    [ObservableProperty]
    private DockerBlkioStats blkioStats = new();

    [ObservableProperty]
    private uint numProcs;

    [ObservableProperty]
    private DockerStorageStats storageStats = new();

    [ObservableProperty]
    private DockerCpuStats cpuStats = new();

    [ObservableProperty]
    private DockerCpuStats preCpuStats = new();

    [ObservableProperty]
    private DockerMemoryStats memoryStats = new();

    [ObservableProperty]
    private string name = string.Empty;

    [ObservableProperty]
    private string id = string.Empty;

    [ObservableProperty]
    private IDictionary<string, DockerNetworkStats> networks = new Dictionary<string, DockerNetworkStats>();
}

public partial class DockerPidsStats : ViewModelBase
{
    [ObservableProperty]
    private ulong current;

    [ObservableProperty]
    private ulong limit;
}

public partial class DockerBlkioStatEntry : ViewModelBase
{
    [ObservableProperty]
    private ulong major;

    [ObservableProperty]
    private ulong minor;

    [ObservableProperty]
    private string op = string.Empty;

    [ObservableProperty]
    private ulong value;
}

public partial class DockerBlkioStats : ViewModelBase
{
    [ObservableProperty]
    private IList<DockerBlkioStatEntry> ioServiceBytesRecursive =
        new List<DockerBlkioStatEntry>();

    [ObservableProperty]
    private IList<DockerBlkioStatEntry> ioServicedRecursive =
        new List<DockerBlkioStatEntry>();

    [ObservableProperty]
    private IList<DockerBlkioStatEntry> ioQueuedRecursive =
        new List<DockerBlkioStatEntry>();

    [ObservableProperty]
    private IList<DockerBlkioStatEntry> ioServiceTimeRecursive =
        new List<DockerBlkioStatEntry>();

    [ObservableProperty]
    private IList<DockerBlkioStatEntry> ioWaitTimeRecursive =
        new List<DockerBlkioStatEntry>();

    [ObservableProperty]
    private IList<DockerBlkioStatEntry> ioMergedRecursive =
        new List<DockerBlkioStatEntry>();

    [ObservableProperty]
    private IList<DockerBlkioStatEntry> ioTimeRecursive =
        new List<DockerBlkioStatEntry>();

    [ObservableProperty]
    private IList<DockerBlkioStatEntry> sectorsRecursive =
        new List<DockerBlkioStatEntry>();
}

public partial class DockerNetworkStats : ViewModelBase
{
    [ObservableProperty]
    private ulong rxBytes;

    [ObservableProperty]
    private ulong rxPackets;

    [ObservableProperty]
    private ulong rxErrors;

    [ObservableProperty]
    private ulong rxDropped;

    [ObservableProperty]
    private ulong txBytes;

    [ObservableProperty]
    private ulong txPackets;

    [ObservableProperty]
    private ulong txErrors;

    [ObservableProperty]
    private ulong txDropped;

    [ObservableProperty]
    private string endpointId = string.Empty;

    [ObservableProperty]
    private string instanceId = string.Empty;
}

public partial class DockerMemoryStats : ViewModelBase
{
    [ObservableProperty]
    private ulong usage;

    [ObservableProperty]
    private ulong maxUsage;

    [ObservableProperty]
    private IDictionary<string, ulong> stats =
        new Dictionary<string, ulong>();

    [ObservableProperty]
    private ulong failcnt;

    [ObservableProperty]
    private ulong limit;

    [ObservableProperty]
    private ulong commit;

    [ObservableProperty]
    private ulong commitPeak;

    [ObservableProperty]
    private ulong privateWorkingSet;
}

public partial class DockerCpuStats : ViewModelBase
{
    [ObservableProperty]
    private DockerCpuUsage cpuUsage = new();

    [ObservableProperty]
    private ulong systemUsage;

    [ObservableProperty]
    private uint onlineCPUs;

    [ObservableProperty]
    private DockerThrottlingData throttlingData = new();
}

public partial class DockerCpuUsage : ViewModelBase
{
    [ObservableProperty]
    private ulong totalUsage;

    [ObservableProperty]
    private IList<ulong> percpuUsage =
        new List<ulong>();

    [ObservableProperty]
    private ulong usageInKernelmode;

    [ObservableProperty]
    private ulong usageInUsermode;
}

public partial class DockerThrottlingData : ViewModelBase
{
    [ObservableProperty]
    private ulong periods;

    [ObservableProperty]
    private ulong throttledPeriods;

    [ObservableProperty]
    private ulong throttledTime;
}

public partial class DockerStorageStats : ViewModelBase
{
    [ObservableProperty]
    private ulong readCountNormalized;

    [ObservableProperty]
    private ulong readSizeBytes;

    [ObservableProperty]
    private ulong writeCountNormalized;

    [ObservableProperty]
    private ulong writeSizeBytes;
}