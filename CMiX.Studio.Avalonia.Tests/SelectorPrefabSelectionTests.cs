using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Texturing.Filters;
using CMiX.Core.Texturing.Sources;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.Views.Controls;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // Drives the real SelectorPrefab in the real Displace view, picking an item from its dropdown.
    public class SelectorPrefabSelectionTests : IDisposable
    {
        private readonly TestWindows windows = new();

        public void Dispose() => windows.Dispose();

        private (ComboBox ComboBox, PrefabManager Selector) ShowDisplaceSelectorPrefab()
        {
            var provider = TestServiceProviderFactory.Create();
            var displace = (Displace)provider.GetRequiredService<ControlFactory>().Create(typeof(Displace));

            var view = new Views.Displace { DataContext = displace };
            var window = windows.Add(new Window { Content = view, Width = 500, Height = 600 });
            window.Show();
            Dispatcher.UIThread.RunJobs();

            var outliner = view.GetVisualDescendants().OfType<SelectorPrefab>().Single();
            var comboBox = outliner.GetVisualDescendants().OfType<ComboBox>().First();
            return (comboBox, displace.TextureSelector);
        }

        // Create an item, clear the selection, create a second item, then pick the first from the dropdown.
        [AvaloniaFact]
        public void Pick_AClearedItemFromTheDropdown_SelectsItAgain()
        {
            var (comboBox, selector) = ShowDisplaceSelectorPrefab();

            selector.AddItemCommand.Execute(typeof(BubbleNoise));
            var bubbleNoise = selector.SelectedItem;
            selector.ClearSelectionCommand.Execute(null);
            selector.AddItemCommand.Execute(typeof(CheckerBoard));
            TestServiceProviderFactory.Pump();

            Assert.IsType<BubbleNoise>(bubbleNoise);
            Assert.IsType<CheckerBoard>(selector.SelectedItem);

            comboBox.SelectedItem = bubbleNoise;
            TestServiceProviderFactory.Pump();

            Assert.Same(bubbleNoise, selector.SelectedItem);
            Assert.Same(bubbleNoise, comboBox.SelectedItem);
        }

        // Two selections before the posts run must not make stale posts flip the selection forever.
        [AvaloniaFact]
        public void Pick_AnItemAfterTwoQuickAdds_SelectsItWithoutLooping()
        {
            var (comboBox, selector) = ShowDisplaceSelectorPrefab();

            selector.AddItemCommand.Execute(typeof(BubbleNoise));
            var bubbleNoise = selector.SelectedItem;
            selector.AddItemCommand.Execute(typeof(CheckerBoard));
            TestServiceProviderFactory.Pump();

            comboBox.SelectedItem = bubbleNoise;
            TestServiceProviderFactory.Pump();

            Assert.Same(bubbleNoise, selector.SelectedItem);
        }
    }
}
