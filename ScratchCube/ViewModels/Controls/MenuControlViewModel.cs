using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ScratchCube.Models;
using ScratchCube.Service;

namespace ScratchCube.ViewModels.Controls;

public partial class MenuControlViewModel : ViewModelBase
{
    [ObservableProperty] private bool _connected;

    [ObservableProperty] private string _engineStatusText;
    
    [ObservableProperty] private Brush _engineStatusColor;
    
    public Action? OnSettingsEngine { get; set; }
    public Action? OnStopEngine { get; set; }
    public Action? OnStartEngine { get; set; }
    public MenuControlViewModel(Action<PageMenuItem> navigateToPage, List<PageMenuItem> menuItems)
    {
        MenuItems = menuItems;
        NavigateToPage = navigateToPage;
        EngineStatusColor = new SolidColorBrush(Colors.Orange, 1);
        EngineStatusText = " Checking";
        DockerService.Instance.OnConnectionStatusChanged += Conected;
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
        EngineStatusText = connect ? " Connected" : " Disconnected";
        EngineStatusColor = connect ? new SolidColorBrush(Colors.Green, 1) : new SolidColorBrush(Colors.Red, 1);
    }
    
    [RelayCommand]
    private void StartEngine()
    {
        OnStartEngine?.Invoke();
    }
    [RelayCommand]
    private void StopEngine()
    {
        OnStopEngine?.Invoke();
    }
    [RelayCommand]
    private void OpenEngineSettings()
    {
        OnSettingsEngine?.Invoke();
    }

}