using CommunityToolkit.Mvvm.ComponentModel;
using ScratchCube.ViewModels;

namespace ScratchCube.Models.Docker;

public partial class DockerMount : ViewModelBase
{
    [ObservableProperty] private string _type;
    [ObservableProperty] private string _source;
    [ObservableProperty] private string _target;
    [ObservableProperty] private bool _readOnly;
    [ObservableProperty] private string _consistency;
    [ObservableProperty] private DockerBindOptions _bindOptions;
    [ObservableProperty] private DockerVolumeOptions _volumeOptions;
    [ObservableProperty] private DockerTmpfsOptions _tmpfsOptions;
}