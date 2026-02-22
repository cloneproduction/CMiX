// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;

namespace CMiX.Core.Transformation
{
    public class Transform2D : IControl
    {
        public Transform2D(GenericValue<float> uniformScale, 
                           Vector2 translate, 
                           Vector2 scale, 
                           GenericValue<float> rotate)
        {
            UniformScale = uniformScale;
            Translate = translate;
            Scale = scale;
            Rotate = rotate;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public GenericValue<float> UniformScale { get; set; }
        public Vector2 Translate { get; set; }
        public Vector2 Scale { get; set; }
        public GenericValue<float> Rotate { get; set; }
    }
}
