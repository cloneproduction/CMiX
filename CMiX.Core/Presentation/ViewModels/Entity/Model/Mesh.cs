// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CMiX.Core.Models.BaseControls;
using CMiX.Core.Network.Messages;
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

            TransformModifierManager = new ModifierManager(meshModel.TransformModifierManager, new ModifierFactory(compositionService));
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


        public IModel GetModel()
        {
            MeshModel meshModelModel = new MeshModel();
            meshModelModel.ID = ID;

            meshModelModel.MeshTypeSelector = (GenericValueModel<MeshType>)MeshTypeSelector.GetModel();

            meshModelModel.Scale = (Vector3Model)Scale.GetModel();
            meshModelModel.Offset = (Vector3Model)Offset.GetModel();
            meshModelModel.Radius = (FloatValueModel)Radius.GetModel();
            meshModelModel.Height = (FloatValueModel)Height.GetModel();
            meshModelModel.Thickness = (FloatValueModel)Thickness.GetModel();
            meshModelModel.Tessellation = (IntegerValueModel)Tessellation.GetModel();
            meshModelModel.TessellationXY = (Integer2Model)TessellationXY.GetModel();
            meshModelModel.GenerateBackFace = (BooleanValueModel)GenerateBackFace.GetModel();
            meshModelModel.Visibility = (BooleanValueModel)Visibility.GetModel();
            meshModelModel.TransformModifierManager = (ModifierManagerModel)TransformModifierManager.GetModel();

            return meshModelModel;
        }

        public void SetViewModel(IModel model)
        {
            MeshModel meshModelModel = new MeshModel();
            this.ID = meshModelModel.ID;

            MeshTypeSelector.SetViewModel(meshModelModel.MeshTypeSelector);

            Scale.SetViewModel(meshModelModel.Scale);
            Offset.SetViewModel(meshModelModel.Offset);
            Radius.SetViewModel(meshModelModel.Radius);
            Height.SetViewModel(meshModelModel.Height);
            Thickness.SetViewModel(meshModelModel.Thickness);
            Tessellation.SetViewModel(meshModelModel.Tessellation);
            TessellationXY.SetViewModel(meshModelModel.TessellationXY);
            GenerateBackFace.SetViewModel(meshModelModel.GenerateBackFace);
            Visibility.SetViewModel(meshModelModel.Visibility);

            TransformModifierManager.SetViewModel(meshModelModel.TransformModifierManager);
        }

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
