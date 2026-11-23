// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.VisualTree;
using CMiX.Core;
using CMiX.Studio.Avalonia.ViewModels;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views;
using Xunit;
using static CMiX.Studio.Avalonia.Tests.Support.TreeTestScreen;
using Composition = CMiX.Core.Compositing.Composition;
using Layer = CMiX.Core.Compositing.Layer;

namespace CMiX.Studio.Avalonia.Tests
{
    // The Tree keys of the main window run through CanRunTreeKey. With the focus in the new list tree, its hooks let them run.
    public class ListTreeWindowKeysTests : IDisposable
    {
        private sealed record Screen(Views.MainWindow Window, MainViewModel ViewModel, ListTreeProject View)
        {
            public ProjectTree Tree => ViewModel.ProjectTree;
            public Composition Composition => (Composition)ViewModel.Project.CompositionManager.ManagerData.Items[0];
            public IList<IControl> Layers => Composition.LayerManager.ManagerData.Items;
            public TreeNode LayerNode(int index) => Tree.CompositionNodes()[0].Children[0].Children[index];
        }

        private Views.MainWindow? window;

        public void Dispose() => window?.Close();

        // The new tree is the only tree. One composition with three layers. The first layer is selected, and its row has the focus.
        private Screen Show()
        {
            var (_, mainWindow, viewModel) = TestServiceProviderFactory.ShowMainWindow();
            window = mainWindow;
            viewModel.LeftPanel = PanelChoice.ListTree;
            viewModel.Project.CompositionManager.AddItem(typeof(Composition));
            var composition = (Composition)viewModel.Project.CompositionManager.ManagerData.Items[0];
            for (var i = 0; i < 3; i++)
                composition.LayerManager.AddItem(typeof(Layer));
            TestServiceProviderFactory.Pump();
            TestServiceProviderFactory.ExpandAll(viewModel.ProjectTree);
            TestServiceProviderFactory.Pump();
            var view = mainWindow.GetVisualDescendants().OfType<ListTreeProject>().Single();
            var screen = new Screen(mainWindow, viewModel, view);
            screen.Tree.SelectedNode = screen.LayerNode(0);
            Settle(mainWindow);
            view.Rows.RowOf(screen.LayerNode(0))!.Focus();
            Settle(mainWindow);
            Assert.True(view.IsKeyboardFocusWithin);
            Assert.True(view.IsActiveView);
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
