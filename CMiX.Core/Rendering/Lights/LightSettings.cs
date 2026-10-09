// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.BaseControls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Rendering.Lights
{
    public partial class LightSettings : ObservableObject, IControl
    {
        public LightSettings(GenericValue<string> lightColor, 
                             Vector3 position, 
                             Vector3 target, 
                             GenericValue<float> radius, 
                             GenericValue<float> angle, 
                             GenericValue<float> softness, 
                             GenericValue<float> intensity,
                             GenericValue<bool> lightHelper,
                             GenericValue<LightType> lightTypeSelector) 
        {
            ID = Guid.NewGuid();
            LightColor = lightColor;
            LightHelper = lightHelper;
            Position = position;
            Target = target;
            Radius = radius;
            Angle = angle;
            Softness = softness;
            Intensity = intensity;
            LightTypeSelector = lightTypeSelector;
        }

        public Guid ID { get; set; }
        public GenericValue<LightType> LightTypeSelector { get; set; }
        public GenericValue<bool> LightHelper { get; set; }
        public GenericValue<string> LightColor { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Target { get; set; }
        public GenericValue<float> Radius { get; set; }
        public GenericValue<float> Angle { get; set; }
        public GenericValue<float> Softness { get; set; }
        public GenericValue<float> Intensity { get; set; }

        public IControlModel ToModel() => new LightSettingsModel
        {
            ID = ID,
            LightTypeSelector = (GenericValueModel<LightType>)LightTypeSelector.ToModel(),
            LightColor = (GenericValueModel<string>)LightColor.ToModel(),
            Position = (Vector3Model)Position.ToModel(),
            Target = (Vector3Model)Target.ToModel(),
            Radius = (GenericValueModel<float>)Radius.ToModel(),
            Angle = (GenericValueModel<float>)Angle.ToModel(),
            Softness = (GenericValueModel<float>)Softness.ToModel(),
            Intensity = (GenericValueModel<float>)Intensity.ToModel(),
            LightHelper = (GenericValueModel<bool>)LightHelper.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (LightSettingsModel)model;
            ID = m.ID;
            LightTypeSelector.FromModel(m.LightTypeSelector);
            LightColor.FromModel(m.LightColor);
            Position.FromModel(m.Position);
            Target.FromModel(m.Target);
            Radius.FromModel(m.Radius);
            Angle.FromModel(m.Angle);
            Softness.FromModel(m.Softness);
            Intensity.FromModel(m.Intensity);
            LightHelper.FromModel(m.LightHelper);
        }
    }
}
