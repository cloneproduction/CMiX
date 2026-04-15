// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CMiX.Studio.Views.BaseControl
{
    public partial class PrefabListItem : UserControl
    {
        public PrefabListItem()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(ImageSource), typeof(PrefabListItem),
                new FrameworkPropertyMetadata(null));

        public ImageSource Icon
        {
            get => (ImageSource)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }
    }
}
