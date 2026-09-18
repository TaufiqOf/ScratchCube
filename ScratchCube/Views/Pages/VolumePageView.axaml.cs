using Avalonia.Controls;
using ScratchCube.Models;
using ScratchCube.ViewModels.Pages;

namespace ScratchCube.Views.Pages;

public partial class VolumePageView : UserControl, IPageView
{
    private readonly IPageViewModel _viewModel;

    public VolumePageView(IPageViewModel viewModel)
    {
        DataContext = (VolumePageViewModel)viewModel;
        _viewModel = viewModel;
        InitializeComponent();
    }

    public IPageViewModel ViewModel => _viewModel;
}