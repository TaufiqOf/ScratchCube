using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using ScratchDocker.ViewModels;

namespace ScratchDocker.Models.Docker;

public partial class DockerContainerConfig : ViewModelBase
{
    [ObservableProperty]
    private string _hostname;

    [ObservableProperty]
    private string _domainname;

    [ObservableProperty]
    private string _user;

    [ObservableProperty]
    private bool _attachStdin;

    [ObservableProperty]
    private bool _attachStdout;

    [ObservableProperty]
    private bool _attachStderr;

    [ObservableProperty]
    private IDictionary<string, DockerEmptyStruct> _exposedPorts;

    [ObservableProperty]
    private bool _tty;

    [ObservableProperty]
    private bool _openStdin;

    [ObservableProperty]
    private bool _stdinOnce;

    [ObservableProperty]
    private IList<string> _env;

    [ObservableProperty]
    private IList<string> _cmd;

    [ObservableProperty]
    private DockerHealthConfig _healthcheck;

    [ObservableProperty]
    private bool _argsEscaped;

    [ObservableProperty]
    private string _image;

    [ObservableProperty]
    private IDictionary<string, DockerEmptyStruct> _volumes;

    [ObservableProperty]
    private string _workingDir;

    [ObservableProperty]
    private IList<string> _entrypoint;

    [ObservableProperty]
    private bool _networkDisabled;

    [ObservableProperty]
    private string _macAddress;

    [ObservableProperty]
    private IList<string> _onBuild;

    [ObservableProperty]
    private IDictionary<string, string> _labels;

    [ObservableProperty]
    private string _stopSignal;

    [ObservableProperty]
    private TimeSpan? _stopTimeout;

    [ObservableProperty]
    private IList<string> _shell;
}