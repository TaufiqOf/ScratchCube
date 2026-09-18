using CommunityToolkit.Mvvm.ComponentModel;
using ScratchCube.ViewModels;

namespace ScratchCube.Models.Docker;

public partial class DockerPortBinding : ViewModelBase
{
    [ObservableProperty]
    private string _hostIP;

    [ObservableProperty]
    private string _hostPort;
}