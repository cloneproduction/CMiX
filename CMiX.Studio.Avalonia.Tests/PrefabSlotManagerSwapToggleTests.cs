using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless.XUnit;
using Avalonia.LogicalTree;
using Avalonia.Threading;
using CMiX.Core;
using CMiX.Core.Compositing;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Studio.Avalonia.Views.Managers;
using CMiX.UI.Controls.Parts;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // The toggle of the slot that opens the list of the items of the repository.
    public class PrefabSlotManagerSwapToggleTests
    {
        private static (PrefabSlotManager View, ObservableCollection<IControl> Repository, IServiceProvider Provider) Show()
        {
            var provider = TestServiceProviderFactory.Create();
            var manager = provider.GetRequiredService<PrefabManager>();
            var repository = new ObservableCollection<IControl>();
            var view = new PrefabSlotManager { DataContext = manager, ItemsSource = repository };
            new Window { Content = view, Width = 400, Height = 400 }.Show();
            Dispatcher.UIThread.RunJobs();
            return (view, repository, provider);
        }

        private static ToggleButton SwapToggle(PrefabSlotManager view) => view.FindControl<ToggleButton>("SwapToggle")!;

        [AvaloniaFact]
        public void ThePopupOfTheToggle_HasTheSharedDropdownFrame_AndKeepsItsPosition()
        {
            var (view, repository, provider) = Show();
            repository.Add(provider.GetRequiredService<ControlFactory>().Create(typeof(Entity)));
            Dispatcher.UIThread.RunJobs();

            SwapToggle(view).IsChecked = true;
            Dispatcher.UIThread.RunJobs();

            var popup = view.GetLogicalDescendants().OfType<Popup>().Single();
            Assert.IsType<DropdownFrame>(popup.Child);
            Assert.Equal(-8, popup.HorizontalOffset);
            Assert.Equal(-1 - 8, popup.VerticalOffset);
        }

        [AvaloniaFact]
        public void RepositoryWithoutItems_DisablesTheToggle()
        {
            var (view, _, _) = Show();

            Assert.False(SwapToggle(view).IsEnabled);
        }

        [AvaloniaFact]
        public void RepositoryWithAnItem_EnablesTheToggle()
        {
            var (view, repository, provider) = Show();

            repository.Add(provider.GetRequiredService<ControlFactory>().Create(typeof(Entity)));
            Dispatcher.UIThread.RunJobs();

            Assert.True(SwapToggle(view).IsEnabled);
        }

        [AvaloniaFact]
        public void RepositoryEmptiedAgain_DisablesTheToggleAgain()
        {
            var (view, repository, provider) = Show();
            var item = provider.GetRequiredService<ControlFactory>().Create(typeof(Entity));
            repository.Add(item);
            Dispatcher.UIThread.RunJobs();

            repository.Remove(item);
            Dispatcher.UIThread.RunJobs();

            Assert.False(SwapToggle(view).IsEnabled);
        }
    }
}
