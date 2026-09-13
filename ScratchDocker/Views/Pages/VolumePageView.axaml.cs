using Avalonia.Controls;
using ScratchDocker.Models;
using ScratchDocker.ViewModels.Pages;

namespace ScratchDocker.Views.Pages;

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