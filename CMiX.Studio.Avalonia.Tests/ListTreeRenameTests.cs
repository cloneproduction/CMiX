// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.VisualTree;
using CMiX.Core.Prefabs;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views;
using CMiX.UI.Controls.String;
using Xunit;
using static CMiX.Studio.Avalonia.Tests.Support.TreeFixtures;

namespace CMiX.Studio.Avalonia.Tests
{
    // A double click on the name or F2 renames a row of the list tree in place. Enter or a click outside keeps the name, Escape cancels.
    // A click selects only, a click on the icon of a card opens or closes it only, a double click away from the name opens or closes it.
    public class ListTreeRenameTests : IDisposable
    {
        private readonly Scene scene = CreateScene([2], s => AddKinds(s.Layer));
        private readonly Window window;
        private readonly ListTreeProject view;

        public ListTreeRenameTests()
        {
            view = new ListTreeProject { DataContext = scene.Tree };
            view.Rows.MoveDuration = TimeSpan.Zero;
            window = new Window { Content = view, Width = 400, Height = 900 };
            window.Show();
            scene.Tree.SelectedNode = Entity;
            Settle();
        }

        public void Dispose() => window.Close();

        private ProjectTree Tree => scene.Tree;

        private TreeNode Layer => scene.LayerNodeOfA(0);

        private TreeNode Entity => scene.Group<EntitiesGroup>(0).Children[0];

        private static string NameOf(TreeNode node) => ((IPrefab)node.Item!).PrefabService.Name.Value;

        private void Settle() => ListTreeTestScreen.Settle(window);

        private RowListTree RowOf(TreeNode node) => Assert.IsType<RowListTree>(view.Rows.RowOf(node));

        private static TextBoxEditable NameBoxOf(RowListTree row) => row.GetVisualDescendants().OfType<TextBoxEditable>().First(box => box.Name == "nameBox");

        private static TextBox? InputOf(RowListTree row) => NameBoxOf(row).GetVisualDescendants().OfType<TextBox>().FirstOrDefault(input => input.IsVisible);

        private static bool IsRenaming(RowListTree row) => InputOf(row) != null;

        private IInputElement? Focused() => window.FocusManager!.GetFocusedElement();

        private Point OnTheName(RowListTree row)
        {
            var box = NameBoxOf(row);
            return box.TranslatePoint(new Point(10, box.Bounds.Height / 2), window)!.Value;
        }

        private Point CenterOf(Control control) =>
            control.TranslatePoint(new Point(control.Bounds.Width / 2, control.Bounds.Height / 2), window)!.Value;

        private Point BlankOf(RowListTree row) => row.TranslatePoint(new Point(row.Bounds.Width - 100, row.Bounds.Height / 2), window)!.Value;

        private void Click(Point point)
        {
            TreeDrag.Render(window);
            window.MouseDown(point, MouseButton.Left);
            window.MouseUp(point, MouseButton.Left);
            Settle();
        }

        private void DoubleClick(Point point)
        {
            TreeDrag.Render(window);
            window.MouseDown(point, MouseButton.Left);
            window.MouseUp(point, MouseButton.Left);
            window.MouseDown(point, MouseButton.Left);
            window.MouseUp(point, MouseButton.Left);
            Settle();
        }

        private void Press(PhysicalKey key)
        {
            window.KeyPressQwerty(key, RawInputModifiers.None);
            Settle();
        }

        private TextBox StartRenameByF2(TreeNode node)
        {
            RowOf(node).Focus();
            Settle();
            Press(PhysicalKey.F2);
            return Assert.IsType<TextBox>(InputOf(RowOf(node)));
        }

        [AvaloniaFact]
        public void DoubleClick_OnTheNameOfACard_RenamesInPlaceAndKeepsTheCardAsItIs()
        {
            var before = Layer.IsExpanded;

            DoubleClick(OnTheName(RowOf(Layer)));

            Assert.True(IsRenaming(RowOf(Layer)));
            Assert.True(InputOf(RowOf(Layer))!.IsFocused);
            Assert.Equal(before, Layer.IsExpanded);
            Assert.Same(Layer, Tree.SelectedNode);
        }

        [AvaloniaFact]
        public void DoubleClick_AwayFromTheNameOfACard_OpensOrClosesItAndRenamesNothing()
        {
            var before = Layer.IsExpanded;

            DoubleClick(BlankOf(RowOf(Layer)));

            Assert.False(IsRenaming(RowOf(Layer)));
            Assert.Equal(!before, Layer.IsExpanded);
            Assert.Same(Layer, Tree.SelectedNode);
        }

        [AvaloniaFact]
        public void DoubleClick_TheIconOfACard_OpensOrClosesItOncePerClick()
        {
            var before = Layer.IsExpanded;

            DoubleClick(CenterOf(RowOf(Layer).IconHit));

            Assert.Equal(before, Layer.IsExpanded);
        }

        [AvaloniaFact]
        public void DoubleClick_AToggleOfACard_KeepsTheCardAsItIs()
        {
            var before = Layer.IsExpanded;
            var toggle = RowOf(Layer).Toggles.GetVisualDescendants().OfType<ToggleButton>().First(button => button.IsEffectivelyVisible);

            DoubleClick(CenterOf(toggle));

            Assert.Equal(before, Layer.IsExpanded);
        }

        [AvaloniaFact]
        public void Click_TheIconOfACard_OpensOrClosesItOnly()
        {
            var before = Layer.IsExpanded;

            Click(CenterOf(RowOf(Layer).IconHit));

            Assert.Equal(!before, Layer.IsExpanded);
            Assert.Same(Entity, Tree.SelectedNode);
            Assert.False(IsRenaming(RowOf(Layer)));
        }

        [AvaloniaFact]
        public void F2_WithTheFocusOnARow_RenamesTheSelectedRowInPlace()
        {
            var input = StartRenameByF2(Entity);

            Assert.True(input.IsFocused);
            Assert.Equal(NameOf(Entity), input.Text);
        }

        [AvaloniaFact]
        public void RenameSelected_TheNewViewIsActive_RenamesTheTargetRowInPlace()
        {
            Tree.RenameSelectedCommand.Execute(null);
            Settle();

            Assert.True(IsRenaming(RowOf(Entity)));
        }

        [AvaloniaFact]
        public void Enter_DuringARename_KeepsTheNameAndGivesTheFocusBackToTheRow()
        {
            var input = StartRenameByF2(Entity);
            input.Text = "Renamed";

            Press(PhysicalKey.Enter);

            Assert.Equal("Renamed", NameOf(Entity));
            Assert.False(IsRenaming(RowOf(Entity)));
            Assert.Same(RowOf(Entity), Focused());
        }

        [AvaloniaFact]
        public void Escape_DuringARename_CancelsAndKeepsTheOldName()
        {
            var before = NameOf(Entity);
            var input = StartRenameByF2(Entity);
            input.Text = "Renamed";

            Press(PhysicalKey.Escape);

            Assert.Equal(before, NameOf(Entity));
            Assert.False(IsRenaming(RowOf(Entity)));
            Assert.Same(RowOf(Entity), Focused());
        }

        [AvaloniaFact]
        public void Click_OutsideTheNameDuringARename_KeepsTheName()
        {
            var input = StartRenameByF2(Entity);
            input.Text = "Renamed";

            Click(BlankOf(RowOf(Layer)));

            Assert.Equal("Renamed", NameOf(Entity));
            Assert.False(IsRenaming(RowOf(Entity)));
        }
    }
}
