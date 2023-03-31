// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Presentations.Service;
using CMiX.Core.Presentations.Texturing.Sampling;
using CMiX.Core.Presentations.Transform;
using CMiX.Core.Texturing.Filters;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentations.ViewModels
{
    public partial class TransformTexture : ObservableObject, ITextureFilter
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

        [ObservableProperty]
        private bool isExpanded;

        public void Dispose()
        {

        }
    }
}
