// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;

namespace CMiX.Core.Transformation
{
    public class Transform2D : IControl
    {
        public Transform2D(Transform2DModel transform2DModel)
        {
            ID = transform2DModel.ID;

            UniformScale = new FloatValue(transform2DModel.UniformScale);
            Translate = new Vector2(transform2DModel.Translate);
            Scale = new Vector2(transform2DModel.Scale);
            Rotate = new FloatValue(transform2DModel.Rotate);
        }

        public Guid ID { get; set; }

        public FloatValue UniformScale { get; set; }
        public Vector2 Translate { get; set; }
        public Vector2 Scale { get; set; }
        public FloatValue Rotate { get; set; }
    }
}
