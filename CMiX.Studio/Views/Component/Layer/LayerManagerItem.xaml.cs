// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CMiX.Studio.Views
{
    /// <summary>
    /// Interaction logic for LayerManagerItem.xaml
    /// </summary>
    public partial class LayerManagerItem : UserControl
    {
        public LayerManagerItem()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty DragHandlerIsPressedProperty =
        DependencyProperty.Register("DragHandlerIsPressed", typeof(bool), typeof(LayerManagerItem), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public bool DragHandlerIsPressed
        {
            get { return (bool)GetValue(DragHandlerIsPressedProperty); }
            set { SetValue(DragHandlerIsPressedProperty, value); }
        }

        private void Button_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragHandlerIsPressed = true;
        }

        private void Button_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            DragHandlerIsPressed = false;
        }
    }
}
