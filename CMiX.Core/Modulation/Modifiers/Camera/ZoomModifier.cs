// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Rendering.Cameras;

namespace CMiX.Core.Modulation.Modifiers
{
    [ModifierPanel(typeof(Camera))]
    public partial class ZoomModifier : Modifier
    {
        public ZoomModifier(PrefabService prefabService,
                            PrefabManager modulatorManager,
                            ControlRepository controlRepository,
                            ModulatableValue<float> bindableDistance,
                            ModulatableValue<float> bindableFOV)
            : base(prefabService, modulatorManager, controlRepository)
        {
            Bindables = new List<ModulatableValue<float>> { bindableDistance, bindableFOV };
        }

        public ModulatableValue<float> Distance => Bindables[0];
        public ModulatableValue<float> FOV => Bindables[1];

        public override IControlModel ToModel()
        {
            var model = new ZoomModifierModel
            {
                Distance = (ModulatableValueModel<float>)Distance.ToModel(),
                FOV = (ModulatableValueModel<float>)FOV.ToModel()
            };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (ZoomModifierModel)model;
            LoadBaseModel(m);
            Distance.FromModel(m.Distance);
            FOV.FromModel(m.FOV);
        }
    }
}
