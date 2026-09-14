using CommunityToolkit.Mvvm.ComponentModel;
using ScratchDocker.ViewModels;

namespace ScratchDocker.Models.Docker;

public partial class DockerBindOptions : ViewModelBase
{
    [ObservableProperty] private string _propagation;
    [ObservableProperty] private bool _nonRecursive;
}