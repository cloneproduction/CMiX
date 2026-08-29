// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Rendering.Cameras;
using CMiX.Core.Rendering.Cameras.Modifiers;

namespace CMiX.Core.Modulation
{
    // Discoverable on Camera, matching the old CameraLFO's own scope exactly - both stay addable
    // side by side until CameraLFO is confirmed superseded by a live VL check, per this session's
    // migration approach. CameraLFO is not touched by this change. PingPong and Axis are ported
    // as-is (non-modulatable); From and To both become modulatable channels.
    [ModifierPanel(typeof(Camera))]
    public partial class CameraLFOModifier : Modifier, ICameraModifier
    {
        public CameraLFOModifier(PrefabService prefabService,
                                 PrefabManager modulatorManager,
                                 GenericValue<bool> pingPong,
                                 GenericValue<CameraAxis> axis,
                                 Modulatable from,
                                 Modulatable to)
            : base(prefabService, modulatorManager)
        {
            PingPong = pingPong;
            Axis = axis;
            from.Label = "From";
            to.Label = "To";
            // Matches the old CameraLFO's own default (From 0, To 1) - a freshly added CameraLFO
            // otherwise oscillates 0 to 0, a no-op.
            to.Value.Value = 1.0f;
            Channels = new List<Modulatable> { from, to };
        }

        // Convenience accessors into Channels, purely for the view's ModulatableValue bindings -
        // Channels itself stays the source of truth (used by Modifier's own ToModel/FromModel).
        public Modulatable From => Channels[0];
        public Modulatable To => Channels[1];

        // Non-modulatable, ported as-is from CameraLFO for one-to-one field parity.
        public GenericValue<bool> PingPong { get; set; }
        public GenericValue<CameraAxis> Axis { get; set; }

        public override IControlModel ToModel()
        {
            var model = new CameraLFOModifierModel
            {
                PingPong = (GenericValueModel<bool>)PingPong.ToModel(),
                Axis = (GenericValueModel<CameraAxis>)Axis.ToModel()
            };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (CameraLFOModifierModel)model;
            LoadBaseModel(m);
            PingPong.FromModel(m.PingPong);
            Axis.FromModel(m.Axis);
        }
    }
}
