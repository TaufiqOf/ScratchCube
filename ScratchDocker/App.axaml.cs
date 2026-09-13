using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using ScratchDocker.ViewModels;
using ScratchDocker.Views;
using MainViewModel = ScratchDocker.ViewModels.Windows.MainViewModel;
using MainWindow = ScratchDocker.Views.Windows.MainWindow;

namespace ScratchDocker;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainViewModel(),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}