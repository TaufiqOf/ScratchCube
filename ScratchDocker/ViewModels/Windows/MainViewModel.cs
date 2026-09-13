using System.Collections.Generic;
using System.Timers;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using FluentIcons.Common;
using ScratchDocker.Models;
using ScratchDocker.Service;
using ScratchDocker.ViewModels.Controls;
using ScratchDocker.ViewModels.Pages;

namespace ScratchDocker.ViewModels.Windows;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    public partial IPageView CurrentPage { get; set; }

    public MenuControlViewModel MenuControlViewModel { get; set; }
    
    private Timer LazyLoadTimer { get; set; } = new Timer(300);
    public MainViewModel()
    {
        DockerService.Instance.AddDefaultDockerInstance();
        MenuControlViewModel = new MenuControlViewModel(NavigateToPage, new List<PageMenuItem>
        {
            new("Containers", Icon.BoxMultiple, new Views.Pages.ContainerPageView(new ContainerPageViewModel())),
            new("Images", Icon.Layer, new Views.Pages.ImagePageView(new ImagePageViewModel())),
            new("Volumes", Icon.Database, new Views.Pages.VolumePageView(new VolumePageViewModel())),
        });
        LazyLoadTimer.Elapsed+= LazyLoadTimerOnElapsed;
        MenuControlViewModel.SelectedMenuItem = MenuControlViewModel.MenuItems[0];
    }

    private async void LazyLoadTimerOnElapsed(object? sender, ElapsedEventArgs e)
    {
        LazyLoadTimer.Stop();
        Dispatcher.UIThread.Post(async void () => await CurrentPage.ViewModel.LoadData(), DispatcherPriority.Background);
    }

    private void NavigateToPage(PageMenuItem pageMenuItem)
    {
        
        CurrentPage = pageMenuItem.PageView;
        LazyLoadTimer.Start();
    }
}