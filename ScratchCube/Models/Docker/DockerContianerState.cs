using CommunityToolkit.Mvvm.ComponentModel;
using ScratchCube.ViewModels;

namespace ScratchCube.Models.Docker;

public partial class DockerContianerState : ViewModelBase
{
    [ObservableProperty] private string _status;
    [ObservableProperty] private bool _running;
    [ObservableProperty] private bool _paused;
    [ObservableProperty] private bool _restarting;
    [ObservableProperty] private bool _oomKilled;
    [ObservableProperty] private bool _dead;
    [ObservableProperty] private long _pid;
    [ObservableProperty] private long _exitCode;
    [ObservableProperty] private string _error;
    [ObservableProperty] private string _startedAt;
    [ObservableProperty] private string _finishedAt;
    [ObservableProperty] private DockerHealth _health;
}