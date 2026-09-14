using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using ScratchDocker.ViewModels;

namespace ScratchDocker.Models.Docker;

public partial class DockerGraphDriverData : ViewModelBase
{
    [ObservableProperty]
    private IDictionary<string, string> _data;

    [ObservableProperty]
    private string _name;
}