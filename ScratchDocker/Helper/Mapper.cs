using System.Collections.Generic;
using System.Linq;
using Docker.DotNet.Models;
using ScratchDocker.Models.Docker;

namespace ScratchDocker.Helper;

public static class Mapper
{
    public static DockerContainerInspect ConvertToDockerContainerInspect(ContainerInspectResponse source)
    {
        return new DockerContainerInspect
        {
            Id = source.ID,
            Created = source.Created,
            Path = source.Path,
            Args = source.Args,
            State = source.State == null
                ? new DockerContainerState()
                : new DockerContainerState
                {
                    Status = source.State.Status,
                    Running = source.State.Running,
                    Paused = source.State.Paused,
                    Restarting = source.State.Restarting,
                    OomKilled = source.State.OOMKilled,
                    Dead = source.State.Dead,
                    Pid = source.State.Pid,
                    ExitCode = source.State.ExitCode,
                    Error = source.State.Error,
                    StartedAt = source.State.StartedAt,
                    FinishedAt = source.State.FinishedAt,
                    Health = source.State.Health == null
                        ? new DockerHealth()
                        : new DockerHealth
                        {
                            Status = source.State.Health.Status,
                            FailingStreak = source.State.Health.FailingStreak,
                            Log = source.State.Health.Log?.Select(x =>
                                new DockerHealthcheckResult
                                {
                                    Start = x.Start,
                                    End = x.End,
                                    ExitCode = x.ExitCode,
                                    Output = x.Output
                                }).ToList() ?? new List<DockerHealthcheckResult>()
                        }
                },

            Image = source.Image,
            ResolvConfPath = source.ResolvConfPath,
            HostnamePath = source.HostnamePath,
            HostsPath = source.HostsPath,
            LogPath = source.LogPath,

            Name = source.Name,
            RestartCount = source.RestartCount,
            Driver = source.Driver,
            Platform = source.Platform,
            MountLabel = source.MountLabel,
            ProcessLabel = source.ProcessLabel,
            AppArmorProfile = source.AppArmorProfile,
            ExecIDs = source.ExecIDs,

            SizeRw = source.SizeRw,
            SizeRootFs = source.SizeRootFs,

            Mounts = source.Mounts?.Select(x => new DockerMountPoint
            {
                Type = x.Type,
                Name = x.Name,
                Source = x.Source,
                Destination = x.Destination,
                Driver = x.Driver,
                Mode = x.Mode,
                Rw = x.RW,
                Propagation = x.Propagation
            }).ToList() ?? new List<DockerMountPoint>(),

            Config = source.Config == null
                ? new DockerContainerConfig()
                : ConvertConfig(source.Config),

            HostConfig = source.HostConfig == null
                ? new DockerHostConfig()
                : ConvertHostConfig(source.HostConfig),

            GraphDriver = source.GraphDriver == null
                ? new DockerGraphDriverData()
                : new DockerGraphDriverData
                {
                    Data = source.GraphDriver.Data,
                    Name = source.GraphDriver.Name
                },

            NetworkSettings = source.NetworkSettings == null
                ? new DockerNetworkSetting()
                : ConvertNetworkSettings(source.NetworkSettings)
        };
    }

    private static DockerHostConfig ConvertHostConfig(HostConfig source)
    {
        return new DockerHostConfig
        {
            Binds = source.Binds,
            ContainerIDFile = source.ContainerIDFile,
            LogConfig = source.LogConfig == null
                ? new DockerLogConfig()
                : new DockerLogConfig
                {
                    Type = source.LogConfig.Type,
                    Config = source.LogConfig.Config
                },

            NetworkMode = source.NetworkMode,

            PortBindings = source.PortBindings?.ToDictionary(
                x => x.Key,
                x => (IList<DockerPortBinding>)x.Value
                    .Select(p => new DockerPortBinding
                    {
                        HostIP = p.HostIP,
                        HostPort = p.HostPort
                    })
                    .ToList()) ?? new Dictionary<string, IList<DockerPortBinding>>(),

            RestartPolicy = source.RestartPolicy == null
                ? new DockerRestartPolicy()
                : ConvertRestartPolicy(source.RestartPolicy),

            AutoRemove = source.AutoRemove,
            VolumeDriver = source.VolumeDriver,
            VolumesFrom = source.VolumesFrom,

            CapAdd = source.CapAdd,
            CapDrop = source.CapDrop,

            CgroupnsMode = source.CgroupnsMode,

            Dns = source.DNS,
            DnsOptions = source.DNSOptions,
            DnsSearch = source.DNSSearch,
            ExtraHosts = source.ExtraHosts,
            GroupAdd = source.GroupAdd,

            IpcMode = source.IpcMode,
            Cgroup = source.Cgroup,
            Links = source.Links,

            OomScoreAdj = source.OomScoreAdj,
            PidMode = source.PidMode,

            Privileged = source.Privileged,
            PublishAllPorts = source.PublishAllPorts,
            ReadonlyRootfs = source.ReadonlyRootfs,

            SecurityOpt = source.SecurityOpt,
            StorageOpt = source.StorageOpt,
            Tmpfs = source.Tmpfs,

            UtsMode = source.UTSMode,
            UsernsMode = source.UsernsMode,

            ShmSize = source.ShmSize,
            Sysctls = source.Sysctls,

            Runtime = source.Runtime,
            ConsoleSize = source.ConsoleSize,
            Isolation = source.Isolation,

            CpuShares = source.CPUShares,
            Memory = source.Memory,
            NanoCPUs = source.NanoCPUs,

            CgroupParent = source.CgroupParent,

            BlkioWeight = source.BlkioWeight,

            BlkioDeviceReadBps = ConvertThrottleDevices(
                source.BlkioDeviceReadBps),

            BlkioDeviceWriteBps = ConvertThrottleDevices(
                source.BlkioDeviceWriteBps),

            BlkioDeviceReadIOps = ConvertThrottleDevices(
                source.BlkioDeviceReadIOps),

            BlkioDeviceWriteIOps = ConvertThrottleDevices(
                source.BlkioDeviceWriteIOps),

            CpuPeriod = source.CPUPeriod,
            CpuQuota = source.CPUQuota,
            CpuRealtimePeriod = source.CPURealtimePeriod,
            CpuRealtimeRuntime = source.CPURealtimeRuntime,

            CpusetCpus = source.CpusetCpus,
            CpusetMems = source.CpusetMems,

            Devices = source.Devices?.Select(x => new DockerDeviceMapping
            {
                PathOnHost = x.PathOnHost,
                PathInContainer = x.PathInContainer,
                CgroupPermissions = x.CgroupPermissions
            }).ToList() ?? new List<DockerDeviceMapping>(),

            DeviceCgroupRules = source.DeviceCgroupRules,

            DeviceRequests = source.DeviceRequests?.Select(x =>
                new DockerDeviceRequest
                {
                    Driver = x.Driver,
                    Count = x.Count,
                    DeviceIDs = x.DeviceIDs,
                    Capabilities = x.Capabilities,
                    Options = x.Options
                }).ToList() ?? new List<DockerDeviceRequest>(),

            KernelMemory = source.KernelMemory,
            KernelMemoryTCP = source.KernelMemoryTCP,

            MemoryReservation = source.MemoryReservation,
            MemorySwap = source.MemorySwap,
            MemorySwappiness = source.MemorySwappiness,

            OomKillDisable = source.OomKillDisable,
            PidsLimit = source.PidsLimit,

            Ulimits = source.Ulimits?.Select(x => new DockerUlimit
            {
                Name = x.Name,
                Hard = x.Hard,
                Soft = x.Soft
            }).ToList() ?? new List<DockerUlimit>(),

            CpuCount = source.CPUCount,
            CpuPercent = source.CPUPercent,

            IoMaximumIOps = source.IOMaximumIOps,
            IoMaximumBandwidth = source.IOMaximumBandwidth,

            Mounts = source.Mounts?.Select(x => new DockerMount
            {
                Type = x.Type,
                Source = x.Source,
                Target = x.Target,
                ReadOnly = x.ReadOnly,
                Consistency = x.Consistency,

                BindOptions = x.BindOptions == null
                    ? new DockerBindOptions()
                    : new DockerBindOptions
                    {
                        Propagation = x.BindOptions.Propagation,
                        NonRecursive = x.BindOptions.NonRecursive
                    },

                VolumeOptions = x.VolumeOptions == null
                    ? new DockerVolumeOptions()
                    : new DockerVolumeOptions
                    {
                        NoCopy = x.VolumeOptions.NoCopy,
                        Labels = x.VolumeOptions.Labels,
                        DriverConfig = x.VolumeOptions.DriverConfig == null
                            ? new DockerDriver()
                            : new DockerDriver
                            {
                                Name = x.VolumeOptions.DriverConfig.Name,
                                Options = x.VolumeOptions.DriverConfig.Options
                            }
                    },

                TmpfsOptions = x.TmpfsOptions == null
                    ? new DockerTmpfsOptions()
                    : new DockerTmpfsOptions
                    {
                        SizeBytes = x.TmpfsOptions.SizeBytes,
                        Mode = x.TmpfsOptions.Mode
                    }
            }).ToList() ?? new List<DockerMount>(),

            MaskedPaths = source.MaskedPaths,
            ReadonlyPaths = source.ReadonlyPaths,

            Init = source.Init
        };
    }

    private static DockerContainerConfig ConvertConfig(Config source)
    {
        return new DockerContainerConfig
        {
            Hostname = source.Hostname,
            Domainname = source.Domainname,
            User = source.User,
            AttachStdin = source.AttachStdin,
            AttachStdout = source.AttachStdout,
            AttachStderr = source.AttachStderr,
            ExposedPorts = ConvertEmptyStructDictionary(source.ExposedPorts),
            Tty = source.Tty,
            OpenStdin = source.OpenStdin,
            StdinOnce = source.StdinOnce,
            Env = source.Env,
            Cmd = source.Cmd,
            Healthcheck = source.Healthcheck == null
                ? new DockerHealthConfig()
                : ConvertHealthConfig(source.Healthcheck),
            ArgsEscaped = source.ArgsEscaped,
            Image = source.Image,
            Volumes = ConvertEmptyStructDictionary(source.Volumes),
            WorkingDir = source.WorkingDir,
            Entrypoint = source.Entrypoint,
            NetworkDisabled = source.NetworkDisabled,
            MacAddress = source.MacAddress,
            OnBuild = source.OnBuild,
            Labels = source.Labels,
            StopSignal = source.StopSignal,
            StopTimeout = source.StopTimeout,
            Shell = source.Shell
        };
    }

    private static DockerNetworkSetting ConvertNetworkSettings(
        NetworkSettings source)
    {
        return new DockerNetworkSetting
        {
            Bridge = source.Bridge,
            SandboxID = source.SandboxID,
            HairpinMode = source.HairpinMode,
            LinkLocalIPv6Address = source.LinkLocalIPv6Address,
            LinkLocalIPv6PrefixLen = source.LinkLocalIPv6PrefixLen,
            SandboxKey = source.SandboxKey,
            EndpointID = source.EndpointID,
            Gateway = source.Gateway,
            GlobalIPv6Address = source.GlobalIPv6Address,
            GlobalIPv6PrefixLen = source.GlobalIPv6PrefixLen,
            IpAddress = source.IPAddress,
            IpPrefixLen = source.IPPrefixLen,
            Ipv6Gateway = source.IPv6Gateway,
            MacAddress = source.MacAddress,

            Ports = source.Ports?.ToDictionary(
                x => x.Key,
                x => (IList<DockerPortBinding>)x.Value?.Select(p =>
                    new DockerPortBinding
                    {
                        HostIP = p.HostIP,
                        HostPort = p.HostPort
                    }).ToList()) ?? new Dictionary<string, IList<DockerPortBinding>>(),

            SecondaryIPAddresses = source.SecondaryIPAddresses?
                .Select(ConvertAddress)
                .ToList() ?? new List<DockerAddress>(),

            SecondaryIPv6Addresses = source.SecondaryIPv6Addresses?
                .Select(ConvertAddress)
                .ToList() ?? new List<DockerAddress>(),

            Networks = source.Networks?.ToDictionary(
                x => x.Key,
                x => ConvertEndpointSettings(x.Value)) ?? new Dictionary<string, DockerEndpointSettings>()
        };
    }

    private static DockerAddress ConvertAddress(Address source)
    {
        return new DockerAddress
        {
            Addr = source.Addr,
            PrefixLen = source.PrefixLen
        };
    }

    private static IDictionary<string, DockerEmptyStruct> ConvertEmptyStructDictionary(
        IDictionary<string, EmptyStruct>? source)
    {
        return source?.ToDictionary(x => x.Key, _ => new DockerEmptyStruct())
               ?? new Dictionary<string, DockerEmptyStruct>();
    }

    private static IList<DockerThrottleDevice> ConvertThrottleDevices(
        IList<ThrottleDevice>? source)
    {
        return source?.Select(x => new DockerThrottleDevice
        {
            Path = x.Path,
            Rate = x.Rate
        }).ToList() ?? new List<DockerThrottleDevice>();
    }

    private static DockerEndpointSettings ConvertEndpointSettings(EndpointSettings source)
    {
        return new DockerEndpointSettings
        {
            IpamConfig = source.IPAMConfig == null
                ? new DockerEndpointIPAMConfig()
                : new DockerEndpointIPAMConfig
                {
                    Ipv4Address = source.IPAMConfig.IPv4Address,
                    Ipv6Address = source.IPAMConfig.IPv6Address,
                    LinkLocalIPs = source.IPAMConfig.LinkLocalIPs
                },
            Links = source.Links,
            Aliases = source.Aliases,
            NetworkID = source.NetworkID,
            EndpointID = source.EndpointID,
            Gateway = source.Gateway,
            IpAddress = source.IPAddress,
            IpPrefixLen = source.IPPrefixLen,
            IPv6Gateway = source.IPv6Gateway,
            GlobalIPv6Address = source.GlobalIPv6Address,
            GlobalIPv6PrefixLen = source.GlobalIPv6PrefixLen,
            MacAddress = source.MacAddress,
            DriverOpts = source.DriverOpts
        };
    }

    private static DockerRestartPolicy ConvertRestartPolicy(
        RestartPolicy source)
    {
        return new DockerRestartPolicy
        {
            Name = (DockerRestartPolicyKind)(int)source.Name,
            MaximumRetryCount = source.MaximumRetryCount
        };
    }

    private static DockerHealthConfig ConvertHealthConfig(HealthConfig source)
    {
        return new DockerHealthConfig
        {
            Test = source.Test,
            Interval = source.Interval,
            Timeout = source.Timeout,
            StartPeriod = source.StartPeriod,
            Retries = source.Retries
        };
    }

    public static DockerContainer ConvertToDockerContainer(ContainerListResponse container)
    {
        var dockerContainer = new DockerContainer
        {
            Id = container.ID,
            Names = container.Names,
            Image = container.Image,
            ImageID = container.ImageID,
            Command = container.Command,
            Created = container.Created,
            SizeRw = container.SizeRw,
            SizeRootFs = container.SizeRootFs,
            Labels = container.Labels,
            State = container.State,
            Status = container.Status,
        };

        foreach (var dockerContainerPort in container.Ports)
        {
            dockerContainer.Ports.Add(new DockerPort
            {
                Ip = dockerContainerPort.IP,
                PrivatePort = dockerContainerPort.PrivatePort,
                PublicPort = dockerContainerPort.PublicPort,
                Type = dockerContainerPort.Type,
            });
        }

        foreach (var containerMount in container.Mounts)
        {
            dockerContainer.Mounts.Add(new DockerMountPoint
            {
                Name = containerMount.Name,
                Source = containerMount.Source,
                Destination = containerMount.Destination,
                Driver = containerMount.Driver,
                Mode = containerMount.Mode,
                Rw = containerMount.RW,
                Propagation = containerMount.Propagation
            });
        }

        dockerContainer.NetworkSettings = new DockerSummaryNetworkSettings();
        container.NetworkSettings.Networks?.ToList().ForEach(network =>
        {
            dockerContainer.NetworkSettings.Networks.Add(
                network.Key,
                new DockerEndpointSettings
                {
                    IpAddress = network.Value.IPAddress,
                    Gateway = network.Value.Gateway,
                    MacAddress = network.Value.MacAddress,
                    NetworkID = network.Value.NetworkID
                });
        });

        return dockerContainer;
    }
    
    
    public static DockerImage ConvertToDockerImage(ImagesListResponse image)
    {
        return new DockerImage
        {
            Id = image.ID,
            RepoTags = image.RepoTags ?? new List<string>(),
            Created = image.Created,
            Size = image.Size,
            Containers = image.Containers
        };
    }

    public static DockerVolume ConvertToDockerVolume(VolumeResponse volume)
    {
        return new DockerVolume
        {
            Name = volume.Name ?? string.Empty,
            Driver = volume.Driver ?? string.Empty,
            Mountpoint = volume.Mountpoint ?? string.Empty,
            Scope = volume.Scope ?? string.Empty,
            CreatedAt = volume.CreatedAt ?? string.Empty,
            RefCount = volume.UsageData?.RefCount ?? 0,
            Size = volume.UsageData?.Size ?? 0
        };
    }
}