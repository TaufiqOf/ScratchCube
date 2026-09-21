using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ScratchCube.Helper;
using ScratchCube.Models;
using ScratchCube.Models.Docker;
using ScratchCube.Service;
using ScratchCube.ViewModels.Controls;
using ScratchCube.Views.Controls.DialogControl;

namespace ScratchCube.ViewModels.Pages;

public partial class ContainerPageViewModel : ViewModelBase, IPageViewModel
{
    [ObservableProperty] private string _searchText = string.Empty;

    [ObservableProperty]
    public partial ObservableCollection<DockerContainer> Containers { get; private set; }
        = new();

    [ObservableProperty]
    public partial ObservableCollection<DockerContainer> FilteredContainers { get; private set; }
        = new();

    [ObservableProperty]
    private ModelViewerControlViewModel<DockerContainerInspect>? _inspectModelViewerControlViewModel;

    [ObservableProperty]
    private ModelViewerControlViewModel<DockerContainerStatsResponse>? _statsModelViewerControlViewModel;

    [ObservableProperty] private string _selectedContainerLogs;
    [ObservableProperty] private bool _isConnected;

    public int SelectedTabIndex
    {
        get;
        set
        {
            if (Equals(value, field)) return;
            field = value;
            OnPropertyChanged();
            _ = StartContainerMonitor();
        }
    } = 0;


    public DockerContainer? SelectedContainer
    {
        get;
        set
        {
            if (Equals(value, field)) return;
            field = value;
            OnPropertyChanged();
            _ = StartContainerMonitor();
        }
    }

    private readonly DockerService _dockerService = DockerService.Instance;


    public ContainerPageViewModel()
    {
        Containers = new ObservableCollection<DockerContainer>();
        _inspectModelViewerControlViewModel = new ModelViewerControlViewModel<DockerContainerInspect>(null);
        _statsModelViewerControlViewModel = new ModelViewerControlViewModel<DockerContainerStatsResponse>(null);
        DockerService.Instance.OnConnectionStatusChanged += OnConnectionStatusChanged;

    }

    public async Task LoadData()
    {
        DockerService.Instance.OnContainerAdded = container =>
        {
            Containers.Add(container);
            ApplySearch();
        };
        DockerService.Instance.OnContainerRemoved = container =>
        {
            Containers.Remove(container);
            ApplySearch();
        };
        OnConnectionStatusChanged(DockerService.Instance.IsConnected);
    }

    private async void OnConnectionStatusChanged(bool status)
    {
        {
            if (IsConnected != status)
            {
                if (!status)
                {
                    Containers.Clear();
                    FilteredContainers.Clear();
                }
                else
                {
                    (await _dockerService.GetContainers()).ToList().ForEach(Containers.Add);
                }
                ApplySearch();
            }
            IsConnected = status;
        };
    }

    partial void OnSearchTextChanged(string value)
    {
        ApplySearch();
    }

    private void ApplySearch()
    {
        var search = SearchText.Trim();

        if (string.IsNullOrWhiteSpace(search))
        {
            FilteredContainers = new ObservableCollection<DockerContainer>(
                Containers);

            return;
        }

        var results = Containers.Where(container =>
            container.Id.Contains(search, StringComparison.OrdinalIgnoreCase)
            || container.DisplayName.Contains(search, StringComparison.OrdinalIgnoreCase)
            || container.Image.Contains(search, StringComparison.OrdinalIgnoreCase)
            || container.Status.Contains(search, StringComparison.OrdinalIgnoreCase)
            || container.State.Contains(search, StringComparison.OrdinalIgnoreCase)
            || container.Command.Contains(search, StringComparison.OrdinalIgnoreCase));

        FilteredContainers = new ObservableCollection<DockerContainer>(
            results);
    }
    
    [RelayCommand]
    private async Task OnRunContainer()
    {
        var control = new RunContainerControl();

        control.OnRunClicked += async (image, name, command, ports) =>
        {
            await DockerService.Instance.RunContainer(
                image,
                name,
                command,
                ports);

        };
        await NotificationHelper.ShowDialogBoxAsync("Run Container", control);
    }

    [RelayCommand]
    private async Task Refresh()
    {
        await DockerService.Instance.RefreshContainers();
        Containers = DockerService.Instance.DockerContainers;
        ApplySearch();
    }

    CancellationTokenSource? _cancellationTokenSource;

    private async Task StartContainerMonitor()
    {
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource = new CancellationTokenSource();
        if(SelectedTabIndex == 0 && SelectedContainer != null)
        {
            await OnContainerSelectionChanged();
        }
        else if (SelectedTabIndex == 1 && SelectedContainer != null)
        {
            await OnStartStats(_cancellationTokenSource.Token);
        }

        else if (SelectedTabIndex == 2 && SelectedContainer != null)
        {
            await OnLogsStats(_cancellationTokenSource.Token);
        }
    }


    private async Task OnContainerSelectionChanged()
    {
        if (SelectedContainer == null)
        {
            return;
        }
        var inspect = await DockerService.Instance.InspectContainer(SelectedContainer);
        InspectModelViewerControlViewModel?.SetInspect(inspect);
    }

    private async Task OnStartStats(CancellationToken token)
    {

        if (SelectedContainer == null)
        {
            return;
        }

        await DockerService.Instance.StatsContainer(SelectedContainer,
            new Progress<DockerContainerStatsResponse>(stats =>
            {
                Dispatcher.UIThread.Post(() => { StatsModelViewerControlViewModel?.SetInspect(stats); },
                    DispatcherPriority.Background);
            }), token);
    }

    private async Task OnLogsStats(CancellationToken token)
    {
        SelectedContainerLogs = string.Empty;

        if (SelectedContainer == null)
            return;

        await DockerService.Instance.LogsContainer(
            SelectedContainer,
            new Progress<string>(log =>
            {
                var cleaned = LogCleaner.Clean(log);

                Dispatcher.UIThread.Post(() =>
                {
                    SelectedContainerLogs += cleaned;

                    if (!cleaned.EndsWith('\n'))
                        SelectedContainerLogs += Environment.NewLine;

                }, DispatcherPriority.Background);
            }),
            token);
    }
    public static class LogCleaner
    {
        private static readonly Regex AnsiRegex =
            new(@"\x1B(?:[@-_]|\[[0-?]*[ -/]*[@-~])",
                RegexOptions.Compiled);

        private static readonly Regex ControlRegex =
            new(@"[\x00-\x08\x0B\x0C\x0E-\x1F\x7F]",
                RegexOptions.Compiled);

        public static string Clean(string? text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            // Remove ANSI terminal sequences
            text = AnsiRegex.Replace(text, string.Empty);

            // Remove other control characters
            text = ControlRegex.Replace(text, string.Empty);

            return text
                .Replace("\r\n", "\n")
                .Replace("\r", "\n");
        }
    }
}

