using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using ScratchCube.ViewModels.Windows;
using ScratchCube.Views.Windows;

namespace ScratchCube;

public partial class App : Application
{
    private MainWindow? _mainWindow;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            _mainWindow = new MainWindow
            {
                DataContext = new MainViewModel()
            };

            desktop.MainWindow = _mainWindow;

            // Keep the process alive when the window is closed.
            desktop.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            _mainWindow.Closing += MainWindow_OnClosing;
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void MainWindow_OnClosing(
        object? sender,
        WindowClosingEventArgs e)
    {
        // Don't actually close the application.
        e.Cancel = true;

        // Move the application to the tray.
        _mainWindow?.Hide();
    }

    private void ShowWindow_OnClick(
        object? sender,
        EventArgs e)
    {
        ShowMainWindow();
    }

    private void Exit_OnClick(
        object? sender,
        EventArgs e)
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.Shutdown();
        }
    }

    private void ShowMainWindow()
    {
        if (_mainWindow == null)
            return;

        _mainWindow.Show();
        _mainWindow.WindowState = WindowState.Normal;
        _mainWindow.Activate();
    }
}