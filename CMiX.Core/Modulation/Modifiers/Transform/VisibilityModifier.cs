// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Compositing;
using CMiX.Core.Modulation;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Rendering.Lights;

namespace CMiX.Core.Modulation.Modifiers
{
    [ModifierPanel(typeof(LightEntity))]
    [ModifierPanel(typeof(Entity))]
    public partial class VisibilityModifier : Modifier
    {
        public VisibilityModifier(PrefabService prefabService,
                                  PrefabManager modulatorManager,
                                  ModulatableFloat bindableValue)
            : base(prefabService, modulatorManager)
        {
            bindableValue.Value.Value = 0.5f;
            Bindables = new List<ModulatableFloat> { bindableValue };
        }

        public ModulatableFloat Value => Bindables[0];

        public override IControlModel ToModel()
        {
            var model = new VisibilityModifierModel();
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (VisibilityModifierModel)model;
            LoadBaseModel(m);
        }
    }
}
