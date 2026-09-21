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
using ScratchCube.Helper;
using ScratchCube.Models;
using ScratchCube.Models.Docker;
using ScratchCube.ViewModels;

namespace ScratchCube.Service;

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

    public Action<DockerContainer>? OnContainerAdded { get; set; }
    public Action<DockerContainer>? OnContainerRemoved { get; set; }
    public Action<DockerImage>? OnImageAdded { get; set; }
    public Action<DockerImage>? OnImageRemoved { get; set; }
    public Action<DockerVolume>? OnVolumeAdded { get; set; }
    public Action<DockerVolume>? OnVolumeRemoved { get; set; }
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
                    DockerContainers.Add(dockerContainer);
                    OnContainerAdded?.Invoke(dockerContainer);
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
                    DockerImages.Add(dockerImage);
                    OnImageAdded?.Invoke(dockerImage);
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

    public async Task<string> RunContainer(
        string image,
        string? name = null,
        string? command = null,
        string? ports = null)
    {
        if (string.IsNullOrWhiteSpace(image))
        {
            throw new ArgumentException(
                "Docker image cannot be empty.",
                nameof(image));
        }

        if (_client == null)
        {
            throw new InvalidOperationException(
                "Docker instance is not connected.");
        }

        var config = new Config
        {
            Image = image,
            Tty = true,
            OpenStdin = true
        };

        /*
         * Command
         *
         * Example:
         *
         * "nginx -g daemon off;"
         *
         * becomes:
         *
         * ["nginx", "-g", "daemon", "off;"]
         */
        if (!string.IsNullOrWhiteSpace(command))
        {
            config.Cmd = command
                .Split(
                    ' ',
                    StringSplitOptions.RemoveEmptyEntries);
        }

        var hostConfig = new HostConfig();

        /*
         * Ports
         *
         * Example:
         *
         * 8080:80
         * 8080:80,8443:443
         */
        if (!string.IsNullOrWhiteSpace(ports))
        {
            config.ExposedPorts =
                new Dictionary<string, EmptyStruct>();

            hostConfig.PortBindings =
                new Dictionary<string, IList<PortBinding>>();

            foreach (var portMapping in ports.Split(
                         ',',
                         StringSplitOptions.RemoveEmptyEntries))
            {
                var mapping = portMapping.Trim();

                var parts = mapping.Split(':', 2);

                if (parts.Length != 2)
                {
                    throw new ArgumentException(
                        $"Invalid port mapping '{mapping}'. " +
                        "Expected format: HOST:CONTAINER");
                }

                var hostPort = parts[0].Trim();
                var containerPort = parts[1].Trim();

                if (!int.TryParse(hostPort, out var hostPortNumber) ||
                    !int.TryParse(containerPort, out var containerPortNumber))
                {
                    throw new ArgumentException(
                        $"Invalid port mapping '{mapping}'. " +
                        "Ports must be numeric.");
                }

                if (hostPortNumber is < 1 or > 65535 ||
                    containerPortNumber is < 1 or > 65535)
                {
                    throw new ArgumentException(
                        $"Invalid port mapping '{mapping}'. " +
                        "Ports must be between 1 and 65535.");
                }

                var dockerPort = $"{containerPortNumber}/tcp";

                config.ExposedPorts[dockerPort] = default;

                hostConfig.PortBindings[dockerPort] =
                    new List<PortBinding>
                    {
                        new()
                        {
                            HostPort = hostPortNumber.ToString()
                        }
                    };
            }
        }

        var response = await _client.Containers.CreateContainerAsync(
            new CreateContainerParameters(config)
            {
                Name = name,
                HostConfig = hostConfig
            });

        if (string.IsNullOrWhiteSpace(response.ID))
        {
            throw new InvalidOperationException(
                "Docker did not return a container ID.");
        }

        var started = await _client.Containers.StartContainerAsync(
            response.ID,
            new ContainerStartParameters());

        if (!started)
        {
            throw new InvalidOperationException(
                $"Docker created container '{response.ID}' " +
                "but failed to start it.");
        }

        return response.ID;
    }

    public async Task PullImage(
        string imageName,
        IProgress<DockerPullProgress>? progress = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(imageName))
        {
            throw new ArgumentException(
                "Image name cannot be empty.",
                nameof(imageName));
        }

        if (_client == null)
        {
            throw new InvalidOperationException(
                "Docker instance is not connected.");
        }

        var repository = imageName;
        var tag = "latest";

        // Split only the tag.
        // nginx:latest       -> nginx / latest
        // ubuntu             -> ubuntu / latest
        // ubuntu:24.04       -> ubuntu / 24.04
        //
        // Don't blindly split registry URLs here because
        // registry.example.com:5000/image contains a port.
        var lastSlash = imageName.LastIndexOf('/');
        var lastColon = imageName.LastIndexOf(':');

        if (lastColon > lastSlash)
        {
            repository = imageName[..lastColon];
            tag = imageName[(lastColon + 1)..];

            if (string.IsNullOrWhiteSpace(tag))
            {
                tag = "latest";
            }
        }

        var parameters = new ImagesCreateParameters
        {
            FromImage = repository,
            Tag = tag
        };

        await _client.Images.CreateImageAsync(
            parameters,
            null,
            new Progress<JSONMessage>(message =>
            {
                if (!string.IsNullOrWhiteSpace(message.ErrorMessage))
                {
                    progress?.Report(new DockerPullProgress
                    {
                        Id = message.ID,
                        Status = message.Status,
                        Error = message.ErrorMessage
                    });

                    return;
                }

                long? current = message.Progress?.Current;
                long? total = message.Progress?.Total;

                double? percentage = null;

                if (current.HasValue &&
                    total.HasValue &&
                    total.Value > 0)
                {
                    percentage = current.Value * 100.0 / total.Value;
                }

                progress?.Report(new DockerPullProgress
                {
                    Id = message.ID,
                    Status = message.Status,
                    Current = current,
                    Total = total,
                    Percentage = percentage
                });
            }),
            cancellationToken);
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