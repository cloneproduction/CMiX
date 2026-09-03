// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modulation;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Rendering.Cameras;
using CMiX.Core.Rendering.Cameras.Modifiers;

namespace CMiX.Core.Modulation.Modifiers
{
    [ModifierPanel(typeof(Camera))]
    public partial class CameraRandomModifier : Modifier, ICameraModifier
    {
        public CameraRandomModifier(PrefabService prefabService,
                                    PrefabManager modulatorManager,
                                    GenericValue<bool> pingPong,
                                    GenericValue<CameraAxis> axis,
                                    ModulatableFloat width)
            : base(prefabService, modulatorManager)
        {
            PingPong = pingPong;
            Axis = axis;
            width.Label = "Width";
            Bindables = new List<ModulatableFloat> { width };
        }

        public ModulatableFloat Width => Bindables[0];

        public GenericValue<bool> PingPong { get; set; }
        public GenericValue<CameraAxis> Axis { get; set; }

        public override IControlModel ToModel()
        {
            var model = new CameraRandomModifierModel
            {
                PingPong = (GenericValueModel<bool>)PingPong.ToModel(),
                Axis = (GenericValueModel<CameraAxis>)Axis.ToModel()
            };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (CameraRandomModifierModel)model;
            LoadBaseModel(m);
            PingPong.FromModel(m.PingPong);
            Axis.FromModel(m.Axis);
        }
    }
}
