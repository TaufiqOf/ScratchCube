using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ScratchDocker.Models;
using ScratchDocker.Models.Docker;
using ScratchDocker.Service;

namespace ScratchDocker.ViewModels.Pages;

public partial class ContainerPageViewModel : ViewModelBase, IPageViewModel
{
    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    public partial ObservableCollection<DockerContainer> Containers { get; private set; }
        = new();

    [ObservableProperty]
    public partial ObservableCollection<DockerContainer> FilteredContainers { get; private set; }
        = new();

    private readonly DockerService _dockerService = DockerService.Instance;

    private DockerInstance? _selectedInstance;
    public ContainerPageViewModel()
    {
        _selectedInstance = _dockerService.DockerInstances[0];
        _selectedInstance.OnContainerAdded = container =>
        {
            Containers.Add(container);
            ApplySearch();
        };
        _dockerService.Connect(_selectedInstance);
    }

    public async Task LoadData()
    {
        Containers = await _dockerService.GetContainers(_selectedInstance);

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
            FilteredContainers = new ObservableCollection<DockerContainer>(
                Containers);

            return;
        }

        var results = Containers.Where(container =>
            container.Id.Contains(search, StringComparison.OrdinalIgnoreCase)
            || container.DisplayName.Contains(search, StringComparison.OrdinalIgnoreCase)
            || container.Image.Contains(search, StringComparison.OrdinalIgnoreCase)
            || container.Status.Contains(search, StringComparison.OrdinalIgnoreCase)
            || container.State.Contains(search, StringComparison.OrdinalIgnoreCase)
            || container.Command.Contains(search, StringComparison.OrdinalIgnoreCase));

        FilteredContainers = new ObservableCollection<DockerContainer>(
            results);
    }

    [RelayCommand]
    public async Task Refresh()
    {
        await _selectedInstance.RefreshContainers();
        Containers = _selectedInstance.Containers;
        ApplySearch();
    }
}