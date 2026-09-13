using System;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using ScratchDocker.ViewModels;

namespace ScratchDocker.Models.Docker;

public partial class DockerContainer : ViewModelBase
{
    [ObservableProperty]
    private string _id = string.Empty;

    [ObservableProperty]
    private IList<string> _names = new List<string>();

    public string DisplayName =>
        _names.Count > 0
            ? string.Join(", ", _names).Replace("/", "")
            : _id;


    [ObservableProperty]
    private string _image = string.Empty;

    [ObservableProperty]
    private string _imageID = string.Empty;

    [ObservableProperty]
    private string _command = string.Empty;  

    [ObservableProperty]
    private DateTime _created;

    [ObservableProperty]
    private IList<DockerPort> _ports = new List<DockerPort>();

    public string DisplayPorts =>
        _ports.Count > 0
            ? string.Join(", ", _ports.Select(p => p))
            : string.Empty;


    [ObservableProperty]
    private long _sizeRw;

    [ObservableProperty]
    private long _sizeRootFs;

    [ObservableProperty]
    private IDictionary<string, string> _labels =
        new Dictionary<string, string>();

    [ObservableProperty]
    private string _state = string.Empty;

    [ObservableProperty]
    private string _status = string.Empty;

    [ObservableProperty]
    private DockerSummaryNetworkSettings _networkSettings =
        new DockerSummaryNetworkSettings();

    [ObservableProperty]
    private IList<DockerMountPoint> _mounts =
        new List<DockerMountPoint>();
    
    public void UpdateToDockerContainer(DockerContainer fromContainer)
    {
        Id = fromContainer.Id;
        Names = fromContainer.Names;
        Image = fromContainer.Image;
        ImageID = fromContainer.ImageID;
        Command = fromContainer.Command;
        Created = fromContainer.Created;
        Ports = fromContainer.Ports;
        SizeRw = fromContainer.SizeRw;
        SizeRootFs = fromContainer.SizeRootFs;
        Labels = fromContainer.Labels;
        State = fromContainer.State;
        Status = fromContainer.Status;
        NetworkSettings = fromContainer.NetworkSettings;
        Mounts = fromContainer.Mounts;
    }

}