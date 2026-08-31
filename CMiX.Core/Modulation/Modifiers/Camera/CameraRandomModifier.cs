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
    // Discoverable on Camera, matching the old CameraRandom's own scope exactly - both stay
    // addable side by side until CameraRandom is confirmed superseded by a live VL check, per
    // this session's migration approach. CameraRandom is not touched by this change. PingPong and
    // Axis are ported as-is (non-modulatable); Width becomes a modulatable channel.
    [ModifierPanel(typeof(Camera))]
    public partial class CameraRandomModifier : Modifier, ICameraModifier
    {
        public CameraRandomModifier(PrefabService prefabService,
                                    PrefabManager modulatorManager,
                                    GenericValue<bool> pingPong,
                                    GenericValue<CameraAxis> axis,
                                    Modulatable width)
            : base(prefabService, modulatorManager)
        {
            PingPong = pingPong;
            Axis = axis;
            width.Label = "Width";
            Channels = new List<Modulatable> { width };
        }

        // Convenience accessor into Channels, purely for the view's ModulatableValue binding -
        // Channels itself stays the source of truth (used by Modifier's own ToModel/FromModel).
        public Modulatable Width => Channels[0];

        // Non-modulatable, ported as-is from CameraRandom for one-to-one field parity.
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
