using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Studio.Avalonia.Views;
using CMiX.Studio.Avalonia.Views.Managers;
using CMiX.UI.Controls;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // The list of a PrefabSlotManager shows the number of rows that the application asks for, and the
    // token ListVisibleItems when it asks for none.
    public class PrefabSlotManagerMinItemCountTests
    {
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

        private static ListBoxCMiX Show(PrefabSlotManager manager)
        {
            new Window { Content = manager, Width = 400, Height = 600 }.Show();
            Dispatcher.UIThread.RunJobs();
            return manager.GetVisualDescendants().OfType<ListBoxCMiX>().Single();
        }

        [AvaloniaFact]
        public void WithoutAnyCount_TheListTakesTheToken()
        {
            var list = Show(new PrefabSlotManager());

            Assert.Equal(Token<int>("ListVisibleItems"), list.MinItemCount);
            Assert.Equal(HeightOfRows(Token<int>("ListVisibleItems")), list.MinHeight);
        }

        [AvaloniaFact]
        public void WithACount_TheListShowsThatNumberOfRows()
        {
            var list = Show(new PrefabSlotManager { MinItemCount = 2 });

            Assert.Equal(2, list.MinItemCount);
            Assert.Equal(HeightOfRows(2), list.MinHeight);
        }

        [AvaloniaFact]
        public void TheTextureList_ShowsTwoRows()
        {
            var texture = new DiffuseTexture();
            new Window { Content = texture, Width = 400, Height = 800 }.Show();
            Dispatcher.UIThread.RunJobs();
            var list = texture.GetVisualDescendants().OfType<ListBoxCMiX>().First();

            Assert.Equal(2, list.MinItemCount);
            Assert.Equal(HeightOfRows(2), list.MinHeight);
        }

        [AvaloniaFact]
        public void InsideTheMask_TheTextureListIsOneMarginFromTheEdge()
        {
            var mask = new MaskTexture();
            new Window { Content = mask, Width = 400, Height = 800 }.Show();
            Dispatcher.UIThread.RunJobs();

            var manager = mask.GetVisualDescendants().OfType<PrefabSlotManager>().First();
            var left = manager.TranslatePoint(new Point(0, 0), mask)!.Value.X;

            Assert.Equal(Token<double>("SpacingMD"), left);
        }
    }
}
