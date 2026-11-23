using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Layout;
using Avalonia.Themes.Simple;
using TreeDrag.Core;

namespace TreeDrag.Demo;

public static class Program
{
    [STAThread]
    public static void Main(string[] args) =>
        AppBuilder.Configure<App>().UsePlatformDetect().StartWithClassicDesktopLifetime(args);
}

public sealed class App : Application
{
    public override void Initialize() => Styles.Add(new SimpleTheme());

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            desktop.MainWindow = CreateWindow();
        base.OnFrameworkInitializationCompleted();
    }

    private static Window CreateWindow()
    {
        var panel = new TreePanel
        {
            Width = 300,
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
            // Demo rules: no drop into a leaf, and Camera stays at the top level.
            CanDrop = (node, parent) =>
                (parent.Id == "root" || parent.Children.Count > 0) && (node.Id != "Camera" || parent.Id == "root"),
            Root = Node.Branch("root",
                Node.Branch("Layers",
                    Node.Leaf("Background"),
                    Node.Branch("Group", Node.Leaf("Particles"), Node.Leaf("Text"))),
                Node.Branch("Masks", Node.Leaf("Circle"), Node.Leaf("Gradient")),
                Node.Leaf("Camera"),
                Node.Branch("Hidden", Node.Leaf("Inside")) with { IsExpanded = false }),
        };
        return new Window
        {
            Title = "TreeDrag prototype",
            Width = 420,
            Height = 480,
            Content = new StackPanel
            {
                Margin = new Thickness(16),
                Spacing = 12,
                Children =
                {
                    new TextBlock { TextWrapping = Avalonia.Media.TextWrapping.Wrap, Text = "Click an arrow to expand or collapse. Drag a row. Move left or right to change the depth. Esc cancels. Rules: no drop into a leaf, Camera stays at the top level." },
                    panel,
                },
            },
        };
    }
}
