using System.Windows.Input;
using CMiX.Core.Networking;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Prefabs.Messages;
using CMiX.Core.Undo;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.Prefabs.Managers
{
    public abstract partial class PrefabManagerBase : ReceivableControl, IControl, IRecipient<IMessage>
    {
        protected PrefabManagerBase(ControlRepository controlRepository,
                                     ControlMessenger controlMessenger,
                                     MessageFactory messageFactory,
                                     ControlActivationService activationService,
                                     UndoManager undoManager)
        {
            ControlRepository = controlRepository;
            ControlMessenger = controlMessenger;
            MessageFactory = messageFactory;
            UndoManager = undoManager;
            IsActive = false;
            activationService.Register(this);
        }

        public ControlRepository ControlRepository { get; }
        public ControlMessenger ControlMessenger { get; }
        public MessageFactory MessageFactory { get; }

        public abstract ICommand AddItemCommand { get; }
        public abstract ICommand RemoveSelectedItemCommand { get; }
        public abstract IControl SelectedItem { get; set; }
        public Guid ID { get; set; }

        public abstract void Receive(IMessage message);
        public abstract IControlModel ToModel();
        public abstract void FromModel(IControlModel model);
    }
}

