using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using ScratchDocker.ViewModels;

namespace ScratchDocker.Models.Docker;

public partial class DockerDriver : ViewModelBase
{
    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private IDictionary<string, string> _options;
}