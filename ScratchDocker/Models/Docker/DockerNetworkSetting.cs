using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using ScratchDocker.ViewModels;

namespace ScratchDocker.Models.Docker;

public partial class DockerNetworkSetting : ViewModelBase
{
    [ObservableProperty]
    private string _bridge;

    [ObservableProperty]
    private string _sandboxID;

    [ObservableProperty]
    private bool _hairpinMode;

    [ObservableProperty]
    private string _linkLocalIPv6Address;

    [ObservableProperty]
    private long _linkLocalIPv6PrefixLen;

    [ObservableProperty]
    private IDictionary<string, IList<DockerPortBinding>> _ports;

    [ObservableProperty]
    private string _sandboxKey;

    [ObservableProperty]
    private IList<DockerAddress> _secondaryIPAddresses;

    [ObservableProperty]
    private IList<DockerAddress> _secondaryIPv6Addresses;

    [ObservableProperty]
    private string _endpointID;

    [ObservableProperty]
    private string _gateway;

    [ObservableProperty]
    private string _globalIPv6Address;

    [ObservableProperty]
    private long _globalIPv6PrefixLen;

    [ObservableProperty]
    private string _ipAddress;

    [ObservableProperty]
    private long _ipPrefixLen;

    [ObservableProperty]
    private string _ipv6Gateway;

    [ObservableProperty]
    private string _macAddress;

    [ObservableProperty]
    private IDictionary<string, DockerEndpointSettings> _networks;
}

public partial class DockerAddress : ViewModelBase
{
    [ObservableProperty]
    private string _addr;

    [ObservableProperty]
    private long _prefixLen;
}