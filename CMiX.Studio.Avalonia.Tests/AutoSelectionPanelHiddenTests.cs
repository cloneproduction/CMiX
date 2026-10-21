using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using CMiX.Core.Prefabs;
using CMiX.Studio.Avalonia.Views.Managers;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    public class HiddenMenuOwner
    {
    }

    [ModifierPanel(typeof(HiddenMenuOwner))]
    public class VisibleMenuModifier
    {
    }

    [HiddenInMenu]
    [ModifierPanel(typeof(HiddenMenuOwner))]
    public class HiddenMenuModifier
    {
    }

    public interface IMenuItem
    {
    }

    public class VisibleMenuItem : IMenuItem
    {
    }

    [HiddenInMenu]
    public class HiddenMenuItem : IMenuItem
    {
    }

    // The add menus do not offer a type with HiddenInMenu.
    public class AutoSelectionPanelHiddenTests
    {
        private static List<object?> Offered(AutoSelectionPanel panel) =>
            ((Grid)panel.Content!).Children
                .OfType<StackPanel>()
                .SelectMany(stack => stack.Children.OfType<Button>())
                .Select(button => button.CommandParameter)
                .ToList();

        [AvaloniaFact]
        public void ByAttribute_ALabelledTypeIsOffered()
        {
            var panel = new AutoSelectionPanel { PanelOwner = typeof(HiddenMenuOwner) };

            Assert.Contains(typeof(VisibleMenuModifier), Offered(panel));
        }

        [AvaloniaFact]
        public void ByAttribute_AHiddenTypeIsNotOffered()
        {
            var panel = new AutoSelectionPanel { PanelOwner = typeof(HiddenMenuOwner) };

            Assert.DoesNotContain(typeof(HiddenMenuModifier), Offered(panel));
        }

        [AvaloniaFact]
        public void ByInterface_AHiddenTypeIsNotOffered()
        {
            var panel = new AutoSelectionPanel { Interface = typeof(IMenuItem) };

            Assert.Contains(typeof(VisibleMenuItem), Offered(panel));
            Assert.DoesNotContain(typeof(HiddenMenuItem), Offered(panel));
        }
    }
}
