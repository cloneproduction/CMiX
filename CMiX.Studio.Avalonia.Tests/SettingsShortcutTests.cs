using Avalonia;
using Avalonia.Controls;
using Ellipse = Avalonia.Controls.Shapes.Ellipse;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Studio.Avalonia.Settings;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.ViewModels;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views;
using CMiX.UI.Controls.String;
using CMiX.UI.Panels;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // The Shortcuts section of the Settings tab.
    public class SettingsShortcutTests : IDisposable
    {
        private readonly TempFolder temp = new("shortcuts", create: false);

        public void Dispose() => temp.Dispose();

        private (ShortcutMap Map, ShortcutSettings Settings, SettingsShortcut View) Show()
        {
            var map = new ShortcutMap(temp.PathOf(ShortcutMap.FileName));
            var settings = new ShortcutSettings(map);
            var view = new SettingsShortcut { DataContext = settings };
            new Window { Content = view, Width = 400, Height = 600 }.Show();
            Dispatcher.UIThread.RunJobs();
            return (map, settings, view);
        }

        private static TextBoxKeyGesture Box(Control view, string name) =>
            view.GetVisualDescendants().OfType<TextBoxKeyGesture>().Single(box => box.DataContext is ShortcutRow row && row.Name == name);

        [AvaloniaFact]
        public void Show_TheView_ShowsOneTextBoxKeyGesturePerRow()
        {
            var (map, _, view) = Show();

            var boxes = view.GetVisualDescendants().OfType<TextBoxKeyGesture>().ToList();

            Assert.Equal(map.Names, boxes.Select(box => ((ShortcutRow)box.DataContext!).Name));
            Assert.Equal(map["Tree.Rename"], Box(view, "Tree.Rename").Gesture);
        }

        [AvaloniaFact]
        public void Chord_PressedInABox_ChangesTheTreeBinding()
        {
            var (_, window, viewModel) = TestServiceProviderFactory.ShowMainWindow();
            TestServiceProviderFactory.SelectMainTab(window, "Settings");
            var expander = window.GetVisualDescendants().OfType<ExpanderIndented>().Single(e => Equals(e.Header, "Shortcuts"));
            expander.IsExpanded = true;
            TestServiceProviderFactory.Pump();
            var box = Box(window, "Tree.Rename");
            box.Focus();
            TestServiceProviderFactory.Pump();
            Assert.True(box.IsFocused);

            window.KeyPress(Key.R, RawInputModifiers.Control | RawInputModifiers.Shift, PhysicalKey.None, null);
            TestServiceProviderFactory.Pump();

            var gesture = new KeyGesture(Key.R, KeyModifiers.Control | KeyModifiers.Shift);
            Assert.Equal(gesture, viewModel.Shortcuts["Tree.Rename"]);
            // The key of the rename is a binding of the main window (see ListTreeWindowKeysTests).
            var rename = window.KeyBindings.Single(binding => ReferenceEquals(binding.Command, viewModel.ProjectTree.RenameSelectedByKeyCommand));
            Assert.Equal(gesture, rename.Gesture);
        }

        [AvaloniaFact]
        public void Show_AConflictRow_ShowsTheMark()
        {
            var (map, _, view) = Show();
            map.Set("Tree.Rename", new KeyGesture(Key.Delete));
            Dispatcher.UIThread.RunJobs();

            var marks = view.GetVisualDescendants().OfType<Ellipse>().Where(e => e.Name == "conflictMark" && e.IsVisible).ToList();

            Assert.Equal(new[] { "Tree.Rename", "Tree.Remove" }, marks.Select(mark => ((ShortcutRow)mark.DataContext!).Name));
            var expected = ((ISolidColorBrush)Application.Current!.FindResource("WarningMutedBrush")!).Color;
            Assert.All(marks, mark => Assert.Equal(expected, ((ISolidColorBrush)mark.Fill!).Color));
            Assert.Contains("Remove", ToolTip.GetTip(Box(view, "Tree.Rename").GetVisualAncestors().OfType<CMiX.UI.Controls.Parts.ContentCaptioned>().First()) as string);
        }

        [AvaloniaFact]
        public void Show_TheResetAllButton_HoldsTheResetAllCommand()
        {
            var (_, settings, view) = Show();

            var button = view.GetVisualDescendants().OfType<Button>().Single(b => b.Name == "resetAllButton");

            Assert.Same(settings.ResetAllCommand, button.Command);
        }

        [AvaloniaFact]
        public void ResetMenu_OfABox_HoldsTheResetCommand()
        {
            var (_, _, view) = Show();
            var box = Box(view, "Tree.Open");
            var row = (ShortcutRow)box.DataContext!;

            var reset = box.ContextMenu!.Items.OfType<MenuItem>().Single(item => Equals(item.Header, "Reset"));

            Assert.Same(row.ResetCommand, box.ResetCommand);
            Assert.Same(row.ResetCommand, reset.Command);
        }

        [AvaloniaFact]
        public void SaveError_AfterAFailedSave_ShowsTheTextInTheDangerColor()
        {
            var (_, settings, view) = Show();
            var text = view.GetVisualDescendants().OfType<TextBlock>().Single(t => t.Name == "saveErrorText");
            Assert.False(text.IsVisible);
            // A file in the place of the folder makes each save fail.
            File.WriteAllText(temp.Path, string.Empty);

            Box(view, "Tree.Open").Gesture = new KeyGesture(Key.O);
            Dispatcher.UIThread.RunJobs();

            Assert.NotNull(settings.SaveError);
            Assert.True(text.IsVisible);
            Assert.Equal(settings.SaveError, text.Text);
            Assert.Equal(((ISolidColorBrush)Application.Current!.FindResource("PopupCardDangerBrush")!).Color, ((ISolidColorBrush)text.Foreground!).Color);
        }
    }
}
