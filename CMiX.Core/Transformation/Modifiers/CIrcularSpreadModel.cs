// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;

namespace CMiX.Core.Transformation.Modifiers
{
    public class CircularSpreadModel : IPrefabModel
    {
        public CircularSpreadModel()
        {
            ID = Guid.NewGuid();
            PrefabService = new PrefabServiceModel();
            Width = new Vector2Model(1.0f, 1.0f);
            Phase = new GenericValueModel<float>();
            Factor = new GenericValueModel<float>(1.0f);
            ModifierModeSelector = new ModifierModeSelectorModel();
        }

        public Guid ID { get; set; }
        public PrefabServiceModel PrefabService { get; set; }
        public ModifierModeSelectorModel ModifierModeSelector { get; set; }
        public Vector2Model Width { get; set; }
        public GenericValueModel<float> Phase { get; set; }
        public GenericValueModel<float> Factor { get; set; }
    }
}
