using CommunityToolkit.Mvvm.ComponentModel;
using ScratchDocker.ViewModels;

namespace ScratchDocker.Models.Docker;

public partial class DockerRestartPolicy : ViewModelBase
{
    [ObservableProperty]
    private DockerRestartPolicyKind _name;

    [ObservableProperty]
    private long _maximumRetryCount;
}