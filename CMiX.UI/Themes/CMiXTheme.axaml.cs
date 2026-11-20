// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using CMiX.UI.AttachedProperties;

namespace CMiX.UI.Themes
{
    // The theme of the CMiX controls: the tokens (colors, sizes, brushes, converters) and the
    // styles. An application adds one instance to Application.Styles, after its base theme.
    public class CMiXTheme : Styles
    {
        public CMiXTheme()
        {
            // Before the styles are loaded: the tooltips of every control open below it.
            ToolTipDefaults.Apply();
            AvaloniaXamlLoader.Load(this);
        }
    }
}
