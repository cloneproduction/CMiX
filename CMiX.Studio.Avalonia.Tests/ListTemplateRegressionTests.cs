using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core;
using CMiX.Core.Colors.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Texturing.Sources;
using CMiX.Studio.Avalonia.Views.Controls;
using CMiX.Studio.Avalonia.Views.Managers;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // Reproduces the regression fixed in commits f0406d5b and 82b09f3c: CheckerBoard, TouchBlob
    // and ColorPalette had no DataTemplate in the CMiXListBox.DataTemplates collections that
    // RepositoryManager and PrefabSlotManager inline, so a list row for one of these types fell
    // through to the Application level reflection view locator (Views.ViewModelToViewTemplate),
    // which resolves by exact type name to the full settings editing view (for example
    // Views.CheckerBoard, the heavy editor panel) instead of the small PrefabListItem row used
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
        public void CheckerBoard_TouchBlob_ColorPalette_RealizeAsPrefabListItem()
        {
            var provider = TestServiceProviderFactory.Create();
            var controlFactory = provider.GetRequiredService<ControlFactory>();

            var checkerBoard = controlFactory.Create(typeof(CheckerBoard));
            var touchBlob = controlFactory.Create(typeof(TouchBlob));
            var colorPalette = controlFactory.Create(typeof(ColorPalette));

            var repositoryManager = CreateShownRepositoryManager();
            var listBox = repositoryManager.GetVisualDescendants().OfType<CMiXListBox>().Single();

            var items = new List<IControl> { checkerBoard, touchBlob, colorPalette };
            listBox.ItemsSource = items;
            TestServiceProviderFactory.Pump();

            foreach (var item in items)
            {
                var container = listBox.ContainerFromItem(item);
                Assert.True(container != null, $"{item.GetType().Name} did not realize a list container.");

                var realizedAsListItem = container.GetVisualDescendants().OfType<PrefabListItem>().Any();
                Assert.True(
                    realizedAsListItem,
                    $"{item.GetType().Name} did not realize as a PrefabListItem; it likely fell through " +
                    "to the reflection view locator and rendered its full editing view instead.");
            }
        }
    }
}
