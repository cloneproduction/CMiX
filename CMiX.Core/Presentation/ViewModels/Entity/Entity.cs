// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CMiX.Core.Network.Messages;
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


        private Material _material;
        public Material Material
        {
            get => _material;
            set
            {
                SetProperty(ref _material, value);
                SendMessage(new MessageChangePrefab(this.ID, value, nameof(Material)));
            }
        }



        public void SetViewModel(IModel model)
        {
            EntityModel entityModel = model as EntityModel;
            this.ID = entityModel.ID;
            this.Mesh.SetViewModel(entityModel.Mesh);
            this.TransformSRT.SetViewModel(entityModel.TransformSRT);
            this.MaterialManager.SetViewModel(entityModel.MaterialManager);
            this.ModifierManager.SetViewModel(entityModel.ModifierManager);
        }

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


        public void ChangePrefab(string propertyName, Guid prefabID)
        {
            var prefab = CompositionService.GetPrefab(prefabID);
            this.GetType().GetProperty(propertyName).SetValue(this, prefab);
        }



        bool CanSend = true;
        public void SendMessage(IMessage message)
        {
            if (CanSend)
                WeakReferenceMessenger.Default.Send<IMessage, int>(message, MessageType.Out);
        }


        public void Receive(MessageRequestControl message)
        {
            if (message.ID == this.ID && !message.HasReceivedResponse)
                message.Reply(this);
        }
    }
}
