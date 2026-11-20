// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using CMiX.UI.Controls.Parts;
using Xunit;

namespace CMiX.UI.Tests
{
    public class LogicalPanelContentTests
    {
        private sealed class PanelHost : Control
        {
            public static readonly StyledProperty<Control?> PanelProperty =
                LogicalPanelContent.Register<PanelHost>(nameof(Panel));
            public Control? Panel
            {
                get => GetValue(PanelProperty);
                set => SetValue(PanelProperty, value);
            }
        }

        [AvaloniaFact]
        public void Register_PanelAssigned_BecomesALogicalChild()
        {
            var host = new PanelHost();
            var first = new Border();
            var second = new Border();

            host.Panel = first;
            var parentOfFirst = first.Parent;
            host.Panel = second;

            Assert.Same(host, parentOfFirst);
            Assert.Null(first.Parent);
            Assert.Same(host, second.Parent);
        }
    }
}
