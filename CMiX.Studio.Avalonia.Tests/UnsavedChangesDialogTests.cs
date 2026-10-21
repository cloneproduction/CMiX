using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Studio.Avalonia.Services;
using CMiX.Studio.Avalonia.Views.Dialogs;
using CMiX.UI.Controls;
using CMiX.UI.Controls.Parts;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // The question about the unsaved changes: the frame of the menus, a warning, the text, and three buttons.
    public class UnsavedChangesDialogTests
    {
        private static Window Owner()
        {
            var owner = new Window { Width = 600, Height = 400 };
            owner.Show();
            Dispatcher.UIThread.RunJobs();
            return owner;
        }

        private static Button ButtonNamed(Control dialog, string name) =>
            dialog.GetVisualDescendants().OfType<Button>().Single(button => button.Name == name);

        [AvaloniaFact]
        public void TheDialog_ShowsTheWarningTheQuestionAndThreeButtons()
        {
            var dialog = new UnsavedChangesDialog();
            dialog.Show(Owner());
            Dispatcher.UIThread.RunJobs();

            var texts = dialog.GetVisualDescendants().OfType<TextBlock>().Select(block => block.Text).ToList();
            Assert.Contains("Save change before closing ?", texts);
            Assert.Equal("Save", ButtonNamed(dialog, "SaveButton").Content);
            Assert.Equal("Don't Save", ButtonNamed(dialog, "DontSaveButton").Content);
            Assert.Equal("Cancel", ButtonNamed(dialog, "CancelButton").Content);
            Assert.Equal("Warning", dialog.GetVisualDescendants().OfType<AppIcon>().Single().IconKey);
        }

        [AvaloniaFact]
        public void TheDialog_UsesTheFrameOfTheMenusAndFlyouts()
        {
            var dialog = new UnsavedChangesDialog();
            dialog.Show(Owner());
            Dispatcher.UIThread.RunJobs();

            Assert.Single(dialog.GetVisualDescendants().OfType<PopupFrame>());
        }

        [AvaloniaTheory]
        [InlineData("SaveButton", UnsavedChangesChoice.Save)]
        [InlineData("DontSaveButton", UnsavedChangesChoice.DontSave)]
        [InlineData("CancelButton", UnsavedChangesChoice.Cancel)]
        public async Task EachButton_ClosesTheDialogWithItsChoice(string buttonName, UnsavedChangesChoice expected)
        {
            var dialog = new UnsavedChangesDialog();
            var answer = dialog.ShowDialog<UnsavedChangesChoice>(Owner());
            Dispatcher.UIThread.RunJobs();

            ButtonNamed(dialog, buttonName).RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            Dispatcher.UIThread.RunJobs();

            Assert.Equal(expected, await answer);
        }

        [AvaloniaFact]
        public async Task Escape_IsCancel()
        {
            var dialog = new UnsavedChangesDialog();
            var answer = dialog.ShowDialog<UnsavedChangesChoice>(Owner());
            Dispatcher.UIThread.RunJobs();

            dialog.RaiseEvent(new KeyEventArgs { RoutedEvent = InputElement.KeyDownEvent, Key = Key.Escape, Source = dialog });
            Dispatcher.UIThread.RunJobs();

            Assert.Equal(UnsavedChangesChoice.Cancel, await answer);
        }

        [AvaloniaFact]
        public async Task ClosingTheDialogWithoutAnAnswer_IsCancel()
        {
            var dialog = new UnsavedChangesDialog();
            var answer = dialog.ShowDialog<UnsavedChangesChoice>(Owner());
            Dispatcher.UIThread.RunJobs();

            dialog.Close();
            Dispatcher.UIThread.RunJobs();

            Assert.Equal(UnsavedChangesChoice.Cancel, await answer);
        }
    }
}
