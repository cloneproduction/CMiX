using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Studio.Avalonia.Views;
using CMiX.UI.Panels;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // The settings of a texture sit directly in the view. Only the Filter section is an expander,
    // and its background is transparent, so it shows the color of the parent.
    public class TextureViewTests
    {
        private static Texture Show()
        {
            var view = new Texture();
            new Window { Content = view, Width = 400, Height = 600 }.Show();
            Dispatcher.UIThread.RunJobs();
            return view;
        }

        [AvaloniaFact]
        public void TheView_HasOnlyTheFilterExpander()
        {
            var expander = Assert.Single(Show().GetVisualDescendants().OfType<ExpanderIndented>());

            Assert.Equal("Filter", expander.Header);
        }

        [AvaloniaFact]
        public void TheFilterExpander_HasATransparentBackground()
        {
            var expander = Show().GetVisualDescendants().OfType<ExpanderIndented>().Single();

            var brush = Assert.IsAssignableFrom<ISolidColorBrush>(expander.Background);
            Assert.Equal(Colors.Transparent, brush.Color);
        }
    }
}
