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
using CMiX.UI.Tests.Support;
using Xunit;

namespace CMiX.UI.Tests
{
    // An icon has one of two opacities: OpacityIconAction for the icon of a button that does an action, OpacityIconIdle for a quiet icon.
    public class IconOpacityTests
    {
        private static T Show<T>(T control) where T : Control
        {
            TestWindow.Show(new StackPanel { HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left, Children = { control } }, 300, 200);
            return control;
        }

        private static AppIcon IconOf(Control control) => control.GetVisualDescendants().OfType<AppIcon>().First();

        [AvaloniaFact]
        public void Show_AButtonIcon_HasTheActionOpacity()
        {
            var button = Show(new ButtonIcon { IconKey = "Add" });

            Assert.Equal(Tokens.Get<double>("OpacityIconAction"), IconOf(button).Opacity);
        }

        [AvaloniaFact]
        public void Show_AWindowButtonIcon_HasTheIdleOpacityAndTheLargeSize()
        {
            var button = Show(new ButtonIcon { IconKey = "Close", Classes = { "window" } });

            Assert.Equal(Tokens.Get<double>("OpacityIconIdle"), IconOf(button).Opacity);
            Assert.Equal(Tokens.Get<double>("ControlHeightXL"), button.Bounds.Width);
            Assert.Equal(Tokens.Get<double>("ControlHeightXL"), button.Bounds.Height);
        }

        [AvaloniaFact]
        public void Show_AHandleButtonIcon_HasTheHandCursor()
        {
            var button = Show(new ButtonIcon { IconKey = "DragHandle", Classes = { "handle" } });

            Assert.NotNull(button.Cursor);
            Assert.Equal(Tokens.Get<double>("OpacityIconAction"), IconOf(button).Opacity);
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

            Assert.Equal(Tokens.Get<double>("OpacityIconAction"), IconOf(button).Opacity);
        }

        [AvaloniaFact]
        public void Show_AListItemIcon_HasTheIdleOpacity()
        {
            var item = Show(new ListItemIcon { Icon = "Folder", Title = "Item" });

            Assert.Equal(Tokens.Get<double>("OpacityIconIdle"), IconOf(item).Opacity);
        }

        [AvaloniaFact]
        public void Show_TheIconOfATextBoxSearch_HasTheIdleOpacity()
        {
            var box = Show(new TextBoxSearch { Width = 160 });

            Assert.Equal(Tokens.Get<double>("OpacityIconIdle"), IconOf(box).Opacity);
        }
    }
}
