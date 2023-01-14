// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using CMiX.Core.Presentation.Extensions;

namespace CMiX.Studio.Behaviors
{
    public class BindableSelectedItemBehavior : Behavior<TreeView>
    {

        public static readonly DependencyProperty SelectedParentProperty =
        DependencyProperty.Register(nameof(SelectedParent), typeof(object), typeof(BindableSelectedItemBehavior), new PropertyMetadata(null));
        public object SelectedParent
        {
            get { return (object)GetValue(SelectedParentProperty); }
            set { SetValue(SelectedParentProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemProperty =
        DependencyProperty.Register("SelectedItem", typeof(object), typeof(BindableSelectedItemBehavior), new UIPropertyMetadata(null, OnSelectedItemChanged));
        public object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }


        private static void OnSelectedItemChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {

            //var item = e.NewValue as TreeViewItem;
            //if (item != null)
            //{
            //    item.SetValue(TreeViewItem.IsSelectedProperty, true);
            //}


        }

        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.SelectedItemChanged += OnTreeViewSelectedItemChanged;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            if (this.AssociatedObject != null)
            {
                this.AssociatedObject.SelectedItemChanged -= OnTreeViewSelectedItemChanged;
            }
        }

        private void OnTreeViewSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            this.SelectedItem = e.NewValue;

            TreeView tv = sender as TreeView;

            var generator = tv.ItemContainerGenerator;
            var tvi = generator.ContainerFromItemRecursive(e.NewValue) as TreeViewItem;

            var parent = tvi.FindParent<TreeViewItem>();


            if(tvi != null)
            {
                if (tvi.IsSelected)
                {
                    SelectedItem = tvi.DataContext;

                    if (parent != null)
                    {
                        SelectedParent = parent.DataContext;
                    }
                    else
                        SelectedParent = null;
                }
            }
        }
    }
}
