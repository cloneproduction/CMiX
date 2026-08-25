// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CommunityToolkit.Mvvm.ComponentModel;
using static CMiX.Core.ControlExtensions;

namespace CMiX.Core.Materials.Modifiers
{
    [ModifierPanel(typeof(Material))]
    public partial class SelectRandomTexture : BeatModifiableModifierBase, IModifier
    {
        public SelectRandomTexture(PrefabManager beatModifierManager,
                                  PrefabService prefabService,
                                  GenericValue<TextureFrom> textureFrom)
            : base(prefabService, beatModifierManager)
        {
            TextureFrom = textureFrom;
        }

        public GenericValue<TextureFrom> TextureFrom { get; set; }

        public override IControlModel ToModel()
        {
            var model = new SelectRandomTextureModel
            {
                TextureFrom = (GenericValueModel<TextureFrom>)TextureFrom.ToModel()
            };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (SelectRandomTextureModel)model;
            LoadBaseModel(m);
            TextureFrom.FromModel(m.TextureFrom);
        }
    }
}
