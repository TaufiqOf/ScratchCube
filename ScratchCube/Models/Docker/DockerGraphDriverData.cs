using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using ScratchCube.ViewModels;

namespace ScratchCube.Models.Docker;

public partial class DockerGraphDriverData : ViewModelBase
{
    [ObservableProperty]
    private IDictionary<string, string> _data;

    [ObservableProperty]
    private string _name;
}