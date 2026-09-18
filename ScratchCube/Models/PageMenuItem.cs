using Avalonia.Themes.Fluent;
using FluentIcons.Common;

namespace ScratchCube.Models;

public class PageMenuItem
{
    public string Name { get; set; }
    public Icon Icon { get; set; }
    public IPageView PageView { get; set; }

    public PageMenuItem(string name, Icon icon, IPageView pageView)
    {
        Name = name;
        Icon = icon;
        PageView = pageView;
    }
}