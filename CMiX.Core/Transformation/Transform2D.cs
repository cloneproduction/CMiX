// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;

namespace CMiX.Core.Transformation
{
    public class Transform2D : IControl
    {
        public Transform2D(FloatValue uniformScale, Vector2 translate, Vector2 scale, FloatValue rotate)
        {
            UniformScale = uniformScale;// new FloatValue(1.0f);
            Translate = translate; // new Vector2(0.0f, 0.0f);
            Scale = scale; // new Vector2(1.0f, 1.0f);
            Rotate = rotate;// new FloatValue(0.0f);
        }

        public Guid ID { get; set; } = Guid.NewGuid();

        public FloatValue UniformScale { get; set; }
        public Vector2 Translate { get; set; }
        public Vector2 Scale { get; set; }
        public FloatValue Rotate { get; set; }
    }
}
