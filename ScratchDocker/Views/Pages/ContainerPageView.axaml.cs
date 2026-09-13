using Avalonia.Controls;
using ScratchDocker.Models;
using ScratchDocker.ViewModels.Pages;

namespace ScratchDocker.Views.Pages;

public partial class ContainerPageView : UserControl, IPageView
{
    private readonly IPageViewModel _viewModel;

    public ContainerPageView(IPageViewModel viewModel)
    {
        DataContext = (ContainerPageViewModel)viewModel;
        _viewModel = viewModel;
        InitializeComponent();
    }

    public IPageViewModel ViewModel => _viewModel;
}