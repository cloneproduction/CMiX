using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Threading;
using CMiX.UI.Controls.Enum;
using CMiX.UI.Controls.Number;
using CommunityToolkit.Mvvm.Input;
using Xunit;

namespace CMiX.UI.Tests
{
    // A right-click on the leading or the trailing content of an editor, such as the assign button,
    // is not for the editor. Its Reset menu must not open for it.
    public class SideContentContextMenuTests
    {
        private static Control Editor(string kind, Button side) => kind switch
        {
            "drag-trailing" => new DragValue { ResetCommand = new RelayCommand(() => { }), TrailingContent = side },
            "drag-leading" => new DragValue { ResetCommand = new RelayCommand(() => { }), LeadingContent = side },
            "slider-trailing" => new SliderCMiX { ResetCommand = new RelayCommand(() => { }), TrailingContent = side },
            _ => new ComboBoxCaptioned { ResetCommand = new RelayCommand(() => { }), TrailingContent = side }
        };

        private static Control Show(string kind, Button side)
        {
            var editor = Editor(kind, side);
            editor.Width = 400;
            editor.HorizontalAlignment = HorizontalAlignment.Left;
            editor.VerticalAlignment = VerticalAlignment.Top;
            new Window { Content = editor, Width = 600, Height = 200, FontSize = 12.5 }.Show();
            Dispatcher.UIThread.RunJobs();
            return editor;
        }

        private static void Request(Control source)
        {
            source.RaiseEvent(new ContextRequestedEventArgs { Source = source });
            Dispatcher.UIThread.RunJobs();
        }

        [AvaloniaTheory]
        [InlineData("drag-trailing")]
        [InlineData("drag-leading")]
        [InlineData("slider-trailing")]
        [InlineData("combo-trailing")]
        public void ARightClickOnTheSideContent_OpensNoMenu(string kind)
        {
            var side = new Button { Content = "side", Width = 30 };
            var editor = Show(kind, side);

            Request(side);

            Assert.False(editor.ContextMenu!.IsOpen);
        }

        [AvaloniaTheory]
        [InlineData("drag-trailing")]
        [InlineData("drag-leading")]
        [InlineData("slider-trailing")]
        [InlineData("combo-trailing")]
        public void ARightClickOnTheEditorItself_StillOpensTheMenu(string kind)
        {
            var editor = Show(kind, new Button { Content = "side", Width = 30 });

            Request(editor);

            Assert.True(editor.ContextMenu!.IsOpen);
        }
    }
}
