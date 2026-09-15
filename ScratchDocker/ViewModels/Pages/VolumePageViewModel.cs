using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ScratchDocker.Models;
using ScratchDocker.Models.Docker;
using ScratchDocker.Service;

namespace ScratchDocker.ViewModels.Pages;

public partial class VolumePageViewModel : ViewModelBase, IPageViewModel
{
    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    public partial ObservableCollection<DockerVolume> Volumes { get; private set; } = new();

    [ObservableProperty]
    public partial ObservableCollection<DockerVolume> FilteredVolumes { get; private set; } = new();

    private readonly DockerService _dockerService = DockerService.Instance;
    [ObservableProperty] private bool _isConnected;

    public VolumePageViewModel()
    {
        DockerService.OnConnectionStatusChanged += OnConnectionStatusChanged;

    }

    public async Task LoadData()
    {

        DockerService.CurrentInstance.OnVolumeAdded = volume =>
        {
            Volumes.Add(volume);
            ApplySearch();
        };
        DockerService.CurrentInstance.OnVolumeRemoved = volume =>
        {
            Volumes.Remove(volume);
            ApplySearch();
        };
        OnConnectionStatusChanged(DockerService.CurrentInstance.IsConnected);
    }
    private async void OnConnectionStatusChanged(bool status)
    {
        {
            if (IsConnected != status)
            {
                if (!status)
                {
                    Volumes.Clear();
                    FilteredVolumes.Clear();
                }
                else
                {
                    (await _dockerService.GetVolumes(DockerService.CurrentInstance)).ToList().ForEach(Volumes.Add);
                }
                ApplySearch();
            }
            IsConnected = status;
        };
    }
    partial void OnSearchTextChanged(string value)
    {
        ApplySearch();
    }

    private void ApplySearch()
    {
        var search = SearchText.Trim();

        if (string.IsNullOrWhiteSpace(search))
        {
            FilteredVolumes = new ObservableCollection<DockerVolume>(Volumes);
            return;
        }

        var results = Volumes.Where(volume =>
            volume.Name.Contains(search, StringComparison.OrdinalIgnoreCase)
            || volume.Driver.Contains(search, StringComparison.OrdinalIgnoreCase)
            || volume.Mountpoint.Contains(search, StringComparison.OrdinalIgnoreCase)
            || volume.Scope.Contains(search, StringComparison.OrdinalIgnoreCase));

        FilteredVolumes = new ObservableCollection<DockerVolume>(results);
    }

    [RelayCommand]
    public async Task Refresh()
    {
        Volumes = await _dockerService.GetVolumes(DockerService.CurrentInstance);
        ApplySearch();
    }
}