using System;
using System.Collections.Generic;
using System.Linq;
using Docker.DotNet;
using Docker.DotNet.Models;
using ScratchDocker.Models.Docker;

namespace ScratchDocker.Service;

public class DockerInstance
{
    private DockerClient _client;
    private string _uri;

    public DockerInstance(string uri)
    {
        _uri = uri;
    }

    public void Connect()
    {
        _client = new DockerClientConfiguration(
                new Uri(_uri))
            .CreateClient();
    }
    
    public List<DockerContainer> ListContainers()
    {
        var containers = _client.Containers.ListContainersAsync(new Docker.DotNet.Models.ContainersListParameters()).Result;
        var containerNames = new List<DockerContainer>();
        foreach (var container in containers)
        {
            var dockerContainer = ConvertToDockerContainer(container);
            containerNames.Add(dockerContainer);
        }
        return containerNames;
    }

    private static DockerContainer ConvertToDockerContainer(ContainerListResponse container)
    {
        var dockerContainer = new DockerContainer
        {
            ID = container.ID,
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
                IP = dockerContainerPort.IP,
                PrivatePort = dockerContainerPort.PrivatePort,
                PublicPort = dockerContainerPort.PublicPort,
                Type = dockerContainerPort.Type
            });
        }
        foreach (var dockerContainerLabel in container.Labels)
        {
            dockerContainer.Labels.Add(dockerContainerLabel.Key, dockerContainerLabel.Value);
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
                RW = containerMount.RW,
                Propagation = containerMount.Propagation
            });
        }

        dockerContainer.NetworkSettings = new DockerSummaryNetworkSettings();
        container.NetworkSettings.Networks?.ToList().ForEach(network =>
        {
            dockerContainer.NetworkSettings.Networks.Add(network.Key, new DockerEndpointSettings
            {
                IPAddress = network.Value.IPAddress,
                Gateway = network.Value.Gateway,
                MacAddress = network.Value.MacAddress,
                NetworkID = network.Value.NetworkID
            });
        });
        return dockerContainer;
    }
}