using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentIcons.Common;
using ScratchDocker.Models;
using ScratchDocker.Service;
using ScratchDocker.ViewModels.Controls;
using ScratchDocker.ViewModels.Pages;

namespace ScratchDocker.ViewModels.Windows;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty] public partial IPageView CurrentPage { get; set; }

    public MenuControlViewModel MenuControlViewModel { get; set; }

    private Timer LazyLoadTimer { get; set; } = new Timer(300);
    private Timer LazyLoadConnectionTimer { get; set; } = new Timer(3000);

    public MainViewModel()
    {
        DockerService.Instance.AddDefaultDockerInstance();
        DockerService.Instance.CurrentInstance = DockerService.Instance.DockerInstances[0];
        MenuControlViewModel = new MenuControlViewModel(NavigateToPage, new List<PageMenuItem>
        {
            new("Containers", Icon.BoxMultiple, new Views.Pages.ContainerPageView(new ContainerPageViewModel())),
            new("Images", Icon.Layer, new Views.Pages.ImagePageView(new ImagePageViewModel())),
            new("Volumes", Icon.Database, new Views.Pages.VolumePageView(new VolumePageViewModel())),
        });
        MenuControlViewModel.OnStartEngine += OnStartEngine;
        MenuControlViewModel.OnStopEngine += OnStopEngine;
        MenuControlViewModel.OnSettingsEngine += OnSettingsEngine;
        LazyLoadTimer.Elapsed += LazyLoadTimerOnElapsed;
        LazyLoadConnectionTimer.Elapsed += LazyLoadConnectionTimerOnElapsed;
        LazyLoadConnectionTimer.Start();
        MenuControlViewModel.SelectedMenuItem = MenuControlViewModel.MenuItems[0];
    }

    private void OnSettingsEngine()
    {
    }

    private void OnStopEngine()
    {
        DockerService.Instance.StopEngine();
    }

    private void OnStartEngine()
    {
        DockerService.Instance.StartEngine();
    }

    private async void LazyLoadConnectionTimerOnElapsed(object? sender, ElapsedEventArgs e)
    {
        LazyLoadConnectionTimer.Stop();

        Dispatcher.UIThread.Post(async () =>
        {
            LazyLoadConnectionTimer.Stop();
            var connected = DockerService.Instance.IsConnected;
            if(!connected)
            {
                await DockerService.Instance.Connect();
       
            }
            LazyLoadConnectionTimer.Interval = 2000; // 2 seconds
            LazyLoadConnectionTimer.Start();
        }, DispatcherPriority.Background);

    }

    private async void LazyLoadTimerOnElapsed(object? sender, ElapsedEventArgs e)
    {
        LazyLoadTimer.Stop();
        Dispatcher.UIThread.Post(async void () => await CurrentPage.ViewModel.LoadData(),
            DispatcherPriority.Background);
    }

    private async void NavigateToPage(PageMenuItem pageMenuItem)
    {
        CurrentPage = pageMenuItem.PageView;
        LazyLoadTimer.Start();
    }
    
    
}