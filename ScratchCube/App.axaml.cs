using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using ScratchCube.ViewModels;
using ScratchCube.Views;
using MainViewModel = ScratchCube.ViewModels.Windows.MainViewModel;
using MainWindow = ScratchCube.Views.Windows.MainWindow;

namespace ScratchCube;

using MainViewModel = ViewModels.Windows.MainViewModel;
using MainWindow = Views.Windows.MainWindow;

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