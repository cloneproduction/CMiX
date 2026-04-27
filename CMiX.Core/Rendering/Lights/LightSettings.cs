// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

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
                             GenericValue<LightType> lightTypeSelector) 
        {
            ID = Guid.NewGuid();
            LightColor = lightColor;
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
            Grey700Brush = (GenericValueModel<string>)LightColor.ToModel(),
            Position = (Vector3Model)Position.ToModel(),
            Target = (Vector3Model)Target.ToModel(),
            Radius = (GenericValueModel<float>)Radius.ToModel(),
            Angle = (GenericValueModel<float>)Angle.ToModel(),
            Softness = (GenericValueModel<float>)Softness.ToModel(),
            Intensity = (GenericValueModel<float>)Intensity.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (LightSettingsModel)model;
            ID = m.ID;
            LightTypeSelector.FromModel(m.LightTypeSelector);
            LightColor.FromModel(m.Grey700Brush);
            Position.FromModel(m.Position);
            Target.FromModel(m.Target);
            Radius.FromModel(m.Radius);
            Angle.FromModel(m.Angle);
            Softness.FromModel(m.Softness);
            Intensity.FromModel(m.Intensity);
        }
    }
}
