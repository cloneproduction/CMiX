using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.UI.Controls.Number;
using CMiX.UI.Interaction;
using CMiX.UI.Tests.Support;
using Xunit;

namespace CMiX.UI.Tests
{
    // The value editors open one drag per gesture, through the InteractionHost.
    public class InteractionHostTests
    {
        private sealed class Counter
        {
            public int Begun;
            public int Ended;
            public object? LastValue;
        }

        private sealed class CountingScope : IDisposable
        {
            private readonly Counter _counter;
            private bool _ended;

            public CountingScope(Counter counter) => _counter = counter;

            // Like the real drag handle, a second dispose of the same handle ends nothing.
            public void Dispose()
            {
                if (_ended)
                    return;

                _ended = true;
                _counter.Ended++;
            }
        }

        private static Counter Install()
        {
            var counter = new Counter();
            InteractionHost.BeginDragHandler = value =>
            {
                counter.Begun++;
                counter.LastValue = value;
                return new CountingScope(counter);
            };
            return counter;
        }

        private static void Press(Window window, Control target) =>
            PointerInput.Click(window, target.TranslatePoint(new Point(target.Bounds.Width / 2, target.Bounds.Height / 2), window)!.Value);

        [AvaloniaFact]
        public void Drag_WithoutAHandler_OpensNoScope()
        {
            var saved = InteractionHost.BeginDragHandler;
            try
            {
                InteractionHost.BeginDragHandler = null;

                Assert.Null(InteractionHost.BeginDrag(null));
            }
            finally
            {
                InteractionHost.BeginDragHandler = saved;
            }
        }

        [AvaloniaFact]
        public void DragValue_Drag_OpensOneScopeAndEndsItOnRelease()
        {
            var saved = InteractionHost.BeginDragHandler;
            try
            {
                var counter = Install();
                var drag = new DragValue { Minimum = 0, Maximum = 10 };
                var window = TestWindow.ShowWithoutClips(drag, 300, 100);
                var border = drag.GetVisualDescendants().OfType<Border>()
                    .Single(b => b.Name == "borderValueDisplay" && b.TemplatedParent == drag);

                Press(window, border);

                Assert.Equal(1, counter.Begun);
                Assert.Equal(1, counter.Ended);
            }
            finally
            {
                InteractionHost.BeginDragHandler = saved;
            }
        }

        [AvaloniaFact]
        public void SliderCMiX_Drag_OpensOneScopeAndEndsItOnRelease()
        {
            var saved = InteractionHost.BeginDragHandler;
            try
            {
                var counter = Install();
                var slider = new SliderCMiX { Minimum = 0, Maximum = 1, Width = 200, Height = 20 };
                var window = TestWindow.ShowWithoutClips(slider, 300, 100);

                Press(window, slider);

                Assert.Equal(1, counter.Begun);
                Assert.Equal(1, counter.Ended);
            }
            finally
            {
                InteractionHost.BeginDragHandler = saved;
            }
        }

        [AvaloniaFact]
        public void Drag_OfAnEditor_HandsItsDataContextToTheHost()
        {
            var saved = InteractionHost.BeginDragHandler;
            try
            {
                var counter = Install();
                var context = new object();
                var drag = new DragValue { Minimum = 0, Maximum = 10, DataContext = context };
                var window = TestWindow.ShowWithoutClips(drag, 300, 100);
                var border = drag.GetVisualDescendants().OfType<Border>()
                    .Single(b => b.Name == "borderValueDisplay" && b.TemplatedParent == drag);

                Press(window, border);

                Assert.Same(context, counter.LastValue);
            }
            finally
            {
                InteractionHost.BeginDragHandler = saved;
            }
        }

        [AvaloniaFact]
        public void Drag_OfAnEditorWithoutDataContext_HandsNull()
        {
            var saved = InteractionHost.BeginDragHandler;
            try
            {
                var counter = Install();
                var drag = new DragValue { Minimum = 0, Maximum = 10 };
                var window = TestWindow.ShowWithoutClips(drag, 300, 100);
                var border = drag.GetVisualDescendants().OfType<Border>()
                    .Single(b => b.Name == "borderValueDisplay" && b.TemplatedParent == drag);

                Press(window, border);

                Assert.Null(counter.LastValue);
                Assert.Equal(1, counter.Begun);
            }
            finally
            {
                InteractionHost.BeginDragHandler = saved;
            }
        }
    }
}
