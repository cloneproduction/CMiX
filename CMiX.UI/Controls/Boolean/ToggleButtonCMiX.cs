// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using CMiX.UI.Controls.Parts;

namespace CMiX.UI.Controls.Boolean
{
    // A ToggleButton for a model value. It shows a Reset menu while it has a ResetCommand.
    public class ToggleButtonCMiX : ToggleButton
    {
        public static readonly StyledProperty<ICommand?> ResetCommandProperty =
            AvaloniaProperty.Register<ToggleButtonCMiX, ICommand?>(nameof(ResetCommand));
        public ICommand? ResetCommand
        {
            get => GetValue(ResetCommandProperty);
            set => SetValue(ResetCommandProperty, value);
        }

        protected override Type StyleKeyOverride => typeof(ToggleButton);

        public ToggleButtonCMiX()
        {
            ResetMenu.Follow(this, ResetCommandProperty);
        }
    }
}
