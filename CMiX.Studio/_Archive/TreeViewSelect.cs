// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows.Controls;
using System.Windows.Input;

namespace CMiX.Studio.Views.BaseControl
{
    public class TreeViewSelect : TreeView
    {
        public TreeViewSelect()
        {

        }

        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseRightButtonDown(e);
            Keyboard.ClearFocus();
        }
    }
}
