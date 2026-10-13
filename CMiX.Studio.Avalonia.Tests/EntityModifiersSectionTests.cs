using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core.Compositing;
using CMiX.Core.Prefabs;
using CMiX.Studio.Avalonia.Views;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // The Modifiers section of an entity: its add button opens a popup with one button for each modifier
    // that fits the entity, and a click on one adds that modifier to the entity.
    public class EntityModifiersSectionTests
    {
        private static (Entity entity, EntityModifiersSection section, ToggleButton add, Popup popup) Show()
        {
            var entity = (Entity)TestServiceProviderFactory.Create().GetRequiredService<ControlFactory>().Create(typeof(Entity));
            var section = new EntityModifiersSection { PanelOwner = typeof(Entity), DataContext = entity };
            var window = new Window { Content = section, Width = 400, Height = 800 };
            window.Show();
            Dispatcher.UIThread.RunJobs();

            // The section starts folded: open it, as a click on its header does.
            var expander = section.GetVisualDescendants().OfType<CMiX.UI.Panels.ExpanderIndented>().First();
            expander.IsExpanded = true;
            Dispatcher.UIThread.RunJobs();

            var add = section.GetVisualDescendants().OfType<ToggleButton>().Single(t => Equals(t.Content, "Add Modifier"));
            var popup = section.GetVisualDescendants().OfType<Popup>().Single();
            return (entity, section, add, popup);
        }

        private static List<Button> ButtonsOf(Popup popup) => popup.Child!.GetVisualDescendants().OfType<Button>().ToList();

        [AvaloniaFact]
        public void TheAddButton_OpensThePopup_WithAButtonForEachModifierThatFitsTheEntity()
        {
            var (_, _, add, popup) = Show();

            add.IsChecked = true;
            Dispatcher.UIThread.RunJobs();

            Assert.True(popup.IsOpen);
            Assert.NotEmpty(ButtonsOf(popup));
        }

        [AvaloniaFact]
        public void AClickOnAModifierButton_AddsItToTheEntity()
        {
            var (entity, _, add, popup) = Show();
            add.IsChecked = true;
            Dispatcher.UIThread.RunJobs();
            var button = ButtonsOf(popup).First();
            Assert.Empty(entity.ModifierManager.ManagerData.Items);

            Assert.True(button.Command!.CanExecute(button.CommandParameter));
            button.Command.Execute(button.CommandParameter);
            Dispatcher.UIThread.RunJobs();

            Assert.Single(entity.ModifierManager.ManagerData.Items);
        }
    }
}
