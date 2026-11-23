// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using Avalonia.VisualTree;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.ViewModels.ListTree;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views;
using CMiX.UI.Controls;
using CMiX.UI.Controls.String;
using Xunit;
using static CMiX.Studio.Avalonia.Tests.Support.TreeFixtures;

namespace CMiX.Studio.Avalonia.Tests
{
    // The panel of the new tree during a drag: the rows at the places of a candidate, the dragged row, the dimmed rows and the drop target.
    public class PanelListTreeRowsDragTests : IDisposable
    {
        private readonly TreeDropService service = new();
        private readonly Scene scene;
        private readonly Window window;
        private readonly ListTreeProject view;

        // Layer 1 of A has two entities, a filter and modifiers. Layer 2 of A is empty. Every card with rows is open.
        public PanelListTreeRowsDragTests()
        {
            scene = CreateScene([2, 1], s => AddKinds(s.Layer));
            TestServiceProviderFactory.ExpandAll(scene.Tree);
            var screen = ListTreeTestScreen.Show(scene.Tree, 400, 900);
            view = screen.View;
            window = screen.Window;
        }

        public void Dispose() => window.Close();

        private PanelListTreeRows Panel => view.Rows;

        private TreeNode Entity(int index) => scene.Group<EntitiesGroup>(0).Children[index];

        private void Settle() => TreeTestScreen.Settle(window);

        private ListTreeDragLook LookOf(TreeNode source, Func<ListTreeCandidate, bool> pick)
        {
            var live = ListTreeSnapshot.Of(scene.Tree.Roots);
            var row = live.RowOf(source)!;
            var candidates = new ListTreeCandidates(new ListTreeSlots(service), service, ListTreeSizes.Default).Of(live, row);
            var liveRows = ListTreeLiveRows.Of(live);
            return ListTreeDragLook.Of(liveRows, row, candidates.First(pick), ListTreeBrightRows.Of(liveRows, row, candidates));
        }

        private ListTreeDragLook Show(ListTreeDragLook look)
        {
            Panel.ShowDrag(look);
            Settle();
            return look;
        }

        private static bool IsInside(ListTreeCandidate candidate, TreeNode target) =>
            candidate.Slot.Position == DropPosition.Inside && ReferenceEquals(candidate.Slot.ShownTarget.Model, target.Item);

        private RowListTree RowOf(ListTreeRowKey key) => Assert.IsType<RowListTree>(Panel.RowOf(key));

        private static Color ColorOf(IBrush? brush) => brush is ISolidColorBrush solid ? solid.Color : Colors.Transparent;

        private static Color Token(string key) => ColorOf((IBrush)Application.Current!.FindResource(key)!);

        private static Color TextOf(RowListTree row) =>
            ColorOf(row.GetVisualDescendants().OfType<TextBoxEditable>().FirstOrDefault(box => box.IsVisible)?.Foreground
                ?? row.GetVisualDescendants().OfType<TextBlock>().First(text => text.Name == "groupName").Foreground);

        private static Color IconOf(RowListTree row) =>
            ColorOf(row.GetVisualDescendants().OfType<AppIcon>().First(icon => icon.Name == "kindIcon").Foreground);

        [AvaloniaFact]
        public void ShowDrag_ACandidate_PlacesEveryRowAtItsPlaceInTheCandidate()
        {
            var look = Show(LookOf(Entity(0), candidate => IsInside(candidate, scene.LayerNodeOfA(1))));

            Assert.All(look.Layout.Places, place => Assert.Equal(place.Y, PanelListTreeRows.GetTop(RowOf(place.Key))));
            Assert.Equal(look.Layout.TotalHeight, Panel.Bounds.Height);
        }

        [AvaloniaFact]
        public void ShowDrag_TheDraggedRow_SnapsIntoItsLandingPlaceAtItsNewDepth()
        {
            var look = Show(LookOf(Entity(0), candidate => IsInside(candidate, scene.LayerNodeOfA(1))));
            var dragged = RowOf(look.Dragged);

            Assert.Equal(look.Candidate.Place.Y, PanelListTreeRows.GetTop(dragged));
            Assert.Equal(look.Candidate.Place.Indent, dragged.IndentMargin.Left);
            Assert.Equal(ListTreeLayers.DraggedRow, dragged.ZIndex);
            Assert.Null(dragged.Transitions);
            Assert.Equal(Token("ListTreeRowDraggedBrush"), ColorOf(dragged.Background));
            Assert.Equal(Token("ListTreeTextSelectedBrush"), TextOf(dragged));
        }

        [AvaloniaFact]
        public void ShowDrag_ADraggedCard_ShowsItClosed()
        {
            var layer = scene.LayerNodeOfA(0);
            var look = Show(LookOf(layer, candidate => candidate.Slot.Position == DropPosition.After && ReferenceEquals(candidate.Slot.Target, scene.LayerNodeOfA(1))));

            Assert.DoesNotContain(look.Layout.Places, place => look.Dragged.Equals(place.Key.Parent));
            Assert.True(layer.IsExpanded);
        }

        [AvaloniaFact]
        public void ShowDrag_ARowThatCanNotTakeTheDrop_IsAReadableGreyWithNoOpacity()
        {
            var look = Show(LookOf(Entity(0), candidate => IsInside(candidate, scene.LayerNodeOfA(1))));
            var modifier = RowOf(ListTreeKeys.Of(scene.Group<ModifiersGroup>(0).Children[0]));

            Assert.Equal(ListTreeRowLight.Dimmed, look.LightOf(ListTreeKeys.Of(scene.Group<ModifiersGroup>(0).Children[0])));
            Assert.Equal(Token("ListTreeRowDimmedBandBrush"), ColorOf(modifier.Band.Background));
            Assert.Equal(Token("ListTreeTextDimmedBrush"), TextOf(modifier));
            Assert.Equal(Token("ListTreeIconDimmedBrush"), IconOf(modifier));
            Assert.All(modifier.Toggles.GetVisualDescendants().OfType<ToggleButton>(),
                toggle => Assert.Equal(Token("ListTreeTextDimmedBrush"), ColorOf(toggle.GetValue(TextElement.ForegroundProperty))));
            Assert.Equal(1, modifier.Opacity);
        }

        private void PointAt(RowListTree row)
        {
            TreeDrag.Render(window);
            window.MouseMove(row.TranslatePoint(new Point(row.Bounds.Width - 100, row.Bounds.Height / 2), window)!.Value);
            Settle();
        }

        private static Border Named(RowListTree row, string name) => row.GetVisualDescendants().OfType<Border>().First(border => border.Name == name);

        [AvaloniaFact]
        public void ShowDrag_TheDraggedRowUnderThePointer_HasTheDraggedSurface()
        {
            var look = Show(LookOf(Entity(0), candidate => IsInside(candidate, scene.LayerNodeOfA(1))));
            var dragged = RowOf(look.Dragged);

            PointAt(dragged);

            Assert.True(dragged.IsPointerOver);
            Assert.Equal(Token("ListTreeRowDraggedBrush"), ColorOf(dragged.Background));
            Assert.False(Named(dragged, "mark").IsVisible);
        }

        [AvaloniaFact]
        public void ShowDrag_ADimmedRowUnderThePointer_ShowsNoHover()
        {
            Show(LookOf(Entity(0), candidate => IsInside(candidate, scene.LayerNodeOfA(1))));
            var modifier = RowOf(ListTreeKeys.Of(scene.Group<ModifiersGroup>(0).Children[0]));

            PointAt(modifier);

            Assert.True(modifier.IsPointerOver);
            Assert.Equal(Colors.Transparent, ColorOf(modifier.Background));
            Assert.Equal(Token("ListTreeRowDimmedBandBrush"), ColorOf(modifier.Band.Background));
            Assert.False(Named(modifier, "mark").IsVisible);
        }

        [AvaloniaFact]
        public void ShowDrag_ARowThatCanTakeTheDrop_StaysBright()
        {
            var look = Show(LookOf(Entity(0), candidate => IsInside(candidate, scene.LayerNodeOfA(1))));
            var sibling = RowOf(ListTreeKeys.Of(Entity(1)));

            Assert.Equal(ListTreeRowLight.Rest, look.LightOf(ListTreeKeys.Of(Entity(1))));
            Assert.Equal(Token("ListTreeRowRestBandBrush"), ColorOf(sibling.Band.Background));
            Assert.Equal(Token("ListTreeTextRestBrush"), TextOf(sibling));
        }

        [AvaloniaFact]
        public void ShowDrag_AMoveIntoAnotherCard_OutlinesThatCard()
        {
            var target = scene.LayerNodeOfA(1);
            Show(LookOf(Entity(0), candidate => IsInside(candidate, target)));

            Assert.Same(target, Panel.Surfaces.DropTarget);
            Assert.True(Panel.Surfaces.Outline.IsVisible);
            Assert.Equal(ListTreeKeys.Of(target), Panel.Surfaces.Outline.Place!.Key);
        }

        [AvaloniaFact]
        public void ShowDrag_ANewCandidate_KeepsTheAreaOfACardThatStays()
        {
            var composition = ListTreeKeys.Of(scene.NodeA);
            Show(LookOf(Entity(0), candidate => IsInside(candidate, scene.LayerNodeOfA(1))));
            var area = Panel.Surfaces.CardAreaOf(composition);

            var look = Show(LookOf(Entity(0), candidate => candidate.Slot.Position == DropPosition.After && ReferenceEquals(candidate.Slot.Target, Entity(1))));

            Assert.NotNull(area);
            Assert.Same(area, Panel.Surfaces.CardAreaOf(composition));
            Assert.Equal(look.Layout.PlaceOf(composition), area.Place);
        }

        [AvaloniaFact]
        public void ShowDrag_AReorderInTheSameParent_ShowsNoOutline()
        {
            var look = Show(LookOf(Entity(0), candidate => candidate.Slot.Position == DropPosition.After && ReferenceEquals(candidate.Slot.Target, Entity(1))));

            Assert.Equal(ListTreeKeys.Of(Entity(0)).Parent, look.Dragged.Parent);
            Assert.Null(Panel.Surfaces.DropTarget);
            Assert.False(Panel.Surfaces.Outline.IsVisible);
        }

        [AvaloniaFact]
        public void ShowDrag_ASelectedRow_HidesTheSelectionMark()
        {
            scene.Tree.SelectByClick(Entity(1));
            Settle();

            Show(LookOf(Entity(0), candidate => IsInside(candidate, scene.LayerNodeOfA(1))));

            Assert.False(RowOf(ListTreeKeys.Of(Entity(1))).IsSelected);
        }

        [AvaloniaFact]
        public void ShowLiveTree_AfterADrag_PutsEveryRowBackAtRest()
        {
            var selected = Entity(1);
            scene.Tree.SelectByClick(selected);
            Show(LookOf(Entity(0), candidate => IsInside(candidate, scene.LayerNodeOfA(1))));

            Panel.ShowLiveTree();
            Settle();

            Assert.All(Panel.Layout.Places, place => Assert.Equal(place.Y, PanelListTreeRows.GetTop(RowOf(place.Key))));
            Assert.All(Panel.Layout.Places, place => Assert.DoesNotContain(RowListTree.DimmedClass, RowOf(place.Key).Classes));
            Assert.Null(Panel.Surfaces.DropTarget);
            Assert.True(RowOf(ListTreeKeys.Of(selected)).IsSelected);
        }
    }
}
