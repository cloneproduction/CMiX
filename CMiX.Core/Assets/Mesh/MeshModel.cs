// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Assets.Mesh;
using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Assets;

namespace CMiX.Core
{
    public record MeshModel : IControlModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public GenericValueModel<MeshType> MeshTypeSelector { get;  set; } = new(MeshType.Plane);
        public AssetSelectorModel Geometry { get; set; } = new();
        public Text3DSettingsModel Text3DSettings { get; set; } = new();
        public Vector3Model Scale { get; set; } = new(1.0f, 1.0f, 1.0f);
        public Vector3Model Offset { get; set; } = new(0.0f, 0.0f, 0.0f);
        public GenericValueModel<float> Radius { get; set; } = new(1.0f);
        public GenericValueModel<float> Height { get; set; } = new(1.0f);
        public GenericValueModel<float> Thickness { get; set; } = new(1.0f);
        public GenericValueModel<int> Tessellation { get; set; } = new(16);
        public Integer2Model TessellationXY { get; set; } = new(16, 16);
        public GenericValueModel<bool> GenerateBackFace { get; set; } = new(true);
        public GenericValueModel<bool> Visibility { get; set; } = new(true);
        public GenericValueModel<string> Name { get; set; } = new("Mesh");
        public GenericValueModel<bool> IsRenaming { get; set; } = new(false);
        public GenericValueModel<bool> IsSelected { get; set; } = new(false);
        public PrefabManagerModel ExplodeTriangleTextureManager { get; set; } = new();
        public GenericValueModel<float> ExplodeStrength { get; set; } = new(0.5f);
        public PrefabManagerModel DisplacementTextureManager { get; set; } = new();
        public GenericValueModel<float> DisplacementStrength { get; set; } = new(0.5f);
        public GenericValueModel<float> FlatNormals { get; set; } = new(0.0f);
    }
}
