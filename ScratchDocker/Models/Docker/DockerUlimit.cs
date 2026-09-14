using CommunityToolkit.Mvvm.ComponentModel;
using ScratchDocker.ViewModels;

namespace ScratchDocker.Models.Docker;

public partial class DockerUlimit : ViewModelBase
{
    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private long _hard;

    [ObservableProperty]
    private long _soft;
}