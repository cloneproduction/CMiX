// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using Avalonia;
using Avalonia.Controls;

namespace CMiX.Studio.Avalonia.Views.Controls
{
    public partial class PrefabListItem : UserControl
    {
        public PrefabListItem()
        {
            InitializeComponent();
        }

        public static readonly StyledProperty<string> IconProperty =
            AvaloniaProperty.Register<PrefabListItem, string>(nameof(Icon));

        public string Icon
        {
            get => GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }
    }
}
