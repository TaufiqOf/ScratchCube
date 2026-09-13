using Avalonia.Controls;
using ScratchDocker.Models;
using ScratchDocker.ViewModels.Pages;

namespace ScratchDocker.Views.Pages;

public partial class SettingPageView : UserControl, IPageView
{
    private readonly IPageViewModel _viewModel;

    public SettingPageView(IPageViewModel viewModel)
    {
        _viewModel = viewModel;
        InitializeComponent();
    }

    public IPageViewModel ViewModel => _viewModel;
}