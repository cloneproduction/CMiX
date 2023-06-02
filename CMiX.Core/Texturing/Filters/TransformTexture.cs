// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.Texturing.Sampling;
using CMiX.Core.Transformation;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Filters
{
    public partial class TransformTexture : ObservableObject, IModifier
    {
        public TransformTexture(TransformTextureModel transformTextureModel)
        {
            ID = transformTextureModel.ID;
            Name = transformTextureModel.Name;
            Visible = new BooleanValue(transformTextureModel.Visible);
            SamplerState = new SamplerState(transformTextureModel.SamplerState);
            Transform2D = new Transform2D(transformTextureModel.Transform2D);
            isExpanded = true;
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
