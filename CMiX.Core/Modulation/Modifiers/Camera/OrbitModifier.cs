// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Rendering.Cameras;

namespace CMiX.Core.Modulation.Modifiers
{
    [ModifierPanel(typeof(Camera))]
    public partial class OrbitModifier : Modifier
    {
        public OrbitModifier(PrefabService prefabService,
                             PrefabManager modulatorManager,
                             ModulatableValue<float> bindableYaw,
                             ModulatableValue<float> bindablePitch)
            : base(prefabService, modulatorManager)
        {
            bindableYaw.Label = "Yaw";
            bindablePitch.Label = "Pitch";
            Bindables = new List<ModulatableValue<float>> { bindableYaw, bindablePitch };
        }

        public ModulatableValue<float> Yaw => Bindables[0];
        public ModulatableValue<float> Pitch => Bindables[1];

        public override IControlModel ToModel()
        {
            var model = new OrbitModifierModel();
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (OrbitModifierModel)model;
            LoadBaseModel(m);
        }
    }
}
