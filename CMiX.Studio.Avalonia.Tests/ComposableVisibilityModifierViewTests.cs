using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core.Modulation.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Studio.Avalonia.Views.Modulation;
using CMiX.UI.Controls.Boolean;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    public class ComposableVisibilityModifierViewTests
    {
        private static (ComposableVisibilityModifier Modifier, Views.ComposableVisibilityModifier View) Open()
        {
            var modifier = (ComposableVisibilityModifier)TestServiceProviderFactory.Create().GetRequiredService<ControlFactory>().Create(typeof(ComposableVisibilityModifier));
            var view = new Views.ComposableVisibilityModifier { DataContext = modifier };
            var window = new Window { Content = view, Width = 500, Height = 400 };
            window.Show();
            Dispatcher.UIThread.RunJobs();
            return (modifier, view);
        }

        [AvaloniaFact]
        public void View_ShowsTheVisibleToggleWithAnAssignButton()
        {
            var (modifier, view) = Open();

            var toggle = view.GetVisualDescendants().OfType<ToggleButtonCMiX>().Single(t => (t.Content as string) == "Visible");
            var assign = view.GetVisualDescendants().OfType<ButtonModulatorAssign>().First();

            Assert.True(toggle.IsChecked);
            Assert.Same(modifier.ModulatorManager, assign.ModulatorManager);
        }

        [AvaloniaFact]
        public void Toggle_WritesTheVisibleValue()
        {
            var (modifier, view) = Open();
            var toggle = view.GetVisualDescendants().OfType<ToggleButtonCMiX>().Single(t => (t.Content as string) == "Visible");

            toggle.IsChecked = false;
            Dispatcher.UIThread.RunJobs();

            Assert.False(modifier.Visible.Value);
        }

        [AvaloniaFact]
        public void View_ShowsTheAddModulatorList()
        {
            var (_, view) = Open();

            Assert.Contains(view.GetVisualDescendants().OfType<Views.ModifierManager>(), m => m.Caption == "Add Modulator");
        }

        [AvaloniaTheory]
        [InlineData(typeof(CMiX.Core.Compositing.Composition))]
        [InlineData(typeof(CMiX.Core.Compositing.Layer))]
        public void AddList_OffersAlphaAndVisibility(System.Type owner)
        {
            var panel = new Views.Managers.AutoSelectionPanel { PanelOwner = owner };
            var window = new Window { Content = panel, Width = 400, Height = 400 };
            window.Show();
            Dispatcher.UIThread.RunJobs();

            var labels = panel.GetVisualDescendants().OfType<Button>().Select(b => b.Content as string).ToList();

            Assert.Contains("Alpha", labels);
            Assert.Contains("Visibility", labels);
        }
    }
}
