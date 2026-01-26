// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;

namespace CMiX.Core.Transformation
{
    public class Transform2DModel : IControlModel
    {
        public Guid ID { get; init; } = Guid.NewGuid();
        public Vector2Model Translate { get; init; } = new Vector2Model(0.0f, 0.0f);
        public Vector2Model Scale { get; init; } = new Vector2Model(1.0f, 1.0f);
        public GenericValueModel<float> Rotate { get; init; } = new(0.0f);
        public GenericValueModel<float> UniformScale { get; init; } = new(1.0f);
    }
}
