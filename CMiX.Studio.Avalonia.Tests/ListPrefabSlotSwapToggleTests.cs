using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.LogicalTree;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core;
using CMiX.Core.Compositing;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Studio.Avalonia.Views.Managers;
using CMiX.UI.Controls.Collection;
using CMiX.UI.Controls.Parts;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // The toggle of the slot that opens the list of the items of the repository.
    public class ListPrefabSlotSwapToggleTests
    {
        private static (ListPrefabSlot View, ObservableCollection<IControl> Repository, IServiceProvider Provider) Show()
        {
            var provider = TestServiceProviderFactory.Create();
            var manager = provider.GetRequiredService<PrefabManager>();
            var repository = new ObservableCollection<IControl>();
            var view = new ListPrefabSlot { DataContext = manager, ItemsSource = repository };
            new Window { Content = view, Width = 400, Height = 400 }.Show();
            Dispatcher.UIThread.RunJobs();
            return (view, repository, provider);
        }

        private static ToggleButton SwapToggle(ListPrefabSlot view) => view.FindControl<ToggleButton>("swapToggle")!;

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
        public void ThePanelOfTheToggle_StartsAtTheLeftEdgeAndTheBottomEdgeOfTheToggle()
        {
            var (view, repository, provider) = Show();
            view.Margin = new Thickness(100, 40, 0, 0);
            repository.Add(provider.GetRequiredService<ControlFactory>().Create(typeof(Entity)));
            Dispatcher.UIThread.RunJobs();
            var toggle = SwapToggle(view);

            toggle.IsChecked = true;
            Dispatcher.UIThread.RunJobs();

            var popup = view.GetLogicalDescendants().OfType<Popup>().Single();
            var panel = popup.Child!.GetVisualDescendants().OfType<Border>().Single(border => border.Name == "PART_Surface");
            Assert.Equal(toggle.PointToScreen(new Point(0, 0)).X, panel.PointToScreen(new Point(0, 0)).X);
            Assert.Equal(toggle.PointToScreen(new Point(0, toggle.Bounds.Height)).Y - 1, panel.PointToScreen(new Point(0, 0)).Y);
        }

        [AvaloniaFact]
        public void ALongRepository_ScrollsInThePopup()
        {
            var (view, repository, provider) = Show();
            var factory = provider.GetRequiredService<ControlFactory>();
            for (var i = 0; i < 40; i++)
                repository.Add(factory.Create(typeof(Entity)));
            Dispatcher.UIThread.RunJobs();

            SwapToggle(view).IsChecked = true;
            Dispatcher.UIThread.RunJobs();

            var popup = view.GetLogicalDescendants().OfType<Popup>().Single();
            var scroll = popup.Child!.GetVisualDescendants().OfType<ScrollViewer>().Single(s => s.Name == "PART_Scroll");
            Assert.True(scroll.Extent.Height > scroll.Viewport.Height);
            Assert.True(scroll.Viewport.Height <= scroll.MaxHeight);
        }

        [AvaloniaFact]
        public void ThePopup_ShowsTheListWithASearchField()
        {
            var (view, repository, provider) = Show();
            repository.Add(provider.GetRequiredService<ControlFactory>().Create(typeof(Entity)));
            Dispatcher.UIThread.RunJobs();

            SwapToggle(view).IsChecked = true;
            Dispatcher.UIThread.RunJobs();

            var popup = view.GetLogicalDescendants().OfType<Popup>().Single();
            var list = popup.Child!.GetVisualDescendants().OfType<ListSearchableItemPicker>().Single();
            Assert.True(list.IsVisible);
            Assert.Single(popup.Child!.GetVisualDescendants().OfType<ListItemPicker>());
        }


        [AvaloniaFact]
        public void InTheOpenPopup_TheArrowKeysMoveTheHighlight()
        {
            var (_, _, _, list) = ShowWithOpenPopup(5);

            TopLevel.GetTopLevel(list)!.KeyPressQwerty(PhysicalKey.ArrowDown, RawInputModifiers.None);

            Assert.Equal(1, list.HighlightedIndex);
        }

        private static (ListPrefabSlot View, PrefabManager Manager, Popup Popup, ListSearchableItemPicker List) ShowWithOpenPopup(int repositoryItems)
        {
            var (view, repository, provider) = Show();
            var factory = provider.GetRequiredService<ControlFactory>();
            for (var i = 0; i < repositoryItems; i++)
                repository.Add(factory.Create(typeof(Entity)));
            Dispatcher.UIThread.RunJobs();

            SwapToggle(view).IsChecked = true;
            Dispatcher.UIThread.RunJobs();

            var popup = view.GetLogicalDescendants().OfType<Popup>().Single();
            var list = popup.Child!.GetVisualDescendants().OfType<ListSearchableItemPicker>().Single();
            return (view, (PrefabManager)view.DataContext!, popup, list);
        }

        [AvaloniaFact]
        public void EnterInTheOpenPopup_AddsTheHighlightedItem_AndClosesThePopup()
        {
            var (view, manager, popup, list) = ShowWithOpenPopup(3);
            Assert.True(popup.IsOpen);
            Assert.Empty(manager.ManagerData.Items);

            TopLevel.GetTopLevel(list)!.KeyPressQwerty(PhysicalKey.ArrowDown, RawInputModifiers.None);
            TopLevel.GetTopLevel(list)!.KeyPressQwerty(PhysicalKey.Enter, RawInputModifiers.None);
            Dispatcher.UIThread.RunJobs();

            Assert.Single(manager.ManagerData.Items);
            Assert.Same(view.ItemsSource.Cast<object>().ElementAt(1), manager.ManagerData.Items[0]);
            Assert.False(popup.IsOpen);
            Assert.False(SwapToggle(view).IsChecked);
        }

        [AvaloniaFact]
        public void AClickOnARowInTheOpenPopup_AddsTheItem_AndClosesThePopup()
        {
            var (view, manager, popup, list) = ShowWithOpenPopup(3);

            var window = (Window)TopLevel.GetTopLevel(list)!;
            var row = list.GetVisualDescendants().OfType<Button>().ElementAt(2);
            var point = row.TranslatePoint(new Point(10, 10), window)!.Value;
            window.MouseMove(point);
            window.MouseDown(point, MouseButton.Left);
            window.MouseUp(point, MouseButton.Left);
            Dispatcher.UIThread.RunJobs();

            Assert.Single(manager.ManagerData.Items);
            Assert.False(popup.IsOpen);
            Assert.False(SwapToggle(view).IsChecked);
        }

        [AvaloniaFact]
        public void ThePopup_OpensAgainAfterAPick_WithoutAFilter()
        {
            var (view, _, popup, list) = ShowWithOpenPopup(3);
            TopLevel.GetTopLevel(list)!.KeyTextInput("e");
            TopLevel.GetTopLevel(list)!.KeyPressQwerty(PhysicalKey.Enter, RawInputModifiers.None);
            Dispatcher.UIThread.RunJobs();

            SwapToggle(view).IsChecked = true;
            Dispatcher.UIThread.RunJobs();

            Assert.True(popup.IsOpen);
            Assert.Equal(string.Empty, list.SearchText);
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
