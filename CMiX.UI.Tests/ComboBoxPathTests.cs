using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.UI.AttachedProperties;
using CMiX.UI.Controls.Parts;
using CMiX.UI.Controls.Path;
using CMiX.UI.Panels;
using CMiX.UI.Tests.Support;
using Xunit;

namespace CMiX.UI.Tests
{
    // A path selector: the caption, the browse button, the chosen path and the dropped files.
    public class ComboBoxPathTests : IDisposable
    {
        private readonly string _folder = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "cmix-path-" + Guid.NewGuid().ToString("N"));
        private readonly string _file;
        private readonly string _subFolder;

        public ComboBoxPathTests()
        {
            Directory.CreateDirectory(_folder);
            _file = System.IO.Path.Combine(_folder, "picture.png");
            File.WriteAllText(_file, "x");
            _subFolder = System.IO.Path.Combine(_folder, "frames");
            Directory.CreateDirectory(_subFolder);
        }

        public void Dispose() => Directory.Delete(_folder, true);

        private static (Window Window, ComboBoxPath Selector) Show()
        {
            var selector = new ComboBoxPath();
            return (TestWindow.ShowWithoutClips(selector, 400, 100), selector);
        }

        private static void DropOnTheList(Window window, ComboBoxPath selector, IDataObject data)
        {
            var list = selector.GetVisualDescendants().OfType<ComboBox>().Single();
            var point = list.TranslatePoint(new Point(10, list.Bounds.Height / 2), window)!.Value;
            // A real drag enters and moves over the control before it drops.
            window.DragDrop(point, RawDragEventType.DragEnter, data, DragDropEffects.Copy);
            window.DragDrop(point, RawDragEventType.DragOver, data, DragDropEffects.Copy);
            window.DragDrop(point, RawDragEventType.Drop, data, DragDropEffects.Copy);
            Dispatcher.UIThread.RunJobs();
        }

        [AvaloniaFact]
        public void Show_ByDefault_HasTheFilenameCaptionAndSelectsAFile()
        {
            var (_, selector) = Show();

            Assert.Equal("Filename", selector.Caption);
            Assert.False(selector.SelectsFolder);
        }

        [AvaloniaFact]
        public void Show_ACaption_ShowsItInTheLabel()
        {
            var (_, selector) = Show();
            var label = selector.GetVisualDescendants().OfType<ContentCaptioned>().Single();
            Assert.Equal("Filename", label.Caption);

            selector.Caption = "Folder";
            Dispatcher.UIThread.RunJobs();

            Assert.Equal("Folder", label.Caption);
        }

        [AvaloniaFact]
        public void ChoosePath_APath_RaisesPathChosen()
        {
            var (_, selector) = Show();
            string? chosen = null;
            selector.PathChosen += (s, e) => chosen = e.Path;

            selector.ChoosePath(_file);

            Assert.Equal(_file, chosen);
        }

        [AvaloniaFact]
        public void Drop_AFile_RaisesPathsDropped()
        {
            var (window, selector) = Show();
            IReadOnlyList<string>? dropped = null;
            selector.PathsDropped += (s, e) => dropped = e.Paths;

            DropOnTheList(window, selector, FakeStorageItem.Drag(window, _file));

            Assert.Equal(new[] { _file }, dropped!);
        }

        [AvaloniaFact]
        public void AFileSelector_IgnoresADroppedFolder_AndAFolderSelectorTakesIt()
        {
            var (window, selector) = Show();
            IReadOnlyList<string>? dropped = null;
            selector.PathsDropped += (s, e) => dropped = e.Paths;

            DropOnTheList(window, selector, FakeStorageItem.Drag(window, _subFolder));
            Assert.Null(dropped);

            selector.SelectsFolder = true;
            DropOnTheList(window, selector, FakeStorageItem.Drag(window, _subFolder));
            Assert.Equal(new[] { _subFolder }, dropped!);
        }

        [AvaloniaFact]
        public void Show_TheBrowseButton_IsRoundedOnTheRightOnly()
        {
            var selector = new ComboBoxPath();
            TestWindow.Show(selector, 400, 100);

            var browse = selector.GetVisualDescendants().OfType<Button>().First();

            Assert.Equal(ControlPosition.Right, PositionedControl.GetPosition(browse));
        }
    }
}
