using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using ScratchCube.ViewModels;

namespace ScratchCube.Models.Docker;

public partial class DockerDriver : ViewModelBase
{
    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private IDictionary<string, string> _options;
}