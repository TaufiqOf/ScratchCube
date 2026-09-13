using System;
using System.Collections.Generic;
using Avalonia.Controls;
using ScratchDocker.Models;

namespace ScratchDocker.ViewModels.Controls;

public partial class MenuControlViewModel : ViewModelBase
{
    public MenuControlViewModel(Action<PageMenuItem> navigateToPage, List<PageMenuItem> menuItems)
    {
        MenuItems = menuItems;
        NavigateToPage = navigateToPage;
    }

    public List<PageMenuItem> MenuItems
    {
        get;
        set
        {
            if (Equals(value, field)) return;
            field = value;
            OnPropertyChanged();
        }
    }

    public PageMenuItem SelectedMenuItem
    {
        get;
        set
        {
            if (Equals(value, field)) return;
            field = value;
            NavigateToPage?.Invoke(value);
            OnPropertyChanged();
        }
    }

    public Action<PageMenuItem> NavigateToPage { get; set; }
}