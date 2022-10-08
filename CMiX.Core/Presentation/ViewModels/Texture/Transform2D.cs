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
            Translate = new VectorXY(transformUVModel.Translate);
            Scale = new VectorXY(transformUVModel.Scale);
            Rotate = new Slider(nameof(Rotate), transformUVModel.Rotate);
        }

        public Guid ID { get; set; }

        public VectorXY Translate { get; set; }
        public VectorXY Scale { get; set; }
        public Slider Rotate { get; set; }

        public IModel GetModel()
        {
            Transform2DModel transformUVModel = new Transform2DModel();

            transformUVModel.ID = ID;
            transformUVModel.Translate = (VectorXYModel)Translate.GetModel();
            transformUVModel.Scale = (VectorXYModel)Scale.GetModel();
            transformUVModel.Rotate = (SliderModel)Rotate.GetModel();

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
