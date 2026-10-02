// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CommunityToolkit.Mvvm.ComponentModel;
using static CMiX.Core.ControlExtensions;

namespace CMiX.Core.Texturing.Sources
{
    public partial class TouchBlob : TextureSourceBase, ITextureSource
    {
        public TouchBlob(PrefabService prefabService,
                         PrefabManager textureModifierManager,
                         Integer2 resolution,
                         GenericValue<bool> useCompositionResolution,
                         GenericValue<float> size,
                         GenericValue<string> color,
                         GenericValue<string> background)
            : base(prefabService, textureModifierManager, useCompositionResolution)
        {
            Resolution = resolution;
            Size = size;
            Color = color;
            Background = background;
        }

        public Integer2 Resolution { get; set; }
        public GenericValue<float> Size { get; set; }
        public GenericValue<string> Color { get; set; }
        public GenericValue<string> Background { get; set; }

        public override IControlModel ToModel()
        {
            var model = new TouchBlobModel
            {
                Resolution = (Integer2Model)Resolution.ToModel(),
                Size = (GenericValueModel<float>)Size.ToModel(),
                Color = (GenericValueModel<string>)Color.ToModel(),
                Background = (GenericValueModel<string>)Background.ToModel()
            };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (TouchBlobModel)model;
            LoadBaseModel(m);
            Resolution.FromModel(m.Resolution);
            Size.FromModel(m.Size);
            Color.FromModel(m.Color);
            Background.FromModel(m.Background);
        }
    }
}
