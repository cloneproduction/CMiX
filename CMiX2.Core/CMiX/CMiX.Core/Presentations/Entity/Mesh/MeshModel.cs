// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControl;
using CMiX.Core.Presentations.Prefabs;
using CMiX.Core.Presentations.ViewModels.Modifiers;

namespace CMiX.Core.Presentations.ViewModels
{
    public class MeshModel : IPrefabModel
    {
        public MeshModel()
        {
            ID = Guid.NewGuid();

            MeshTypeSelector = new GenericValueModel<MeshType>(MeshType.Plane);
            Scale = new Vector3Model(1.0f, 1.0f, 1.0f);
            Offset = new Vector3Model(0.0f, 0.0f, 0.0f);
            Radius = new FloatValueModel(1.0f);
            Height = new FloatValueModel(1.0f);
            Thickness = new FloatValueModel(1.0f);
            Tessellation = new IntegerValueModel(16);
            TessellationXY = new Integer2Model(16, 16);

            GenerateBackFace = new BooleanValueModel(true);
            Visibility = new BooleanValueModel(true);
            TransformModifierManager = new ModifierManagerModel();
        }

        public Guid ID { get; set; }
        public GenericValueModel<MeshType> MeshTypeSelector { get; internal set; }
        public Vector3Model Scale { get; internal set; }
        public Vector3Model Offset { get; internal set; }
        public FloatValueModel Radius { get; internal set; }
        public FloatValueModel Height { get; internal set; }
        public FloatValueModel Thickness { get; internal set; }
        public IntegerValueModel Tessellation { get; internal set; }
        public Integer2Model TessellationXY { get; internal set; }
        public BooleanValueModel GenerateBackFace { get; internal set; }
        public BooleanValueModel Visibility { get; internal set; }
        public ModifierManagerModel TransformModifierManager { get; internal set; }
    }
}
