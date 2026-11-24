// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Headless.XUnit;
using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views;
using Xunit;
using static CMiX.Studio.Avalonia.Tests.Support.TreeFixtures;
using Echo = CMiX.Core.Texturing.Filters.Echo;

namespace CMiX.Studio.Avalonia.Tests
{
    // The screen requests of the tree in the list tree: the reveal brings a row to the middle, a close keeps the anchor row still,
    // a selection by a program shows its row.
    public class ListTreeScreenRequestsTests : IDisposable
    {
        private readonly Scene scene;
        private readonly Window window;
        private readonly ListTreeProject view;

        // A with twelve layers. Each layer holds an Echo filter. Every card is open.
        public ListTreeScreenRequestsTests()
        {
            scene = CreateScene([12], s => s.Layer.TextureModifierManager.AddItem(typeof(Echo)));
            for (var i = 1; i < 12; i++)
                scene.LayerOfA(i).TextureModifierManager.AddItem(typeof(Echo));
            foreach (var node in scene.All())
                node.IsExpanded = true;
            (window, view) = ListTreeTestScreen.Show(scene.Tree, 400, 400);
            Settle();
        }

        public void Dispose() => window.Close();

        private ProjectTree Tree => scene.Tree;

        private ScrollViewer Scroller => view.GetVisualDescendants().OfType<ScrollViewer>().First();

        private void Settle()
        {
            for (var i = 0; i < 10; i++)
                ListTreeTestScreen.Settle(window);
        }

        private TreeNode EchoOf(int layer) =>
            scene.All().Single(node => node.Item is Echo && ReferenceEquals(node.Item, scene.LayerOfA(layer).TextureModifierManager.ManagerData.Items[0]));

        private RowListTree RowOf(TreeNode node) => Assert.IsType<RowListTree>(view.Rows.RowOf(node));

        private double TopOf(TreeNode node) => RowOf(node).TranslatePoint(default, Scroller)!.Value.Y;

        private void ScrollTo(double offset)
        {
            Scroller.Offset = Scroller.Offset.WithY(offset);
            Settle();
        }

        // The row of the node at this distance from the top of the view.
        private void ScrollRowTo(TreeNode node, double top) => ScrollTo(Scroller.Offset.Y + TopOf(node) - top);

        private void CloseTheParentsUnderTheComposition(TreeNode node)
        {
            for (var parent = node.Parent; parent != null && parent.Parent?.Parent != null; parent = parent.Parent)
                parent.IsExpanded = false;
            Settle();
        }

        [AvaloniaFact]
        public void RevealSelected_ARowUnderClosedCardsFarBelow_ScrollsTheRowToTheMiddle()
        {
            var target = EchoOf(8);
            Tree.SelectedNode = target;
            CloseTheParentsUnderTheComposition(target);
            ScrollTo(0);

            Tree.RevealSelectedCommand.Execute(null);
            Dispatcher.UIThread.RunJobs();
            Settle();

            Assert.True(Scroller.Offset.Y > 0);
            var middle = TopOf(target) + RowOf(target).Bounds.Height / 2;
            Assert.Equal(Scroller.Viewport.Height / 2, middle, 1.0);
        }

        [AvaloniaFact]
        public void CollapseOthers_ATargetInView_StaysAtItsPlaceWhileTheRowsAboveClose()
        {
            var target = scene.LayerNodeOfA(8);
            Tree.SelectedNode = target;
            ScrollRowTo(target, 120);
            var before = TopOf(target);
            Assert.InRange(before, 0, Scroller.Viewport.Height - 24);

            Tree.CollapseOthersCommand.Execute(null);
            Settle();

            Assert.False(scene.LayerNodeOfA(2).IsExpanded);
            Assert.Equal(before, TopOf(target), 1.0);
        }

        [AvaloniaFact]
        public void CollapseOthers_ATargetOutOfView_BringsItIntoView()
        {
            var target = EchoOf(11);
            Tree.SelectedNode = target;
            ScrollTo(0);
            Assert.True(TopOf(target) > Scroller.Viewport.Height);

            Tree.CollapseOthersCommand.Execute(null);
            Settle();

            Assert.InRange(TopOf(target), 0, Scroller.Viewport.Height - 1);
        }

        [AvaloniaFact]
        public void ClearSearch_AfterTheSearchOpenedCardsAbove_KeepsTheSelectedRowStill()
        {
            var selected = scene.LayerNodeOfA(9);
            for (var i = 0; i < 9; i++)
                CloseTheParentsUnderTheComposition(EchoOf(i));
            Tree.SelectedNode = selected;
            Tree.SearchText = "Echo";
            Settle();
            Assert.True(EchoOf(2).Parent!.IsExpanded);
            ScrollRowTo(selected, 150);
            var before = TopOf(selected);

            Tree.SearchText = string.Empty;
            Settle();

            Assert.False(EchoOf(2).Parent!.IsExpanded);
            Assert.Equal(before, TopOf(selected), 1.0);
        }

        [AvaloniaFact]
        public void Select_ByAProgramARowFarBelow_BringsTheRowIntoView()
        {
            var target = EchoOf(11);
            Tree.Select(scene.NodeA, SelectionOrigin.Pointer);
            ScrollTo(0);

            Tree.Select(target, SelectionOrigin.Program);
            Settle();

            Assert.InRange(TopOf(target), 0, Scroller.Viewport.Height - 1);
        }

        [AvaloniaFact]
        public void Select_ByThePointerARowFarBelow_DoesNotScroll()
        {
            var target = EchoOf(11);
            Tree.Select(scene.NodeA, SelectionOrigin.Pointer);
            ScrollTo(0);

            Tree.Select(target, SelectionOrigin.Pointer);
            Settle();

            Assert.Equal(0, Scroller.Offset.Y);
        }
    }
}
