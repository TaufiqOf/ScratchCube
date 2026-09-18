using CommunityToolkit.Mvvm.ComponentModel;
using ScratchCube.ViewModels;

namespace ScratchCube.Models.Docker;

public partial class DockerThrottleDevice : ViewModelBase
{
    [ObservableProperty] private string _path;
    [ObservableProperty] private ulong _rate;
}