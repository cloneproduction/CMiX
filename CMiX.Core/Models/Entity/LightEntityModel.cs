// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models.BaseControls;
using CMiX.Core.Presentation.ViewModels;

namespace CMiX.Core.Models
{
    public class LightEntityModel : IEntityModel, IPrefabModel
    {
        public LightEntityModel()
        {
            ID = Guid.NewGuid();
            Enabled = true;

            LightColor = new ColorSelectorModel();
            Position = new VectorXYZModel(nameof(Position), 0.0f, 2.0f, 0.0f);
            Target = new VectorXYZModel(nameof(Target), 0.001f, 0.0f, 0.0f);
            Radius = new SliderModel(5.0f);
            Angle = new SliderModel(0.25f);
            Softness = new SliderModel(0.01f);
            Intensity = new SliderModel(1.0f);
            LightTypeSelector = new ComboBoxModel<LightType>(LightType.AmbientLight);
            Visibility = new ToggleButtonModel();
        }

        public bool Enabled { get; set; }
        public Guid ID { get; set; }
        public string Name { get; set; }

        public ColorSelectorModel LightColor { get; set; }
        public VectorXYZModel Position { get; set; }
        public VectorXYZModel Target { get; set; }
        public SliderModel Radius { get; set; }
        public SliderModel Angle { get; set; }
        public SliderModel Softness { get; set; }
        public SliderModel Intensity { get; set; }
        public ComboBoxModel<LightType> LightTypeSelector { get; internal set; }
        public ToggleButtonModel Visibility { get; internal set; }
    }
}
