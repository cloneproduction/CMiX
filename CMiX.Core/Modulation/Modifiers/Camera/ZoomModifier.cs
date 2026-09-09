// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

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
                            ModulatableValue<float> bindableDistance,
                            ModulatableValue<float> bindableFOV)
            : base(prefabService, modulatorManager)
        {
            bindableDistance.Label = "Distance";
            bindableFOV.Label = "FOV";
            Bindables = new List<ModulatableValue<float>> { bindableDistance, bindableFOV };
        }

        public ModulatableValue<float> Distance => Bindables[0];
        public ModulatableValue<float> FOV => Bindables[1];

        public override IControlModel ToModel()
        {
            var model = new ZoomModifierModel();
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (ZoomModifierModel)model;
            LoadBaseModel(m);
        }
    }
}
