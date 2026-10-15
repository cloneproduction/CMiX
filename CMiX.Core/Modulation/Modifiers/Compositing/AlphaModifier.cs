// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Compositing;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Modulation.Modifiers
{
    // Drives the Alpha of a Layer or a Composition on top of its Alpha slider.
    [ModifierPanel(typeof(Layer))]
    [ModifierPanel(typeof(Composition))]
    public partial class AlphaModifier : Modifier
    {
        public AlphaModifier(PrefabService prefabService,
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
            var model = new AlphaModifierModel
            {
                Value = (ModulatableValueModel<float>)Value.ToModel()
            };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (AlphaModifierModel)model;
            LoadBaseModel(m);
            Value.FromModel(m.Value);
        }
    }
}
