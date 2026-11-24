using Avalonia;
using CMiX.UI.Controls.Parts;

namespace CMiX.UI.Tests.Support
{
    // Windows no longer gives the mouse to the window of the test. Avalonia does not see it, so its capture stays.
    internal sealed class MouseTakenByAnotherWindow : IDisposable
    {
        private readonly Func<Visual, bool> previous = DragEditHelper.HoldsMouse;

        public MouseTakenByAnotherWindow() => DragEditHelper.HoldsMouse = _ => false;

        public void Dispose() => DragEditHelper.HoldsMouse = previous;
    }
}
