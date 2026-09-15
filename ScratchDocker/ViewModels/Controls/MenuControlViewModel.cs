using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using ScratchDocker.Models;
using ScratchDocker.Service;

namespace ScratchDocker.ViewModels.Controls;

public partial class MenuControlViewModel : ViewModelBase
{
    [ObservableProperty] private bool _connected;

    [ObservableProperty] private string _engineStatusText;
    
    [ObservableProperty] private Brush _engineStatusColor;
    public MenuControlViewModel(Action<PageMenuItem> navigateToPage, List<PageMenuItem> menuItems)
    {
        MenuItems = menuItems;
        NavigateToPage = navigateToPage;
        EngineStatusColor = new SolidColorBrush(Colors.Orange, 1);
        EngineStatusText = "Checking Engine Status...";
        DockerService.OnConnectionStatusChanged += Conected;
    }

    public List<PageMenuItem> MenuItems
    {
        get;
        set
        {
            if (Equals(value, field)) return;
            field = value;
            OnPropertyChanged();
        }
    }

    public PageMenuItem SelectedMenuItem
    {
        get;
        set
        {
            if (Equals(value, field)) return;
            field = value;
            NavigateToPage?.Invoke(value);
            OnPropertyChanged();
        }
    }

    public Action<PageMenuItem> NavigateToPage { get; set; }

    public void Conected(bool connect)
    {
        Connected = connect;
        EngineStatusText = connect ? "Engine Connected" : "Engine Disconnected";
        EngineStatusColor = connect ? new SolidColorBrush(Colors.Green, 1) : new SolidColorBrush(Colors.Red, 1);
    }
}