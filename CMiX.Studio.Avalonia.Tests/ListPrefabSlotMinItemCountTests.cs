using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.Views;
using CMiX.Studio.Avalonia.Views.Managers;
using CMiX.UI.Controls;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // The list of a ListPrefabSlot shows the number of rows that the application asks for, and the
    // token ListVisibleItems when it asks for none.
    public class ListPrefabSlotMinItemCountTests : IDisposable
    {
        private readonly TestWindows windows = new();

        public void Dispose() => windows.Dispose();

        private static T Token<T>(string key)
        {
            var application = Application.Current!;

            Assert.True(application.TryGetResource(key, application.ActualThemeVariant, out var value), $"The token {key} is missing.");
            return (T)value!;
        }

        private static double HeightOfRows(int rows)
        {
            var margin = Token<Thickness>("ListScrollMargin");
            var spacing = Token<double>("ListSpacing");
            return rows * (Token<double>("ControlHeightMD") + spacing) - spacing + margin.Top + margin.Bottom;
        }

        private ListBoxCMiX Show(ListPrefabSlot manager)
        {
            windows.Add(new Window { Content = manager, Width = 400, Height = 600 }).Show();
            Dispatcher.UIThread.RunJobs();
            return manager.GetVisualDescendants().OfType<ListBoxCMiX>().Single();
        }

        [AvaloniaFact]
        public void Show_AListWithoutACount_TakesTheToken()
        {
            var list = Show(new ListPrefabSlot());

            Assert.Equal(Token<int>("ListVisibleItems"), list.MinItemCount);
            Assert.Equal(HeightOfRows(Token<int>("ListVisibleItems")), list.MinHeight);
        }

        [AvaloniaFact]
        public void Show_AListWithACount_HasThatNumberOfRows()
        {
            var list = Show(new ListPrefabSlot { MinItemCount = 2 });

            Assert.Equal(2, list.MinItemCount);
            Assert.Equal(HeightOfRows(2), list.MinHeight);
        }

        [AvaloniaFact]
        public void Show_TheTextureList_HasTwoRows()
        {
            var texture = new DiffuseTexture();
            windows.Add(new Window { Content = texture, Width = 400, Height = 800 }).Show();
            Dispatcher.UIThread.RunJobs();
            var list = texture.GetVisualDescendants().OfType<ListBoxCMiX>().First();

            Assert.Equal(2, list.MinItemCount);
            Assert.Equal(HeightOfRows(2), list.MinHeight);
        }

        [AvaloniaFact]
        public void Show_TheTextureListInTheMask_IsOneMarginFromTheEdge()
        {
            var mask = new MaskTexture();
            windows.Add(new Window { Content = mask, Width = 400, Height = 800 }).Show();
            Dispatcher.UIThread.RunJobs();

            var manager = mask.GetVisualDescendants().OfType<ListPrefabSlot>().First();
            var left = manager.TranslatePoint(new Point(0, 0), mask)!.Value.X;

            Assert.Equal(Token<double>("SpacingMD"), left);
        }
    }
}
