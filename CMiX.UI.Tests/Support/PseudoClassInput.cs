using Avalonia.Controls;
using Avalonia.VisualTree;
using Avalonia.Threading;

namespace CMiX.UI.Tests.Support
{
    // The headless pointer does not reach every part, so a test sets the pseudo class by hand.
    internal static class PseudoClassInput
    {
        public static void Set(Control control, string pseudoClass, bool on = true) =>
            ((IPseudoClasses)control.Classes).Set(pseudoClass, on);

        // Sets the pseudo class on the root and on every control in its template.
        public static void Force(Control root, string pseudoClass)
        {
            foreach (var part in root.GetSelfAndVisualDescendants().OfType<Control>())
                Set(part, pseudoClass);
            Dispatcher.UIThread.RunJobs();
        }
    }
}
