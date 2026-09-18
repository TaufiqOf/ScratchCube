using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using ScratchCube.ViewModels;

namespace ScratchCube.Models.Docker;

public partial class DockerHealth : ViewModelBase
{
    [ObservableProperty]
    private string _status;

    [ObservableProperty]
    private long _failingStreak;

    [ObservableProperty]
    private IList<DockerHealthcheckResult> _log;
}