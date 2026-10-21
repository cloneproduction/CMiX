using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.UI.Controls.Number;
using CMiX.UI.Controls.Vector;
using CMiX.UI.Tests.Support;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Xunit;

namespace CMiX.UI.Tests
{
    // Each value of a vector or of a range has a Reset menu. The vectors also have Reset All.
    public class VectorResetMenuTests
    {
        private class VectorModel : ObservableObject
        {
            public NumberValue X { get; } = new(5);
            public NumberValue Y { get; } = new(6);
            public NumberValue Z { get; } = new(7);
            public int ResetAllRuns { get; private set; }
            public ICommand ResetAllCommand { get; }

            public VectorModel() => ResetAllCommand = new RelayCommand(() => ResetAllRuns++);
        }

        private static Window Host(Control content)
        {
            var window = new Window { Content = content, Width = 600, Height = 400 };
            window.Show();
            Dispatcher.UIThread.RunJobs();
            return window;
        }

        private static List<DragValue> DragValuesOf(Control control) =>
            control.GetVisualDescendants().OfType<DragValue>().ToList();

        // The menu is opened like a right-click does, because the commands of a menu written in XAML bind when it opens.
        private static void Run(DragValue drag, string header)
        {
            drag.RaiseEvent(new ContextRequestedEventArgs { Source = drag });
            Dispatcher.UIThread.RunJobs();
            var row = drag.ContextMenu!.Items.OfType<MenuItem>().Single(i => Equals(i.Header, header));
            row.Command!.Execute(row.CommandParameter);
            drag.ContextMenu.Close();
            Dispatcher.UIThread.RunJobs();
        }

        [AvaloniaFact]
        public void Vector2_EachValueHasResetAndResetAll()
        {
            var model = new VectorModel();
            var vector = new Vector2 { DataContext = model };
            Host(vector);
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
        public void Vector3_EachValueHasResetAndResetAll()
        {
            var model = new VectorModel();
            var vector = new Vector3 { DataContext = model };
            Host(vector);
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
        public void RangeValue_EachSideHasItsOwnReset()
        {
            var left = 0;
            var right = 0;
            var range = new RangeValue
            {
                LeftResetCommand = new RelayCommand(() => left++),
                RightResetCommand = new RelayCommand(() => right++),
            };
            Host(range);
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
