// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Xaml.Interactivity;
using CMiX.Core.Modulation.Modulators;
using CMiX.UI.Controls.Collection;

namespace CMiX.Studio.Avalonia.Behaviors
{
    // Tells the modulator of a PanelItem while the pointer is over the panel.
    // A PanelItem with another model, such as a modifier, is left alone.
    public class ModulatorHoverBehavior : Behavior<PanelItem>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject!.PropertyChanged += OnPropertyChanged;
        }

        protected override void OnDetaching()
        {
            AssociatedObject!.PropertyChanged -= OnPropertyChanged;
            base.OnDetaching();
        }

        private void OnPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.Property == PanelItem.IsHoveredProperty && AssociatedObject!.DataContext is IModulator modulator)
                modulator.IsHovered = (bool)e.NewValue!;
        }
    }
}
