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
    private TrayIcon? _trayIcon;

    private bool _isExiting;
    private bool _startedFromAutostart;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Detect:
            //
            // ScratchCube --autostart
            //
            _startedFromAutostart =
                Array.Exists(
                    desktop.Args,
                    arg => string.Equals(
                        arg,
                        "--autostart",
                        StringComparison.OrdinalIgnoreCase));

            // ----------------------------------------------------
            // Main window
            // ----------------------------------------------------

            _mainWindow = new MainWindow
            {
                DataContext = new MainViewModel()
            };

            desktop.MainWindow = _mainWindow;

            // ----------------------------------------------------
            // Shutdown behavior
            // ----------------------------------------------------

            // Closing the window normally should NOT terminate
            // the application because it lives in the tray.
            desktop.ShutdownMode =
                ShutdownMode.OnExplicitShutdown;

            desktop.ShutdownRequested +=
                OnShutdownRequested;

            _mainWindow.Closing +=
                MainWindow_OnClosing;

            // ----------------------------------------------------
            // Tray
            // ----------------------------------------------------

            var trayIcons = TrayIcon.GetIcons(this);

            if (trayIcons != null &&
                trayIcons.Count > 0)
            {
                _trayIcon = trayIcons[0];

                _trayIcon.ToolTipText =
                    "ScratchCube";

                RebuildTrayMenu();
            }

            // ----------------------------------------------------
            // Autostart
            // ----------------------------------------------------

            // IMPORTANT:
            //
            // Do NOT call:
            //
            // AutoStartManager.SetEnabled(true);
            //
            // here.
            //
            // Otherwise disabling startup from the tray will be
            // undone every time ScratchCube starts.
            //

            // ----------------------------------------------------
            // Hide when started automatically
            // ----------------------------------------------------

            if (_startedFromAutostart)
            {
                _mainWindow.Hide();
            }
        }

        base.OnFrameworkInitializationCompleted();
    }

    // ============================================================
    // APPLICATION SHUTDOWN
    // ============================================================

    private void OnShutdownRequested(
        object? sender,
        ShutdownRequestedEventArgs e)
    {
        // Linux Mint / desktop session is shutting down.
        //
        // The normal window Closing handler normally hides the
        // window instead of allowing it to close.
        //
        // Set this flag so Closing allows the application to
        // terminate.

        _isExiting = true;
    }

    // ============================================================
    // WINDOW CLOSE
    // ============================================================

    private void MainWindow_OnClosing(
        object? sender,
        WindowClosingEventArgs e)
    {
        // --------------------------------------------------------
        // Real application exit
        // --------------------------------------------------------

        if (_isExiting)
        {
            // Allow the window/application to close.
            return;
        }

        // --------------------------------------------------------
        // Normal X button
        // --------------------------------------------------------

        e.Cancel = true;

        _mainWindow?.Hide();
    }

    // ============================================================
    // TRAY MENU
    // ============================================================

    private void RebuildTrayMenu()
    {
        if (_trayIcon == null)
        {
            return;
        }

        var rootMenu = new NativeMenu();

        // --------------------------------------------------------
        // Show
        // --------------------------------------------------------

        var showItem =
            new NativeMenuItem("Show");

        showItem.Click +=
            ShowWindow_OnClick;

        rootMenu.Items.Add(showItem);

        // --------------------------------------------------------
        // Start on startup
        // --------------------------------------------------------

        var startupItem =
            new NativeMenuItem(
                AutoStartManager.IsEnabled()
                    ? "Don't Start on Startup"
                    : "Start on Startup");

        startupItem.Click +=
            ToggleStartup_OnClick;

        rootMenu.Items.Add(startupItem);

        // --------------------------------------------------------
        // Separator
        // --------------------------------------------------------

        rootMenu.Items.Add(
            new NativeMenuItemSeparator());

        // --------------------------------------------------------
        // Exit
        // --------------------------------------------------------

        var exitItem =
            new NativeMenuItem("Exit");

        exitItem.Click +=
            Exit_OnClick;

        rootMenu.Items.Add(exitItem);

        _trayIcon.Menu = rootMenu;
    }

    // ============================================================
    // SHOW WINDOW
    // ============================================================

    private void ShowWindow_OnClick(
        object? sender,
        EventArgs e)
    {
        ShowMainWindow();
    }

    private void ShowMainWindow()
    {
        if (_mainWindow == null)
        {
            return;
        }

        _mainWindow.Show();

        _mainWindow.WindowState =
            WindowState.Normal;

        _mainWindow.Activate();
    }

    // ============================================================
    // AUTOSTART TOGGLE
    // ============================================================

    private void ToggleStartup_OnClick(
        object? sender,
        EventArgs e)
    {
        var currentlyEnabled =
            AutoStartManager.IsEnabled();

        AutoStartManager.SetEnabled(
            !currentlyEnabled);

        RebuildTrayMenu();
    }

    // ============================================================
    // EXIT
    // ============================================================

    private void Exit_OnClick(
        object? sender,
        EventArgs e)
    {
        _isExiting = true;

        if (ApplicationLifetime
            is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.Shutdown();
        }
    }
    
    private void TrayIconOnClicked(
        object? sender,
        EventArgs e)
    {
        ShowMainWindow();
    }
}