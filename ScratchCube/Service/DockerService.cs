using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Avalonia.Threading;
using ScratchCube.Models.Docker;
using Timer = System.Timers.Timer;

namespace ScratchCube.Service;

public class DockerService
{
    public readonly Timer RefreshTimer = new Timer(5000);
    public readonly ObservableCollection<DockerInstance> DockerInstances;


    private DockerService()
    {
        DockerInstances = new ObservableCollection<DockerInstance>();
        RefreshTimer.Elapsed += RefreshTimerOnElapsed;
      
        RefreshTimer.Start();
    }

    public DockerInstance? CurrentInstance
    {
        get => _currentInstance;
        set
        {
            if (_currentInstance != null)
            {
                _currentInstance.OnConnectionStatusChanged -= CurrentInstanceOnConnectionStatusChanged;
            }
            _currentInstance = value;
            if (_currentInstance != null)
            {
                _currentInstance.OnConnectionStatusChanged += CurrentInstanceOnConnectionStatusChanged;
            }
        }
    }

    public bool IsConnected { get; set; }
    public Action<bool>? OnConnectionStatusChanged;
    public Action<DockerContainer>? OnContainerAdded { get; set; }
    public Action<DockerContainer>? OnContainerRemoved { get; set; }

    public ObservableCollection<DockerContainer> DockerContainers => CurrentInstance?.DockerContainers ?? new ObservableCollection<DockerContainer>();
    public ObservableCollection<DockerImage> DockerImages=> CurrentInstance?.DockerImages ?? new ObservableCollection<DockerImage>();
    public ObservableCollection<DockerVolume> DockerVolumes => CurrentInstance?.DockerVolumes ?? new ObservableCollection<DockerVolume>();
    public Action<DockerImage>? OnImageAdded { get; set; }
    public Action<DockerImage>? OnImageRemoved { get; set; }
    public Action<DockerVolume>? OnVolumeRemoved { get; set; }
    public Action<DockerVolume>? OnVolumeAdded { get; set; }

    private void CurrentInstanceOnConnectionStatusChanged(bool obj)
    {
        IsConnected = obj;
        OnConnectionStatusChanged?.Invoke(obj);
    }


    public static DockerService Instance
    {
        get { return _instance ??= new DockerService(); }
    }

    private static DockerService? _instance;
    private static DockerInstance? _currentInstance;

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

    public async Task Connect()
    {
        if(CurrentInstance == null)
        {
            throw new InvalidOperationException("No Docker instance selected.");
        }
        await CurrentInstance.Connect();
    }

    public async Task<ObservableCollection<DockerContainer>> GetContainers()
    {
        if(CurrentInstance == null)
        {
            throw new InvalidOperationException("No Docker instance selected.");
        }
        return (await CurrentInstance.ListContainers()) ?? new ObservableCollection<DockerContainer>();
    }

    public async Task<ObservableCollection<DockerImage>> GetImages()
    {
        if(CurrentInstance == null)
        {
            throw new InvalidOperationException("No Docker instance selected.");
        }
        return (await CurrentInstance.ListImages()) ?? new ObservableCollection<DockerImage>();
    }

    public async Task<ObservableCollection<DockerVolume>> GetVolumes()
    {
        if(CurrentInstance == null)
        {
            throw new InvalidOperationException("No Docker instance selected.");
        }
        return (await CurrentInstance.ListVolumes()) ?? new ObservableCollection<DockerVolume>();
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
            await RefreshContainers();
            await RefreshImages();
            await RefreshVolumes();
        }

        RefreshTimer.Start();
    }

    public async Task<bool> CheckIsDockerInstalledAsync()
    {
        // First fast check via PATH
        if (IsDockerInstalled()) return true;

        // Fallback check via systemctl
        return await IsDockerServiceInstalledAsync();
    }

    public async Task StartEngine()
    {
        if (!await CheckIsDockerInstalledAsync())
        {
            throw new InvalidOperationException("Docker is not installed on this system.");
        }

        await RunSystemctl("start", "docker.service", "docker.socket");
        await Connect();
    }
    
    public async Task StopEngine()
    {
        RefreshTimer.Stop();

        await RunSystemctl("stop", "docker.service", "docker.socket");

        IsConnected = false;
        OnConnectionStatusChanged?.Invoke(false);
    }

    

    public async Task RefreshContainers()
    {
        if (CurrentInstance != null)
        {
            await CurrentInstance.RefreshContainers();
        }
    }
    
    public async Task RefreshVolumes()
    {
        if (CurrentInstance != null)
        {
            await CurrentInstance.RefreshVolumes();
        }
    }

    public async Task RefreshImages()
    {
        if (CurrentInstance != null)
        {
            await CurrentInstance.RefreshImages();
        }
    }
            
    public async Task<DockerContainerInspect?> InspectContainer(DockerContainer selectedContainer)
    {
        if (CurrentInstance != null)
        {
            return await CurrentInstance.InspectContainer(selectedContainer);
        }
        return null;
    }

    public async Task StatsContainer(DockerContainer selectedContainer,
        Progress<DockerContainerStatsResponse> progress, CancellationToken token)
    {
        if (CurrentInstance != null)
        {
            await CurrentInstance.StatsContainer(selectedContainer, progress, token);
        }
    }

    public async Task LogsContainer(DockerContainer selectedContainer, Progress<string> progress,
        CancellationToken token)
    {
        if (CurrentInstance != null)
        {
            await CurrentInstance.LogsContainer(selectedContainer, progress, token);
        }
    }
    
    private static async Task RunSystemctl(
        string action,
        params string[] units)
    {
        var arguments = $"{action} {string.Join(" ", units)}";

        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "systemctl",
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            }
        };

        process.Start();

        var outputTask = process.StandardOutput.ReadToEndAsync();
        var errorTask = process.StandardError.ReadToEndAsync();

        await process.WaitForExitAsync();

        var output = await outputTask;
        var error = await errorTask;

        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException(
                $"systemctl {arguments} failed ({process.ExitCode}): {error}");
        }
    }
    
    private static async Task<bool> IsDockerServiceInstalledAsync()
    {
        try
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "systemctl",
                    Arguments = "status docker.service",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                }
            };

            process.Start();
            await process.WaitForExitAsync();

            // Exit code 4 specifically means unit file is missing/not found
            return process.ExitCode != 4;
        }
        catch
        {
            // systemctl command itself failed or isn't installed
            return false;
        }
    }

    private static bool IsDockerInstalled()
    {
        var pathEnv = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
        var paths = pathEnv.Split(Path.PathSeparator);

        foreach (var path in paths)
        {
            var fullPath = Path.Combine(path, "docker");
            if (File.Exists(fullPath))
            {
                return true;
            }
        }

        return false;
    }


}