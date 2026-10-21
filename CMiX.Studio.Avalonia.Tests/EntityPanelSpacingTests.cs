using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core.Compositing;
using CMiX.Core.Prefabs;
using CMiX.UI.Panels;
using CMiX.Studio.Avalonia.Views;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // The expanders of every entity panel are cards one under the other, and the space between two cards
    // is the same in all the panels: the margins of the cards, and nothing more.
    public class EntityPanelSpacingTests
    {
        private static ControlFactory Factory() => TestServiceProviderFactory.Create().GetRequiredService<ControlFactory>();

        // The space between each expander and the next one, for the expanders that are not inside another expander.
        private static List<double> Gaps(Control view)
        {
            new Window { Content = view, Width = 400, Height = 1200 }.Show();
            Dispatcher.UIThread.RunJobs();

            var cards = view.GetVisualDescendants().OfType<ExpanderIndented>()
                .Where(card => !card.GetVisualAncestors().OfType<ExpanderIndented>().Any())
                .Select(card => (top: card.TranslatePoint(new Point(), view)!.Value.Y, bottom: card.TranslatePoint(new Point(0, card.Bounds.Height), view)!.Value.Y))
                .OrderBy(card => card.top)
                .ToList();

            return cards.Zip(cards.Skip(1), (above, below) => below.top - above.bottom).ToList();
        }

        [AvaloniaFact]
        public void TheCardsOfAModelEntity_AreAsFarApartAsTheCardsOfACamera()
        {
            var camera = Gaps(new Views.Camera { DataContext = Factory().Create(typeof(CMiX.Core.Rendering.Cameras.Camera)) });
            var entity = Gaps(new ModelEntity { DataContext = Factory().Create(typeof(Entity)) });

            Assert.NotEmpty(camera);
            Assert.NotEmpty(entity);
            Assert.All(entity, gap => Assert.Equal(camera[0], gap, 0.5));
        }
    }
}
