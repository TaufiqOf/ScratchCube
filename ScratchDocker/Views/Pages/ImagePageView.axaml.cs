using Avalonia.Controls;
using ScratchDocker.Models;
using ScratchDocker.ViewModels.Pages;

namespace ScratchDocker.Views.Pages;

public partial class ImagePageView : UserControl, IPageView
{
    private readonly IPageViewModel _viewModel;

    public ImagePageView(IPageViewModel viewModel)
    {
        DataContext = (ImagePageViewModel)viewModel;
        _viewModel = viewModel;
        InitializeComponent();
    }

    public IPageViewModel ViewModel => _viewModel;
}