using Avalonia.Controls;
using ScratchCube.Models;
using ScratchCube.ViewModels.Pages;

namespace ScratchCube.Views.Pages;

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