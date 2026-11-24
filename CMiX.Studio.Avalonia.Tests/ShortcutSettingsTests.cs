using System.IO;
using Avalonia.Input;
using CMiX.Studio.Avalonia.Services;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.ViewModels;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // The view model of the Shortcuts section, against files in a temp folder only.
    public class ShortcutSettingsTests : IDisposable
    {
        private readonly TempFolder temp = new("shortcuts", create: false);

        private string FilePath => temp.PathOf(ShortcutMap.FileName);

        public void Dispose() => temp.Dispose();

        private (ShortcutMap Map, ShortcutSettings Settings) Create()
        {
            var map = new ShortcutMap(FilePath);
            return (map, new ShortcutSettings(map));
        }

        private static ViewModels.ShortcutRow Row(ShortcutSettings settings, string name) =>
            settings.Scopes.SelectMany(scope => scope.Rows).Single(row => row.Name == name);

        [Fact]
        public void Rows_ByDefault_ListEveryNameOfTheMap()
        {
            var (map, settings) = Create();

            Assert.Equal(new[] { "Edit", "File", "Window", "Tree" }, settings.Scopes.Select(scope => scope.Name));
            Assert.Equal(map.Names, settings.Scopes.SelectMany(scope => scope.Rows).Select(row => row.Name));
            Assert.Equal("Redo (alternative)", Row(settings, "Edit.RedoAlt").Label);
            Assert.Equal("Open project", Row(settings, "File.Open").Label);
            Assert.Equal("Move down", Row(settings, "Tree.MoveDown").Label);
            Assert.Equal("Next panel", Row(settings, "Window.NextPanel").Label);
            Assert.Equal("Focus the tree", Row(settings, "Tree.Focus").Label);
        }

        [Fact]
        public void Gesture_SetOnARow_ChangesTheMapAndSaves()
        {
            var (map, settings) = Create();
            map.Load();
            var before = File.ReadAllText(FilePath);
            var gesture = new KeyGesture(Key.R, KeyModifiers.Control | KeyModifiers.Shift);

            Row(settings, "Tree.Rename").Gesture = gesture;

            Assert.Equal(gesture, map["Tree.Rename"]);
            Assert.NotEqual(before, File.ReadAllText(FilePath));
            var copy = new ShortcutMap(FilePath);
            copy.Load();
            Assert.Equal(gesture, copy["Tree.Rename"]);
            Assert.Null(settings.SaveError);
        }

        [Fact]
        public void Gesture_SetWithNoFile_CreatesTheFile()
        {
            var (_, settings) = Create();
            Assert.False(File.Exists(FilePath));

            Row(settings, "Edit.Undo").Gesture = new KeyGesture(Key.U, KeyModifiers.Control);

            Assert.True(File.Exists(FilePath));
        }

        [Fact]
        public void IsConflict_TwoRowsWithOneGesture_MarksBoth()
        {
            var (_, settings) = Create();
            var rename = Row(settings, "Tree.Rename");
            var remove = Row(settings, "Tree.Remove");
            var changed = new List<string?>();
            rename.PropertyChanged += (_, e) => changed.Add(e.PropertyName);

            rename.Gesture = new KeyGesture(Key.Delete);

            Assert.True(rename.IsConflict);
            Assert.True(remove.IsConflict);
            Assert.False(Row(settings, "Tree.Open").IsConflict);
            Assert.Contains("Remove", rename.ConflictText);
            Assert.Contains("Rename", remove.ConflictText);
            Assert.Null(Row(settings, "Tree.Open").ConflictText);
            Assert.Contains(nameof(rename.IsConflict), changed);
        }

        [Fact]
        public void ConflictText_TreeGestureOnUndo_NamesTheGlobalRow()
        {
            var (_, settings) = Create();
            var rename = Row(settings, "Tree.Rename");
            var undo = Row(settings, "Edit.Undo");

            rename.Gesture = new KeyGesture(Key.Z, KeyModifiers.Control);

            Assert.True(rename.IsConflict);
            Assert.True(undo.IsConflict);
            Assert.Contains(ShortcutSettings.LabelOf("Edit.Undo"), rename.ConflictText);
            Assert.Contains(ShortcutSettings.LabelOf("Tree.Rename"), undo.ConflictText);
        }

        [Fact]
        public void ResetAll_AfterChanges_RestoresTheDefaults()
        {
            var (map, settings) = Create();
            Row(settings, "Edit.Undo").Gesture = null;
            Row(settings, "Tree.Open").Gesture = new KeyGesture(Key.O);

            settings.ResetAllCommand.Execute(null);

            Assert.Equal(new KeyGesture(Key.Z, KeyModifiers.Control), map["Edit.Undo"]);
            Assert.Equal(new KeyGesture(Key.Enter), Row(settings, "Tree.Open").Gesture);
            var copy = new ShortcutMap(FilePath);
            copy.Load();
            Assert.Equal(new KeyGesture(Key.Enter), copy["Tree.Open"]);
        }

        [Fact]
        public void ResetCommand_OfARow_RestoresItsDefault()
        {
            var (map, settings) = Create();
            var row = Row(settings, "Tree.Duplicate");
            row.Gesture = null;

            row.ResetCommand.Execute(null);

            Assert.Equal(new KeyGesture(Key.D, KeyModifiers.Shift), map["Tree.Duplicate"]);
        }

        [WindowsFact("Only Windows refuses to write a file that another stream holds with FileShare.None.")]
        public void SaveError_WhenTheFolderIsReadOnlyOrTheFileIsLocked_KeepsTheChangeAndSetsTheError()
        {
            var (map, settings) = Create();
            map.Load();
            var gesture = new KeyGesture(Key.R, KeyModifiers.Control | KeyModifiers.Alt);

            using (new FileStream(FilePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                Row(settings, "Tree.Rename").Gesture = gesture;

            Assert.Equal(gesture, map["Tree.Rename"]);
            Assert.False(string.IsNullOrEmpty(settings.SaveError));

            Row(settings, "Tree.Open").Gesture = new KeyGesture(Key.O);

            Assert.Null(settings.SaveError);
        }
    }
}
