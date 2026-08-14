// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace CMiX.Studio.Avalonia.Views.Controls
{
    public partial class PrefabListItem : UserControl
    {
        public PrefabListItem()
        {
            InitializeComponent();
        }

        // WPF ImageSource becomes the Avalonia IImage interface.
        public static readonly StyledProperty<IImage> IconProperty =
            AvaloniaProperty.Register<PrefabListItem, IImage>(nameof(Icon));

        public IImage Icon
        {
            get => GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }
    }
}
