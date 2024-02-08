// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;

namespace CMiX.Core.ViewModels
{
    public class MeshModel : IControlModel
    {
        public MeshModel()
        {
            ID = Guid.NewGuid();

            MeshTypeSelector = new GenericValueModel<MeshType>(MeshType.Plane);
            Scale = new Vector3Model(1.0f, 1.0f, 1.0f);
            Offset = new Vector3Model(0.0f, 0.0f, 0.0f);
            Radius = new GenericValueModel<float>(1.0f);
            Height = new GenericValueModel<float>(1.0f);
            Thickness = new GenericValueModel<float>(1.0f);
            Tessellation = new GenericValueModel<int>(16);
            TessellationXY = new Integer2Model(16, 16);

            GenerateBackFace = new GenericValueModel<bool>(true);
            Visibility = new GenericValueModel<bool>(true);
            Name = new GenericValueModel<string>("Mesh");
            IsRenaming = new GenericValueModel<bool>(false);
            IsSelected = new GenericValueModel<bool>(false);
        }

        public Guid ID { get; set; }
        public GenericValueModel<MeshType> MeshTypeSelector { get; internal set; }
        public Vector3Model Scale { get; set; }
        public Vector3Model Offset { get; set; }
        public GenericValueModel<float> Radius { get; set; }
        public GenericValueModel<float> Height { get; set; }
        public GenericValueModel<float> Thickness { get; set; }
        public GenericValueModel<int> Tessellation { get; set; }
        public Integer2Model TessellationXY { get; set; }
        public GenericValueModel<bool> GenerateBackFace { get; set; }
        public GenericValueModel<bool> Visibility { get; set; }
        public GenericValueModel<string> Name { get; set; }
        public GenericValueModel<bool> IsRenaming { get; set; }
        public GenericValueModel<bool> IsSelected { get; set; }
    }
}
