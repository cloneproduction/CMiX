using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless.XUnit;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core.Colors.Modifiers;
using CMiX.Core.Compositing;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Texturing;
using CMiX.Studio.Avalonia.Views.Managers;
using CMiX.UI.AttachedProperties;
using CMiX.UI.Controls;
using CMiX.UI.Panels;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Layer = CMiX.Core.Compositing.Layer;

namespace CMiX.Studio.Avalonia.Tests
{
    // The button that adds an item sits in the middle of the row of its slot: the swap toggle is on its left and the
    // arrows are on its right. So it has no rounded corner, whatever the kind of control that the caller gives to the slot.
    public class SlotSelectionPanelLookTests
    {
        private static void Show(Control view)
        {
            new Window { Content = view, Width = 600, Height = 800 }.Show();
            Dispatcher.UIThread.RunJobs();
        }

        // The control that has the corners: the toggle of a PopupButton, or the button itself.
        private static Control Cornered(Control selectionPanel) =>
            selectionPanel is PopupButton
                ? selectionPanel.GetVisualDescendants().OfType<ToggleButton>().Single(toggle => toggle.Name == "PopUpToggle")
                : selectionPanel;

        private static void AssertSquare(PrefabSlotManager slot)
        {
            var cornered = Cornered(slot.SelectionPanel);
            Assert.Equal(ControlPosition.None, PositionedControl.GetPosition(cornered));
            Assert.Equal(default, cornered.GetVisualDescendants().OfType<Border>().First().CornerRadius);
        }

        private static PrefabManager Manager() => TestServiceProviderFactory.Create().GetRequiredService<PrefabManager>();

        [AvaloniaFact]
        public void APlainButton_AsTheSelectionPanel_IsSquare()
        {
            var slot = new PrefabSlotManager { DataContext = Manager(), SelectionPanel = new Button { Content = "New Camera" } };
            Show(slot);

            AssertSquare(slot);
        }

        [AvaloniaFact]
        public void APopupButton_AsTheSelectionPanel_IsSquare()
        {
            var slot = new PrefabSlotManager { DataContext = Manager(), SelectionPanel = new PopupButton { Caption = "Add Entity" } };
            Show(slot);

            AssertSquare(slot);
        }

        [AvaloniaFact]
        public void AButtonThatChoosesItsOwnTheme_AsTheSelectionPanel_IsSquareToo()
        {
            var theme = (ControlTheme)Application.Current!.FindResource("ButtonRoundedRight")!;
            var slot = new PrefabSlotManager { DataContext = Manager(), SelectionPanel = new Button { Content = "New Color", Theme = theme } };
            Show(slot);

            AssertSquare(slot);
        }

        [AvaloniaFact]
        public void ASelectionPanelThatIsReplaced_IsSquareToo()
        {
            var slot = new PrefabSlotManager { DataContext = Manager(), SelectionPanel = new Button { Content = "First" } };
            Show(slot);

            slot.SelectionPanel = new Button { Content = "Second" };
            Dispatcher.UIThread.RunJobs();

            AssertSquare(slot);
        }

        public enum RealSlot { Cameras, Lights, Entities, Colors, Textures }

        public static TheoryData<RealSlot> RealSlots => new() { RealSlot.Cameras, RealSlot.Lights, RealSlot.Entities, RealSlot.Colors, RealSlot.Textures };

        private static Control RealView(RealSlot kind, ControlFactory factory) => kind switch
        {
            RealSlot.Cameras => new Views.LayerCameraTab { DataContext = factory.Create(typeof(Layer)) },
            RealSlot.Lights => new Views.LayerLightTab { DataContext = factory.Create(typeof(Layer)) },
            RealSlot.Entities => new Views.LayerEntityTab { DataContext = factory.Create(typeof(Layer)) },
            RealSlot.Colors => new Views.ColorPaletteModifier { DataContext = factory.Create(typeof(ColorPaletteModifier)) },
            _ => new Views.TextureSlot { DataContext = factory.Create(typeof(MaskTexture)) }
        };

        [AvaloniaTheory]
        [MemberData(nameof(RealSlots))]
        public void TheAddButton_OfEveryRealSlot_IsSquare(RealSlot kind)
        {
            var factory = TestServiceProviderFactory.Create().GetRequiredService<ControlFactory>();
            var view = RealView(kind, factory);
            Show(view);

            AssertSquare(view.GetVisualDescendants().OfType<PrefabSlotManager>().First());
        }
    }
}
