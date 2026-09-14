using CommunityToolkit.Mvvm.ComponentModel;
using ScratchDocker.ViewModels;

namespace ScratchDocker.Models.Docker;

public partial class DockerThrottleDevice : ViewModelBase
{
    [ObservableProperty] private string _path;
    [ObservableProperty] private ulong _rate;
}