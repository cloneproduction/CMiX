// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentation.ViewModels
{
    public class TransformTexture : ObservableObject, ITextureFilter
    {
        public TransformTexture(TransformTextureModel transformTextureModel)
        {
            ID = transformTextureModel.ID;
            Name = transformTextureModel.Name;
            IsExpanded = true;
            Visible = new ToggleButton(transformTextureModel.Visible);
            SamplerState = new SamplerState(transformTextureModel.SamplerStateModel);
            TranslateX = new Slider(nameof(TranslateX), transformTextureModel.TranslateXModel);
            TranslateY = new Slider(nameof(TranslateY), transformTextureModel.TranslateYModel);
            ScaleX = new Slider(nameof(ScaleX), transformTextureModel.ScaleXModel);
            ScaleY = new Slider(nameof(ScaleY), transformTextureModel.ScaleYModel);
            Rotate = new Slider(nameof(Rotate), transformTextureModel.RotateModel);
        }

        public Guid ID { get; set; }
        public TextureFilterName Name { get; set; }
        public SamplerState SamplerState { get; set; }
        public Slider TranslateX { get; set; }
        public Slider TranslateY { get; set; }
        public Slider ScaleX { get; set; }
        public Slider ScaleY { get; set; }
        public Slider Rotate { get; set; }
        public ToggleButton Visible { get; set; }


        private bool _enabled;
        public bool Enabled
        {
            get => _enabled;
            set => SetProperty(ref _enabled, value);
        }

        private bool _isExpanded;
        public bool IsExpanded
        {
            get => _isExpanded;
            set => SetProperty(ref _isExpanded, value);
        }


        public void SetViewModel(IModel model)
        {
            TransformTextureModel transformTextureModel = model as TransformTextureModel;
            ID = transformTextureModel.ID;
            Name = transformTextureModel.Name;

            Visible.SetViewModel(transformTextureModel.Visible);
            SamplerState.SetViewModel(transformTextureModel.SamplerStateModel);
            TranslateX.SetViewModel(transformTextureModel.TranslateXModel);
            TranslateY.SetViewModel(transformTextureModel.TranslateYModel);
            ScaleX.SetViewModel(transformTextureModel.ScaleXModel);
            ScaleY.SetViewModel(transformTextureModel.ScaleYModel);
            Rotate.SetViewModel(transformTextureModel.RotateModel);
        }

        public IModel GetModel()
        {
            TransformTextureModel transformTextureModel = new TransformTextureModel();
            transformTextureModel.ID = ID;
            transformTextureModel.Name = Name;

            transformTextureModel.Visible = (ToggleButtonModel)Visible.GetModel();
            transformTextureModel.SamplerStateModel = (SamplerStateModel)SamplerState.GetModel();

            transformTextureModel.TranslateXModel = (SliderModel)TranslateX.GetModel();
            transformTextureModel.TranslateYModel = (SliderModel)TranslateY.GetModel();

            transformTextureModel.ScaleXModel = (SliderModel)ScaleX.GetModel();
            transformTextureModel.ScaleYModel = (SliderModel)ScaleY.GetModel();
            transformTextureModel.RotateModel = (SliderModel)Rotate.GetModel();

            return transformTextureModel;

        }

        public void Dispose()
        {

        }
    }
}
