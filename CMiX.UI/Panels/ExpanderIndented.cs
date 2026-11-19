// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;

namespace CMiX.UI.Panels
{
    public class ExpanderIndented : Expander
    {
        static ExpanderIndented()
        {
            // A panel is open when it appears. The user closes what is not needed.
            IsExpandedProperty.OverrideDefaultValue<ExpanderIndented>(true);
        }

        public static readonly StyledProperty<int> IndentationProperty =
            AvaloniaProperty.Register<ExpanderIndented, int>(nameof(Indentation), 0, defaultBindingMode: BindingMode.TwoWay);
        public int Indentation
        {
            get => GetValue(IndentationProperty);
            set => SetValue(IndentationProperty, value);
        }

        public static readonly DirectProperty<ExpanderIndented, Thickness> IndentationMarginProperty =
            AvaloniaProperty.RegisterDirect<ExpanderIndented, Thickness>(nameof(IndentationMargin), o => o.IndentationMargin);
        private Thickness _indentationMargin;
        public Thickness IndentationMargin
        {
            get => _indentationMargin;
            private set => SetAndRaise(IndentationMarginProperty, ref _indentationMargin, value);
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);
            if (change.Property == IndentationProperty)
                IndentationMargin = new Thickness(Indentation, 0, 0, 0);
        }
    }
}
