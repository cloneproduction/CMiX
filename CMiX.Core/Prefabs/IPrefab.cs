// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

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
