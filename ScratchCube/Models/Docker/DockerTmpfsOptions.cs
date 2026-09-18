using CommunityToolkit.Mvvm.ComponentModel;
using ScratchCube.ViewModels;

namespace ScratchCube.Models.Docker;

public partial class DockerTmpfsOptions : ViewModelBase
{
    [ObservableProperty] private long _sizeBytes;
    [ObservableProperty] private uint _mode;
}