using CommunityToolkit.Mvvm.ComponentModel;
using ScratchCube.ViewModels;

namespace ScratchCube.Models.Docker;

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