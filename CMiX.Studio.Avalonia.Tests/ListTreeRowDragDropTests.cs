// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.VisualTree;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.ViewModels.ListTree;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views;
using CMiX.UI.Themes;
using Xunit;
using static CMiX.Studio.Avalonia.Tests.Support.TreeFixtures;
using Entity = CMiX.Core.Compositing.Entity;
using ScaleModifier = CMiX.Core.Modulation.Modifiers.ScaleModifier;
using TranslateModifier = CMiX.Core.Modulation.Modifiers.TranslateModifier;
using Layer = CMiX.Core.Compositing.Layer;

namespace CMiX.Studio.Avalonia.Tests
{
    // The new tree after a drop of a drag: the open cards, the flash and the scroll to the dropped row.
    public class ListTreeRowDragDropTests : IDisposable
    {
        private const double PressX = 120;

        private Scene scene = null!;
        private Window window = null!;
        private ListTreeProject view = null!;
        private Point last;

        public void Dispose() => window.Close();

        // Layer 1 of A has two entities, the first with two modifiers. Layer 2 of A and the layer of B are empty. Every card is open.
        private void Show(double height)
        {
            scene = CreateScene([2, 1], s =>
            {
                AddKinds(s.Layer);
                var entity = (Entity)s.Layer.ModelEntityManager.ManagerData.Items[0];
                entity.ModifierManager.AddItem(typeof(TranslateModifier));
                entity.ModifierManager.AddItem(typeof(ScaleModifier));
            });
            TestServiceProviderFactory.ExpandAll(scene.Tree);
            var screen = ListTreeTestScreen.Show(scene.Tree, 400, height);
            view = screen.View;
            window = screen.Window;
        }

        private PanelListTreeRows Panel => view.Rows;

        private ListTreeDrag Drag => Assert.IsType<ListTreeDrag>(view.RowDrag.Drag);

        private ScrollViewer Scroller => view.GetVisualDescendants().OfType<ScrollViewer>().First(scroller => scroller.Name == "scroller");

        private TreeNode Entity(int index) => scene.Group<EntitiesGroup>(0).Children[index];

        private static TreeNode ModifiersOf(TreeNode entity) => entity.Children.Single(child => child.Group is ModifiersGroup);

        private Layer LayerOfB => (Layer)scene.B.LayerManager.ManagerData.Items[0];

        private Point InWindow(double x, double y) => Panel.TranslatePoint(new Point(x, y), window)!.Value;

        private void MoveTo(double x, double y)
        {
            last = InWindow(x, y);
            TreeDrag.MoveTo(window, last);
        }

        private void DropInside(TreeNode source, TreeNode card)
        {
            TreeDrag.Render(window);
            var y = Panel.Layout.PlaceOf(ListTreeKeys.Of(source))!.Y + 16;
            window.MouseDown(InWindow(PressX, y), MouseButton.Left);
            MoveTo(PressX, y + 8);
            var target = Drag.Candidates.First(candidate => candidate.Slot.Position == DropPosition.Inside && ReferenceEquals(candidate.Slot.ShownTarget.Model, card.Item));
            MoveTo(PressX + (target.Place.Depth - Drag.Press.Depth) * 24, target.Place.Y + Drag.Press.GrabOffsetY);
            TreeDrag.Release(window, last);
            ListTreeTestScreen.Settle(window);
        }

        [AvaloniaFact]
        public void Release_ACardIntoAnotherList_GivesBackTheOpenStateOfTheCardAndOfEveryCardUnderIt()
        {
            Show(900);
            var entity = Entity(0).Item!;
            ModifiersOf(Entity(0)).IsExpanded = false;
            ListTreeTestScreen.Settle(window);

            DropInside(Entity(0), scene.LayerNodeOfA(1));

            var dropped = scene.Tree.SelectedNode!;
            Assert.Same(entity, dropped.Item);
            Assert.True(dropped.IsExpanded);
            Assert.False(ModifiersOf(dropped).IsExpanded);
            Assert.True(scene.LayerNodeOfA(1).IsExpanded);
        }

        [AvaloniaFact]
        public void Release_OnACandidate_FlashesTheDroppedRowInTheAccentColour()
        {
            Show(900);

            DropInside(Entity(1), scene.LayerNodeOfA(1));

            var row = Assert.IsType<RowListTree>(Panel.RowOf(scene.Tree.SelectedNode!));
            Assert.Contains(RowListTree.FlashClass, row.Classes);
            Assert.Equal(((ISolidColorBrush)Application.Current!.FindResource("AccentBrush")!).Color, ((ISolidColorBrush)row.FlashSurface.Background!).Color);
        }

        [AvaloniaFact]
        public void Flash_TheStyleOfARow_FadesFromTheTokenInTheFlashTime()
        {
            Show(900);
            var row = new RowListTree();

            var animation = row.Styles.OfType<Style>().SelectMany(style => style.Animations).OfType<Animation>().Single();

            Assert.Equal(TimeSpan.FromSeconds(0.7), Motion.FlashDuration);
            Assert.Equal(Motion.FlashDuration, animation.Duration);
            Assert.Equal(Application.Current!.FindResource("OpacityFlash"), ((Setter)animation.Children[0].Setters[0]).Value);
        }

        [AvaloniaFact]
        public void Release_OnAPlaceOutOfView_BringsTheDroppedRowIntoView()
        {
            Show(300);

            DropInside(Entity(0), scene.LayerNodeOfB(0));

            var row = Assert.IsType<RowListTree>(Panel.RowOf(scene.Tree.SelectedNode!));
            var top = Panel.Bounds.Y + PanelListTreeRows.GetTop(row) - Scroller.Offset.Y;
            Assert.Contains(scene.Tree.SelectedNode!.Item!, LayerOfB.ModelEntityManager.ManagerData.Items);
            Assert.True(Scroller.Offset.Y > 0);
            Assert.InRange(top, 0, Scroller.Viewport.Height - row.Bounds.Height);
        }
    }
}
