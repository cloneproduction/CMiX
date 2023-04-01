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
    public partial class Mesh : ObservableRecipient, IRecipient<MessageRequestControl>, IEntity, IPrefab
    {
        public Mesh(MeshModel meshModel, CompositionService compositionService)
        {
            ID = meshModel.ID;
            Name = new StringValue(meshModel.Name);
            IsRenaming = new BooleanValue(meshModel.IsRenaming);
            IsSelected = new BooleanValue(meshModel.IsSelected);
            isExpanded = true;
            MeshTypeSelector = new GenericValue<MeshType>(meshModel.MeshTypeSelector);
            Scale = new Vector3(meshModel.Scale);
            Offset = new Vector3(meshModel.Offset);
            Radius = new FloatValue(meshModel.Radius);
            Height = new FloatValue(meshModel.Height);
            Thickness = new FloatValue(meshModel.Thickness);
            Tessellation = new IntegerValue(meshModel.Tessellation);
            TessellationXY = new Integer2(meshModel.TessellationXY);
            GenerateBackFace = new BooleanValue(meshModel.GenerateBackFace);
            Visibility = new BooleanValue(meshModel.Visibility);
            TransformModifierManager = new ModifierManager(meshModel.TransformModifierManager, new ModifierFactory(compositionService), compositionService);
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

        public void Receive(MessageRequestControl message)
        {
            if (message.ID == this.ID)
                message.Reply(this);
        }
    }
}
