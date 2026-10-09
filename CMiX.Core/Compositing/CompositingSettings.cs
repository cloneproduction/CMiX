// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.BaseControls;
using CMiX.Core.Texturing;

namespace CMiX.Core.Compositing
{
    public class CompositingSettings : IControl
    {
        public CompositingSettings(GenericValue<float> opacity,
                                   GenericValue<string> backgroundColor, 
                                   GenericValue<BlendMode> blendMode) 
        {
            Opacity = opacity;
            BackgroundColor = backgroundColor;
            BlendMode = blendMode;
        }

        public GenericValue<float> Opacity { get; set; }
        public GenericValue<string> BackgroundColor { get; set; }
        public GenericValue<BlendMode> BlendMode { get; set; }
        public Guid ID { get; set; } = Guid.NewGuid();

        public IControlModel ToModel() => new CompositingSettingsModel
        {
            ID = ID,
            BlendMode = (GenericValueModel<BlendMode>)BlendMode.ToModel(),
            Opacity = (GenericValueModel<float>)Opacity.ToModel(),
            BackgroundColor = (GenericValueModel<string>)BackgroundColor.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (CompositingSettingsModel)model;
            ID = m.ID;
            BlendMode.FromModel(m.BlendMode);
            Opacity.FromModel(m.Opacity);
            BackgroundColor.FromModel(m.BackgroundColor);
        }
    }
}
