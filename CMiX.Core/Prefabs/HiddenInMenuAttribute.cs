// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

namespace CMiX.Core.Prefabs
{
    // A type with this attribute is not offered by the add menus. Saved projects can still hold it.
    [AttributeUsage(AttributeTargets.Class)]
    public class HiddenInMenuAttribute : Attribute
    {
    }
}
