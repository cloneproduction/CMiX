// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

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

        public IControlModel ToModel() => new Transform2DModel
        {
            ID = ID,
            Translate = (Vector2Model)Translate.ToModel(),
            Scale = (Vector2Model)Scale.ToModel(),
            Rotate = (GenericValueModel<float>)Rotate.ToModel(),
            UniformScale = (GenericValueModel<float>)UniformScale.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (Transform2DModel)model;
            ID = m.ID;
            Translate.FromModel(m.Translate);
            Scale.FromModel(m.Scale);
            Rotate.FromModel(m.Rotate);
            UniformScale.FromModel(m.UniformScale);
        }
    }
}
