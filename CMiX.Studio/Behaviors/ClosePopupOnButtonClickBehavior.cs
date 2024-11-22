// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace CMiX.Studio.Behaviors
{
    public class ClosePopupOnButtonClickBehavior : Behavior<Popup>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            if (AssociatedObject != null)
                AssociatedObject.PreviewMouseLeftButtonUp += Popup_MouseUp;  
        }

        private void Popup_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if(e.OriginalSource is Button)
                AssociatedObject.IsOpen = false;
        }
    }
}
