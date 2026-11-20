// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Styling;
using Avalonia.Threading;
using CMiX.UI.Behaviors;
using Xunit;

namespace CMiX.UI.Tests
{
    public class ScrollViewerOverlayKeepOffsetTests
    {
        [AvaloniaFact]
        public void Theme_OnAScrollViewerInAWindow_AttachesOneScrollViewerBehavior()
        {
            var viewer = new ScrollViewer { Theme = (ControlTheme)Application.Current!.FindResource("ScrollViewerOverlayKeepOffset")! };
            new Window { Content = viewer, Width = 300, Height = 200 }.Show();
            Dispatcher.UIThread.RunJobs();

            Assert.IsType<ScrollViewerBehavior>(Assert.Single(Avalonia.Xaml.Interactivity.Interaction.GetBehaviors(viewer)));
        }
    }
}
