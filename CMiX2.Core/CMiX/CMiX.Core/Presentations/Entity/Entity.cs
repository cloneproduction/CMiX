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
using CMiX.Core.Presentations.ViewModels.BaseControl;

namespace CMiX.Core.Presentations.Components
{
    public partial class Entity : ObservableRecipient, IPrefab, IRecipient<MessageRequestControl>
    {
        public Entity(EntityModel entityModel, CompositionService compositionService)
        {
            ID = entityModel.ID;
            Name = new StringValue(entityModel.Name, compositionService);
            IsSelected = new BooleanValue(entityModel.IsSelected, compositionService);
            IsRenaming = new BooleanValue(entityModel.IsRenaming, compositionService);
            CompositionService = compositionService;

            TransformSRT = new TransformSRT(entityModel.TransformSRT);
            Mesh = new Mesh(entityModel.Mesh, compositionService);
            MaterialManager = new PrefabManager<Material>(entityModel.MaterialManager, compositionService);
            ModifierManager = new ModifierManager(entityModel.ModifierManager, new ModifierFactory(compositionService), compositionService);
            IsActive = true;
        }


        public Guid ID { get; set; }
        public CompositionService CompositionService { get; set; }
        public StringValue Name { get; set; }
        public BooleanValue IsRenaming { get; set; }
        public BooleanValue IsSelected { get; set; }
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
