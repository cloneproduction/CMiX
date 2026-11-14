using System.IO;
using Avalonia.Input;
using CMiX.Studio.Avalonia.Services;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // The shortcut map against files in a temp folder only. The file of the user is never touched.
    public class ShortcutMapTests : IDisposable
    {
        private readonly string folder = Path.Combine(Path.GetTempPath(), $"cmix-shortcuts-{Guid.NewGuid():N}");

        private string FilePath => Path.Combine(folder, ShortcutMap.FileName);

        public void Dispose()
        {
            if (Directory.Exists(folder))
                Directory.Delete(folder, true);
        }

        private void WriteFile(string json)
        {
            Directory.CreateDirectory(folder);
            File.WriteAllText(FilePath, json);
        }

        [Fact]
        public void Names_ByDefault_ListEveryNameWithAGesture()
        {
            var map = new ShortcutMap(FilePath);

            Assert.Equal(
                new[] { "Edit.Undo", "Edit.Redo", "Edit.RedoAlt", "File.Open", "Window.NextPanel", "Window.PreviousPanel", "Window.FocusPanel1",
                    "Window.FocusPanel2", "Window.FocusPanel3", "Window.FocusPanel4", "Tree.Focus", "Tree.Add", "Tree.Inspect", "Tree.Menu", "Tree.Expand", "Tree.CollapseOthers", "Tree.ExpandBranch", "Tree.CloseLevel", "Tree.OpenLevel", "Tree.CollapseAll", "Tree.Search", "Tree.ClearSearch", "Tree.Rename", "Tree.Remove", "Tree.Duplicate", "Tree.MoveUp", "Tree.MoveDown", "Tree.Open", "Tree.Reveal", "Tree.Hide", "Tree.ShowAll", "Tree.Grab" },
                map.Names);
            // Tree.Focus has no default: Ctrl+1 to Ctrl+4 go to the panels.
            foreach (var name in map.Names.Where(name => name != "Tree.Focus"))
                Assert.NotNull(map[name]);
            Assert.Null(map["Tree.Focus"]);
            Assert.Equal(new KeyGesture(Key.Tab, KeyModifiers.Control), map["Window.NextPanel"]);
            Assert.Equal(new KeyGesture(Key.Tab, KeyModifiers.Control | KeyModifiers.Shift), map["Window.PreviousPanel"]);
            Assert.Equal(new KeyGesture(Key.D1, KeyModifiers.Control), map["Window.FocusPanel1"]);
            Assert.Equal(new KeyGesture(Key.D4, KeyModifiers.Control), map["Window.FocusPanel4"]);

            Assert.Equal(new KeyGesture(Key.Z, KeyModifiers.Control | KeyModifiers.Shift), map["Edit.Redo"]);
            Assert.Equal(new KeyGesture(Key.Up, KeyModifiers.Alt), map["Tree.MoveUp"]);
            Assert.Equal(new KeyGesture(Key.A, KeyModifiers.Shift), map["Tree.Add"]);
            Assert.Equal(new KeyGesture(Key.O, KeyModifiers.Control), map["File.Open"]);
            Assert.Equal(new KeyGesture(Key.W), map["Tree.Menu"]);
            Assert.Equal(new KeyGesture(Key.E), map["Tree.Expand"]);
            Assert.Equal(new KeyGesture(Key.E, KeyModifiers.Alt), map["Tree.CollapseOthers"]);
            Assert.Equal(new KeyGesture(Key.E, KeyModifiers.Shift), map["Tree.ExpandBranch"]);
            Assert.Equal(new KeyGesture(Key.Q), map["Tree.CloseLevel"]);
            Assert.Equal(new KeyGesture(Key.Q, KeyModifiers.Shift), map["Tree.OpenLevel"]);
            Assert.Equal(new KeyGesture(Key.E, KeyModifiers.Shift | KeyModifiers.Alt), map["Tree.CollapseAll"]);
            Assert.Equal(new KeyGesture(Key.F, KeyModifiers.Control), map["Tree.Search"]);
            Assert.Equal(new KeyGesture(Key.F, KeyModifiers.Alt), map["Tree.ClearSearch"]);
            Assert.Equal(new KeyGesture(Key.D, KeyModifiers.Shift), map["Tree.Duplicate"]);
            Assert.Equal(new KeyGesture(Key.H), map["Tree.Hide"]);
            Assert.Equal(new KeyGesture(Key.H, KeyModifiers.Alt), map["Tree.ShowAll"]);
        }

        [Fact]
        public void Conflicts_ByDefault_IsEmpty()
        {
            Assert.Empty(new ShortcutMap(FilePath).Conflicts);
        }

        [Fact]
        public void Load_AFileEntry_WinsOverTheDefault()
        {
            WriteFile("{ \"Shortcuts\": { \"Tree.Rename\": \"Ctrl+Shift+R\" } }");
            var map = new ShortcutMap(FilePath);

            map.Load();

            Assert.Equal(new KeyGesture(Key.R, KeyModifiers.Control | KeyModifiers.Shift), map["Tree.Rename"]);
            Assert.Equal(new KeyGesture(Key.Z, KeyModifiers.Control), map["Edit.Undo"]);
        }

        // A file from an earlier version holds every default of that version. An old default gives the default of now.
        [Fact]
        public void Load_ARetiredDefault_GivesTheDefaultOfNow()
        {
            WriteFile("{ \"Shortcuts\": { \"Tree.Duplicate\": \"Ctrl+D\", \"Tree.Rename\": \"F2\", \"Tree.Remove\": \"Ctrl+Shift+R\" } }");
            var map = new ShortcutMap(FilePath);

            map.Load();

            Assert.Equal(new KeyGesture(Key.D, KeyModifiers.Shift), map["Tree.Duplicate"]);
            Assert.Equal(new KeyGesture(Key.R, KeyModifiers.Control), map["Tree.Rename"]);
            Assert.Equal(new KeyGesture(Key.R, KeyModifiers.Control | KeyModifiers.Shift), map["Tree.Remove"]);
        }

        [Fact]
        public void Load_ABrokenEntry_KeepsTheDefault()
        {
            WriteFile("{ \"Shortcuts\": { \"Tree.Rename\": \"Ctrl+NoSuchKey\" } }");
            var map = new ShortcutMap(FilePath);

            map.Load();

            Assert.Equal(new KeyGesture(Key.R, KeyModifiers.Control), map["Tree.Rename"]);
        }

        [Fact]
        public void Load_ABrokenFile_KeepsTheDefaults()
        {
            WriteFile("{ not json");
            var map = new ShortcutMap(FilePath);

            map.Load();

            Assert.Equal(new KeyGesture(Key.R, KeyModifiers.Control), map["Tree.Rename"]);
            Assert.Equal("{ not json", File.ReadAllText(FilePath));
        }

        [Fact]
        public void Load_AnUnknownName_IsIgnored()
        {
            WriteFile("{ \"Shortcuts\": { \"Tree.NoSuchCommand\": \"Ctrl+R\" } }");
            var map = new ShortcutMap(FilePath);

            map.Load();

            Assert.DoesNotContain("Tree.NoSuchCommand", map.Names);
            Assert.Null(map["Tree.NoSuchCommand"]);
            Assert.Empty(map.Conflicts);
        }

        [Fact]
        public void Load_AnEmptyEntry_RemovesTheGesture()
        {
            WriteFile("{ \"Shortcuts\": { \"Tree.Open\": \"\" } }");
            var map = new ShortcutMap(FilePath);

            map.Load();

            Assert.Null(map["Tree.Open"]);
        }

        [Fact]
        public void Load_AMissingFile_WritesTheDefaults()
        {
            var map = new ShortcutMap(FilePath);

            map.Load();

            Assert.True(File.Exists(FilePath));
            var copy = new ShortcutMap(FilePath);
            copy.Set("Edit.Undo", null);
            copy.Load();
            foreach (var name in map.Names)
                Assert.Equal(map[name], copy[name]);
        }

        // KeyGesture.Parse alone reads "1" as the key value 1 and "3" as Tab.
        [Fact]
        public void Load_ADigitKey_GivesTheNumberKey()
        {
            WriteFile("{ \"Shortcuts\": { \"Window.FocusPanel1\": \"Ctrl+1\", \"Window.FocusPanel3\": \"Ctrl+3\", \"Window.FocusPanel4\": \"Ctrl+Shift+1\", \"Tree.Focus\": \"Ctrl+D2\" } }");
            var map = new ShortcutMap(FilePath);

            map.Load();

            Assert.Equal(new KeyGesture(Key.D1, KeyModifiers.Control), map["Window.FocusPanel1"]);
            Assert.Equal(new KeyGesture(Key.D3, KeyModifiers.Control), map["Window.FocusPanel3"]);
            Assert.Equal(new KeyGesture(Key.D1, KeyModifiers.Control | KeyModifiers.Shift), map["Window.FocusPanel4"]);
            Assert.Equal(new KeyGesture(Key.D2, KeyModifiers.Control), map["Tree.Focus"]);
        }

        [Fact]
        public void Save_ADigitKey_WritesTheDFormAndLoadsItBack()
        {
            WriteFile("{ \"Shortcuts\": { \"Tree.Focus\": \"Ctrl+5\" } }");
            var map = new ShortcutMap(FilePath);
            map.Load();

            map.Save();

            using (var json = System.Text.Json.JsonDocument.Parse(File.ReadAllText(FilePath)))
                Assert.Equal("Ctrl+D5", json.RootElement.GetProperty("Shortcuts").GetProperty("Tree.Focus").GetString());
            var copy = new ShortcutMap(FilePath);
            copy.Load();
            Assert.Equal(new KeyGesture(Key.D5, KeyModifiers.Control), copy["Tree.Focus"]);
        }

        [Fact]
        public void Save_AfterSet_WritesTheChange()
        {
            var map = new ShortcutMap(FilePath);
            map.Set("Tree.Duplicate", new KeyGesture(Key.D, KeyModifiers.Control | KeyModifiers.Shift));

            map.Save();

            var copy = new ShortcutMap(FilePath);
            copy.Load();
            Assert.Equal(new KeyGesture(Key.D, KeyModifiers.Control | KeyModifiers.Shift), copy["Tree.Duplicate"]);
        }

        [Fact]
        public void Set_AGesture_RaisesTheChangeOfTheIndexer()
        {
            var map = new ShortcutMap(FilePath);
            var names = new List<string?>();
            map.PropertyChanged += (_, e) => names.Add(e.PropertyName);

            map.Set("Edit.Undo", new KeyGesture(Key.U, KeyModifiers.Control));

            Assert.Contains("Item[]", names);
        }

        [Fact]
        public void Reset_AfterSet_GivesTheDefault()
        {
            var map = new ShortcutMap(FilePath);
            map.Set("Edit.Undo", null);
            map.Set("Edit.Redo", null);

            map.Reset("Edit.Undo");
            Assert.Equal(new KeyGesture(Key.Z, KeyModifiers.Control), map["Edit.Undo"]);
            Assert.Null(map["Edit.Redo"]);

            map.ResetAll();
            Assert.Equal(new KeyGesture(Key.Z, KeyModifiers.Control | KeyModifiers.Shift), map["Edit.Redo"]);
        }

        [Fact]
        public void Conflicts_TwoNamesWithOneGesture_ListsThem()
        {
            var map = new ShortcutMap(FilePath);

            map.Set("Tree.Rename", new KeyGesture(Key.Delete));
            Assert.Equal(new[] { "Tree.Remove", "Tree.Rename" }, map.Conflicts);
        }

        [Fact]
        public void Conflicts_TreeGestureOnUndo_ReportsTheConflict()
        {
            var map = new ShortcutMap(FilePath);

            map.Set("Tree.Rename", new KeyGesture(Key.Z, KeyModifiers.Control));

            Assert.Equal(new[] { "Edit.Undo", "Tree.Rename" }, map.Conflicts);
        }

        [Fact]
        public void Conflicts_TreeGestureOnNextPanel_ReportsTheConflict()
        {
            var map = new ShortcutMap(FilePath);

            map.Set("Tree.Rename", new KeyGesture(Key.Tab, KeyModifiers.Control));

            Assert.True(ShortcutMap.IsGlobalScope("Window"));
            Assert.Equal(new[] { "Tree.Rename", "Window.NextPanel" }, map.Conflicts);
        }

        [Fact]
        public void ConflictsOf_TwoScopesThatAreNotGlobal_MayShareAGesture()
        {
            var gesture = new KeyGesture(Key.R);
            var entries = new Dictionary<string, KeyGesture?>
            {
                ["Tree.Rename"] = gesture,
                ["Timeline.Record"] = gesture,
            };

            Assert.Empty(ShortcutMap.ConflictsOf(entries));

            entries["File.Recent"] = gesture;
            Assert.Equal(new[] { "File.Recent", "Timeline.Record", "Tree.Rename" }, ShortcutMap.ConflictsOf(entries));
        }
    }
}
