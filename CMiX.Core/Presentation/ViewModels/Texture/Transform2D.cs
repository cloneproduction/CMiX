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
        public Transform2D(Transform2DModel transformUVModel, CompositionService compositionService)
        {
            ID = transformUVModel.ID;
            Translate = new Vector2(transformUVModel.Translate);
            Scale = new Vector2(transformUVModel.Scale);
            Rotate = new FloatValue(transformUVModel.Rotate);
        }

        public Guid ID { get; set; }

        public Vector2 Translate { get; set; }
        public Vector2 Scale { get; set; }
        public FloatValue Rotate { get; set; }

        public IModel GetModel()
        {
            Transform2DModel transformUVModel = new Transform2DModel();

            transformUVModel.ID = ID;
            transformUVModel.Translate = (Vector2Model)Translate.GetModel();
            transformUVModel.Scale = (Vector2Model)Scale.GetModel();
            transformUVModel.Rotate = (FloatValueModel)Rotate.GetModel();

            return transformUVModel;

        }

        public void SetViewModel(IModel model)
        {
            Transform2DModel modelUVModel = model as Transform2DModel;

            ID = modelUVModel.ID;
            Translate.SetViewModel(modelUVModel.Translate);
            Scale.SetViewModel(modelUVModel.Scale);
            Rotate.SetViewModel(modelUVModel.Rotate);
        }
    }
}
