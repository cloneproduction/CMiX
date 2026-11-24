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
    // The value editors open one interaction scope per gesture, through the InteractionHost.
    public class InteractionHostTests
    {
        private sealed class Counter
        {
            public int Begun;
            public int Ended;
        }

        private sealed class CountingScope : IDisposable
        {
            private readonly Counter _counter;
            private bool _ended;

            public CountingScope(Counter counter) => _counter = counter;

            // Like the real scope, a second dispose of the same handle ends nothing.
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
            InteractionHost.BeginScopeHandler = () =>
            {
                counter.Begun++;
                return new CountingScope(counter);
            };
            return counter;
        }

        private static void Press(Window window, Control target) =>
            PointerInput.Click(window, target.TranslatePoint(new Point(target.Bounds.Width / 2, target.Bounds.Height / 2), window)!.Value);

        [AvaloniaFact]
        public void WithoutAHandler_NoScopeIsOpened()
        {
            var saved = InteractionHost.BeginScopeHandler;
            try
            {
                InteractionHost.BeginScopeHandler = null;

                Assert.Null(InteractionHost.BeginScope());
            }
            finally
            {
                InteractionHost.BeginScopeHandler = saved;
            }
        }

        [AvaloniaFact]
        public void DragValue_OpensOneScopeAndEndsItOnRelease()
        {
            var saved = InteractionHost.BeginScopeHandler;
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
                InteractionHost.BeginScopeHandler = saved;
            }
        }

        [AvaloniaFact]
        public void SliderCMiX_OpensOneScopeAndEndsItOnRelease()
        {
            var saved = InteractionHost.BeginScopeHandler;
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
                InteractionHost.BeginScopeHandler = saved;
            }
        }
    }
}
