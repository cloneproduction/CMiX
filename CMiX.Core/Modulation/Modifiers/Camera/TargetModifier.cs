// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

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
                              ModulatableFloat bindableX,
                              ModulatableFloat bindableY,
                              ModulatableFloat bindableZ)
            : base(prefabService, modulatorManager)
        {
            bindableX.Label = "X";
            bindableY.Label = "Y";
            bindableZ.Label = "Z";
            Bindables = new List<ModulatableFloat> { bindableX, bindableY, bindableZ };
        }

        public ModulatableFloat X => Bindables[0];
        public ModulatableFloat Y => Bindables[1];
        public ModulatableFloat Z => Bindables[2];

        public override IControlModel ToModel()
        {
            var model = new TargetModifierModel();
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (TargetModifierModel)model;
            LoadBaseModel(m);
        }
    }
}
