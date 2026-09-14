using CommunityToolkit.Mvvm.ComponentModel;
using ScratchDocker.ViewModels;

namespace ScratchDocker.Models.Docker;

public partial class DockerTmpfsOptions : ViewModelBase
{
    [ObservableProperty] private long _sizeBytes;
    [ObservableProperty] private uint _mode;
}