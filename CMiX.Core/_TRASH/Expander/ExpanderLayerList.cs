// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CMiX.Core.Presentation.Controls
{
    //public class ExpanderLayerList : Expander
    //{
    //    private static readonly DependencyPropertyKey IsMouseDoubleClickedPropertyKey = DependencyProperty.RegisterReadOnly(
    //      "IsMouseDoubleClicked",
    //      typeof(Boolean),
    //      typeof(ExpanderLayerList),
    //      new FrameworkPropertyMetadata(false));

    //    public static readonly DependencyProperty IsMouseDoubleClickedProperty = IsMouseDoubleClickedPropertyKey.DependencyProperty;

    //    public Boolean IsMouseDoubleClicked
    //    {
    //        get { return (Boolean)GetValue(IsMouseDoubleClickedProperty); }
    //    }

    //    static ExpanderLayerList()
    //    {
    //        DefaultStyleKeyProperty.OverrideMetadata(typeof(ExpanderLayerList), new FrameworkPropertyMetadata(typeof(ExpanderLayerList)));
    //    }

    //    protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
    //    {
    //        base.OnMouseDoubleClick(e);
    //    }

    //    public override void OnApplyTemplate()
    //    {
    //        base.OnApplyTemplate();
    //        ContentControl contentControl = base.GetTemplateChild("HeaderSite") as ContentControl;
    //        if (contentControl != null)
    //        {
    //            contentControl.AddHandler(ContentControl.MouseDoubleClickEvent, new MouseButtonEventHandler(ExpanderHeader_MouseDoubleClick), true);
    //        }
    //    }

    //    private void ExpanderHeader_MouseDoubleClick(Object sender, MouseButtonEventArgs e)
    //    {
    //        base.SetValue(IsMouseDoubleClickedPropertyKey, !IsMouseDoubleClicked);
    //    }
    //}
}
