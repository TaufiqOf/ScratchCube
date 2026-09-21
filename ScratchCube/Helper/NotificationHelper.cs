using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using ScratchCube.Views.Windows;

namespace ScratchCube.Helper;

public static class NotificationHelper
{
    private static Window _window;
    private static WindowNotificationManager? _manager;

    public static void Initialize(Window window)
    {
        _window = window;
        _manager = new WindowNotificationManager(window)
        {
            Position = NotificationPosition.BottomRight,
            MaxItems = 3
        };
    }

    public static void ShowNotification(
        string title,
        string message,
        NotificationType type = NotificationType.Information,
        TimeSpan? expiration = null)
    {
        if (_manager == null)
            return;

        _manager.Show(new Notification(
            title,
            message,
            type,
            expiration ?? TimeSpan.FromSeconds(3)));
    }

    public static void Success(string title, string message)
    {
        ShowNotification(
            title,
            message,
            NotificationType.Success);
    }

    public static void Info(string title, string message)
    {
        ShowNotification(
            title,
            message,
            NotificationType.Information);
    }

    public static void Warning(string title, string message)
    {
        ShowNotification(
            title,
            message,
            NotificationType.Warning);
    }

    public static void Error(string title, string message)
    {
        ShowNotification(
            title,
            message,
            NotificationType.Error,
            TimeSpan.FromSeconds(5));
    }
    
    public static Task ShowMessageBoxAsync(string title, string message)
    {
        var messageDialogControl = new Views.Controls.DialogControl.MessageDialogControl
        {
            Message = message
        };
        var dialog = new MessageBoxWindow(title, messageDialogControl);
        return dialog.ShowDialog(_window);
    }
}