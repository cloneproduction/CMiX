// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace CMiX.Studio.Avalonia.Views.Controls
{
    // A ToggleButton for a model value. It shows a Reset menu while it has a ResetCommand.
    public class CMiXToggleButton : ToggleButton
    {
        public static readonly StyledProperty<ICommand?> ResetCommandProperty =
            AvaloniaProperty.Register<CMiXToggleButton, ICommand?>(nameof(ResetCommand));
        public ICommand? ResetCommand
        {
            get => GetValue(ResetCommandProperty);
            set => SetValue(ResetCommandProperty, value);
        }

        protected override Type StyleKeyOverride => typeof(ToggleButton);

        private ContextMenu? _resetMenu;
        private bool _attached;

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);
            _attached = true;
            UpdateResetMenu();
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);
            if (_attached && change.Property == ResetCommandProperty)
                UpdateResetMenu();
        }

        private void UpdateResetMenu() => DefaultResetMenu.Update(this, ResetCommand, ref _resetMenu);
    }
}
