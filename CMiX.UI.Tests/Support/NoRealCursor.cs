using System.Runtime.CompilerServices;
using CMiX.UI.Controls.Parts;

namespace CMiX.UI.Tests.Support
{
    // The controls move the real cursor when a drag ends. In the tests that would move the cursor of the person who runs them.
    internal static class NoRealCursor
    {
        [ModuleInitializer]
        internal static void Install() => DragEditHelper.CursorPlacer = _ => { };
    }
}
