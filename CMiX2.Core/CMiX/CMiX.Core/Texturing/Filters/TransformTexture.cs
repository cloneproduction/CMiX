// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CMiX.Core.Presentation.ViewModels.Service;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentation.ViewModels
{
    public class TransformTexture : ObservableObject, ITextureFilter
    {
        public TransformTexture(TransformTextureModel transformTextureModel, CompositionService compositionService)
        {
            ID = transformTextureModel.ID;
            Name = transformTextureModel.Name;
            IsExpanded = true;
            Visible = new BooleanValue(transformTextureModel.Visible);
            SamplerState = new SamplerState(transformTextureModel.SamplerStateModel, compositionService);
            Transform2D = new Transform2D(transformTextureModel.Transform2D, compositionService);
        }

        public Guid ID { get; set; }
        public TextureFilterName Name { get; set; }
        public SamplerState SamplerState { get; set; }
        public Transform2D Transform2D { get; set; }
        public BooleanValue Visible { get; set; }



        private bool _isExpanded;
        public bool IsExpanded
        {
            get => _isExpanded;
            set => SetProperty(ref _isExpanded, value);
        }


        public IModel GetModel()
        {
            TransformTextureModel transformTextureModel = new TransformTextureModel();
            transformTextureModel.ID = ID;
            transformTextureModel.Name = Name;

            transformTextureModel.Visible = (BooleanValueModel)Visible.GetModel();
            transformTextureModel.SamplerStateModel = (SamplerStateModel)SamplerState.GetModel();
            transformTextureModel.Transform2D = (Transform2DModel)Transform2D.GetModel();

            return transformTextureModel;
        }

        public void Dispose()
        {

        }
    }
}
