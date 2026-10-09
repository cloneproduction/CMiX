// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

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
                                  ControlRepository controlRepository,
                                  ModulatableValue<float> bindableValue)
            : base(prefabService, modulatorManager, controlRepository)
        {
            Bindables = new List<ModulatableValue<float>> { bindableValue };
        }

        public ModulatableValue<float> Value => Bindables[0];

        public override IControlModel ToModel()
        {
            var model = new VisibilityModifierModel
            {
                Value = (ModulatableValueModel<float>)Value.ToModel()
            };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (VisibilityModifierModel)model;
            LoadBaseModel(m);
            Value.FromModel(m.Value);
        }
    }
}
