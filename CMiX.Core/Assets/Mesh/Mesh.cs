// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.ViewModels
{
    public partial class Mesh : ObservableRecipient, IControl
    {
        public Mesh(GenericValue<MeshType> meshTypeSelector, 
            Vector3 scale, 
            Vector3 offset,
            FloatValue radius,
            FloatValue height,
            FloatValue thickness,
            IntegerValue tessellation,
            Integer2 tessellationXY,
            BooleanValue generateBackFace,
            BooleanValue visibility)
        {
            MeshTypeSelector = meshTypeSelector;// new GenericValue<MeshType>();
            Scale = scale; // new Vector3(1.0f, 1.0f, 1.0f);
            Offset = offset; // new Vector3();
            Radius = radius; // new FloatValue(1.0f);
            Height = height; // new FloatValue(1.0f);
            Thickness = thickness; // new FloatValue(1.0f);
            Tessellation = tessellation; // new IntegerValue(16);
            TessellationXY = tessellationXY; // new Integer2(16, 16);
            GenerateBackFace = generateBackFace; // new BooleanValue();
            Visibility = visibility; // new BooleanValue();
        }

        [ObservableProperty]
        private bool isExpanded = false;

        public Guid ID { get; set; } = Guid.NewGuid();
        public GenericValue<MeshType> MeshTypeSelector { get; set; }
        public Vector3 Scale { get; set; }
        public Vector3 Offset { get; set; }
        public FloatValue Radius { get; set; }
        public FloatValue Height { get; set; }
        public FloatValue Thickness { get; set; }
        public IntegerValue Tessellation { get; set; }
        public Integer2 TessellationXY { get; set; }
        public BooleanValue GenerateBackFace { get; set; }
        public BooleanValue Visibility { get; set; }
    }
}
