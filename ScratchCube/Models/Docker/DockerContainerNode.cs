using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using ScratchCube.ViewModels;

namespace ScratchCube.Models.Docker;

public partial class DockerContainerNode : ViewModelBase
{
    [ObservableProperty] private string _id;
    [ObservableProperty] private string _ipAddress;
    [ObservableProperty] private string _addr;
    [ObservableProperty] private string _name;
    [ObservableProperty] private long _cpus;
    [ObservableProperty] private long _memory;
    [ObservableProperty] private IDictionary<string, string> _labels;
}