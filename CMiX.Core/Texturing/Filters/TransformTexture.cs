// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Texturing.Sampling;
using CMiX.Core.Transformation;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Filters
{
    public partial class TransformTexture : ObservableObject, ITextureModifier
    {
        public TransformTexture(BooleanValue visible, SamplerState samplerState, Transform2D transform2D)
        {
            Visible = visible;
            SamplerState = samplerState;
            Transform2D = transform2D;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public SamplerState SamplerState { get; set; }
        public Transform2D Transform2D { get; set; }
        public BooleanValue Visible { get; set; }

        [ObservableProperty]
        private bool isExpanded = true;
    }
}
