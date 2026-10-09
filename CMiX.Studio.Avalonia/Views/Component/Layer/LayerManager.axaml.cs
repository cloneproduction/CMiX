// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using Avalonia;
using Avalonia.Controls;

namespace CMiX.Studio.Avalonia.Views
{
    public partial class LayerManager : UserControl
    {
        public static readonly StyledProperty<Type?> ItemTypeProperty =
            AvaloniaProperty.Register<LayerManager, Type?>(nameof(ItemType));

        public Type? ItemType
        {
            get => GetValue(ItemTypeProperty);
            set => SetValue(ItemTypeProperty, value);
        }

        public LayerManager()
        {
            InitializeComponent();
        }
    }
}
