// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.Transformation.Modifiers;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.ViewModels
{
    public partial class Mesh : ObservableRecipient
    {
        public Mesh()
        {
            Name = new StringValue();
            IsRenaming = new BooleanValue();
            IsSelected = new BooleanValue();
            MeshTypeSelector = new GenericValue<MeshType>();
            Scale = new Vector3();
            Offset = new Vector3();
            Radius = new FloatValue();
            Height = new FloatValue();
            Thickness = new FloatValue();
            Tessellation = new IntegerValue();
            TessellationXY = new Integer2();
            GenerateBackFace = new BooleanValue();
            Visibility = new BooleanValue();
            TransformModifierManager = new ModifierManager(new ModifierFactory());
        }

        [ObservableProperty]
        private bool isExpanded = false;

        public Guid ID { get; set; } = Guid.NewGuid();
        public BooleanValue IsRenaming { get; set; }
        public StringValue Name { get; set; }
        public BooleanValue IsSelected { get; set; }
        public ModifierManager TransformModifierManager { get; set; }
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
