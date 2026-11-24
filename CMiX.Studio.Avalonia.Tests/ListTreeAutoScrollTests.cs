// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.Platform.Storage;
using Avalonia.VisualTree;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.ViewModels.ListTree;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views;
using Xunit;
using static CMiX.Studio.Avalonia.Tests.Support.TreeFixtures;

namespace CMiX.Studio.Avalonia.Tests
{
    // The scroll of the tree at its top and bottom edges, during a drag of a row and during a drag of files.
    public class ListTreeAutoScrollTests : IDisposable
    {
        private const double PressX = 120;

        private readonly string folder = Path.Combine(Path.GetTempPath(), "cmix-list-tree-scroll-" + Guid.NewGuid().ToString("N"));
        private readonly Scene scene;
        private readonly Window window;
        private readonly ListTreeProject view;

        // Every card is open, and the window shows only a part of the rows.
        public ListTreeAutoScrollTests()
        {
            Directory.CreateDirectory(folder);
            scene = CreateScene([2, 1], s => AddKinds(s.Layer));
            TestServiceProviderFactory.ExpandAll(scene.Tree);
            var screen = ListTreeTestScreen.Show(scene.Tree, 400, 300);
            view = screen.View;
            window = screen.Window;
        }

        public void Dispose()
        {
            window.Close();
            Directory.Delete(folder, true);
        }

        private PanelListTreeRows Panel => view.Rows;

        private ScrollViewer Scroller => view.GetVisualDescendants().OfType<ScrollViewer>().First(scroller => scroller.Name == "scroller");

        private TreeNode Entity(int index) => scene.Group<EntitiesGroup>(0).Children[index];

        private Point InScroller(double y) => Scroller.TranslatePoint(new Point(PressX, y), window)!.Value;

        private void StartDrag()
        {
            TreeDrag.Render(window);
            var y = Panel.Layout.PlaceOf(ListTreeKeys.Of(Entity(0)))!.Y + 16;
            var point = Panel.TranslatePoint(new Point(PressX, y), window)!.Value;
            window.MouseDown(point, MouseButton.Left);
            TreeDrag.MoveTo(window, point + new Vector(0, 8));
        }

        private DataObject FileData()
        {
            var path = Path.Combine(folder, "one.png");
            File.WriteAllText(path, "x");
            var data = new DataObject();
            data.Set(DataFormats.Files, new[] { (IStorageItem)window.StorageProvider.TryGetFileFromPathAsync(new Uri(path)).GetAwaiter().GetResult()! });
            return data;
        }

        [AvaloniaFact]
        public void ScrollStep_ADragNearTheBottomEdge_ScrollsDown()
        {
            StartDrag();

            TreeDrag.MoveTo(window, InScroller(Scroller.Bounds.Height - 4));
            var step = view.AutoScroll.ScrollStep();

            Assert.True(view.AutoScroll.IsRunning);
            Assert.True(step > 0);
            Assert.Equal(step, Scroller.Offset.Y);
        }

        [AvaloniaFact]
        public void ScrollStep_ADragNearTheTopEdge_ScrollsUp()
        {
            Scroller.Offset = new Vector(0, 100);
            ListTreeTestScreen.Settle(window);
            StartDrag();

            TreeDrag.MoveTo(window, InScroller(4));
            var step = view.AutoScroll.ScrollStep();

            Assert.True(step < 0);
        }

        [AvaloniaFact]
        public void Wheel_DuringADrag_ScrollsTheTreeAndKeepsTheDrag()
        {
            StartDrag();
            var point = InScroller(Scroller.Bounds.Height / 2);
            TreeDrag.MoveTo(window, point);

            window.MouseWheel(point, new Vector(0, -1));
            ListTreeTestScreen.Settle(window);

            Assert.True(Scroller.Offset.Y > 0);
            Assert.True(view.RowDrag.IsDragging);
        }

        [AvaloniaFact]
        public void ScrollStep_ADragInTheMiddle_DoesNotScroll()
        {
            StartDrag();

            TreeDrag.MoveTo(window, InScroller(Scroller.Bounds.Height / 2));

            Assert.Equal(0, view.AutoScroll.ScrollStep());
        }

        [AvaloniaFact]
        public void ScrollStep_UnderAStillPointer_PicksAgainAtTheNewPlace()
        {
            StartDrag();
            TreeDrag.MoveTo(window, InScroller(Scroller.Bounds.Height - 4));
            var before = view.RowDrag.Drag!.Look!.Candidate.Place.Y;

            for (var i = 0; i < 20; i++)
                view.AutoScroll.ScrollStep();
            ListTreeTestScreen.Settle(window);

            Assert.True(view.RowDrag.Drag!.Look!.Candidate.Place.Y > before);
        }

        [AvaloniaFact]
        public void Release_AfterADragAtTheEdge_StopsTheScroll()
        {
            StartDrag();
            var edge = InScroller(Scroller.Bounds.Height - 4);
            TreeDrag.MoveTo(window, edge);

            TreeDrag.Release(window, edge);

            Assert.False(view.AutoScroll.IsRunning);
        }

        [AvaloniaFact]
        public void DragOver_FilesNearTheBottomEdge_ScrollsUntilTheDrop()
        {
            var data = FileData();
            var edge = InScroller(Scroller.Bounds.Height - 4);
            var effects = DragDropEffects.Copy | DragDropEffects.Move;

            window.DragDrop(edge, RawDragEventType.DragEnter, data, effects);
            window.DragDrop(edge, RawDragEventType.DragOver, data, effects);
            var running = view.AutoScroll.IsRunning;
            var step = view.AutoScroll.ScrollStep();
            window.DragDrop(edge, RawDragEventType.Drop, data, effects);

            Assert.True(running);
            Assert.True(step > 0);
            Assert.False(view.AutoScroll.IsRunning);
        }
    }
}
