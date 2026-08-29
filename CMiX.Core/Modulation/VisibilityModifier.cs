// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Compositing;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Rendering.Lights;

namespace CMiX.Core.Modulation
{
    // Discoverable on both Entity and LightEntity, matching RandomVisibility's own scope exactly -
    // both stay addable side by side until RandomVisibility is confirmed superseded by a live VL
    // check, per this session's migration approach. RandomVisibility is not touched by this change.
    [ModifierPanel(typeof(LightEntity))]
    [ModifierPanel(typeof(Entity))]
    public partial class VisibilityModifier : Modifier
    {
        public VisibilityModifier(PrefabService prefabService,
                                  PrefabManager modulatorManager,
                                  Modulatable channelValue)
            : base(prefabService, modulatorManager)
        {
            // Matches the old RandomVisibility's own default - a freshly added modifier
            // otherwise starts at 0 instead of half-visible.
            channelValue.Value.Value = 0.5f;
            Channels = new List<Modulatable> { channelValue };
        }

        // Convenience accessor into Channels, purely for the view's ChannelValue binding -
        // Channels itself stays the source of truth (used by Modifier's own ToModel/FromModel).
        public Modulatable Value => Channels[0];

        public override IControlModel ToModel()
        {
            var model = new VisibilityModifierModel();
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (VisibilityModifierModel)model;
            LoadBaseModel(m);
        }
    }
}
