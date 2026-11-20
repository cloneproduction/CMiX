// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using CMiX.UI.Controls.Parts;

namespace CMiX.Studio.Avalonia.Views.Modulation
{
    // The Min and Max of a FloatRange in one row, with an optional caption.
    public partial class EditorFloatRange : UserControlCaptioned
    {
        public EditorFloatRange()
        {
            InitializeComponent();
            UpdateBare();
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);
            if (change.Property == CaptionProperty)
                UpdateBare();
        }

        private void UpdateBare()
        {
            captioned?.Classes.Set("bare", string.IsNullOrEmpty(Caption));
        }
    }
}
