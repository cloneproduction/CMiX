// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CMiX.Core.Models.BaseControls;
using CMiX.Core.Presentation.ViewModels.BaseControl;
using CMiX.Core.Presentation.ViewModels.Service;

namespace CMiX.Core.Presentation.ViewModels
{
    public class Transform2D : IControl
    {
        public Transform2D(Transform2DModel transform2DModel, CompositionService compositionService)
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

        public IModel GetModel()
        {
            Transform2DModel transform2DModel = new Transform2DModel();

            transform2DModel.ID = ID;
            transform2DModel.UniformScale = (FloatValueModel)UniformScale.GetModel();
            transform2DModel.Translate = (Vector2Model)Translate.GetModel();
            transform2DModel.Scale = (Vector2Model)Scale.GetModel();
            transform2DModel.Rotate = (FloatValueModel)Rotate.GetModel();

            return transform2DModel;

        }

        public void SetViewModel(IModel model)
        {
            Transform2DModel transform2DModel = model as Transform2DModel;

            ID = transform2DModel.ID;
            UniformScale.SetViewModel(transform2DModel.UniformScale);
            Translate.SetViewModel(transform2DModel.Translate);
            Scale.SetViewModel(transform2DModel.Scale);
            Rotate.SetViewModel(transform2DModel.Rotate);
        }
    }
}
