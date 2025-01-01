// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Colors.Modifiers
{
    public interface IColorableEntity
    {
        public PrefabManager ColorPaletteManager { get; set; }
    }
}
