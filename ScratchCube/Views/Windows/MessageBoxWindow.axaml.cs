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

    public MessageBoxWindow(string title, string message) : this()
    {
        Title = title;
        MessageText.Text = message;
    }

    private void OnOkClicked(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    public static async Task ShowAsync(Window parentWindow, string title, string message)
    {
        var dialog = new MessageBoxWindow(title, message);
        await dialog.ShowDialog(parentWindow);
    }
}