using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Timers;
using Avalonia.Threading;
using ScratchDocker.Models.Docker;

namespace ScratchDocker.Service;

public class DockerService
{
    public static readonly Timer RefreshTimer = new Timer(5000);
    public readonly ObservableCollection<DockerInstance> DockerInstances;
    public static Action<bool>? OnConnectionStatusChanged;


    private DockerService()
    {
        DockerInstances = new ObservableCollection<DockerInstance>();
        RefreshTimer.Elapsed += RefreshTimerOnElapsed;
        RefreshTimer.Start();
    }

    public static DockerInstance CurrentInstance
    {
        get => _currentInstance;
        set
        {
            if (_currentInstance != value)
            {
                _currentInstance = value;
                _currentInstance.OnConnectionStatusChanged+= CurrentInstanceOnConnectionStatusChanged;
            }
        }
    }

    private static void CurrentInstanceOnConnectionStatusChanged(bool obj)
    {
        OnConnectionStatusChanged?.Invoke(obj);
    }


    public static DockerService Instance
    {
        get { return _instance ??= new DockerService(); }
    }

    private static DockerService _instance;
    private static DockerInstance _currentInstance;

    public void AddDockerInstance(string uri)
    {
        var instance = new DockerInstance(uri);
        DockerInstances.Add(instance);
    }

    public void AddDefaultDockerInstance()
    {
        var uri = "unix:///var/run/docker.sock";
        var instance = new DockerInstance(uri);
        DockerInstances.Add(instance);
    }

    public void RemoveDockerInstance(DockerInstance instance)
    {
        DockerInstances.Remove(instance);
    }

    public void ClearDockerInstances()
    {
        DockerInstances.Clear();
    }
    
    public void Connect(DockerInstance instance)
    {
        instance.Connect();
    }

    public async Task<ObservableCollection<DockerContainer>> GetContainers(DockerInstance instanceDockerInstance)
    {
        return await instanceDockerInstance.ListContainers();
    }

    public async Task<ObservableCollection<DockerImage>> GetImages(DockerInstance instanceDockerInstance)
    {
        return await instanceDockerInstance.ListImages();
    }

    public async Task<ObservableCollection<DockerVolume>> GetVolumes(DockerInstance instanceDockerInstance)
    {
        return await instanceDockerInstance.ListVolumes();
    }

    private async void RefreshTimerOnElapsed(object? sender, ElapsedEventArgs e)
    {
        Dispatcher.UIThread.Post(async void () => await UpdateInstanceData(), DispatcherPriority.Background);
    }

    private async Task UpdateInstanceData()
    {
        RefreshTimer.Stop();
        foreach (var instance in DockerInstances)
        {
            await instance.RefreshContainers();
            await instance.RefreshImages();
            await instance.RefreshVolumes();
        }

        RefreshTimer.Start();
    }
}