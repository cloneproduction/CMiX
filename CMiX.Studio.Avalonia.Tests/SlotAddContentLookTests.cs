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
    public class SlotAddContentLookTests
    {
        private static void Show(Control view)
        {
            new Window { Content = view, Width = 600, Height = 800 }.Show();
            Dispatcher.UIThread.RunJobs();
        }

        // The control that has the corners: the toggle of a ButtonPopup, or the button itself.
        private static Control Cornered(Control selectionPanel) =>
            selectionPanel is ButtonPopup
                ? selectionPanel.GetVisualDescendants().OfType<ToggleButton>().Single(toggle => toggle.Name == "popupToggle")
                : selectionPanel;

        private static void AssertSquare(ListPrefabSlot slot)
        {
            var cornered = Cornered(slot.AddContent!);
            Assert.Equal(ControlPosition.None, PositionedControl.GetPosition(cornered));
            Assert.Equal(default, cornered.GetVisualDescendants().OfType<Border>().First().CornerRadius);
        }

        private static PrefabManager Manager() => TestServiceProviderFactory.Create().GetRequiredService<PrefabManager>();

        [AvaloniaFact]
        public void APlainButton_AsTheAddContent_IsSquare()
        {
            var slot = new ListPrefabSlot { DataContext = Manager(), AddContent = new Button { Content = "New Camera" } };
            Show(slot);

            AssertSquare(slot);
        }

        [AvaloniaFact]
        public void AButtonPopup_AsTheAddContent_IsSquare()
        {
            var slot = new ListPrefabSlot { DataContext = Manager(), AddContent = new ButtonPopup { Caption = "Add Entity" } };
            Show(slot);

            AssertSquare(slot);
        }

        [AvaloniaFact]
        public void AButtonThatChoosesItsOwnTheme_AsTheAddContent_IsSquareToo()
        {
            var theme = (ControlTheme)Application.Current!.FindResource("ButtonRoundedRightNoHover")!;
            var slot = new ListPrefabSlot { DataContext = Manager(), AddContent = new Button { Content = "New Color", Theme = theme } };
            Show(slot);

            AssertSquare(slot);
        }

        [AvaloniaFact]
        public void AnAddContentThatIsReplaced_IsSquareToo()
        {
            var slot = new ListPrefabSlot { DataContext = Manager(), AddContent = new Button { Content = "First" } };
            Show(slot);

            slot.AddContent = new Button { Content = "Second" };
            Dispatcher.UIThread.RunJobs();

            AssertSquare(slot);
        }

        [AvaloniaFact]
        public void TheButtonsOfTheRow_TakeTheirCornersFromTheirPlace()
        {
            var slot = new ListPrefabSlot { DataContext = Manager(), AddContent = new Button { Content = "New Camera" } };
            Show(slot);

            ControlPosition PositionOf(string name) =>
                PositionedControl.GetPosition(slot.GetVisualDescendants().OfType<Control>().Single(control => control.Name == name));

            Assert.Equal(ControlPosition.Left, PositionOf("swapToggle"));
            Assert.Equal(ControlPosition.None, PositionOf("moveDownButton"));
            Assert.Equal(ControlPosition.None, PositionOf("moveUpButton"));
            Assert.Equal(ControlPosition.None, PositionOf("duplicateButton"));
            Assert.Equal(ControlPosition.Right, PositionOf("removeButton"));
        }

        public enum RealSlot { Cameras, Lights, Entities, Colors, Textures }

        public static TheoryData<RealSlot> RealSlots => new() { RealSlot.Cameras, RealSlot.Lights, RealSlot.Entities, RealSlot.Colors, RealSlot.Textures };

        private static Control RealView(RealSlot kind, ControlFactory factory) => kind switch
        {
            RealSlot.Cameras => new Views.TabLayerCamera { DataContext = factory.Create(typeof(Layer)) },
            RealSlot.Lights => new Views.TabLayerLight { DataContext = factory.Create(typeof(Layer)) },
            RealSlot.Entities => new Views.TabLayerEntity { DataContext = factory.Create(typeof(Layer)) },
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

            AssertSquare(view.GetVisualDescendants().OfType<ListPrefabSlot>().First());
        }
    }
}
