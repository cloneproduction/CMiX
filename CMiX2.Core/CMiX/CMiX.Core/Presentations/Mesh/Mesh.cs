// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CMiX.Core.Models.BaseControls;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Presentation.ViewModels.BaseControl;
using CMiX.Core.Presentation.ViewModels.Modifiers;
using CMiX.Core.Presentation.ViewModels.Prefab;
using CMiX.Core.Presentation.ViewModels.Service;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.Presentation.ViewModels
{
    public class Mesh : ObservableRecipient, IRecipient<MessageRequestControl>, IEntity, IPrefab
    {
        public Mesh(MeshModel meshModel, CompositionService compositionService)
        {
            ID = meshModel.ID;
            Name = this.GetType().Name;
            IsRenaming = false;
            IsExpanded = true;

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


        public Guid ID { get; set; }

        private bool _isRenaming;
        public bool IsRenaming
        {
            get => _isRenaming;
            set => SetProperty(ref _isRenaming, value);
        }

        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        private bool _isExpanded;
        public bool IsExpanded
        {
            get => _isExpanded;
            set => SetProperty(ref _isExpanded, value);
        }


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
