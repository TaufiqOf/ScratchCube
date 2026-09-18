using CommunityToolkit.Mvvm.ComponentModel;
using ScratchCube.ViewModels;

namespace ScratchCube.Models.Docker;

public partial class DockerBindOptions : ViewModelBase
{
    [ObservableProperty] private string _propagation;
    [ObservableProperty] private bool _nonRecursive;
}