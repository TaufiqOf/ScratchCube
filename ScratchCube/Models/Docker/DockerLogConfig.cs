using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using ScratchCube.ViewModels;

namespace ScratchCube.Models.Docker;

public partial class DockerLogConfig : ViewModelBase
{
    [ObservableProperty]
    private string _type;

    [ObservableProperty]
    private IDictionary<string, string> _config;
}