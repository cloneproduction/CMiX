// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Transformation;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Filters
{
    public partial class TransformTexture : ObservableObject, IPrefab, ITextureFilter
    {
        public TransformTexture(PrefabService prefabService, 
                                SamplerState samplerState, 
                                Transform2D transform2D,
                                GenericValue<float> control)
        {
            PrefabService = prefabService;
            SamplerState = samplerState;
            Transform2D = transform2D;
            Control = control;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public SamplerState SamplerState { get; set; }
        public Transform2D Transform2D { get; set; }
        public PrefabService PrefabService { get; set; }
        public GenericValue<float> Control { get; set; }

        [ObservableProperty]
        private bool isExpanded = true;
    }
}
