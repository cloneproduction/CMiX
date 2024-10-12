// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Assets.Mesh;
using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.ViewModels.Assets;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.ViewModels
{
    public partial class Mesh : ObservableRecipient, IControl
    {
        public Mesh(GenericValue<MeshType> meshTypeSelector, 
                    Vector3 scale, 
                    Vector3 offset,
                    GenericValue<float> radius,
                    GenericValue<float> height,
                    GenericValue<float> thickness,
                    GenericValue<int> tessellation,
                    Integer2 tessellationXY,
                    GenericValue<bool> generateBackFace,
                    GenericValue<bool> visibility,
                    GenericValue<IAsset> geometry,
                    Text3DSettings text3DSettings,
                    PrefabManager explodeTriangleTextureManager,
                    GenericValue<float> explodeStrength)
        {
            MeshTypeSelector = meshTypeSelector;
            Scale = scale;
            Offset = offset;
            Radius = radius;
            Height = height;
            Thickness = thickness;
            Tessellation = tessellation;
            TessellationXY = tessellationXY;
            GenerateBackFace = generateBackFace;
            Visibility = visibility;
            Geometry = geometry;
            Text3DSettings = text3DSettings;

            ExplodeTriangleTextureManager = explodeTriangleTextureManager;
            ExplodeStrength = explodeStrength;
        }

        [ObservableProperty]
        private bool isExpanded = false;

        public Guid ID { get; set; } = Guid.NewGuid();
        public GenericValue<MeshType> MeshTypeSelector { get; set; }
        public Text3DSettings Text3DSettings { get; set; }
        public GenericValue<IAsset> Geometry { get; set; }
        public Vector3 Scale { get; set; }
        public Vector3 Offset { get; set; }
        public GenericValue<float> Radius { get; set; }
        public GenericValue<float> Height { get; set; }
        public GenericValue<float> Thickness { get; set; }
        public GenericValue<int> Tessellation { get; set; }
        public Integer2 TessellationXY { get; set; }
        public GenericValue<bool> GenerateBackFace { get; set; }
        public GenericValue<bool> Visibility { get; set; }



        public PrefabManager ExplodeTriangleTextureManager { get; set; }
        public GenericValue<float> ExplodeStrength { get; set; }
    }
}
