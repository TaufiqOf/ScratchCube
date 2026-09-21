using Avalonia;
using Avalonia.Controls;
using Avalonia.Input.Platform;
using Avalonia.Interactivity;

namespace ScratchCube.Views.Controls.DialogControl;

public partial class MessageDialogControl : UserControl
{
    public static readonly StyledProperty<string?> MessageProperty =
        AvaloniaProperty.Register<MessageDialogControl, string?>(nameof(Message));

    public string? Message
    {
        get => GetValue(MessageProperty);
        set
        {
            SetValue(MessageProperty, value);
        }
    }

    public MessageDialogControl()
    {
        InitializeComponent();
    }

    private async void CopyOnClick(object? sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(Message))
            return;

        var topLevel = TopLevel.GetTopLevel(this);

        if (topLevel?.Clipboard is not null)
        {
            await topLevel.Clipboard.SetTextAsync(Message);
        }
        
    }
}