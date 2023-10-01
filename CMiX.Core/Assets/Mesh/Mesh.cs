// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.Services;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.ViewModels
{
    public partial class Mesh : ObservableRecipient
    {
        public Mesh(CompositionService compositionService)
        {
            Name = new StringValue();

            IsRenaming = new BooleanValue();
            IsSelected = new BooleanValue();
            MeshTypeSelector = new GenericValue<MeshType>();
            Scale = new Vector3(1.0f, 1.0f, 1.0f);
            Offset = new Vector3();
            Radius = new FloatValue(1.0f);
            Height = new FloatValue(1.0f);
            Thickness = new FloatValue(1.0f);
            Tessellation = new IntegerValue(16);
            TessellationXY = new Integer2(16, 16);
            GenerateBackFace = new BooleanValue();
            Visibility = new BooleanValue();
        }

        [ObservableProperty]
        private bool isExpanded = false;

        public Guid ID { get; set; } = Guid.NewGuid();
        public BooleanValue IsRenaming { get; set; }
        public StringValue Name { get; set; }
        public BooleanValue IsSelected { get; set; }
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
