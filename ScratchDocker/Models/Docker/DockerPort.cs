using CommunityToolkit.Mvvm.ComponentModel;
using ScratchDocker.ViewModels;

namespace ScratchDocker.Models.Docker;

public partial class DockerPort : ViewModelBase
{
    [ObservableProperty]
    private string? _ip;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Display))]
    private ushort _privatePort;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Display))]
    private ushort _publicPort;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Display))]
    private string? _type;

    public string Display =>
        PublicPort > 0
            ? $"{PublicPort}:{PrivatePort}"
            : $"{PrivatePort}/{Type}";

    public override string ToString()
    {
        return Display;
    }
}