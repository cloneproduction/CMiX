// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;

namespace CMiX.Core.Transformation.Modifiers
{
    public class GridModel : IPrefabModel
    {
        public GridModel()
        {
            ID = Guid.NewGuid();
            PrefabService = new PrefabServiceModel();
            Count = new Integer3Model(1, 1, 1);
            Width = new Vector3Model(0, 0, 0);
            Phase = new Vector3Model(0, 0, 0);
            ModifierModeSelector = new ModifierModeSelectorModel();
            ModifierModeSelector.Count.Value = 1;
            ModifierModeSelector.Mode.Value = ModifierMode.ToSpread;
        }

        public Guid ID { get; set; }

        public ModifierModeSelectorModel ModifierModeSelector { get; set; }
        public PrefabServiceModel PrefabService { get; set; }
        public Integer3Model Count { get; set; }
        public Vector3Model Width { get; set; }
        public Vector3Model Phase { get; set; }
    }
}
