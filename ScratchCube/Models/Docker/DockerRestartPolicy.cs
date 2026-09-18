using CommunityToolkit.Mvvm.ComponentModel;
using ScratchCube.ViewModels;

namespace ScratchCube.Models.Docker;

public partial class DockerRestartPolicy : ViewModelBase
{
    [ObservableProperty]
    private DockerRestartPolicyKind _name;

    [ObservableProperty]
    private long _maximumRetryCount;
}