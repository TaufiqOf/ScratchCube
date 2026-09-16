using Avalonia.Controls;
using Avalonia.Interactivity;

namespace ScratchDocker.Views.Controls;

public partial class MenuControlView : UserControl
{
    public MenuControlView()
    {
        InitializeComponent();
    }
    
    private void MoreButton_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.ContextMenu is { } contextMenu)
        {
            contextMenu.Open(button);
        }
    }
}