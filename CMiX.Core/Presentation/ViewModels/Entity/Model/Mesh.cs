// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CMiX.Core.Models.BaseControls;
using CMiX.Core.Network.Messages;
using CMiX.Core.Presentation.ViewModels.BaseControl;
using CMiX.Core.Presentation.ViewModels.Beat;
using CMiX.Core.Presentation.ViewModels.Modifiers;
using CMiX.Core.Presentation.ViewModels.Network;
using CMiX.Core.Presentation.ViewModels.Prefab;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.Presentation.ViewModels
{
    public class Mesh : ObservableRecipient, IRecipient<IMessage>, IEntity, IPrefab
    {
        public Mesh(MeshModel meshModel)
        {
            ID = meshModel.ID;
            Name = this.GetType().Name;
            IsRenaming = false;
            IsExpanded = true;

            MeshTypeSelector = new ComboBox<MeshType>(meshModel.MeshTypeSelector);

            Scale = new VectorXYZ(meshModel.Scale);
            Offset = new VectorXYZ(meshModel.Offset);

            Radius = new Slider(nameof(Radius), meshModel.Radius);
            Height = new Slider(nameof(Height), meshModel.Height);
            Thickness = new Slider(nameof(Thickness), meshModel.Thickness);

            Tessellation = new Counter(meshModel.Tessellation);
            TessellationX = new Counter(meshModel.TessellationX);
            TessellationY = new Counter(meshModel.TessellationY);
            GenerateBackFace = new ToggleButton(meshModel.GenerateBackFace);
            Visibility = new ToggleButton(meshModel.Visibility);

            TransformModifierManager = new ModifierManager(meshModel.TransformModifierManager, new TransformModifierFactory());
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

        private Material _material;
        public Material Material
        {
            get => _material;
            set
            {
                SetProperty(ref _material, value);
                WeakReferenceMessenger.Default.Send<IMessage, int>(new MessageSelectedMaterial(this.ID, Material.ID), MessageType.Out);
            }
        }


        private Coloration _coloration;
        public Coloration Coloration
        {
            get => _coloration;
            set
            {
                SetProperty(ref _coloration, value);
                //WeakReferenceMessenger.Default.Send<IMessage, int>(new MessageSelectedMaterial(this.ID, Material.ID), MessageType.Out);
            }
        }


        public ModifierManager TransformModifierManager { get; set; }

        public ComboBox<MeshType> MeshTypeSelector { get; set; }

        public VectorXYZ Scale { get; set; }
        public VectorXYZ Offset { get; set; }

        public Slider Radius { get; set; }
        public Slider Height { get; set; }
        public Slider Thickness { get; set; }

        public Counter Tessellation { get; set; }
        public Counter TessellationX { get; set; }
        public Counter TessellationY { get; set; }

        public ToggleButton GenerateBackFace { get; set; }
        public ToggleButton Visibility { get; set; }


        public void SetMasterBeat(MasterBeat masterBeat)
        {
            TransformModifierManager.SetMasterBeat(masterBeat);
        }


        public IModel GetModel()
        {
            MeshModel meshModelModel = new MeshModel();
            meshModelModel.ID = ID;

            meshModelModel.MeshTypeSelector = (ComboBoxModel<MeshType>)MeshTypeSelector.GetModel();

            meshModelModel.Scale = (VectorXYZModel)Scale.GetModel();
            meshModelModel.Offset = (VectorXYZModel)Offset.GetModel();
            meshModelModel.Radius = (SliderModel)Radius.GetModel();
            meshModelModel.Height = (SliderModel)Height.GetModel();
            meshModelModel.Thickness = (SliderModel)Thickness.GetModel();
            meshModelModel.Tessellation = (CounterModel)Tessellation.GetModel();
            meshModelModel.TessellationX = (CounterModel)TessellationX.GetModel();
            meshModelModel.TessellationY = (CounterModel)TessellationY.GetModel();
            meshModelModel.GenerateBackFace = (ToggleButtonModel)GenerateBackFace.GetModel();
            meshModelModel.Visibility = (ToggleButtonModel)Visibility.GetModel();
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
            TessellationX.SetViewModel(meshModelModel.TessellationX);
            TessellationY.SetViewModel(meshModelModel.TessellationY);
            GenerateBackFace.SetViewModel(meshModelModel.GenerateBackFace);
            Visibility.SetViewModel(meshModelModel.Visibility);

            TransformModifierManager.SetViewModel(meshModelModel.TransformModifierManager);
        }

        public void Dispose()
        {

        }

        public void Receive(IMessage message)
        {
            if (message.ID != ID)
                return;
        }
    }
}
