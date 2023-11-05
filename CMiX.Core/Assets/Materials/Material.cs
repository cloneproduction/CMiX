// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Assets.Materials;
using CMiX.Core.Texturing;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Materials
{
    public partial class Material : ObservableObject, IControl
    {
        public Material(MaterialSettings materialSettings, DiffuseTexture diffuseTexture, MaskTexture maskTexture)
        {
            ID = Guid.NewGuid();
            DiffuseTexture = diffuseTexture;
            MaskTexture = maskTexture;
            MaterialSettings = materialSettings;
        }

        public Guid ID { get; set; } 
        public DiffuseTexture DiffuseTexture { get; set; }
        public MaskTexture MaskTexture { get; set; }
        public MaterialSettings MaterialSettings { get; set; }


        [ObservableProperty]
        private bool isExpanded = false;

        [ObservableProperty]
        private bool surfaceIsExpanded = false;
    }
}
