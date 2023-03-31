// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Networking.Messages;
using CMiX.Core.Presentations.Materials;
using CMiX.Core.Presentations.Prefabs;
using CMiX.Core.Presentations.ViewModels;
using CMiX.Core.Presentations.Service;
using CMiX.Core.Presentations.Modifiers.Transform;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.Presentations.Components
{
    public partial class Entity : ObservableRecipient, IPrefab, IRecipient<MessageRequestControl>
    {
        public Entity(EntityModel entityModel, CompositionService compositionService)
        {
            ID = entityModel.ID;
            name = GetType().Name + ID.ToString();
            CompositionService = compositionService;

            TransformSRT = new TransformSRT(entityModel.TransformSRT);
            Mesh = new Mesh(entityModel.Mesh, compositionService);
            MaterialManager = new PrefabManager<Material>(entityModel.MaterialManager, compositionService);

            ModifierManager = new ModifierManager(entityModel.ModifierManager, new ModifierFactory(compositionService), compositionService);
            IsActive = true;
        }


        public Guid ID { get; set; }
        public CompositionService CompositionService { get; set; }


        [ObservableProperty]
        private string name;

        [ObservableProperty]
        private bool isRenaming;

        [ObservableProperty]
        private bool isSelected;


        public PrefabManager<Material> MaterialManager { get; set; }
        public TransformSRT TransformSRT { get; set; }
        public ModifierManager ModifierManager { get; set; }
        public Mesh Mesh { get; set; }


        public void Receive(MessageRequestControl message)
        {
            if (message.ID == ID && !message.HasReceivedResponse)
                message.Reply(this);
        }
    }
}
