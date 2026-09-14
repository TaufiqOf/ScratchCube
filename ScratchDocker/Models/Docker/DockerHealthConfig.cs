using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using ScratchDocker.ViewModels;

namespace ScratchDocker.Models.Docker;

public partial class DockerHealthConfig : ViewModelBase
{
    [ObservableProperty]
    private IList<string> _test;

    [ObservableProperty]
    private TimeSpan _interval;

    [ObservableProperty]
    private TimeSpan _timeout;

    [ObservableProperty]
    private long _startPeriod;

    [ObservableProperty]
    private long _retries;
}