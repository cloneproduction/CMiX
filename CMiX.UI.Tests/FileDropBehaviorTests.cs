using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.Threading;
using CMiX.UI.Behaviors;
using CMiX.UI.Tests.Support;
using Xunit;

namespace CMiX.UI.Tests
{
    // Files dragged from the file explorer: the behavior finds the paths and reports them.
    public class FileDropBehaviorTests : IDisposable
    {
        private readonly string _folder = Path.Combine(Path.GetTempPath(), "cmix-drop-" + Guid.NewGuid().ToString("N"));
        private readonly string _file;
        private readonly string _subFolder;

        public FileDropBehaviorTests()
        {
            Directory.CreateDirectory(_folder);
            _file = Path.Combine(_folder, "picture.png");
            File.WriteAllText(_file, "x");
            _subFolder = Path.Combine(_folder, "frames");
            Directory.CreateDirectory(_subFolder);
        }

        public void Dispose() => Directory.Delete(_folder, true);

        private static (Window Window, Border Target, FileDropBehavior Drop) Show(FileDropMode mode)
        {
            var target = new Border { Background = Avalonia.Media.Brushes.Gray, Width = 200, Height = 100 };
            var drop = new FileDropBehavior { Mode = mode };
            Avalonia.Xaml.Interactivity.Interaction.GetBehaviors(target).Add(drop);
            var window = new Window { Content = target, Width = 300, Height = 200 };
            window.Show();
            Dispatcher.UIThread.RunJobs();
            return (window, target, drop);
        }

        private static void Drag(Window window, Border target, RawDragEventType type, IDataObject data)
        {
            var point = target.TranslatePoint(new Point(100, 50), window)!.Value;
            // A real drag enters and moves over the control before it drops.
            var effects = DragDropEffects.Copy | DragDropEffects.Move;
            window.DragDrop(point, RawDragEventType.DragEnter, data, effects);
            window.DragDrop(point, RawDragEventType.DragOver, data, effects);
            window.DragDrop(point, type, data, effects);
            Dispatcher.UIThread.RunJobs();
        }

        [AvaloniaFact]
        public void DroppedFiles_AreReportedAsPaths()
        {
            var (window, target, drop) = Show(FileDropMode.Files);
            IReadOnlyList<string>? received = null;
            drop.PathsDropped += (s, e) => received = e.Paths;

            Drag(window, target, RawDragEventType.Drop, FakeStorageItem.Drag(window, _file));

            Assert.NotNull(received);
            Assert.Equal(new[] { _file }, received!);
        }

        [AvaloniaFact]
        public void InFilesMode_AFolderIsIgnored()
        {
            var (window, target, drop) = Show(FileDropMode.Files);
            var calls = 0;
            drop.PathsDropped += (s, e) => calls++;

            Drag(window, target, RawDragEventType.Drop, FakeStorageItem.Drag(window, _subFolder));

            Assert.Equal(0, calls);
        }

        [AvaloniaFact]
        public void InFoldersMode_OnlyTheFoldersAreReported()
        {
            var (window, target, drop) = Show(FileDropMode.Folders);
            IReadOnlyList<string>? received = null;
            drop.PathsDropped += (s, e) => received = e.Paths;

            Drag(window, target, RawDragEventType.Drop, FakeStorageItem.Drag(window, _file, _subFolder));

            Assert.Equal(new[] { _subFolder }, received!);
        }

        [AvaloniaFact]
        public void ADropWithoutFiles_IsIgnored()
        {
            var (window, target, drop) = Show(FileDropMode.Files);
            var calls = 0;
            drop.PathsDropped += (s, e) => calls++;
            var text = new DataObject();
            text.Set(DataFormats.Text, "hello");

            Drag(window, target, RawDragEventType.Drop, text);

            Assert.Equal(0, calls);
        }

        [AvaloniaFact]
        public void TheBehavior_AllowsTheDropOnItsControl()
        {
            var (_, target, _) = Show(FileDropMode.Files);

            Assert.True(DragDrop.GetAllowDrop(target));
        }
    }
}
