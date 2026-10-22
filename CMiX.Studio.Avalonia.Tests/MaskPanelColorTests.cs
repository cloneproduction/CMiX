using Avalonia.Controls;
using AvaloniaColor = Avalonia.Media.Color;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core.Compositing;
using CMiX.Core.Prefabs;
using CMiX.UI.Panels;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // The mask panel inside the texture panel has the color of the texture panel, and not the darker color of a deeper level.
    public class MaskPanelColorTests
    {
        private static AvaloniaColor ContentOf(ExpanderIndented expander) =>
            ((ISolidColorBrush)expander.GetVisualDescendants().OfType<Border>().First(border => border.Name == "contentBackground").Background!).Color;

        private static AvaloniaColor HeaderOf(ExpanderIndented expander) =>
            ((ISolidColorBrush)expander.GetVisualDescendants().OfType<Border>().First(border => border.Name == "mainBorder").Background!).Color;

        private static ExpanderIndented Named(Control view, string title) =>
            view.GetVisualDescendants().OfType<ExpanderIndented>().First(expander =>
                Equals(expander.Header, title) || (expander.Header is Panel panel && panel.GetVisualDescendants().OfType<TextBlock>().Any(text => text.Text == title)));

        public static TheoryData<string> Panels => new() { "ModelEntity", "FullscreenQuad" };

        [AvaloniaTheory]
        [MemberData(nameof(Panels))]
        public void TheMaskPanel_HasTheColorOfTheTexturePanel(string kind)
        {
            var factory = TestServiceProviderFactory.Create().GetRequiredService<ControlFactory>();
            Control view = kind == "ModelEntity"
                ? new Views.ModelEntity { DataContext = factory.Create(typeof(Entity)) }
                : new Views.FullscreenQuad { DataContext = factory.Create(typeof(FullscreenQuad)) };
            new Window { Content = new ScrollViewer { Content = view }, Width = 400, Height = 1200 }.Show();
            Dispatcher.UIThread.RunJobs();

            var texture = Named(view, "Texture");
            texture.IsExpanded = true;
            Dispatcher.UIThread.RunJobs();
            var mask = Named(view, "Mask");
            mask.IsExpanded = true;
            Dispatcher.UIThread.RunJobs();

            Assert.Equal(ContentOf(texture), ContentOf(mask));
            Assert.Equal(HeaderOf(texture), HeaderOf(mask));
        }
    }
}
