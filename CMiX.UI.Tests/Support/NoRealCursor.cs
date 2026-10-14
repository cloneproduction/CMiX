using System.Runtime.CompilerServices;
using CMiX.UI.Controls.Parts;

namespace CMiX.UI.Tests.Support
{
    // The controls move the real cursor when a drag ends, and when it reaches the edge of the screen. In the tests that would
    // move the cursor of the person who runs them. The tests have no screen unless a test sets a fake one.
    internal static class NoRealCursor
    {
        [ModuleInitializer]
        internal static void Install()
        {
            DragEditHelper.CursorPlacer = _ => { };
            DragEditHelper.ScreenBoundsAt = (_, _) => null;
        }
    }
}
