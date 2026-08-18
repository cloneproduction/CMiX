// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Texturing;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Rendering
{
    public class OutputSettings : ObservableObject, IControl
    {
        public OutputSettings(Integer2 resolution, 
                              GenericValue<string> backgroundColor,
                              GenericValue<TexcoordSemantic> texcoordSemantic,
                              GenericValue<BlendModeEnum> blendMode,
                              GenericValue<float> opacity)
        {
            Resolution = resolution;
            BackgroundColor = backgroundColor;
            TexcoordSemantic = texcoordSemantic;
            BlendMode = blendMode;
            Opacity = opacity;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public Integer2 Resolution { get; set; }
        public GenericValue<string> BackgroundColor { get; set; }
        public GenericValue<TexcoordSemantic> TexcoordSemantic { get; set; }
        public GenericValue<BlendModeEnum> BlendMode { get; set; }
        public GenericValue<float> Opacity { get; set; }

        public IControlModel ToModel() => new OutputSettingsModel
        {
            ID = ID,
            Resolution = (Integer2Model)Resolution.ToModel(),
            BackgroundColor = (GenericValueModel<string>)BackgroundColor.ToModel(),
            TexcoordSemantic = (GenericValueModel<TexcoordSemantic>)TexcoordSemantic.ToModel(),
            BlendMode = (GenericValueModel<BlendModeEnum>)BlendMode.ToModel(),
            Opacity = (GenericValueModel<float>)Opacity.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (OutputSettingsModel)model;
            ID = m.ID;
            Resolution.FromModel(m.Resolution);
            BackgroundColor.FromModel(m.BackgroundColor);
            TexcoordSemantic.FromModel(m.TexcoordSemantic);
            BlendMode.FromModel(m.BlendMode);
            Opacity.FromModel(m.Opacity);
        }
    }
}
