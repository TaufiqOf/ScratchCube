using Avalonia.Controls;
using ScratchDocker.Models;
using ScratchDocker.ViewModels.Pages;

namespace ScratchDocker.Views.Pages;

public partial class MainPageView : UserControl, IPageView
{
    private readonly IPageViewModel _viewModel;

    public MainPageView(IPageViewModel viewModel)
    {
        _viewModel = viewModel;
        InitializeComponent();
    }

    public IPageViewModel ViewModel => _viewModel;
}