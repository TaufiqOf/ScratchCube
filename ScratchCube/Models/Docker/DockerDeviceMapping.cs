using CommunityToolkit.Mvvm.ComponentModel;
using ScratchCube.ViewModels;

namespace ScratchCube.Models.Docker;

public partial class DockerDeviceMapping : ViewModelBase
{
    [ObservableProperty] private string _pathOnHost;
    [ObservableProperty] private string _pathInContainer;
    [ObservableProperty] private string _cgroupPermissions;
}