// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CommunityToolkit.Mvvm.ComponentModel;
using static CMiX.Core.ControlExtensions;

namespace CMiX.Core.Texturing.Sources
{
    public partial class Gradient : TextureSourceBase, ITextureSource
    {
        public Gradient(PrefabService prefabService,
                        PrefabManager textureModifierManager,
                        Integer2 resolution,
                        GenericValue<string> from,
                        GenericValue<string> to,
                        GenericValue<float> gamma,
                        GenericValue<bool> horizontal)
            : base(prefabService, textureModifierManager)
        {
            Resolution = resolution;
            From = from;
            To = to;
            Gamma = gamma;
            Horizontal = horizontal;
        }

        public Integer2 Resolution { get; set; }
        public GenericValue<string> From { get; set; }
        public GenericValue<string> To { get; set; }
        public GenericValue<float> Gamma { get; set; }
        public GenericValue<bool> Horizontal { get; set; }

        public override IControlModel ToModel()
        {
            var model = new GradientModel
            {
                Resolution = (Integer2Model)Resolution.ToModel(),
                Gamma = (GenericValueModel<float>)Gamma.ToModel(),
                From = (GenericValueModel<string>)From.ToModel(),
                To = (GenericValueModel<string>)To.ToModel(),
                Horizontal = (GenericValueModel<bool>)Horizontal.ToModel()
            };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (GradientModel)model;
            LoadBaseModel(m);
            Resolution.FromModel(m.Resolution);
            Gamma.FromModel(m.Gamma);
            From.FromModel(m.From);
            To.FromModel(m.To);
            Horizontal.FromModel(m.Horizontal);
        }
    }
}
