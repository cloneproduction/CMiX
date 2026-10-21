// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Windows.Input;
using Avalonia;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Media;
using CMiX.UI.Controls.Parts;

namespace CMiX.UI.Controls.ColorPicker
{
    public partial class ColorSelector : UserControlCaptioned, IValueText
    {
        public ColorSelector()
        {
            InitializeComponent();

            ContextMenuGuard.Install(this);
            ValueMenu.Follow(this, ResetCommandProperty);
        }

        public static readonly StyledProperty<ICommand?> ResetCommandProperty =
            AvaloniaProperty.Register<ColorSelector, ICommand?>(nameof(ResetCommand));
        public ICommand? ResetCommand
        {
            get => GetValue(ResetCommandProperty);
            set => SetValue(ResetCommandProperty, value);
        }

        // Copy and Paste use the color as #AARRGGBB. A paste also takes #RRGGBB and the name of a color.
        bool IValueText.CanCopyValueText() => true;

        string IValueText.CopyValueText() => ValueText.Format(SelectedColor);

        bool IValueText.CanPasteValueText(string? text) => Color.TryParse(text?.Trim(), out _);

        void IValueText.PasteValueText(string? text)
        {
            if (Color.TryParse(text?.Trim(), out var color))
                SelectedColor = color;
        }

        public static readonly StyledProperty<Color> SelectedColorProperty =
            AvaloniaProperty.Register<ColorSelector, Color>(nameof(SelectedColor), Colors.Yellow, defaultBindingMode: BindingMode.TwoWay);
        public Color SelectedColor
        {
            get => GetValue(SelectedColorProperty);
            set => SetValue(SelectedColorProperty, value);
        }
    }
}
