// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

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
