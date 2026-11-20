using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.UI.Controls;
using CMiX.UI.Controls.Boolean;
using CMiX.UI.Controls.Collection;
using CMiX.UI.Controls.Number;
using CMiX.UI.Controls.Parts;
using Xunit;

namespace CMiX.UI.Tests
{
    // An icon has one of two opacities: OpacityIconAction for the icon of a button that does an action, OpacityIconIdle for a quiet icon.
    public class IconOpacityTests
    {
        private static double Token(string key) => (double)Application.Current!.FindResource(key)!;

        private static T Show<T>(T control) where T : Control
        {
            new Window { Content = new StackPanel { HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left, Children = { control } }, Width = 300, Height = 200 }.Show();
            Dispatcher.UIThread.RunJobs();
            return control;
        }

        private static AppIcon IconOf(Control control) => control.GetVisualDescendants().OfType<AppIcon>().First();

        [AvaloniaFact]
        public void Show_AButtonIcon_HasTheActionOpacity()
        {
            var button = Show(new ButtonIcon { IconKey = "Add" });

            Assert.Equal(Token("OpacityIconAction"), IconOf(button).Opacity);
        }

        [AvaloniaFact]
        public void Show_AWindowButtonIcon_HasTheIdleOpacityAndTheLargeSize()
        {
            var button = Show(new ButtonIcon { IconKey = "Close", Classes = { "window" } });

            Assert.Equal(Token("OpacityIconIdle"), IconOf(button).Opacity);
            Assert.Equal(Token("ControlHeightXL"), button.Bounds.Width);
            Assert.Equal(Token("ControlHeightXL"), button.Bounds.Height);
        }

        [AvaloniaFact]
        public void Show_AHandleButtonIcon_HasTheHandCursor()
        {
            var button = Show(new ButtonIcon { IconKey = "DragHandle", Classes = { "handle" } });

            Assert.NotNull(button.Cursor);
            Assert.Equal(Token("OpacityIconAction"), IconOf(button).Opacity);
        }

        [AvaloniaTheory]
        [InlineData("SubButton")]
        [InlineData("AddButton")]
        public void Show_AStepButtonOfADragValue_HasTheActionOpacity(string name)
        {
            var drag = Show(new DragValue { Minimum = 0, Maximum = 1, Value = 0.5, Width = 160 });
            var button = drag.GetVisualDescendants().OfType<Button>().First(part => part.Name == name);
            button.IsVisible = true;
            Dispatcher.UIThread.RunJobs();

            Assert.Equal(Token("OpacityIconAction"), IconOf(button).Opacity);
        }

        [AvaloniaFact]
        public void Show_AListItemIcon_HasTheIdleOpacity()
        {
            var item = Show(new ListItemIcon { Icon = "Folder", Title = "Item" });

            Assert.Equal(Token("OpacityIconIdle"), IconOf(item).Opacity);
        }

        [AvaloniaFact]
        public void Show_TheIconOfATextBoxSearch_HasTheIdleOpacity()
        {
            var box = Show(new TextBoxSearch { Width = 160 });

            Assert.Equal(Token("OpacityIconIdle"), IconOf(box).Opacity);
        }
    }
}
