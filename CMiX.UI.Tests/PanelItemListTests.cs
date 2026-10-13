using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.UI.Behaviors;
using CMiX.UI.Controls.Collection;
using Xunit;

namespace CMiX.UI.Tests
{
    // PanelItemList shows its items with the template of the application, and an add button under them.
    public class PanelItemListTests
    {
        private sealed class Model
        {
            public string Name => "model name";
        }

        private sealed class FakeReorder : IReorderService
        {
            public bool DragHandleIsPressed { get; set; }
            public bool CanDrop(int sourceIndex, int insertIndex) => true;
            public void Dropped(int sourceIndex, int insertIndex) { }
        }

        private static (Window window, PanelItemList list) Show(PanelItemList list)
        {
            var window = new Window { Content = list, Width = 400, Height = 500 };
            window.Show();
            Dispatcher.UIThread.RunJobs();
            return (window, list);
        }

        private static PanelItemList NewList(IEnumerable<string>? items = null)
        {
            return new PanelItemList
            {
                ItemsSource = items ?? new[] { "one", "two", "three" },
                ItemTemplate = new FuncDataTemplate<string>((text, _) => new PanelItem { Title = text }),
                AddCaption = "Add"
            };
        }

        [AvaloniaFact]
        public void ShowsOneItemForEachItemOfTheSource_WithTheTemplateOfTheApplication()
        {
            var (_, list) = Show(NewList());

            var titles = list.GetVisualDescendants().OfType<PanelItem>().Select(p => p.Title).ToList();

            Assert.Equal(new[] { "one", "two", "three" }, titles);
        }

        [AvaloniaFact]
        public void FollowsTheChangesOfTheSource()
        {
            var source = new ObservableCollection<string> { "one" };
            var (_, list) = Show(NewList(source));

            source.Add("two");
            Dispatcher.UIThread.RunJobs();

            Assert.Equal(2, list.GetVisualDescendants().OfType<PanelItem>().Count());
        }

        [AvaloniaFact]
        public void TheAddButton_ShowsTheAddCaption_UnderTheItems()
        {
            var (_, list) = Show(NewList());

            var toggle = list.GetVisualDescendants().OfType<ToggleButton>().Single(t => Equals(t.Content, "Add"));
            var lastItem = list.GetVisualDescendants().OfType<PanelItem>().Last();

            Assert.True(toggle.TranslatePoint(new Point(0, 0), list)!.Value.Y >= lastItem.TranslatePoint(new Point(0, lastItem.Bounds.Height), list)!.Value.Y);
        }

        [AvaloniaFact]
        public void TheAddButton_OpensThePopupThatHoldsTheAddContent()
        {
            var panel = new Button { Content = "the panel" };
            var list = NewList();
            list.AddContent = panel;
            Show(list);
            var toggle = list.GetVisualDescendants().OfType<ToggleButton>().Single(t => Equals(t.Content, "Add"));
            var popup = list.GetVisualDescendants().OfType<Popup>().Single();
            Assert.False(popup.IsOpen);

            toggle.IsChecked = true;
            Dispatcher.UIThread.RunJobs();

            Assert.True(popup.IsOpen);
            Assert.Same(panel, ((ContentControl)((Border)popup.Child!).Child!).Content);
        }

        [AvaloniaFact]
        public void TheAddContent_GetsItsBindings_BeforeThePopupOpens()
        {
            var panel = new Button();
            panel.Bind(ContentControl.ContentProperty, new Binding("Name"));
            var list = NewList();
            list.DataContext = new Model();
            list.AddContent = panel;
            Show(list);

            Assert.Equal("model name", panel.Content);
        }

        [AvaloniaFact]
        public void TheReorderService_ReachesTheListOfItems()
        {
            var service = new FakeReorder();
            var list = NewList();
            list.ReorderService = service;
            Show(list);

            var items = list.GetVisualDescendants().OfType<ItemsControl>().Single();

            Assert.Same(service, ListBoxReorder.GetService(items));
        }

        [AvaloniaFact]
        public void WithoutAReorderService_TheListOfItemsHasNone()
        {
            var (_, list) = Show(NewList());

            Assert.Null(ListBoxReorder.GetService(list.GetVisualDescendants().OfType<ItemsControl>().Single()));
        }
    }
}
