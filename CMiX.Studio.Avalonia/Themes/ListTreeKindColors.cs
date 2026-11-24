// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia.Media;

namespace CMiX.Studio.Avalonia.Themes
{
    // The kind colours of a row of the list tree. The borders and the outline are the kind colour with alpha, over what is under them.
    public sealed record ListTreeKindColors(
        Color HoverBorder,
        Color DropOutline,
        Color SelectionBorder);
}
