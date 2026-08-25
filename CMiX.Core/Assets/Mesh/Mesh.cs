// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Assets;
using CommunityToolkit.Mvvm.ComponentModel;
using static CMiX.Core.ControlExtensions;

namespace CMiX.Core.Assets.Mesh
{
    public partial class Mesh : ObservableRecipient, IControl, IDisposable
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
                    GenericValue<float> explodeStrength,
                    PrefabManager displacementTextureManager,
                    GenericValue<float> displacementStrength,
                    GenericValue<float> flatNormals)
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
            DisplacementTextureManager = displacementTextureManager;
            DisplacementStrength = displacementStrength;
            FlatNormals = flatNormals;
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
        public PrefabManager DisplacementTextureManager { get; set; }
        public GenericValue<float> DisplacementStrength { get; set; }
        public GenericValue<float> FlatNormals { get; set; }

        public IControlModel ToModel() => new MeshModel
        {
            ID = ID,
            MeshTypeSelector = (GenericValueModel<MeshType>)MeshTypeSelector.ToModel(),
            Geometry = (GenericValueModel<IAsset>)Geometry.ToModel(),
            Text3DSettings = (Text3DSettingsModel)Text3DSettings.ToModel(),
            Scale = (Vector3Model)Scale.ToModel(),
            Offset = (Vector3Model)Offset.ToModel(),
            Radius = (GenericValueModel<float>)Radius.ToModel(),
            Height = (GenericValueModel<float>)Height.ToModel(),
            Thickness = (GenericValueModel<float>)Thickness.ToModel(),
            Tessellation = (GenericValueModel<int>)Tessellation.ToModel(),
            TessellationXY = (Integer2Model)TessellationXY.ToModel(),
            GenerateBackFace = (GenericValueModel<bool>)GenerateBackFace.ToModel(),
            Visibility = (GenericValueModel<bool>)Visibility.ToModel(),
            ExplodeStrength = (GenericValueModel<float>)ExplodeStrength.ToModel(),
            ExplodeTriangleTextureManager = (PrefabManagerModel)ExplodeTriangleTextureManager.ToModel(),
            DisplacementStrength = (GenericValueModel<float>)DisplacementStrength.ToModel(),
            DisplacementTextureManager = (PrefabManagerModel)DisplacementTextureManager.ToModel(),
            FlatNormals = (GenericValueModel<float>)FlatNormals.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (MeshModel)model;
            ID = m.ID;
            MeshTypeSelector.FromModel(m.MeshTypeSelector);
            Geometry.FromModel(m.Geometry);
            Text3DSettings.FromModel(m.Text3DSettings);
            Scale.FromModel(m.Scale);
            Offset.FromModel(m.Offset);
            Radius.FromModel(m.Radius);
            Height.FromModel(m.Height);
            Thickness.FromModel(m.Thickness);
            Tessellation.FromModel(m.Tessellation);
            TessellationXY.FromModel(m.TessellationXY);
            GenerateBackFace.FromModel(m.GenerateBackFace);
            Visibility.FromModel(m.Visibility);

            ExplodeStrength.FromModel(m.ExplodeStrength);
            LoadManager(ExplodeTriangleTextureManager, m.ExplodeTriangleTextureManager);

            DisplacementStrength.FromModel(m.DisplacementStrength);
            LoadManager(DisplacementTextureManager, m.DisplacementTextureManager);

            FlatNormals.FromModel(m.FlatNormals);
        }
        public void Dispose() => DisposeAll(ExplodeTriangleTextureManager, DisplacementTextureManager);
    }
}
