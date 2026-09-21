using System.Linq;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ScratchCube.Helper;
using ScratchCube.Models;
using ScratchCube.Models.Docker;
using ScratchCube.Service;

namespace ScratchCube.ViewModels.Pages;

public partial class ImagePageViewModel : ViewModelBase, IPageViewModel
{
    [ObservableProperty] private string _searchText = string.Empty;

    [ObservableProperty] public partial ObservableCollection<DockerImage> Images { get; private set; } = new();

    [ObservableProperty] public partial ObservableCollection<DockerImage> FilteredImages { get; private set; } = new();

    private readonly DockerService _dockerService = DockerService.Instance;
    [ObservableProperty] private bool _isConnected;

    public ImagePageViewModel()
    {
        DockerService.Instance.OnConnectionStatusChanged += OnConnectionStatusChanged;
    }

    public async Task LoadData()
    {
        DockerService.Instance.OnImageAdded = image =>
        {
            Images.Add(image);
            ApplySearch();
        };
        DockerService.Instance.OnImageRemoved = image =>
        {
            Images.Remove(image);
            ApplySearch();
        };
        OnConnectionStatusChanged(DockerService.Instance.IsConnected);
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
                    (await _dockerService.GetImages()).ToList().ForEach(Images.Add);
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
        await DockerService.Instance.RefreshImages();
        Images =DockerService.Instance.DockerImages;
        ApplySearch();
    }
    
    [RelayCommand]
    public async Task OpenPullDialog()
    {
        var dialog = new Views.Controls.DialogControl.DockerPullControl();
        dialog.OnPullClicked = async imageName =>
        {
            await DockerService.Instance.PullImage(imageName,dialog.Progress);
        };
        await NotificationHelper.ShowDialogBoxAsync("Pull Docker Image", dialog);
    }
}