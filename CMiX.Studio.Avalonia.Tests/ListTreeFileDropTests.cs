// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.Themes;
using CMiX.Studio.Avalonia.ViewModels.ListTree;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views;
using Xunit;
using static CMiX.Studio.Avalonia.Tests.Support.TreeFixtures;
using Composition = CMiX.Core.Compositing.Composition;

namespace CMiX.Studio.Avalonia.Tests
{
    // Files from disk over the list tree: the rules of TreeFileDropRules, the marks of the drop, the outline of the target.
    public class ListTreeFileDropTests : IDisposable
    {
        private readonly string folder = Path.Combine(Path.GetTempPath(), "cmix-list-tree-drop-" + Guid.NewGuid().ToString("N"));
        private Window? window;
        private ListTreeProject? view;
        private Scene? scene;

        public ListTreeFileDropTests()
        {
            Directory.CreateDirectory(folder);
        }

        public void Dispose()
        {
            window?.Close();
            Directory.Delete(folder, true);
        }

        private ListTreeProject View => view!;

        private Scene Shown => scene!;

        private PanelListTreeRows Rows => View.Rows;

        // One layer with entities, a texture modifier and modifiers, every card open.
        private void Show(int[] layers)
        {
            scene = CreateScene(layers, s => { if (layers.Length > 0) AddKinds(s.Layer); });
            TestServiceProviderFactory.ExpandAll(scene.Tree);
            var screen = ListTreeTestScreen.Show(scene.Tree, 400, 700);
            view = screen.View;
            window = screen.Window;
            scene.ClearUndo();
        }

        private string NewFile(string name)
        {
            var path = Path.Combine(folder, name);
            File.WriteAllText(path, "x");
            return path;
        }

        private DataObject FilesOf(params string[] paths)
        {
            var provider = window!.StorageProvider;
            var items = paths.Select(path => (IStorageItem)provider.TryGetFileFromPathAsync(new Uri(path)).GetAwaiter().GetResult()!).ToArray();
            var data = new DataObject();
            data.Set(DataFormats.Files, items);
            return data;
        }

        private void Drag(Point point, DataObject data, RawDragEventType last)
        {
            var effects = DragDropEffects.Copy | DragDropEffects.Move;
            window!.DragDrop(point, RawDragEventType.DragEnter, data, effects);
            window.DragDrop(point, RawDragEventType.DragOver, data, effects);
            if (last != RawDragEventType.DragOver)
                window.DragDrop(point, last, data, effects);
            Dispatcher.UIThread.RunJobs();
            ListTreeTestScreen.Settle(window);
        }

        private Point RowPoint(TreeNode node)
        {
            var row = Assert.IsType<RowListTree>(Rows.RowOf(node));
            return row.TranslatePoint(new Point(row.Bounds.Width / 2, row.Bounds.Height / 2), window!)!.Value;
        }

        private Point UnderTheRows() => Rows.TranslatePoint(new Point(200, Rows.Layout.TotalHeight + 40), window!)!.Value;

        private TreeNode Entities => Shown.Group<EntitiesGroup>(0);

        private TreeNode TextureModifiers => Shown.LayerNode.Children.First(child => child.Group != null && child.Group is not EntitiesGroup);

        private Composition CompositionAt(int index) => (Composition)Shown.Project.CompositionManager.ManagerData.Items[index];

        [AvaloniaFact]
        public void Drop_FilesUnderTheRowsOfAnEmptyTree_AddsACompositionALayerAndTheEntity()
        {
            Show([]);

            Drag(UnderTheRows(), FilesOf(NewFile("one.png")), RawDragEventType.Drop);

            var layer = (CMiX.Core.Compositing.Layer)CompositionAt(0).LayerManager.ManagerData.Items[0];
            Assert.Single(layer.ModelEntityManager.ManagerData.Items);
        }

        [AvaloniaFact]
        public void Drop_FilesOnACompositionRow_AddsALayerUnderTheComposition()
        {
            Show([1]);

            Drag(RowPoint(Shown.CompositionNode), FilesOf(NewFile("one.png")), RawDragEventType.Drop);

            Assert.Equal(2, CompositionAt(0).LayerManager.ManagerData.Items.Count);
        }

        [AvaloniaFact]
        public void Drop_FilesOnAGroupThatPassesTheDrop_AddsTheEntityToTheLayer()
        {
            Show([1]);
            var before = Shown.Layer.ModelEntityManager.ManagerData.Items.Count;

            Drag(RowPoint(TextureModifiers), FilesOf(NewFile("one.png")), RawDragEventType.Drop);

            Assert.Equal(before + 1, Shown.Layer.ModelEntityManager.ManagerData.Items.Count);
        }

        [AvaloniaFact]
        public void DragOver_FilesOverEachRow_MarksOnlyTheRowThatTakesThem()
        {
            Show([1]);
            var data = FilesOf(NewFile("one.png"));

            Drag(RowPoint(Shown.LayerNode), data, RawDragEventType.DragOver);
            Assert.Same(Shown.LayerNode, Rows.Surfaces.DropTarget);

            Drag(RowPoint(Entities), data, RawDragEventType.DragOver);
            Assert.Same(Entities, Rows.Surfaces.DropTarget);

            Drag(RowPoint(TextureModifiers), data, RawDragEventType.DragOver);
            Assert.Null(Rows.Surfaces.DropTarget);

            Drag(RowPoint(Shown.CompositionNode), data, RawDragEventType.DragOver);
            Assert.Same(Shown.CompositionNode, Rows.Surfaces.DropTarget);

            Drag(UnderTheRows(), data, RawDragEventType.DragOver);
            Assert.Null(Rows.Surfaces.DropTarget);
            Assert.False(Rows.Surfaces.Outline.IsVisible);
        }

        [AvaloniaFact]
        public void DragOver_ACard_FramesTheCardTwoPixelsOutsideInTheKindColour()
        {
            Show([1]);
            var layer = Shown.LayerNode;

            Drag(RowPoint(layer), FilesOf(NewFile("one.png")), RawDragEventType.DragOver);

            var place = Rows.Layout.PlaceOf(ListTreeKeys.Of(layer))!;
            var outline = Rows.Surfaces.Outline;
            Assert.True(outline.IsVisible);
            Assert.Equal(new Rect(-2, place.Y - 2, Rows.Bounds.Width + 4, place.AreaHeight + 4), outline.Bounds);
            Assert.Equal(new Thickness(1.5), outline.BorderThickness);
            var expected = CategoryPalette.FromApplication().ForListTree(layer.Category!.Value, layer.ComposableDepth).DropOutline;
            Assert.Equal(expected, ((ISolidColorBrush)outline.BorderBrush!).Color);
            Assert.Contains(FrameListTreeCard.DropTargetClass, Rows.Surfaces.CardAreaOf(ListTreeKeys.Of(layer))!.Classes);
        }

        [AvaloniaFact]
        public void DragLeave_AfterADragOverARow_TakesTheMarkAway()
        {
            Show([1]);
            var data = FilesOf(NewFile("one.png"));
            Drag(RowPoint(Shown.LayerNode), data, RawDragEventType.DragOver);

            window!.DragDrop(new Point(-10, -10), RawDragEventType.DragLeave, data, DragDropEffects.Copy);
            Dispatcher.UIThread.RunJobs();

            Assert.Null(Rows.Surfaces.DropTarget);
            Assert.False(Rows.Surfaces.Outline.IsVisible);
        }

        [AvaloniaFact]
        public void Drop_FilesThatTheTreeRefuses_AddsNothing()
        {
            Show([]);

            Drag(UnderTheRows(), FilesOf(NewFile("notes.xyz")), RawDragEventType.Drop);

            Assert.Empty(Shown.Project.CompositionManager.ManagerData.Items);
        }

        [AvaloniaFact]
        public void DragOver_FilesThatTheTreeRefusesAtTheBottomEdge_ScrollsNot()
        {
            Show([3]);
            var edge = View.Scroller.TranslatePoint(new Point(200, View.Scroller.Bounds.Height - 4), window!)!.Value;

            Drag(edge, FilesOf(NewFile("notes.xyz")), RawDragEventType.DragOver);

            Assert.False(View.AutoScroll.IsRunning);
        }
    }
}
