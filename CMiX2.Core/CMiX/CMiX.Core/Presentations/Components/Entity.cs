// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Presentation.ViewModels.Modifiers;
using CMiX.Core.Presentation.ViewModels.Network;
using CMiX.Core.Presentation.ViewModels.Prefab;
using CMiX.Core.Presentation.ViewModels.Service;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.Presentation.ViewModels.Components
{
    public class Entity : ObservableRecipient, IPrefab, IRecipient<MessageRequestControl>
    {
        public Entity(EntityModel entityModel, CompositionService compositionService)
        {
            ID = entityModel.ID;
            Name = this.GetType().Name + ID.ToString();
            CompositionService = compositionService;

            TransformSRT = new TransformSRT(entityModel.TransformSRT);
            Mesh = new Mesh(entityModel.Mesh, compositionService);
            MaterialManager = new PrefabManager<Material>(entityModel.MaterialManager, compositionService);

            ModifierManager = new ModifierManager(entityModel.ModifierManager, new ModifierFactory(compositionService));
            IsActive = true;
        }


        public Guid ID { get; set; }
        public CompositionService CompositionService { get; set; }

        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private bool _isRenaming;
        public bool IsRenaming
        {
            get => _isRenaming;
            set => SetProperty(ref _isRenaming, value);
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }


        public PrefabManager<Material> MaterialManager { get; set; }
        public TransformSRT TransformSRT { get; set; }
        public ModifierManager ModifierManager { get; set; }
        public Mesh Mesh { get; set; }


        public IModel GetModel()
        {
            EntityModel entityModel = new EntityModel() ;
            entityModel.ID = ID;
            entityModel.Mesh = (MeshModel)Mesh.GetModel();
            entityModel.TransformSRT = (TransformSRTModel)TransformSRT.GetModel();
            entityModel.MaterialManager = (PrefabManagerModel)MaterialManager.GetModel();
            entityModel.ModifierManager = (ModifierManagerModel)ModifierManager.GetModel();
            return entityModel;
        }

        public void Receive(MessageRequestControl message)
        {
            if (message.ID == this.ID && !message.HasReceivedResponse)
                message.Reply(this);
        }
    }
}
