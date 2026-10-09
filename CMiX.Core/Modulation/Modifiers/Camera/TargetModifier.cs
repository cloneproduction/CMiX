// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Rendering.Cameras;

namespace CMiX.Core.Modulation.Modifiers
{
    [ModifierPanel(typeof(Camera))]
    public partial class TargetModifier : Modifier
    {
        public TargetModifier(PrefabService prefabService,
                              PrefabManager modulatorManager,
                              ControlRepository controlRepository,
                              ModulatableValue<float> bindableX,
                              ModulatableValue<float> bindableY,
                              ModulatableValue<float> bindableZ)
            : base(prefabService, modulatorManager, controlRepository)
        {
            Bindables = new List<ModulatableValue<float>> { bindableX, bindableY, bindableZ };
        }

        public ModulatableValue<float> X => Bindables[0];
        public ModulatableValue<float> Y => Bindables[1];
        public ModulatableValue<float> Z => Bindables[2];

        public override IControlModel ToModel()
        {
            var model = new TargetModifierModel
            {
                X = (ModulatableValueModel<float>)X.ToModel(),
                Y = (ModulatableValueModel<float>)Y.ToModel(),
                Z = (ModulatableValueModel<float>)Z.ToModel()
            };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (TargetModifierModel)model;
            LoadBaseModel(m);
            X.FromModel(m.X);
            Y.FromModel(m.Y);
            Z.FromModel(m.Z);
        }
    }
}
