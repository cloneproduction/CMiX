using CMiX.UI.Controls.Parts;
using CMiX.UI.Panels;
using CMiX.UI.Themes;
using Xunit;

namespace CMiX.UI.Tests
{
    // Every expander moves the same way: the panel and the chevron take their time from Motion.
    public class MotionTests
    {
        [Avalonia.Headless.XUnit.AvaloniaFact]
        public void Duration_APanelAndAChevron_AreTheDurationOfMotion()
        {
            Assert.Equal(Motion.Duration, new PanelCollapse().Duration);
            Assert.Equal(Motion.Duration, new Chevron().Duration);
        }
    }
}
