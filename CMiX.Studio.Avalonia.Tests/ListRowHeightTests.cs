using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core.Colors.Modifiers;
using CMiX.Core.Compositing;
using CMiX.Core.Prefabs;
using CMiX.UI.Controls.ColorPicker;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // Every control is as high as a row, so a row fits its content and the rows are apart by the token ListSpacing.
    public class ListRowHeightTests
    {
        private static double Token(string key) => (double)Application.Current!.FindResource(key)!;

        [AvaloniaFact]
        public void AColorRow_IsAsHighAsTheRow_AndTheRowsAreApartBySpacing()
        {
            var factory = TestServiceProviderFactory.Create().GetRequiredService<ControlFactory>();
            var palette = (ColorPaletteModifier)factory.Create(typeof(ColorPaletteModifier));
            palette.ColorManager.AddItem(typeof(Color));
            palette.ColorManager.AddItem(typeof(Color));
            var view = new Views.ColorPaletteModifier { DataContext = palette };
            new Window { Content = view, Width = 600, Height = 900 }.Show();
            Dispatcher.UIThread.RunJobs();

            var rows = view.GetVisualDescendants().OfType<ListBoxItem>().ToList();
            var swatch = rows[0].GetVisualDescendants().OfType<ColorSelector>().Single();

            Assert.Equal(Token("ControlHeightMD"), rows[0].Bounds.Height);
            Assert.Equal(rows[0].Bounds.Height, swatch.Bounds.Height);
            Assert.Equal(Token("ListSpacing"), rows[1].Bounds.Y - rows[0].Bounds.Bottom);
        }
    }
}
