using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace ScratchCube.Views.Controls.DialogControl;

public partial class RunContainerControl : UserControl
{
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

    public RunContainerControl()
    {
        InitializeComponent();
    }

    private async void RunOnClick(object? sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(ImageName))
        {
            Status = "Enter a Docker image name.";
            return;
        }

        IsRunning = true;
        Status = $"Starting {ImageName}...";

        try
        {
            await RunContainerAsync();

            Status = $"Successfully started {ImageName}.";
        }
        catch (Exception ex)
        {
            Status = $"Failed to start {ImageName}: {ex.Message}";
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