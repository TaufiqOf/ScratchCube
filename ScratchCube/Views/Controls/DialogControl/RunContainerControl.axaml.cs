using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using ScratchCube.Models.Docker;

namespace ScratchCube.Views.Controls.DialogControl;

public partial class RunContainerControl : UserControl
{
    private ObservableCollection<DockerImage> _dockerServiceDockerImages;
    public Func<string, string?, string?, string?, Task>? OnRunClicked { get; set; }

    public static readonly StyledProperty<string?> ImageNameProperty =
        AvaloniaProperty.Register<RunContainerControl, string?>(
            nameof(ImageName));

    public static readonly StyledProperty<string?> ContainerNameProperty =
        AvaloniaProperty.Register<RunContainerControl, string?>(
            nameof(ContainerName));

    public static readonly StyledProperty<string?> CommandProperty =
        AvaloniaProperty.Register<RunContainerControl, string?>(
            nameof(Command));

    public static readonly StyledProperty<string?> PortsProperty =
        AvaloniaProperty.Register<RunContainerControl, string?>(
            nameof(Ports));

    public static readonly StyledProperty<bool> IsRunningProperty =
        AvaloniaProperty.Register<RunContainerControl, bool>(
            nameof(IsRunning));

    public static readonly StyledProperty<string?> StatusProperty =
        AvaloniaProperty.Register<RunContainerControl, string?>(
            nameof(Status));
    

    public static readonly StyledProperty<DockerImage?> SelectedImageProperty =
        AvaloniaProperty.Register<RunContainerControl, DockerImage?>(
            nameof(SelectedImage));

    public string? ImageName
    {
        get => GetValue(ImageNameProperty);
        set => SetValue(ImageNameProperty, value);
    }

    public string? ContainerName
    {
        get => GetValue(ContainerNameProperty);
        set => SetValue(ContainerNameProperty, value);
    }

    public string? Command
    {
        get => GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public string? Ports
    {
        get => GetValue(PortsProperty);
        set => SetValue(PortsProperty, value);
    }

    public bool IsRunning
    {
        get => GetValue(IsRunningProperty);
        set => SetValue(IsRunningProperty, value);
    }

    public string? Status
    {
        get => GetValue(StatusProperty);
        set => SetValue(StatusProperty, value);
    }

    public ObservableCollection<DockerImage> DockerServiceDockerImages
    {
        get => _dockerServiceDockerImages;
        private set => _dockerServiceDockerImages = value;
    }
    

    public DockerImage? SelectedImage
    {
        get => GetValue(SelectedImageProperty);
        set => SetValue(SelectedImageProperty, value);
    }


    public RunContainerControl(ObservableCollection<DockerImage> dockerServiceDockerImages)
    {
        DockerServiceDockerImages = dockerServiceDockerImages;
        InitializeComponent();
    }

    private async void RunOnClick(object? sender, RoutedEventArgs e)
    {
        if (SelectedImage == null)
        {
            Status = "Select a Docker image.";
            return;
        }

        IsRunning = true;
        Status = $"Starting {SelectedImage.DisplayName}...";

        try
        {
            await OnRunClicked!.Invoke(
                SelectedImage.DisplayName,
                ContainerName,
                Command,
                Ports);

            Status = $"Successfully started {SelectedImage.DisplayName}.";
        }
        catch (Exception ex)
        {
            Status = $"Failed to start {SelectedImage.DisplayName}: {ex.Message}";
        }
        finally
        {
            IsRunning = false;
        }
    }

    private async Task RunContainerAsync()
    {
        if (OnRunClicked != null)
        {
            await OnRunClicked.Invoke(
                ImageName!,
                ContainerName,
                Command,
                Ports);
        }
    }
}