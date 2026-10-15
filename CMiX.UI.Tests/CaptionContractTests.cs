using Avalonia.Controls;
using CMiX.UI.Controls.Number;
using CMiX.UI.Controls.Parts;
using Xunit;

namespace CMiX.UI.Tests
{
    // A control of the library with a Caption must be an ICaptioned. ControlTitle reads the caption through the interface,
    // so a control without it would have a menu without a title and nothing would tell.
    public class CaptionContractTests
    {
        [Fact]
        public void EveryControlWithACaption_IsACaptioned()
        {
            var violations = typeof(DragValue).Assembly.GetExportedTypes()
                .Where(type => typeof(Control).IsAssignableFrom(type))
                .Where(type => type.GetProperty("Caption", typeof(string)) != null)
                .Where(type => !typeof(ICaptioned).IsAssignableFrom(type))
                .Select(type => type.FullName)
                .ToList();

            Assert.True(violations.Count == 0, "These controls have a Caption but are not ICaptioned: " + string.Join(", ", violations));
        }

        [Fact]
        public void TheScan_FindsTheControlsThatItShouldCheck()
        {
            var captioned = typeof(DragValue).Assembly.GetExportedTypes()
                .Where(type => typeof(Control).IsAssignableFrom(type))
                .Where(type => type.GetProperty("Caption", typeof(string)) != null)
                .ToList();

            Assert.Contains(typeof(DragValue), captioned);
            Assert.Contains(typeof(SliderCMiX), captioned);
            Assert.True(captioned.Count >= 8, $"The scan found only {captioned.Count} controls. The scan is broken.");
        }
    }
}
