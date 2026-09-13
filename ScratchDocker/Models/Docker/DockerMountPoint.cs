using CommunityToolkit.Mvvm.ComponentModel;
using ScratchDocker.ViewModels;

namespace ScratchDocker.Models.Docker;

public partial class DockerMountPoint : ViewModelBase
{
    [ObservableProperty]
    private string _type = string.Empty;

    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private string _source = string.Empty;

    [ObservableProperty]
    private string _destination = string.Empty;

    [ObservableProperty]
    private string _driver = string.Empty;

    [ObservableProperty]
    private string _mode = string.Empty;

    [ObservableProperty]
    private bool _rw;

    [ObservableProperty]
    private string _propagation = string.Empty;
}