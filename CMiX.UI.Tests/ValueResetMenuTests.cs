using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.UI.AttachedProperties;
using CMiX.UI.Controls.ColorPicker;
using CMiX.UI.Controls.Number;
using CMiX.UI.Controls.Vector;
using CMiX.UI.Tests.Support;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
using Xunit;

namespace CMiX.UI.Tests
{
    // A value editor shows a Reset menu only when it can reset, and the menu must always work: the title,
    // Reset All, the vectors and the range.
    public class ValueResetMenuTests
    {
        // A value editor shows a Reset menu only when it can reset. The menu must always work.

        private static bool HasResetItem(Control c) =>
            c.ContextMenu?.Items.OfType<MenuItem>().Any(i => Equals(i.Header, "Reset")) == true;

        [AvaloniaFact]
        public void ColorPicker_InAWindow_HasNoResetMenu()
        {
            var window = TestWindow.Show(new ColorPickerFull(), 600, 900);

            var withReset = window.GetVisualDescendants().OfType<Control>().Where(HasResetItem).ToList();

            Assert.Empty(withReset);
        }

        [AvaloniaFact]
        public void Slider_WithoutResetCommand_HasNoMenu()
        {
            var slider = new SliderCMiX();
            TestWindow.Show(slider, 600, 900);

            Assert.Null(slider.ContextMenu);
        }

        [AvaloniaFact]
        public void Slider_WithResetCommand_HasAResetMenuThatRunsIt()
        {
            var ran = false;
            var slider = new SliderCMiX { ResetCommand = new RelayCommand(() => ran = true) };
            TestWindow.Show(slider, 600, 900);

            var reset = slider.ContextMenu!.Items.OfType<MenuItem>().Where(i => i.Header is string h && h.StartsWith("Reset")).Single();
            reset.Command!.Execute(null);

            Assert.True(ran);
        }

        [AvaloniaFact]
        public void Slider_LosesTheMenu_WhenTheCommandIsRemoved()
        {
            var slider = new SliderCMiX { ResetCommand = new RelayCommand(() => { }) };
            TestWindow.Show(slider, 600, 900);
            Assert.NotNull(slider.ContextMenu);

            slider.ResetCommand = null!;

            Assert.Null(slider.ContextMenu);
        }

        [AvaloniaFact]
        public void DragValue_WithoutResetCommand_HasNoMenu()
        {
            var drag = new DragValue();
            TestWindow.Show(drag, 600, 900);

            Assert.Null(drag.ContextMenu);
        }

        [AvaloniaFact]
        public void DragValue_WithResetCommand_HasAResetMenuThatRunsIt()
        {
            var ran = false;
            var drag = new DragValue { ResetCommand = new RelayCommand(() => ran = true) };
            TestWindow.Show(drag, 600, 900);

            var reset = drag.ContextMenu!.Items.OfType<MenuItem>().Where(i => i.Header is string h && h.StartsWith("Reset")).Single();
            reset.Command!.Execute(null);

            Assert.True(ran);
        }

        [AvaloniaFact]
        public void DragValue_WithAMenuSetInXaml_KeepsThatMenu()
        {
            var own = new ContextMenu();
            var drag = new DragValue { ContextMenu = own, ResetCommand = new RelayCommand(() => { }) };
            TestWindow.Show(drag, 600, 900);

            Assert.Same(own, drag.ContextMenu);
        }

        // The menu of a value editor starts with the name of the control, then a line, then the rows.

        private static DragValue Show(DragValue box)
        {
            box.ResetCommand = new RelayCommand(() => { });
            TestWindow.Show(box, 300, 60);
            return box;
        }

        private static MenuItem[] Rows(Control box) => box.ContextMenu!.Items.OfType<MenuItem>().ToArray();

        [AvaloniaFact]
        public void Title_ACaption_IsTheCaptionWithoutTheSpacesAtTheEnds()
        {
            var box = Show(new DragValue { Caption = " Position X" });

            var rows = Rows(box);
            Assert.Equal("Position X", rows[0].Header);
            Assert.False(rows[0].IsEnabled);
            Assert.Equal("-", rows[1].Header);
            Assert.Equal("Copy", rows[2].Header);
        }

        [AvaloniaFact]
        public void Title_ACaptionThatIsAPartOfTheName_IsTheControlTitle()
        {
            var box = new DragValue { Caption = "Y" };
            ControlTitle.SetText(box, "Position Y");
            Show(box);

            Assert.Equal("Position Y", Rows(box)[0].Header);
        }

        [AvaloniaFact]
        public void Title_WithoutAName_HidesTheTitleAndTheLine()
        {
            var box = Show(new DragValue());

            var rows = Rows(box);
            Assert.False(rows[0].IsVisible);
            Assert.False(rows[1].IsVisible);
            Assert.True(rows[2].IsVisible);
        }

        [AvaloniaFact]
        public void Open_WithResetAll_EndsWithResetAndResetAll()
        {
            var box = new DragValue { Caption = "Position X", ResetAllCommand = new RelayCommand(() => { }) };
            Show(box);

            Assert.Equal(new object?[] { "Position X", "-", "Copy", "Paste", "-", "Reset", "Reset All" }, Rows(box).Select(r => r.Header).ToArray());
        }

        // Each value of a vector or of a range has a Reset menu. The vectors also have Reset All.

        private class VectorModel : ObservableObject
        {
            public NumberValue X { get; } = new(5);
            public NumberValue Y { get; } = new(6);
            public NumberValue Z { get; } = new(7);
            public int ResetAllRuns { get; private set; }
            public ICommand ResetAllCommand { get; }

            public VectorModel() => ResetAllCommand = new RelayCommand(() => ResetAllRuns++);
        }

        private static List<DragValue> DragValuesOf(Control control) =>
            control.GetVisualDescendants().OfType<DragValue>().ToList();

        // The menu is opened like a right-click does, because the commands of a menu written in XAML bind when it opens.
        private static void Run(DragValue drag, string header)
        {
            ContextMenuInput.Request(drag);
            var row = drag.ContextMenu!.Items.OfType<MenuItem>().Single(i => Equals(i.Header, header));
            row.Command!.Execute(row.CommandParameter);
            drag.ContextMenu.Close();
            Dispatcher.UIThread.RunJobs();
        }

        [AvaloniaFact]
        public void Vector2_EachValue_HasResetAndResetAll()
        {
            var model = new VectorModel();
            var vector = new Vector2 { DataContext = model };
            TestWindow.Show(vector, 600, 400);
            var drags = DragValuesOf(vector);

            Assert.Equal(2, drags.Count);
            Assert.All(drags, d => Assert.Equal(new object?[] { "Reset", "Reset All" }, d.ContextMenu!.Items.OfType<MenuItem>().Where(i => i.Header is string h && h.StartsWith("Reset")).Select(i => i.Header).ToArray()));

            model.X.Value = 1;
            model.Y.Value = 2;
            Run(drags[0], "Reset");
            Assert.Equal(0, model.X.Value);
            Assert.Equal(2, model.Y.Value);

            Run(drags[1], "Reset All");
            Assert.Equal(1, model.ResetAllRuns);
        }

        [AvaloniaFact]
        public void Vector3_EachValue_HasResetAndResetAll()
        {
            var model = new VectorModel();
            var vector = new Vector3 { DataContext = model };
            TestWindow.Show(vector, 600, 400);
            var drags = DragValuesOf(vector);

            Assert.Equal(3, drags.Count);
            Assert.All(drags, d => Assert.Equal(new object?[] { "Reset", "Reset All" }, d.ContextMenu!.Items.OfType<MenuItem>().Where(i => i.Header is string h && h.StartsWith("Reset")).Select(i => i.Header).ToArray()));

            model.Z.Value = 1;
            model.X.Value = 2;
            Run(drags[2], "Reset");
            Assert.Equal(0, model.Z.Value);
            Assert.Equal(2, model.X.Value);

            Run(drags[0], "Reset All");
            Assert.Equal(1, model.ResetAllRuns);
        }

        [AvaloniaFact]
        public void RangeValue_EachSide_HasItsOwnReset()
        {
            var left = 0;
            var right = 0;
            var range = new RangeValue
            {
                LeftResetCommand = new RelayCommand(() => left++),
                RightResetCommand = new RelayCommand(() => right++),
            };
            TestWindow.Show(range, 600, 400);
            var drags = DragValuesOf(range);

            Assert.Equal(2, drags.Count);
            Assert.All(drags, d => Assert.Equal(new object?[] { "Reset" }, d.ContextMenu!.Items.OfType<MenuItem>().Where(i => i.Header is string h && h.StartsWith("Reset")).Select(i => i.Header).ToArray()));

            Run(drags[0], "Reset");
            Assert.Equal((1, 0), (left, right));

            Run(drags[1], "Reset");
            Assert.Equal((1, 1), (left, right));
        }
    }
}
