using CommunityToolkit.Mvvm.ComponentModel;
using ScratchDocker.ViewModels;

namespace ScratchDocker.Models.Docker;

public partial class DockerDeviceMapping : ViewModelBase
{
    [ObservableProperty] private string _pathOnHost;
    [ObservableProperty] private string _pathInContainer;
    [ObservableProperty] private string _cgroupPermissions;
}