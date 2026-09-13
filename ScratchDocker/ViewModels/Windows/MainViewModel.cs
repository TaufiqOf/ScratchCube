using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using FluentIcons.Common;
using ScratchDocker.Models;
using ScratchDocker.ViewModels.Controls;
using ScratchDocker.ViewModels.Pages;

namespace ScratchDocker.ViewModels.Windows;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    public partial IPageView CurrentPage { get; set; }

    public MenuControlViewModel MenuControlViewModel { get; set; }

    public MainViewModel()
    {
        MenuControlViewModel = new MenuControlViewModel(NavigateToPage, new List<PageMenuItem>
        {
            new("Main", Icon.Home, new Views.Pages.MainPageView(new MainPageViewModel())),
            new("Settings", Icon.Settings, new Views.Pages.SettingPageView(new SettingPageViewModel())),
        });
        MenuControlViewModel.SelectedMenuItem = MenuControlViewModel.MenuItems[0];
    }

    private void NavigateToPage(PageMenuItem pageMenuItem)
    {
        CurrentPage = pageMenuItem.PageView;
    }
}