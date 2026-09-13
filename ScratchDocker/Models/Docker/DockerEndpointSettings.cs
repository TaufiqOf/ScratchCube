using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using Docker.DotNet.Models;
using ScratchDocker.ViewModels;

namespace ScratchDocker.Models.Docker;

public partial class DockerEndpointSettings : ViewModelBase
{
    [ObservableProperty]
    private EndpointIPAMConfig _ipamConfig = default!;

    [ObservableProperty]
    private IList<string> _links = new List<string>();

    [ObservableProperty]
    private IList<string> _aliases = new List<string>();

    [ObservableProperty]
    private string _networkID = string.Empty;

    [ObservableProperty]
    private string _endpointID = string.Empty;

    [ObservableProperty]
    private string _gateway = string.Empty;

    [ObservableProperty]
    private string _ipAddress = string.Empty;

    [ObservableProperty]
    private long _ipPrefixLen;

    [ObservableProperty]
    private string _iPv6Gateway = string.Empty;

    [ObservableProperty]
    private string _globalIPv6Address = string.Empty;

    [ObservableProperty]
    private long _globalIPv6PrefixLen;

    [ObservableProperty]
    private string _macAddress = string.Empty;

    [ObservableProperty]
    private IDictionary<string, string> _driverOpts =
        new Dictionary<string, string>();
}