// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Materials;
using CMiX.Core.Materials.Modifiers;
using CMiX.Core.Modifiers;
using CMiX.Core.Modulation;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Modulation.Modifiers
{
    // Discoverable on Material, matching the old SelectRandomTexture's own scope exactly - both
    // stay addable side by side until SelectRandomTexture is confirmed superseded by a live VL
    // check, per this session's migration approach. SelectRandomTexture is not touched by this
    // change. TextureFrom is ported as-is (non-modulatable) - like FlipModifier, SelectRandomTexture
    // had no numeric fields at all in the old system, so this Modifier has zero Bindables; it's
    // ported for VL naming consistency, not because it gains any new modulation capability.
    [ModifierPanel(typeof(Material))]
    public partial class SelectTextureModifier : Modifier
    {
        public SelectTextureModifier(PrefabService prefabService,
                                     PrefabManager modulatorManager,
                                     GenericValue<TextureFrom> textureFrom)
            : base(prefabService, modulatorManager)
        {
            TextureFrom = textureFrom;
        }

        // Non-modulatable, ported as-is from SelectRandomTexture for one-to-one field parity.
        public GenericValue<TextureFrom> TextureFrom { get; set; }

        public override IControlModel ToModel()
        {
            var model = new SelectTextureModifierModel
            {
                TextureFrom = (GenericValueModel<TextureFrom>)TextureFrom.ToModel()
            };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (SelectTextureModifierModel)model;
            LoadBaseModel(m);
            TextureFrom.FromModel(m.TextureFrom);
        }
    }
}
