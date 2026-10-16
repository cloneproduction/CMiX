// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using CMiX.Core.Prefabs;

namespace CMiX.Studio.Avalonia.Views
{
    public class ModifierEntry
    {
        public required Type Type { get; set; }
        public string Label => (Type != null ? ControlFactory.StringHelper.LabelOf(Type) : null)
                               ?? ControlFactory.StringHelper.PascalCaseToDisplay(Type?.Name ?? "");
    }
}
