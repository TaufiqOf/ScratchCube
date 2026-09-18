using Avalonia;
using Avalonia.Controls;
using ScratchCube.Models;
using ScratchCube.ViewModels.Pages;

namespace ScratchCube.Views.Pages;

public partial class ContainerPageView : UserControl, IPageView
{
    private readonly IPageViewModel _viewModel;

    public ContainerPageView(IPageViewModel viewModel)
    {
        DataContext = (ContainerPageViewModel)viewModel;
        _viewModel = viewModel;
        InitializeComponent();
        LogTextBox.PropertyChanged += LogTextBoxOnPropertyChanged;
    }

    public IPageViewModel ViewModel => _viewModel;
    private void LogTextBoxOnPropertyChanged(
        object? sender,
        AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == TextBox.TextProperty)
        {
            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            {
                LogTextBox.CaretIndex = LogTextBox.Text?.Length ?? 0;
            });
        }
    }
}