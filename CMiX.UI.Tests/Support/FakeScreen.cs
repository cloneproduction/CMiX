using Avalonia;
using CMiX.UI.Controls.Parts;

namespace CMiX.UI.Tests.Support
{
    // A screen as big as the window of the test.
    internal sealed class FakeScreen : IDisposable
    {
        private readonly Func<Visual, PixelPoint, PixelRect?> previous = DragEditHelper.ScreenBoundsAt;

        public FakeScreen(PixelRect bounds) => DragEditHelper.ScreenBoundsAt = (_, _) => bounds;

        public void Dispose() => DragEditHelper.ScreenBoundsAt = previous;
    }
}
