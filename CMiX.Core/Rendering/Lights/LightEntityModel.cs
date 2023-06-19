// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefab;
using CMiX.Core.Rendering.Lights;

namespace CMiX.Core.Entities.Lights
{
    public class LightEntityModel : IPrefabModel
    {
        public LightEntityModel()
        {
            ID = Guid.NewGuid();
            LightColor = new ColorSelectorModel();
            Position = new Vector3Model(nameof(Position), 0.0f, 2.0f, 0.0f);
            Target = new Vector3Model(nameof(Target), 0.001f, 0.0f, 0.0f);
            Radius = new FloatValueModel(5.0f);
            Angle = new FloatValueModel(0.25f);
            Softness = new FloatValueModel(0.01f);
            Intensity = new FloatValueModel(1.0f);
            LightTypeSelector = new GenericValueModel<LightType>(LightType.AmbientLight);
            Visibility = new BooleanValueModel();
            IsSelected = new BooleanValueModel(false);
            Name = new StringValueModel("Light " + ID.ToString());
            IsRenaming = new BooleanValueModel(false);
        }

        public Guid ID { get; set; }
        public StringValueModel Name { get; set; }

        public ColorSelectorModel LightColor { get; set; }
        public Vector3Model Position { get; set; }
        public Vector3Model Target { get; set; }
        public FloatValueModel Radius { get; set; }
        public FloatValueModel Angle { get; set; }
        public FloatValueModel Softness { get; set; }
        public FloatValueModel Intensity { get; set; }
        public GenericValueModel<LightType> LightTypeSelector { get; set; }
        public BooleanValueModel Visibility { get; set; }
        public BooleanValueModel IsSelected { get; set; }
        public BooleanValueModel IsRenaming { get; set; }
    }
}
