// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Compositing;
using CMiX.Core.Modulation;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Modulation.Modifiers
{
    // Discoverable on TextEntity, matching the old CharWriter's own scope exactly - both stay
    // addable side by side until CharWriter is confirmed superseded by a live VL check, per this
    // session's migration approach. CharWriter is not touched by this change. Like FlipModifier
    // and SelectTextureModifier, CharWriter had no extra fields at all beyond the shared
    // BeatModifiableModifierBase scaffolding, so this Modifier has zero Channels and no
    // additional properties - ported for VL naming consistency, not new capability.
    [ModifierPanel(typeof(TextEntity))]
    public partial class CharWriterModifier : Modifier
    {
        public CharWriterModifier(PrefabService prefabService, PrefabManager modulatorManager)
            : base(prefabService, modulatorManager)
        {
        }

        public override IControlModel ToModel()
        {
            var model = new CharWriterModifierModel();
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (CharWriterModifierModel)model;
            LoadBaseModel(m);
        }
    }
}
