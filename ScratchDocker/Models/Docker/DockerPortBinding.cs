using CommunityToolkit.Mvvm.ComponentModel;
using ScratchDocker.ViewModels;

namespace ScratchDocker.Models.Docker;

public partial class DockerPortBinding : ViewModelBase
{
    [ObservableProperty]
    private string _hostIP;

    [ObservableProperty]
    private string _hostPort;
}