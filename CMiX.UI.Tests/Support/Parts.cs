using Avalonia;
using Avalonia.Controls;
using Avalonia.VisualTree;

namespace CMiX.UI.Tests.Support
{
    internal static class Parts
    {
        // The first part of the template with the given name.
        public static T Find<T>(Visual root, string name) where T : Control =>
            root.GetVisualDescendants().OfType<T>().First(part => part.Name == name);
    }
}
