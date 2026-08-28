// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Compositing;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Modulation
{
    // Discoverable on Entity, matching the old Flip's own scope exactly - both stay addable side
    // by side until Flip is confirmed superseded by a live VL check, per this session's migration
    // approach. Flip is not touched by this change. DirectionXYZ is ported as-is (non-modulatable)
    // - unlike every other ported modifier so far, Flip had no numeric fields at all in the old
    // system, so this Modifier has zero Channels; it's ported for VL naming consistency, not
    // because it gains any new modulation capability.
    [ModifierPanel(typeof(Entity))]
    public partial class FlipModifier : Modifier
    {
        public FlipModifier(PrefabService prefabService,
                            PrefabManager modulatorManager,
                            DirectionXYZ directionXYZ)
            : base(prefabService, modulatorManager)
        {
            DirectionXYZ = directionXYZ;
        }

        // Non-modulatable, ported as-is from Flip for one-to-one field parity.
        public DirectionXYZ DirectionXYZ { get; set; }

        public override IControlModel ToModel()
        {
            var model = new FlipModifierModel
            {
                DirectionXYZ = (DirectionXYZModel)DirectionXYZ.ToModel()
            };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (FlipModifierModel)model;
            LoadBaseModel(m);
            DirectionXYZ.FromModel(m.DirectionXYZ);
        }
    }
}
