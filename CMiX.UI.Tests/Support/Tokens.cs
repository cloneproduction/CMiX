using Avalonia;
using Xunit;

namespace CMiX.UI.Tests
{
    // The sizes of the tests come from the same tokens as the controls, so no test holds a height as a number.
    public static class Tokens
    {
        public static T Get<T>(string key)
        {
            var application = Application.Current!;

            Assert.True(application.TryGetResource(key, application.ActualThemeVariant, out var value), $"The token {key} is missing.");
            return (T)value!;
        }

        public static double RowHeight => Get<double>("ControlHeightMD");

        // The space above and below the rows of a ListBoxCMiX.
        public static double ListMargin => Get<Thickness>("ListScrollMargin").Top + Get<Thickness>("ListScrollMargin").Bottom;

        // The height of a ListBoxCMiX that shows the given number of rows.
        public static double ListHeight(int rows) => rows * RowHeight + ListMargin;

        public static int ListVisibleItems => Get<int>("ListVisibleItems");
    }
}
