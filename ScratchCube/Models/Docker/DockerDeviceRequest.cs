using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using ScratchCube.ViewModels;

namespace ScratchCube.Models.Docker;

public partial class DockerDeviceRequest : ViewModelBase
{
    [ObservableProperty] private string _driver;
    [ObservableProperty] private long _count;
    [ObservableProperty] private IList<string> _deviceIDs;
    [ObservableProperty] private IList<IList<string>> _capabilities;
    [ObservableProperty] private IDictionary<string, string> _options;
}