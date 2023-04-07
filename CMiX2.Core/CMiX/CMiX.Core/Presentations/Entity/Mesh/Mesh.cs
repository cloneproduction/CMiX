// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Networking.Messages;
using CMiX.Core.Presentations.Modifiers.Transform;
using CMiX.Core.Presentations.Prefabs;
using CMiX.Core.Presentations.Service;
using CMiX.Core.Presentations.ViewModels.BaseControl;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.Presentations.ViewModels
{
    public partial class Mesh : ObservableRecipient, IEntity, IPrefab
    {
        public Mesh(MeshModel meshModel, CompositionService compositionService)
        {
            ID = meshModel.ID;
            Name = new StringValue(meshModel.Name, compositionService);
            IsRenaming = new BooleanValue(meshModel.IsRenaming, compositionService);
            IsSelected = new BooleanValue(meshModel.IsSelected, compositionService);
            MeshTypeSelector = new GenericValue<MeshType>(meshModel.MeshTypeSelector, compositionService);
            Scale = new Vector3(meshModel.Scale, compositionService);
            Offset = new Vector3(meshModel.Offset, compositionService);
            Radius = new FloatValue(meshModel.Radius, compositionService);
            Height = new FloatValue(meshModel.Height, compositionService);
            Thickness = new FloatValue(meshModel.Thickness, compositionService);
            Tessellation = new IntegerValue(meshModel.Tessellation, compositionService);
            TessellationXY = new Integer2(meshModel.TessellationXY, compositionService);
            GenerateBackFace = new BooleanValue(meshModel.GenerateBackFace, compositionService);
            Visibility = new BooleanValue(meshModel.Visibility, compositionService);
            TransformModifierManager = new ModifierManager(meshModel.TransformModifierManager, new ModifierFactory(compositionService), compositionService);
            isExpanded = true;
        }

        [ObservableProperty]
        private bool isExpanded;

        public Guid ID { get; set; }
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

        public void Dispose()
        {

        }
    }
}
