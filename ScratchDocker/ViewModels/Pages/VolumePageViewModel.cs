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
    private readonly DockerInstance _selectedInstance;

    public VolumePageViewModel()
    {
        _selectedInstance = _dockerService.DockerInstances[0];
        _dockerService.Connect(_selectedInstance);
    }

    public async Task LoadData()
    {
        Volumes = await _dockerService.GetVolumes(_selectedInstance);
        ApplySearch();
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
        Volumes = await _dockerService.GetVolumes(_selectedInstance);
        ApplySearch();
    }
}