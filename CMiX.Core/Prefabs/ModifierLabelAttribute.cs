// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

namespace CMiX.Core.Prefabs
{
    // The name of a modifier in the lists, when it must differ from its class name.
    [AttributeUsage(AttributeTargets.Class)]
    public class ModifierLabelAttribute : Attribute
    {
        public string Label { get; }
        public ModifierLabelAttribute(string label) => Label = label;
    }
}
