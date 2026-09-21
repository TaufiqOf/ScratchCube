using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using ScratchCube.Models;

namespace ScratchCube.Views.Controls.DialogControl;

public partial class DockerPullControl : UserControl
{
    public Func<string, Task>? OnPullClicked { get; set; }
    
    public static readonly StyledProperty<double?> ProgressValueProperty =
        AvaloniaProperty.Register<DockerPullControl, double?>(
            nameof(ProgressValue));
    
    public static readonly StyledProperty<string?> ImageNameProperty =
        AvaloniaProperty.Register<DockerPullControl, string?>(
            nameof(ImageName));

    public static readonly StyledProperty<bool> IsPullingProperty =
        AvaloniaProperty.Register<DockerPullControl, bool>(
            nameof(IsPulling));

    public static readonly StyledProperty<IProgress<DockerPullProgress>?> ProgressProperty =
        AvaloniaProperty.Register<DockerPullControl, IProgress<DockerPullProgress>?>(
            nameof(Progress));

    public static readonly StyledProperty<string?> StatusProperty =
        AvaloniaProperty.Register<DockerPullControl, string?>(
            nameof(Status));

    public string? ImageName
    {
        get => GetValue(ImageNameProperty);
        set => SetValue(ImageNameProperty, value);
    }

    public bool IsPulling
    {
        get => GetValue(IsPullingProperty);
        set => SetValue(IsPullingProperty, value);
    }

    public IProgress<DockerPullProgress>? Progress
    {
        get => GetValue(ProgressProperty);
        set => SetValue(ProgressProperty, value);
    }

    public string? Status
    {
        get => GetValue(StatusProperty);
        set => SetValue(StatusProperty, value);
    }
    
    public double? ProgressValue
    {
        get => GetValue(ProgressValueProperty);
        set => SetValue(ProgressValueProperty, value);
    }

    public DockerPullControl()
    {
        InitializeComponent();
        Progress = new Progress<DockerPullProgress>(progress =>
        {
            if (progress.Percentage.HasValue)
            {
                ProgressValue = progress.Percentage.Value;
            }
            Status = progress.Status;
        });
    }

    private async void PullOnClick(object? sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(ImageName))
        {
            Status = "Enter a Docker image name.";
            return;
        }

        IsPulling = true;
        Status = $"Pulling {ImageName}...";

        try
        {
            // Docker pull implementation goes here.
            await PullImageAsync(ImageName);
            
            Status = $"Successfully pulled {ImageName}.";
        }
        catch (Exception ex)
        {
            Status = $"Failed to pull {ImageName}: {ex.Message}";
        }
        finally
        {
            IsPulling = false;
        }
    }

    private async Task PullImageAsync(string imageName)
    {
       if (OnPullClicked != null)
       {
           await OnPullClicked.Invoke(imageName);
       }
    }
}