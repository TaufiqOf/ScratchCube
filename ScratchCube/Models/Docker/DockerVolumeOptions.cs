using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using ScratchCube.ViewModels;

namespace ScratchCube.Models.Docker;

public partial class DockerVolumeOptions : ViewModelBase
{
    [ObservableProperty] private bool _noCopy;
    [ObservableProperty] private IDictionary<string, string> _labels;
    [ObservableProperty] private DockerDriver _driverConfig;
}