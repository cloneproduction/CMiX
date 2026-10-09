// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Reflection;
using System.Text.RegularExpressions;

namespace CMiX.Core.Prefabs
{
    public interface IPrefab : IControl
    {
        PrefabService PrefabService { get; set; }

        public virtual string DisplayName =>
            Regex.Replace(
                Regex.Replace(GetType().Name, @"(\P{Ll})(\P{Ll}\p{Ll})", "$1 $2"),
                @"(\p{Ll})(\P{Ll})", "$1 $2");
    }
}
