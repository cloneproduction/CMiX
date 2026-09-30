// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Materials;
using CMiX.Core.Materials.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Modulation.Modifiers
{
    [ModifierPanel(typeof(Material))]
    public partial class SelectTextureModifier : Modifier
    {
        public SelectTextureModifier(PrefabService prefabService,
                                     PrefabManager modulatorManager,
                                     ControlRepository controlRepository,
                                     GenericValue<TextureFrom> textureFrom)
            : base(prefabService, modulatorManager, controlRepository)
        {
            TextureFrom = textureFrom;
        }

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
