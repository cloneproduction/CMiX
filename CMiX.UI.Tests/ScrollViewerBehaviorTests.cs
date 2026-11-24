// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Styling;
using Avalonia.Threading;
using CMiX.UI.Behaviors;
using CMiX.UI.Tests.Support;
using Xunit;

namespace CMiX.UI.Tests
{
    // The theme ScrollViewerOverlayKeepOffset attaches ScrollViewerBehavior, which keeps the offset when the content grows.
    public class ScrollViewerBehaviorTests
    {
        [AvaloniaFact]
        public void Theme_OnAScrollViewerInAWindow_AttachesOneScrollViewerBehavior()
        {
            var viewer = new ScrollViewer { Theme = Tokens.Theme("ScrollViewerOverlayKeepOffset") };
            TestWindow.Show(viewer, 300, 200);

            Assert.IsType<ScrollViewerBehavior>(Assert.Single(Avalonia.Xaml.Interactivity.Interaction.GetBehaviors(viewer)));
        }
    }
}
