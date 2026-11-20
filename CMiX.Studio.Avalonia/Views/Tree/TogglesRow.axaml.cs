// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia.Controls;

namespace CMiX.Studio.Avalonia.Views
{
    // The visibility toggle first, then the mask toggle.
    public partial class TogglesRow : UserControl
    {
        public const int Slots = 2;

        public TogglesRow()
        {
            InitializeComponent();
        }

        public static double WidthFor(double rowHeight, double spacing) => Slots * (rowHeight + spacing);
    }
}
