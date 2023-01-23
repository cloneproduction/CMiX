// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CMiX.Core.Presentation.ViewModels.Beat;
using CMiX.Core.Presentation.ViewModels.Service;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentation.ViewModels
{
    public class CameraRandom : ObservableObject, ICameraModifier, IBeatModifiable
    {
        public CameraRandom(CameraRandomModel randomModel, CompositionService compositionService)
        {
            ID = randomModel.ID;

            Visible = new BooleanValue(randomModel.Visible);
            BeatModifier = new BeatModifier(randomModel.BeatModifier, compositionService);

            PingPong = new BooleanValue(randomModel.PingPong);
            Axis = new GenericValue<CameraAxis>(randomModel.Axis);
            Easing = new Easing(randomModel.Easing);

            Width = new FloatValue(randomModel.Width);
            IsExpanded = true;
        }


        public Guid ID { get; set; }
        public BooleanValue Visible { get; set; }

        private bool _isExpanded;
        public bool IsExpanded
        {
            get => _isExpanded;
            set => SetProperty(ref _isExpanded, value);
        }

        public BeatModifier BeatModifier { get; set; }
        public BooleanValue PingPong { get; set; }
        public GenericValue<CameraAxis> Axis { get; set; }
        public Easing Easing { get; set; }
        public FloatValue Width { get; set; }


        public IModel GetModel()
        {
            CameraRandomModel cameraRandomModel = new CameraRandomModel();

            cameraRandomModel.ID = ID;
            cameraRandomModel.Visible = (BooleanValueModel)Visible.GetModel();
            cameraRandomModel.BeatModifier = (BeatModifierModel)BeatModifier.GetModel();
            cameraRandomModel.PingPong = (BooleanValueModel)PingPong.GetModel();
            cameraRandomModel.Axis = (GenericValueModel<CameraAxis>)Axis.GetModel();
            cameraRandomModel.Easing = (EasingModel)Easing.GetModel();
            cameraRandomModel.Width = (FloatValueModel)Width.GetModel();

            return cameraRandomModel;
        }

        public void SetViewModel(IModel model)
        {
            CameraRandomModel cameraRandomModel = model as CameraRandomModel;

            ID = cameraRandomModel.ID;
            Visible.SetViewModel(cameraRandomModel.Visible);
            BeatModifier.SetViewModel(cameraRandomModel.BeatModifier);
            PingPong.SetViewModel(cameraRandomModel.PingPong);
            Axis.SetViewModel(cameraRandomModel.Axis);
            Easing.SetViewModel(cameraRandomModel.Easing);
            Width.SetViewModel(cameraRandomModel.Width);
        }

        public void Dispose()
        {

        }
    }
}
