using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentIcons.Common;
using ScratchCube.Helper;
using ScratchCube.Models;
using ScratchCube.Service;
using ScratchCube.ViewModels.Controls;
using ScratchCube.ViewModels.Pages;
using ScratchCube.Views.Windows;

namespace ScratchCube.ViewModels.Windows;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty] public partial IPageView CurrentPage { get; set; }

    public MenuControlViewModel MenuControlViewModel { get; set; }
    private bool? _isConnected = null;

    private Timer LazyLoadTimer { get; set; } = new Timer(300);
    private Timer LazyLoadConnectionTimer { get; set; } = new Timer(3000);

    public MainViewModel()
    {
        DockerService.Instance.AddDefaultDockerInstance();
        DockerService.Instance.CurrentInstance = DockerService.Instance.DockerInstances[0];
        DockerService.Instance.OnConnectionStatusChanged += ConnectionStatusChanged;
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

    private async void OnStartEngine()
    {
        try
        {
            await DockerService.Instance.StartEngine();
        }
        catch (InvalidOperationException ex)
        {
            var message = 
                "Docker is not installed on this system (or current user lacks socket permissions).\n" +
                "Run the following commands to install Docker and configure permissions:\n\n" +
                "# 1. Set up Docker's GPG key\n" +
                "sudo apt-get update\n" +
                "sudo apt-get install -y ca-certificates curl\n" +
                "sudo install -m 0755 -d /etc/apt/keyrings\n" +
                "sudo curl -fsSL https://download.docker.com/linux/ubuntu/gpg -o /etc/apt/keyrings/docker.asc\n" +
                "sudo chmod a+r /etc/apt/keyrings/docker.asc\n\n" +
                "# 2. Add Ubuntu-compatible repository (works on Ubuntu & Linux Mint)\n" +
                "echo \"deb [arch=$(dpkg --print-architecture) signed-by=/etc/apt/keyrings/docker.asc] https://download.docker.com/linux/ubuntu $(. /etc/os-release && echo \"${UBUNTU_CODENAME:-noble}\") stable\" | sudo tee /etc/apt/sources.list.d/docker.list > /dev/null\n\n" +
                "# 3. Install Docker Engine and start service\n" +
                "sudo apt-get update\n" +
                "sudo apt-get install -y docker-ce docker-ce-cli containerd.io docker-buildx-plugin docker-compose-plugin\n" +
                "sudo systemctl enable --now docker\n\n" +
                "# 4. Grant permissions to current user and Docker socket\n" +
                "sudo groupadd -f docker\n" +
                "sudo usermod -aG docker $USER\n" +
                "sudo chown root:docker /var/run/docker.sock\n" +
                "sudo chmod 660 /var/run/docker.sock\n\n" +
                "#IMPORTANT: After running these commands, LOG OUT and log back in (or restart) for permissions to take effect desktop-wide.";

            await NotificationHelper.ShowMessageBoxAsync("Docker Not Installed", message);
            Console.WriteLine(ex);
        }
        catch (Exception e)
        {
            NotificationHelper.Error("Failed to start Docker Engine", e.Message);
            Console.WriteLine(e);
        }
    }
    private void ConnectionStatusChanged(bool connected)
    {
        if(connected == _isConnected)
            return;
        if (connected)
        {
            NotificationHelper.Success("Docker Engine", connected ? "Connected" : "Disconnected");
        }
        else
        {
            NotificationHelper.Error("Docker Engine", connected ? "Connected" : "Disconnected");
        }

        _isConnected = connected;
    }

    private async void LazyLoadConnectionTimerOnElapsed(object? sender, ElapsedEventArgs e)
    {
        LazyLoadConnectionTimer.Stop();

        Dispatcher.UIThread.Post(async () =>
        {
            LazyLoadConnectionTimer.Stop();
            var connected = DockerService.Instance.IsConnected;
            if (!connected)
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