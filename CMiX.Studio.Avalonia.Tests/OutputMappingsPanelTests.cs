using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core.Compositing;
using CMiX.Core.Rendering;
using CMiX.Studio.Avalonia.Views.Managers;
using CMiX.UI.Controls;
using CMiX.UI.Controls.Enum;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // The list of the outputs. The count of outputs is fixed, so the list shows all of them and has no resize handle.
    public class OutputMappingsPanelTests
    {
        private static OutputMappingsPanel Panel() => new()
        {
            DataContext = TestServiceProviderFactory.Create().GetRequiredService<Project>().OutputMappingManager
        };

        private static void Show(Control content, double windowHeight = 1200)
        {
            new Window { Content = content, Width = 500, Height = windowHeight }.Show();
            Dispatcher.UIThread.RunJobs();
        }

        private static ListBox OutputList(Control panel) => panel.GetVisualDescendants().OfType<ListBox>().Single();

        [AvaloniaFact]
        public void TheList_ShowsAllTheOutputs_WithoutScrolling()
        {
            var panel = Panel();
            Show(panel);
            var scroll = OutputList(panel).GetVisualDescendants().OfType<ScrollViewer>().First();

            Assert.Equal(OutputMappingManager.SlotCount, OutputList(panel).ItemCount);
            Assert.Equal(scroll.Extent.Height, scroll.Viewport.Height);
        }

        [AvaloniaFact]
        public void TheRowsOfTheList_HaveNoVisibilityToggle()
        {
            var panel = Panel();
            Show(panel);

            Assert.Empty(OutputList(panel).GetVisualDescendants().OfType<ToggleButton>());
        }

        [AvaloniaFact]
        public void TheOutputMappingOfAComposition_CanBeAnyOfTheOutputs()
        {
            var provider = TestServiceProviderFactory.Create();
            var project = provider.GetRequiredService<Project>();
            project.CompositionManager.AddItem(typeof(Composition));
            var view = new Views.CompositionSettings { DataContext = project.CompositionManager.SelectedItem };
            Show(view);

            var combo = view.GetVisualDescendants().OfType<ComboBoxCaptioned>().Single();

            Assert.Equal(OutputMappingManager.SlotCount, combo.ItemsSource!.Cast<object>().Count());
        }

        [AvaloniaFact]
        public void TheList_HasNoResizeHandle()
        {
            var panel = Panel();
            Show(panel);

            Assert.Empty(OutputList(panel).GetVisualDescendants().OfType<Border>().Where(border => border.Name == "resizeBorder"));
            Assert.IsNotType<ListBoxCMiX>(OutputList(panel));
        }

        [AvaloniaFact]
        public void TheWheelOverTheList_ScrollsThePanelAround()
        {
            var panel = Panel();
            var outer = new ScrollViewer { Content = panel };
            Show(outer, windowHeight: 220);
            Assert.True(outer.Extent.Height > outer.Viewport.Height);
            var presenter = OutputList(panel).GetVisualDescendants().OfType<ScrollContentPresenter>().First();

            presenter.RaiseEvent(new PointerWheelEventArgs(
                presenter, new Pointer(Pointer.GetNextFreeId(), PointerType.Mouse, true), TopLevel.GetTopLevel(presenter)!,
                new Point(20, 20), 0, new PointerPointProperties(RawInputModifiers.None, PointerUpdateKind.Other),
                KeyModifiers.None, new Vector(0, -1))
            { RoutedEvent = InputElement.PointerWheelChangedEvent });
            Dispatcher.UIThread.RunJobs();

            Assert.True(outer.Offset.Y > 0);
        }
    }
}
