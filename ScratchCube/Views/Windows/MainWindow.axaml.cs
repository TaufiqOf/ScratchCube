using Avalonia.Controls;
using ScratchCube.Helper;

namespace ScratchCube.Views.Windows;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        NotificationHelper.Initialize(this);
    }

    public void ForceExit()
    {
        Close();
    }
}