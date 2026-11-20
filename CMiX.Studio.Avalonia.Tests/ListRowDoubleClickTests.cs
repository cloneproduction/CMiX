using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core.Compositing;
using CMiX.Core.Prefabs;
using CMiX.UI.Controls.String;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Layer = CMiX.Core.Compositing.Layer;

namespace CMiX.Studio.Avalonia.Tests
{
    // The row of a list of layers and compositions. A double click on the text of the name renames it.
    // A double click on the rest of the row does nothing.
    public class ListRowDoubleClickTests
    {
        private static (Window Window, TextBoxEditable Name) Show()
        {
            var layer = TestServiceProviderFactory.Create().GetRequiredService<ControlFactory>().Create(typeof(Layer));
            var row = new Views.RowComposable { DataContext = layer };
            var window = new Window { Content = row, Width = 400, Height = 60 };
            window.Show();
            Dispatcher.UIThread.RunJobs();
            return (window, row.GetVisualDescendants().OfType<TextBoxEditable>().First());
        }

        private static bool IsRenaming(TextBoxEditable name) => name.GetVisualDescendants().OfType<TextBox>().Any(input => input.IsVisible);

        private static void DoubleClick(Window window, Point point)
        {
            window.UpdateLayout();
            AvaloniaHeadlessPlatform.ForceRenderTimerTick();
            Dispatcher.UIThread.RunJobs();
            window.MouseDown(point, MouseButton.Left);
            window.MouseUp(point, MouseButton.Left);
            window.MouseDown(point, MouseButton.Left);
            window.MouseUp(point, MouseButton.Left);
            Dispatcher.UIThread.RunJobs();
        }

        [AvaloniaFact]
        public void DoubleClick_OnTheTextOfTheName_StartsTheRename()
        {
            var (window, name) = Show();
            var text = name.GetVisualDescendants().OfType<TextBlock>().First(block => block.Name == "textDisplay");

            DoubleClick(window, text.TranslatePoint(new Point(text.Bounds.Width / 2, text.Bounds.Height / 2), window)!.Value);

            Assert.True(IsRenaming(name));
        }

        [AvaloniaFact]
        public void DoubleClick_AwayFromTheText_DoesNothing()
        {
            var (window, name) = Show();

            DoubleClick(window, name.TranslatePoint(new Point(name.Bounds.Width - 12, name.Bounds.Height / 2), window)!.Value);

            Assert.False(IsRenaming(name));
        }
    }
}
