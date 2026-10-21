using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.UI.AttachedProperties;
using CMiX.UI.Controls.Number;
using CMiX.UI.Controls.Parts;
using CommunityToolkit.Mvvm.Input;
using Xunit;

namespace CMiX.UI.Tests
{
    // A menu that opens at the pointer puts the row that was used last under the pointer, as in Blender.
    public class MenuAnchorTests
    {
        private static DragValue Box(bool withResetAll = false) => new()
        {
            Caption = "Position X",
            ResetCommand = new RelayCommand(() => { }),
            ResetAllCommand = withResetAll ? new RelayCommand(() => { }) : null
        };

        private static Window ShowIn(params Control[] editors)
        {
            var panel = new StackPanel();
            foreach (var editor in editors)
                panel.Children.Add(editor);

            var window = new Window { Content = panel, Width = 600, Height = 400, FontSize = 12.5 };
            window.Show();
            Dispatcher.UIThread.RunJobs();
            return window;
        }

        // The menu opens through a context request, as it does for the user. A call to Open does not raise Opening.
        private static PopupFrame Open(Control editor)
        {
            editor.ContextMenu!.Close();
            editor.RaiseEvent(new ContextRequestedEventArgs());
            Dispatcher.UIThread.RunJobs();
            return editor.ContextMenu.GetVisualDescendants().OfType<PopupFrame>().Single();
        }

        private static void Use(Control editor, string header)
        {
            var row = editor.ContextMenu!.Items.OfType<MenuItem>().Single(item => Equals(item.Header, header));
            row.RaiseEvent(new RoutedEventArgs(MenuItem.ClickEvent));
            Dispatcher.UIThread.RunJobs();
        }

        private static MenuItem RowOf(Control editor, string header) =>
            editor.ContextMenu!.Items.OfType<MenuItem>().Single(item => Equals(item.Header, header));

        // The vertical offset that puts the middle of the row under the pointer.
        private static double OffsetOf(Control editor, PopupFrame frame, string header)
        {
            var row = editor.ContextMenu!.Items.OfType<MenuItem>().Single(item => Equals(item.Header, header));
            var middle = row.TranslatePoint(new Point(row.Bounds.Width / 2, row.Bounds.Height / 2), frame)!.Value;
            return -middle.Y;
        }

        [AvaloniaFact]
        public void TheFirstTime_CopyIsUnderThePointer()
        {
            var box = Box();
            ShowIn(box);

            var frame = Open(box);

            Assert.Equal(OffsetOf(box, frame, "Copy"), frame.PointerOffset(frame.Bounds.Size).Y);
            Assert.Equal(-frame.Bounds.Width / 2, frame.PointerOffset(frame.Bounds.Size).X);
        }

        [AvaloniaFact]
        public void TheRowUsedLast_IsUnderThePointer_OnAnotherEditorOfTheWindow()
        {
            var first = Box();
            var second = Box();
            ShowIn(first, second);
            Open(first);

            Use(first, "Reset");
            var frame = Open(second);

            Assert.Equal(OffsetOf(second, frame, "Reset"), frame.PointerOffset(frame.Bounds.Size).Y);
        }

        [AvaloniaFact]
        public void ARowThatTheMenuDoesNotHave_FallsBackToCopy()
        {
            var withAll = Box(withResetAll: true);
            var plain = Box();
            var window = ShowIn(withAll, plain);
            Open(withAll);

            Use(withAll, "Reset All");
            Assert.Equal(MenuAnchor.GetId(RowOf(withAll, "Reset All")), MenuAnchor.GetLastUsed(window));
            var frame = Open(plain);

            Assert.Equal(OffsetOf(plain, frame, "Copy"), frame.PointerOffset(frame.Bounds.Size).Y);
        }

        [AvaloniaFact]
        public void TheMemory_BelongsToTheWindow()
        {
            var first = Box();
            var second = Box();
            ShowIn(first);
            var otherWindow = new Window { Content = second, Width = 300, Height = 200, FontSize = 12.5 };
            otherWindow.Show();
            Dispatcher.UIThread.RunJobs();
            Open(first);
            Use(first, "Paste");

            var frame = Open(second);

            Assert.Equal(OffsetOf(second, frame, "Copy"), frame.PointerOffset(frame.Bounds.Size).Y);
        }

        [AvaloniaFact]
        public void ARowIsKnownByItsId_NotByItsLabel()
        {
            var first = Box();
            var second = Box();
            ShowIn(first, second);
            Open(first);
            Use(first, "Reset");

            // The label of the row in the other menu is not the same, as in a translation.
            RowOf(second, "Reset").Header = "Réinitialiser";
            var frame = Open(second);

            Assert.Equal(OffsetOf(second, frame, "Réinitialiser"), frame.PointerOffset(frame.Bounds.Size).Y);
        }

        [AvaloniaFact]
        public void ResetAllIsRecognizedById_AfterItsLabelChanged()
        {
            var box = Box(withResetAll: true);
            ShowIn(box);
            var resetAllRow = RowOf(box, "Reset All");
            resetAllRow.Header = "Tout réinitialiser";

            box.ResetAllCommand = null;
            Dispatcher.UIThread.RunJobs();

            Assert.DoesNotContain(resetAllRow, box.ContextMenu!.Items.OfType<MenuItem>());
        }

        [AvaloniaFact]
        public void WithoutAnAnchor_TheMiddleOfTheFrameIsUnderThePointer()
        {
            var frame = new PopupFrame { Content = new TextBlock { Text = "Item" } };
            new Window { Content = frame, Width = 300, Height = 200 }.Show();
            Dispatcher.UIThread.RunJobs();

            Assert.Equal(-frame.Bounds.Height / 2, frame.PointerOffset(frame.Bounds.Size).Y);
        }
    }
}
