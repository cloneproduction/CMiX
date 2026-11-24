// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using CMiX.Core;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.ViewModels;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views;
using Xunit;
using static CMiX.Studio.Avalonia.Tests.Support.ListTreeTestScreen;

namespace CMiX.Studio.Avalonia.Tests
{
    // The Tree keys of the main window run through CanRunTreeKey. With the focus in the list tree, its hooks let them run.
    public class ListTreeWindowKeysTests : IDisposable
    {
        private sealed record Screen : ListTreeMainScreen
        {
            public Screen(ListTreeMainScreen shown) : base(shown) { }

            public IList<IControl> Layers => Composition.LayerManager.ManagerData.Items;
        }

        private Views.MainWindow? window;

        public void Dispose() => window?.Close();

        // One composition with three layers. The first layer is selected, and its row has the focus.
        private Screen Show()
        {
            var screen = new Screen(ListTreeMainScreen.ShowWithThreeLayers(PanelChoice.Tree));
            window = screen.Window;
            screen.Tree.SelectedNode = screen.LayerNode(0);
            Settle(screen.Window);
            screen.View.Rows.RowOf(screen.LayerNode(0))!.Focus();
            Settle(screen.Window);
            Assert.True(screen.View.IsKeyboardFocusWithin);
            Assert.True(screen.View.IsActiveView);
            return screen;
        }

        private static void Press(Screen screen, PhysicalKey key, RawInputModifiers modifiers = RawInputModifiers.None)
        {
            screen.Window.KeyPressQwerty(key, modifiers);
            Settle(screen.Window);
        }

        [AvaloniaFact]
        public void Press_ShiftDWithTheFocusInTheNewView_DuplicatesTheSelectedRow()
        {
            var screen = Show();

            Press(screen, PhysicalKey.D, RawInputModifiers.Shift);

            Assert.Equal(4, screen.Layers.Count);
        }

        [AvaloniaFact]
        public void Press_AltDownWithTheFocusInTheNewView_MovesTheRowAndKeepsTheFocusOnIt()
        {
            var screen = Show();
            var first = screen.Layers[0];

            Press(screen, PhysicalKey.ArrowDown, RawInputModifiers.Alt);

            Assert.Same(first, screen.Layers[1]);
            Assert.Same(screen.View.Rows.RowOf(screen.Tree.SelectedNode!), screen.Window.FocusManager!.GetFocusedElement());
        }

        [AvaloniaFact]
        public void Press_EWithTheFocusInTheNewView_ClosesTheTargetRow()
        {
            var screen = Show();
            var composition = screen.Tree.CompositionNodes()[0];
            screen.Tree.CursorNode = composition;

            Press(screen, PhysicalKey.E);

            Assert.False(composition.IsExpanded);
        }

        [AvaloniaFact]
        public void Press_AltEWithTheFocusInTheNewView_ClosesTheOtherRows()
        {
            var screen = Show();
            var target = screen.LayerNode(0);
            var other = screen.LayerNode(1);
            Assert.True(other.IsExpanded);

            Press(screen, PhysicalKey.E, RawInputModifiers.Alt);

            Assert.False(other.IsExpanded);
            Assert.True(target.IsExpanded);
        }
    }
}
