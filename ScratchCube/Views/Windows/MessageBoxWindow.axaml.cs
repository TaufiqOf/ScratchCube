using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace ScratchCube.Views.Windows;

public partial class MessageBoxWindow : Window
{
    public MessageBoxWindow()
    {
        InitializeComponent();
    }

    public MessageBoxWindow(string title, UserControl content) : this()
    {
        Title = title;
        ContentControl.Content = content;
    }

    private void OnOkClicked(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    public static async Task ShowAsync(Window parentWindow, string title, UserControl content)
    {
        var dialog = new MessageBoxWindow(title, content);
        await dialog.ShowDialog(parentWindow);
    }
}