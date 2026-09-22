using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using ScratchCube.Helper;
using ScratchCube.ViewModels.Windows;
using ScratchCube.Views.Windows;

namespace ScratchCube;

public partial class App : Application
{
    private MainWindow? _mainWindow;
    private TrayIcon _trayIcon;

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
            var trayIcons = TrayIcon.GetIcons(this);
            if (trayIcons != null && trayIcons.Count > 0)
            {
                _trayIcon = trayIcons[0];
                _trayIcon.ToolTipText = "ScratchCube";
                RebuildTrayMenu();
            }
            desktop.MainWindow = _mainWindow;

            // Keep the process alive when the window is closed.
            desktop.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            _mainWindow.Closing += MainWindow_OnClosing;
            AutoStartManager.SetEnabled(true);
        }

        base.OnFrameworkInitializationCompleted();
    }
    private async void ShowOnStartUp(object? sender, EventArgs e)
    {
        bool currentlyEnabled = AutoStartManager.IsEnabled();
        AutoStartManager.SetEnabled(!currentlyEnabled);
        RebuildTrayMenu();
    }
    private void RebuildTrayMenu()
    {
        var rootMenu = new NativeMenu();
        
        var show = new NativeMenuItem("Show");
        show.Click += ShowWindow_OnClick;
        rootMenu.Items.Add(show);
        
        var statOnStartup = new NativeMenuItem(AutoStartManager.IsEnabled() ? "Start on Startup" : "Don't Start on Startup");
        statOnStartup.Click += ShowOnStartUp;
        rootMenu.Items.Add(statOnStartup);
        
        rootMenu.Items.Add(new NativeMenuItemSeparator());
        var exitItem = new NativeMenuItem("Exit");
        exitItem.Click += Exit_OnClick;
        rootMenu.Items.Add(exitItem);

        _trayIcon.Menu = rootMenu;
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

    private void TrayIconOnClicked(object? sender, EventArgs e)
    {
        ShowMainWindow();
    }
}