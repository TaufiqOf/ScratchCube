using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Docker.DotNet;
using Docker.DotNet.Models;
using ScratchDocker.Models.Docker;
using ScratchDocker.ViewModels;

namespace ScratchDocker.Service;

public partial class DockerInstance : ViewModelBase
{
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

    public void Connect()
    {
        _client = new DockerClientConfiguration(new Uri(_uri)).CreateClient();
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
                var dockerContainer = ConvertToDockerContainer(container);
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
                var dockerImage = ConvertToDockerImage(image);
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
                DockerVolumes.Add(ConvertToDockerVolume(volume));
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
                .Select(ConvertToDockerContainer)
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

            var newImages = images.Select(ConvertToDockerImage).ToList();
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

            var newVolumes = volumes.Volumes.Select(ConvertToDockerVolume).ToList();
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
    

    private async void StartStopContainer(DockerContainer container)
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
    

    private static DockerContainer ConvertToDockerContainer(ContainerListResponse container)
    {
        var dockerContainer = new DockerContainer
        {
            Id = container.ID,
            Names = container.Names,
            Image = container.Image,
            ImageID = container.ImageID,
            Command = container.Command,
            Created = container.Created,
            SizeRw = container.SizeRw,
            SizeRootFs = container.SizeRootFs,
            Labels = container.Labels,
            State = container.State,
            Status = container.Status,
        };

        foreach (var dockerContainerPort in container.Ports)
        {
            dockerContainer.Ports.Add(new DockerPort
            {
                Ip = dockerContainerPort.IP,
                PrivatePort = dockerContainerPort.PrivatePort,
                PublicPort = dockerContainerPort.PublicPort,
                Type = dockerContainerPort.Type,
            });
        }

        foreach (var containerMount in container.Mounts)
        {
            dockerContainer.Mounts.Add(new DockerMountPoint
            {
                Name = containerMount.Name,
                Source = containerMount.Source,
                Destination = containerMount.Destination,
                Driver = containerMount.Driver,
                Mode = containerMount.Mode,
                Rw = containerMount.RW,
                Propagation = containerMount.Propagation
            });
        }

        dockerContainer.NetworkSettings = new DockerSummaryNetworkSettings();
        container.NetworkSettings.Networks?.ToList().ForEach(network =>
        {
            dockerContainer.NetworkSettings.Networks.Add(
                network.Key,
                new DockerEndpointSettings
                {
                    IpAddress = network.Value.IPAddress,
                    Gateway = network.Value.Gateway,
                    MacAddress = network.Value.MacAddress,
                    NetworkID = network.Value.NetworkID
                });
        });

        return dockerContainer;
    }

    private static DockerImage ConvertToDockerImage(ImagesListResponse image)
    {
        return new DockerImage
        {
            Id = image.ID,
            RepoTags = image.RepoTags ?? new List<string>(),
            Created = image.Created,
            Size = image.Size,
            Containers = image.Containers
        };
    }

    private static DockerVolume ConvertToDockerVolume(VolumeResponse volume)
    {
        return new DockerVolume
        {
            Name = volume.Name ?? string.Empty,
            Driver = volume.Driver ?? string.Empty,
            Mountpoint = volume.Mountpoint ?? string.Empty,
            Scope = volume.Scope ?? string.Empty,
            CreatedAt = volume.CreatedAt ?? string.Empty,
            RefCount = volume.UsageData?.RefCount ?? 0,
            Size = volume.UsageData?.Size ?? 0
        };
    }
}