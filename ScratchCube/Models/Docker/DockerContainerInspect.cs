using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using ScratchCube.ViewModels;

namespace ScratchCube.Models.Docker;

public partial class DockerContainerInspect : ViewModelBase
{
    [ObservableProperty] private string _id;
    [ObservableProperty] private DateTime _created;
    [ObservableProperty] private string _path;
    [ObservableProperty] private IList<string> _args;
    [ObservableProperty] private DockerContianerState _state;
    [ObservableProperty] private string _image;
    [ObservableProperty] private string _resolvConfPath;
    [ObservableProperty] private string _hostnamePath;
    [ObservableProperty] private string _hostsPath;
    [ObservableProperty] private string _logPath;
    [ObservableProperty] private DockerContainerNode _node;
    [ObservableProperty] private string _name;
    [ObservableProperty] private long _restartCount;
    [ObservableProperty] private string _driver;
    [ObservableProperty] private string _platform;
    [ObservableProperty] private string _mountLabel;
    [ObservableProperty] private string _processLabel;
    [ObservableProperty] private string _appArmorProfile;
    [ObservableProperty] private IList<string> _execIDs;
    [ObservableProperty] private DockerHostConfig _hostConfig;
    [ObservableProperty] private DockerGraphDriverData _graphDriver;
    [ObservableProperty] private long? _sizeRw;
    [ObservableProperty] private long? _sizeRootFs;
    [ObservableProperty] private IList<DockerMountPoint> _mounts;
    [ObservableProperty] private DockerContainerConfig _config;
    [ObservableProperty] private DockerNetworkSetting _networkSettings;
}