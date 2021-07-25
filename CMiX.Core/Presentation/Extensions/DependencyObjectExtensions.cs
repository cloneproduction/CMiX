// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CMiX.Core.Presentation.Extensions
{
    public static class DependencyObjectExtensions
    {
        public static TreeViewItem ContainerFromItemRecursive(this ItemContainerGenerator root, object item)
        {
            var treeViewItem = root.ContainerFromItem(item) as TreeViewItem;
            if (treeViewItem != null)
                return treeViewItem;
            foreach (var subItem in root.Items)
            {
                treeViewItem = root.ContainerFromItem(subItem) as TreeViewItem;
                var search = treeViewItem?.ItemContainerGenerator.ContainerFromItemRecursive(item);
                if (search != null)
                    return search;
            }
            return null;
        }

        public static T FindParent<T>(this DependencyObject dependencyObject) where T : DependencyObject
        {
            if (dependencyObject != null)
            {
                var parent = VisualTreeHelper.GetParent(dependencyObject);

                if (parent == null)
                    return null;

                if (parent is T)
                    return parent as T;
                else
                    return FindParent<T>(parent);
            }
            else
                return null;
        }


        public static IEnumerable<T> FindVisualChildren<T>(this DependencyObject dependencyObject) where T : DependencyObject
        {
            if (dependencyObject != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(dependencyObject); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(dependencyObject, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }


        public static T FindVisualAncestor<T>(this DependencyObject dependencyObject) where T : DependencyObject
        {
            var parent = VisualTreeHelper.GetParent(dependencyObject.FindVisualTreeRoot());

            while (parent != null)
            {
                if (parent is T)
                {
                    return (T)parent;
                }

                parent = VisualTreeHelper.GetParent(parent);
            }

            return null;
        }


        public static DependencyObject FindVisualTreeRoot(this DependencyObject obj)
        {
            var current = obj;
            var result = obj;

            while (current != null)
            {
                result = current;
                if (current is Visual)//|| current is Visual3D)
                {
                    break;
                }

                // If the current item is not a visual, try to walk up the logical tree.
                current = LogicalTreeHelper.GetParent(current);
            }

            return result;
        }
    }
}
