using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core;
using CMiX.Core.Colors.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Texturing.Sources;
using CMiX.Studio.Avalonia.Views.Managers;
using CMiX.UI.Controls;
using CMiX.UI.Controls.Collection;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // Reproduces the regression fixed in commits f0406d5b and 82b09f3c: CheckerBoard, TouchBlob
    // and ColorPaletteModifier had no DataTemplate in the ListBoxCMiX.DataTemplates collections that
    // RepositoryManager and PrefabSlotManager inline, so a list row for one of these types fell
    // through to the Application level reflection view locator (Views.ViewModelToViewTemplate),
    // which resolves by exact type name to the full settings editing view (for example
    // Views.CheckerBoard, the heavy editor panel) instead of the small ListItemIcon row used
    // by every other prefab type.
    public class ListTemplateRegressionTests
    {
        private static RepositoryManager CreateShownRepositoryManager()
        {
            var repositoryManager = new RepositoryManager();
            var window = new Window { Content = repositoryManager, Width = 400, Height = 600 };
            window.Show();
            Dispatcher.UIThread.RunJobs();
            return repositoryManager;
        }

        [AvaloniaFact]
        public void CheckerBoard_TouchBlob_ColorPaletteModifier_RealizeAsListItemIcon()
        {
            var provider = TestServiceProviderFactory.Create();
            var controlFactory = provider.GetRequiredService<ControlFactory>();

            var checkerBoard = controlFactory.Create(typeof(CheckerBoard));
            var touchBlob = controlFactory.Create(typeof(TouchBlob));
            var colorPalette = controlFactory.Create(typeof(ColorPaletteModifier));

            var repositoryManager = CreateShownRepositoryManager();
            var listBox = repositoryManager.GetVisualDescendants().OfType<ListBoxCMiX>().Single();

            var items = new List<IControl> { checkerBoard, touchBlob, colorPalette };
            listBox.ItemsSource = items;
            TestServiceProviderFactory.Pump();

            foreach (var item in items)
            {
                var container = listBox.ContainerFromItem(item);
                Assert.True(container != null, $"{item.GetType().Name} did not realize a list container.");

                var realizedAsListItem = container.GetVisualDescendants().OfType<ListItemIcon>().Any();
                Assert.True(
                    realizedAsListItem,
                    $"{item.GetType().Name} did not realize as a ListItemIcon; it likely fell through " +
                    "to the reflection view locator and rendered its full editing view instead.");
            }
        }

        [AvaloniaFact]
        public void ARow_ShowsThePrefabName_AndItsToggleWritesTheVisibility()
        {
            var checkerBoard = (CheckerBoard)TestServiceProviderFactory.Create().GetRequiredService<ControlFactory>().Create(typeof(CheckerBoard));
            var listBox = CreateShownRepositoryManager().GetVisualDescendants().OfType<ListBoxCMiX>().Single();
            listBox.ItemsSource = new List<IControl> { checkerBoard };
            TestServiceProviderFactory.Pump();

            var row = listBox.ContainerFromItem(checkerBoard)!.GetVisualDescendants().OfType<ListItemIcon>().Single();
            Assert.Equal(checkerBoard.PrefabService.Name.Value, row.Title);

            var before = checkerBoard.PrefabService.Visibility.Value;
            row.IsChecked = !before;
            Dispatcher.UIThread.RunJobs();

            Assert.Equal(!before, checkerBoard.PrefabService.Visibility.Value);
        }
    }
}
