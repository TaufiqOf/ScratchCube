using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace ScratchCube.Helper;

public static class AutoStartManager
{
    private const string AppName = "ScratchCube";

    private static readonly string? FlatpakId =
        Environment.GetEnvironmentVariable("FLATPAK_ID");

    public static bool IsFlatpak =>
        !string.IsNullOrWhiteSpace(FlatpakId);

    // ------------------------------------------------------------
    // Linux
    // ------------------------------------------------------------

    private static string LinuxAutostartDir =>
        Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            ".config",
            "autostart");

    private static string LinuxDesktopFilePath =>
        Path.Combine(
            LinuxAutostartDir,
            "scratchcube.desktop");

    // ------------------------------------------------------------
    // macOS
    // ------------------------------------------------------------

    private static string MacLaunchAgentsDir =>
        Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            "Library",
            "LaunchAgents");

    private static string MacPlistFilePath =>
        Path.Combine(
            MacLaunchAgentsDir,
            "com.scratchcube.autostart.plist");

    // ------------------------------------------------------------
    // Public API
    // ------------------------------------------------------------

    public static bool IsEnabled()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return File.Exists(LinuxDesktopFilePath);
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            using var key = Registry.CurrentUser.OpenSubKey(
                @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run",
                writable: false);

            return key?.GetValue(AppName) != null;
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return File.Exists(MacPlistFilePath);
        }

        return false;
    }

    public static void SetEnabled(bool enable)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            SetLinuxEnabled(enable);
            return;
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            SetWindowsEnabled(enable);
            return;
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            SetMacEnabled(enable);
            return;
        }
    }

    // ------------------------------------------------------------
    // Linux
    // ------------------------------------------------------------

    private static void SetLinuxEnabled(bool enable)
    {
        var desktopFile = LinuxDesktopFilePath;

        if (!enable)
        {
            if (File.Exists(desktopFile))
            {
                File.Delete(desktopFile);
            }

            return;
        }

        if (!TryGetLinuxExecCommand(out var execCommand))
        {
            return;
        }

        Directory.CreateDirectory(LinuxAutostartDir);

        var content = $"""
            [Desktop Entry]
            Type=Application
            Version=1.0
            Name={AppName}
            Comment=ScratchCube
            Exec={execCommand}
            Terminal=false
            StartupNotify=false
            X-GNOME-Autostart-enabled=true
            X-KDE-autostart-enabled=true
            """

        ;

        File.WriteAllText(desktopFile, content);
    }

    private static bool TryGetLinuxExecCommand(
        out string execCommand)
    {
        // --------------------------------------------------------
        // Flatpak
        // --------------------------------------------------------

        if (IsFlatpak &&
            !string.IsNullOrWhiteSpace(FlatpakId))
        {
            execCommand =
                $"flatpak run {FlatpakId} --autostart";

            return true;
        }

        // --------------------------------------------------------
        // AppImage
        // --------------------------------------------------------

        var appImagePath =
            Environment.GetEnvironmentVariable("APPIMAGE");

        if (!string.IsNullOrWhiteSpace(appImagePath) &&
            File.Exists(appImagePath))
        {
            execCommand =
                $"\"{EscapeDesktopExecArg(appImagePath)}\" --autostart";

            return true;
        }

        // --------------------------------------------------------
        // Normal executable
        // --------------------------------------------------------

        var exePath = Environment.ProcessPath;

        if (!string.IsNullOrWhiteSpace(exePath) &&
            File.Exists(exePath))
        {
            execCommand =
                $"\"{EscapeDesktopExecArg(exePath)}\" --autostart";

            return true;
        }

        execCommand = string.Empty;
        return false;
    }

    private static string EscapeDesktopExecArg(string value)
    {
        return value
            .Replace("\\", "\\\\")
            .Replace("\"", "\\\"");
    }

    // ------------------------------------------------------------
    // Windows
    // ------------------------------------------------------------

    private static void SetWindowsEnabled(bool enable)
    {
        var exePath = Environment.ProcessPath;

        if (string.IsNullOrWhiteSpace(exePath))
        {
            return;
        }

        using var key = Registry.CurrentUser.OpenSubKey(
            @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run",
            writable: true);

        if (key == null)
        {
            return;
        }

        if (enable)
        {
            key.SetValue(
                AppName,
                $"\"{exePath}\" --autostart");
        }
        else
        {
            key.DeleteValue(
                AppName,
                throwOnMissingValue: false);
        }
    }

    // ------------------------------------------------------------
    // macOS
    // ------------------------------------------------------------

    private static void SetMacEnabled(bool enable)
    {
        if (!enable)
        {
            if (File.Exists(MacPlistFilePath))
            {
                File.Delete(MacPlistFilePath);
            }

            return;
        }

        var exePath = Environment.ProcessPath;

        if (string.IsNullOrWhiteSpace(exePath))
        {
            return;
        }

        Directory.CreateDirectory(MacLaunchAgentsDir);

        var plistContent = $"""
            <?xml version="1.0" encoding="UTF-8"?>
            <!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN"
                "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
            <plist version="1.0">
            <dict>
                <key>Label</key>
                <string>com.scratchcube.autostart</string>

                <key>ProgramArguments</key>
                <array>
                    <string>{EscapeXml(exePath)}</string>
                    <string>--autostart</string>
                </array>

                <key>RunAtLoad</key>
                <true/>
            </dict>
            </plist>
            """;

        File.WriteAllText(
            MacPlistFilePath,
            plistContent);
    }

    private static string EscapeXml(string value)
    {
        return value
            .Replace("&", "&amp;")
            .Replace("<", "&lt;")
            .Replace(">", "&gt;")
            .Replace("\"", "&quot;")
            .Replace("'", "&apos;");
    }
}