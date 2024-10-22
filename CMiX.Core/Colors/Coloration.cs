// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Colors
{
    public partial class Coloration : IControl, IModifiable
    {
        public Coloration(PrefabManager colorPaletteManager, PrefabManager modifierManager)
        {
            ColorPaletteManager = colorPaletteManager;
            ModifierManager = modifierManager;
        }

        public Guid ID { get; set; }
        public PrefabManager ColorPaletteManager { get; set; }
        public PrefabManager ModifierManager { get; set; }
    }
}
