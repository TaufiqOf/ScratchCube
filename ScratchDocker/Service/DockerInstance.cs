using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
    [ObservableProperty] public partial ObservableCollection<DockerContainer> Containers { get; private set; }
    public Action<DockerContainer> OnContainerAdded { get; set; }

    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }


    public DockerInstance(string uri)
    {
        _uri = uri;
        Containers = new ObservableCollection<DockerContainer>();
    }

    public void Connect()
    {
        _client = new DockerClientConfiguration(
                new Uri(_uri))
            .CreateClient();
    }

    public async Task<ObservableCollection<DockerContainer>> ListContainers()
    {
        try
        {
            var containers = await _client.Containers.ListContainersAsync(new ContainersListParameters());
            Containers.Clear();
            foreach (var container in containers)
            {
                var dockerContainer = ConvertToDockerContainer(container);
                Containers.Add(dockerContainer);
            }

            return Containers;
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

            var dockerImages = new ObservableCollection<DockerImage>();
            foreach (var image in images)
            {
                dockerImages.Add(new DockerImage
                {
                    Id = image.ID,
                    RepoTags = image.RepoTags ?? new List<string>(),
                    Created = image.Created,
                    Size = image.Size,
                    Containers = image.Containers
                });
            }

            return dockerImages;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new ObservableCollection<DockerImage>();
        }
    }


    public async Task RefreshContainers()
    {
        if (_client == null)
            return;

        try
        {
            var containers = await _client.Containers.ListContainersAsync(
                new ContainersListParameters());

            var newContainers = containers
                .Select(ConvertToDockerContainer)
                .ToList();
            foreach (var dockerContainer in newContainers)
            {
                var existingContainer = Containers.FirstOrDefault(q => q.Id == dockerContainer.Id);
                if(existingContainer == null)
                {
                    OnContainerAdded?.Invoke(dockerContainer);
                    Containers.Add(dockerContainer);
                }
                else
                {
                    Containers[Containers.IndexOf(existingContainer)].UpdateToDockerContainer(dockerContainer);
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
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

}