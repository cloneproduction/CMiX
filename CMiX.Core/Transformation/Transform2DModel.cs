// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;

namespace CMiX.Core.Transformation
{
    public class Transform2DModel : IControlModel
    {
        public Transform2DModel()
        {
            ID = Guid.NewGuid();

            Translate = new Vector2Model(0.0f, 0.0f);
            Scale = new Vector2Model(1.0f, 1.0f);
            Rotate = new GenericValueModel<float>(0.0f);
            UniformScale = new GenericValueModel<float>(1.0f);
        }

        public Guid ID { get; set; }
        public Vector2Model Translate { get; set; }
        public Vector2Model Scale { get; set; }
        public GenericValueModel<float> Rotate { get; set; }
        public GenericValueModel<float> UniformScale { get; set; }
    }
}
