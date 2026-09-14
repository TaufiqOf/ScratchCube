using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using ScratchDocker.ViewModels;

namespace ScratchDocker.Models.Docker;

public partial class DockerVolumeOptions : ViewModelBase
{
    [ObservableProperty] private bool _noCopy;
    [ObservableProperty] private IDictionary<string, string> _labels;
    [ObservableProperty] private DockerDriver _driverConfig;
}