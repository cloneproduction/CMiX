using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Texturing.Filters;
using CMiX.Core.Texturing.Sources;
using CMiX.Studio.Avalonia.Views.Controls;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using AvaloniaComboBox = global::Avalonia.Controls.ComboBox;

namespace CMiX.Studio.Avalonia.Tests
{
    // Drives the real Outliner inside the real Displace view and picks an item from its dropdown.
    // The Outliner once cast the ComboBox to the ComboBox class of its own namespace, got null, and
    // ignored every dropdown pick.
    public class OutlinerSelectionTests
    {
        private static (AvaloniaComboBox ComboBox, PrefabManager Selector) ShowDisplaceOutliner()
        {
            var provider = TestServiceProviderFactory.Create();
            var displace = (Displace)provider.GetRequiredService<ControlFactory>().Create(typeof(Displace));

            var view = new Views.Displace { DataContext = displace };
            var window = new Window { Content = view, Width = 500, Height = 600 };
            window.Show();
            Dispatcher.UIThread.RunJobs();

            var outliner = view.GetVisualDescendants().OfType<Outliner>().Single();
            var comboBox = outliner.GetVisualDescendants().OfType<AvaloniaComboBox>().First();
            return (comboBox, displace.TextureSelector);
        }

        // The steps of the user report: create an item, clear the selection with the minus button,
        // create a second item, then pick the first one again from the dropdown.
        [AvaloniaFact]
        public void PickingAClearedItemFromTheDropdownSelectsItAgain()
        {
            var (comboBox, selector) = ShowDisplaceOutliner();

            selector.AddItemCommand.Execute(typeof(BubbleNoise));
            var bubbleNoise = selector.SelectedItem;
            selector.RemoveSelectedItemCommand.Execute(null);
            selector.AddItemCommand.Execute(typeof(CheckerBoard));
            TestServiceProviderFactory.Pump();

            Assert.IsType<BubbleNoise>(bubbleNoise);
            Assert.IsType<CheckerBoard>(selector.SelectedItem);

            comboBox.SelectedItem = bubbleNoise;
            TestServiceProviderFactory.Pump();

            Assert.Same(bubbleNoise, selector.SelectedItem);
            Assert.Same(bubbleNoise, comboBox.SelectedItem);
        }

        // Two selections happen before the posted assignments run. The Outliner must ignore the
        // ComboBox events that only echo the view model, or the stale posts flip the selection
        // back and forth without end and this test never finishes.
        [AvaloniaFact]
        public void PickingAnItemFromTheDropdownAfterTwoQuickAddsSelectsItWithoutLooping()
        {
            var (comboBox, selector) = ShowDisplaceOutliner();

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
