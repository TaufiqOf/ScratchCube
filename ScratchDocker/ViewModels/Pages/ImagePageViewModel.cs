using System.Linq;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ScratchDocker.Models;
using ScratchDocker.Models.Docker;
using ScratchDocker.Service;

namespace ScratchDocker.ViewModels.Pages;

public partial class ImagePageViewModel : ViewModelBase, IPageViewModel
{
    [ObservableProperty] private string _searchText = string.Empty;

    [ObservableProperty] public partial ObservableCollection<DockerImage> Images { get; private set; } = new();

    [ObservableProperty] public partial ObservableCollection<DockerImage> FilteredImages { get; private set; } = new();

    private readonly DockerService _dockerService = DockerService.Instance;
    [ObservableProperty] private bool _isConnected;

    public ImagePageViewModel()
    {
        DockerService.OnConnectionStatusChanged += OnConnectionStatusChanged;
    }

    public async Task LoadData()
    {
        DockerService.CurrentInstance.OnImageAdded = image =>
        {
            Images.Add(image);
            ApplySearch();
        };
        DockerService.CurrentInstance.OnImageRemoved = image =>
        {
            Images.Remove(image);
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
                    Images.Clear();
                    FilteredImages.Clear();
                }
                else
                {
                    (await _dockerService.GetImages(DockerService.CurrentInstance)).ToList().ForEach(Images.Add);
                }
                ApplySearch();
            }

            IsConnected = status;
        }
        ;
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
            FilteredImages = new ObservableCollection<DockerImage>(Images);
            return;
        }

        var results = Images.Where(image =>
            image.DisplayName.Contains(search, StringComparison.OrdinalIgnoreCase)
            || image.ShortId.Contains(search, StringComparison.OrdinalIgnoreCase));

        FilteredImages = new ObservableCollection<DockerImage>(results);
    }

    [RelayCommand]
    public async Task Refresh()
    {
        Images = await _dockerService.GetImages(DockerService.CurrentInstance);
        ApplySearch();
    }
}