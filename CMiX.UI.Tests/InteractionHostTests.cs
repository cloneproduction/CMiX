using System;
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

        // The headless renderer cannot hit test through a geometry clip, such as the rounded
        // clip of a BorderMask. Without this the pointer is never over the editor.
        private static Window Show(Control control)
        {
            var window = new Window { Content = control, Width = 300, Height = 100 };
            window.Show();
            Dispatcher.UIThread.RunJobs();
            foreach (var visual in window.GetVisualDescendants().Where(v => v.Clip != null))
                visual.Clip = null;
            Dispatcher.UIThread.RunJobs();
            return window;
        }

        private static void Press(Window window, Control target)
        {
            var center = target.TranslatePoint(new Point(target.Bounds.Width / 2, target.Bounds.Height / 2), window)!.Value;
            window.MouseMove(center);
            Dispatcher.UIThread.RunJobs();
            window.MouseDown(center, MouseButton.Left);
            window.MouseUp(center, MouseButton.Left);
            Dispatcher.UIThread.RunJobs();
        }

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
                var window = Show(drag);
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
        public void CMiXSlider_OpensOneScopeAndEndsItOnRelease()
        {
            var saved = InteractionHost.BeginScopeHandler;
            try
            {
                var counter = Install();
                var slider = new CMiXSlider { Minimum = 0, Maximum = 1, Width = 200, Height = 20 };
                var window = Show(slider);

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
