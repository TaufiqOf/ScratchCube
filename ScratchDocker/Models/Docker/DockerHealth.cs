using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using ScratchDocker.ViewModels;

namespace ScratchDocker.Models.Docker;

public partial class DockerHealth : ViewModelBase
{
    [ObservableProperty]
    private string _status;

    [ObservableProperty]
    private long _failingStreak;

    [ObservableProperty]
    private IList<DockerHealthcheckResult> _log;
}