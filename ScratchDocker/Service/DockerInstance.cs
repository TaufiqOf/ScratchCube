using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Docker.DotNet;
using Docker.DotNet.Models;
using ScratchDocker.Helper;
using ScratchDocker.Models.Docker;
using ScratchDocker.ViewModels;

namespace ScratchDocker.Service;

public partial class DockerInstance : ViewModelBase
{
    [ObservableProperty] private bool _isConnected;
    public Action<bool>? OnConnectionStatusChanged;
    private DockerClient _client;
    private string _uri;
    private string _name;
    [ObservableProperty] public partial ObservableCollection<DockerContainer> DockerContainers { get; private set; }
    [ObservableProperty] public partial ObservableCollection<DockerVolume> DockerVolumes { get; private set; }
    [ObservableProperty] public partial ObservableCollection<DockerImage> DockerImages { get; private set; }

    public Action<DockerContainer> OnContainerAdded { get; set; }
    public Action<DockerContainer> OnContainerRemoved { get; set; }
    public Action<DockerImage> OnImageAdded { get; set; }
    public Action<DockerImage> OnImageRemoved { get; set; }
    public Action<DockerVolume> OnVolumeAdded { get; set; }
    public Action<DockerVolume> OnVolumeRemoved { get; set; }
    public string Uri => _uri;
    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    public DockerInstance(string uri)
    {
        _uri = uri;
        DockerContainers = new ObservableCollection<DockerContainer>();
        DockerVolumes = new ObservableCollection<DockerVolume>();
        DockerImages = new ObservableCollection<DockerImage>();
    }

    public async Task<bool> GetIsConnected()
    {
        var currentConnection = IsConnected;
        try
        {
            await _client.System.PingAsync();
            IsConnected = true;
            return true;
        }
        catch
        {
            IsConnected = false;
            return false;
        }
        finally
        {
            if (currentConnection != IsConnected)
                OnConnectionStatusChanged?.Invoke(IsConnected);
        }
    }

    public async Task<bool> Connect()
    {
        try
        {
            _client = new DockerClientConfiguration(new Uri(_uri))
                .CreateClient();

            await _client.System.PingAsync();

            IsConnected = true;
            OnConnectionStatusChanged?.Invoke(true);
            return true;
        }
        catch (Exception e)
        {
            IsConnected = false;
            OnConnectionStatusChanged?.Invoke(false);
            Console.WriteLine(e);
            return false;
        }
    }


    public async Task<ObservableCollection<DockerContainer>> ListContainers()
    {
        try
        {
            var containers =
                await _client.Containers.ListContainersAsync(new ContainersListParameters() { All = true });
            DockerContainers.Clear();
            foreach (var container in containers)
            {
                var dockerContainer = Mapper.ConvertToDockerContainer(container);
                DockerContainers.Add(dockerContainer);
                dockerContainer.OnStartStop += StartStopContainer;
                dockerContainer.OnDelete += DeleteContainer;
            }

            return DockerContainers;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new ObservableCollection<DockerContainer>();
        }
    }

    public async Task<ObservableCollection<DockerImage>> ListImages()
    {
        try
        {
            var images = await _client.Images.ListImagesAsync(new ImagesListParameters
            {
                All = true
            });

            DockerImages = new ObservableCollection<DockerImage>();
            foreach (var image in images)
            {
                var dockerImage = Mapper.ConvertToDockerImage(image);
                dockerImage.OnDelete += DeleteImage;
                DockerImages.Add(dockerImage);
            }

            return DockerImages;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new ObservableCollection<DockerImage>();
        }
    }

    public async Task<ObservableCollection<DockerVolume>> ListVolumes()
    {
        try
        {
            var volumes = await _client.Volumes.ListAsync();
            DockerVolumes = new ObservableCollection<DockerVolume>();

            if (volumes.Volumes == null)
            {
                return DockerVolumes;
            }

            foreach (var volume in volumes.Volumes)
            {
                DockerVolumes.Add(Mapper.ConvertToDockerVolume(volume));
            }

            return DockerVolumes;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new ObservableCollection<DockerVolume>();
        }
    }

    public async Task RefreshContainers()
    {
        try
        {
            var containers = await _client.Containers.ListContainersAsync(
                new ContainersListParameters
                {
                    All = true
                });

            var newContainers = containers
                .Select(Mapper.ConvertToDockerContainer)
                .ToList();

            // Add / update
            foreach (var dockerContainer in newContainers)
            {
                var existingContainer = DockerContainers
                    .FirstOrDefault(q => q.Id == dockerContainer.Id);

                if (existingContainer == null)
                {
                    OnContainerAdded?.Invoke(dockerContainer);
                    DockerContainers.Add(dockerContainer);
                    dockerContainer.OnStartStop += StartStopContainer;
                    dockerContainer.OnDelete += DeleteContainer;
                }
                else
                {
                    existingContainer.Update(dockerContainer);
                }
            }

            // Remove containers that no longer exist in Docker
            var newContainerIds = newContainers
                .Select(q => q.Id)
                .ToHashSet();

            var removedContainers = DockerContainers
                .Where(q => !newContainerIds.Contains(q.Id))
                .ToList();

            foreach (var container in removedContainers)
            {
                DockerContainers.Remove(container);
                OnContainerRemoved?.Invoke(container);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public async Task RefreshImages()
    {
        try
        {
            var images = await _client.Images.ListImagesAsync(new ImagesListParameters
            {
                All = true
            });

            var newImages = images.Select(Mapper.ConvertToDockerImage).ToList();
            foreach (var dockerImage in newImages)
            {
                var existingImage = DockerImages.FirstOrDefault(q => q.Id == dockerImage.Id);
                if (existingImage == null)
                {
                    OnImageAdded?.Invoke(dockerImage);
                    DockerImages.Add(dockerImage);
                    dockerImage.OnDelete += DeleteImage;
                }
                else
                {
                    DockerImages[DockerImages.IndexOf(existingImage)].Update(dockerImage);
                }
            }

            // Remove images that no longer exist in Docker
            var newVolumeIds = newImages
                .Select(q => q.Id)
                .ToHashSet();

            var dockerImages = DockerImages
                .Where(q => !newVolumeIds.Contains(q.Id))
                .ToList();

            foreach (var image in dockerImages)
            {
                DockerImages.Remove(image);
                OnImageRemoved?.Invoke(image);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public async Task RefreshVolumes()
    {
        try
        {
            var volumes = await _client.Volumes.ListAsync();

            var newVolumes = volumes.Volumes.Select(Mapper.ConvertToDockerVolume).ToList();
            foreach (var dockerVolume in newVolumes)
            {
                var existingVolume = DockerVolumes.FirstOrDefault(q => q.Mountpoint == dockerVolume.Mountpoint);
                if (existingVolume == null)
                {
                    OnVolumeAdded?.Invoke(dockerVolume);
                    DockerVolumes.Add(dockerVolume);
                    dockerVolume.OnDelete += DeleteVolume;
                }
                else
                {
                    DockerVolumes[DockerVolumes.IndexOf(existingVolume)].Update(dockerVolume);
                }
            }

            // Remove volumes that no longer exist in Docker
            var newVolumeIds = newVolumes
                .Select(q => q.Name)
                .ToHashSet();

            var removedVolumes = DockerVolumes
                .Where(q => !newVolumeIds.Contains(q.Name))
                .ToList();

            foreach (var volume in removedVolumes)
            {
                DockerVolumes.Remove(volume);
                OnVolumeRemoved?.Invoke(volume);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public async Task<DockerContainerInspect?> InspectContainer(DockerContainer? container)
    {
        if (container == null)
        {
            return null;
        }

        var inspectResponse = await _client.Containers.InspectContainerAsync(container.Id);
        return Mapper.ConvertToDockerContainerInspect(inspectResponse);
    }

    public async Task StatsContainer(
        DockerContainer? container,
        IProgress<DockerContainerStatsResponse>? progress = null,
        CancellationToken cancellationToken = default)
    {
        if (container == null)
            return;

        var dockerProgress = new Progress<ContainerStatsResponse>(stats =>
        {
            var converted = Mapper.ConvertContainerStatsResponse(stats);
            progress?.Report(converted);
        });

        await _client.Containers.GetContainerStatsAsync(
            container.Id,
            new ContainerStatsParameters
            {
                Stream = true
            },
            dockerProgress,
            cancellationToken);
    }

    public async Task LogsContainer(
        DockerContainer? container,
        IProgress<string>? progress = null,
        CancellationToken cancellationToken = default)
    {
        if (container == null)
            return;

        using var stream = await _client.Containers.GetContainerLogsAsync(
            container.Id,
            new ContainerLogsParameters
            {
                ShowStdout = true,
                ShowStderr = true,
                Follow = true,
                Tail = "500"
            },
            cancellationToken);

        using var reader = new StreamReader(stream);

        while (!cancellationToken.IsCancellationRequested)
        {
            var line = await reader.ReadLineAsync(cancellationToken);

            if (line == null)
                break;

            progress?.Report(line + Environment.NewLine);
        }
    }

    private async void StartStopContainer(DockerContainer container)
    {
        try
        {
            var inspectResponse = await _client.Containers.InspectContainerAsync(container.Id);
            if (inspectResponse.State.Running)
            {
                await StopDockerContainer(container);
            }
            else
            {
                await StartDockerContainer(container);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Container state change failed: {ex.Message}");
        }
    }

    private async Task StopDockerContainer(DockerContainer container)
    {
        await _client.Containers.StopContainerAsync(container.Id, new ContainerStopParameters());
    }

    private async Task StartDockerContainer(DockerContainer container)
    {
        await _client.Containers.StartContainerAsync(container.Id, new ContainerStartParameters());
    }


    private async void DeleteContainer(DockerContainer obj)
    {
        await _client.Containers.RemoveContainerAsync(obj.Id, new ContainerRemoveParameters
        {
            Force = true
        });
    }

    private async void DeleteImage(DockerImage obj)
    {
        await _client.Images.DeleteImageAsync(obj.Id, new ImageDeleteParameters
        {
            Force = true,
        });
    }

    private async void DeleteVolume(DockerVolume obj)
    {
        await _client.Volumes.RemoveAsync(obj.Name, true);
    }
}