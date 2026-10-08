// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using Avalonia.Controls;

namespace CMiX.Studio.Avalonia.Views.Controls
{
    // The frame of every popup and menu: shadow, rounded translucent panel and padding.
    public class PopupFrame : ContentControl
    {
        protected override Type StyleKeyOverride => typeof(PopupFrame);
    }
}
