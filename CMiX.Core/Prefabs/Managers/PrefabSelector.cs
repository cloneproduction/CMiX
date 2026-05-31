// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows.Input;
using CMiX.Core.Networking;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Prefabs.Messages;
using CMiX.Core.Undo;
using CommunityToolkit.Mvvm.Input;

namespace CMiX.Core.Prefabs.Managers
{
    public partial class PrefabSelector : PrefabManagerBase
    {
        public PrefabSelector(ControlRepository controlRepository,
                              ControlFactory controlFactory,
                              ControlMessenger controlMessenger,
                              MessageFactory messageFactory,
                              ControlActivationService activationService,
                              UndoManager undoManager)
            : base(controlRepository, controlMessenger, messageFactory, activationService, undoManager)
        {
            ID = Guid.NewGuid();
            ControlFactory = controlFactory;
            AddItemCommand = new RelayCommand<Type>(AddItem);
            RemoveSelectedItemCommand = new RelayCommand(RemoveSelectedItem);
        }

        public ControlFactory ControlFactory { get; }
        public override ICommand AddItemCommand { get; }
        public override ICommand RemoveSelectedItemCommand { get; }

        private IControl _selectedItem;
        public override IControl SelectedItem
        {
            get => _selectedItem;
            set => SetSelectedItem(value);
        }

        private void AddItem(Type type)
        {
            var control = ControlFactory.Create(type);
            ControlMessenger.SendMessage(MessageFactory.CreateMessage<MessageAddItem>(ID, control, 0));
            SelectedItem = control;
        }

        private void RemoveSelectedItem()
        {
            ControlMessenger.SendMessage(MessageFactory.CreateMessage<MessageRemoveSelectedItem>(ID));
            SelectedItem = null;
        }

        public override void Receive(IMessage message)
        {
            if (message.ID != ID) return;

            ReceiveWithoutEcho(() =>
            {
                if (message is MessageRemoveSelectedItem)
                {
                    if (_selectedItem != null) ControlRepository.RemoveControl(_selectedItem, ID);
                    _selectedItem = null;
                }
                else
                {
                    _selectedItem = message switch
                    {
                        MessageAddItem addItem => CreateAndRegister(addItem.Model),
                        MessageSelectedItemChanged changed => ControlRepository.GetControl(changed.ControlID),
                        _ => _selectedItem
                    };
                }
                OnPropertyChanged(nameof(SelectedItem));
            });
        }

        private IControl CreateAndRegister(IControlModel model)
        {
            var control = ControlFactory.Create(model);
            ControlRepository.AddControl(control, ID);
            return control;
        }

        private void SetSelectedItemInternal(IControl value)
        {
            if (_selectedItem == value) return;
            if (_selectedItem != null) ControlRepository.RemoveControl(_selectedItem, ID);
            _selectedItem = value;
            if (_selectedItem != null) ControlRepository.AddControl(_selectedItem, ID);
            OnPropertyChanged(nameof(SelectedItem));
        }

        private void SetSelectedItem(IControl value)
        {
            var previousItem = _selectedItem;
            SetSelectedItemInternal(value);
            ControlMessenger.SendMessage(MessageFactory.CreateMessage<MessageSelectedItemChanged>(
                ID, _selectedItem, _selectedItem != null ? 0 : -1));
            if (UndoManager?.IsApplying == true) return;
            UndoManager?.Push(new SelectPrefabCommand(this, ControlMessenger, MessageFactory, previousItem, _selectedItem));
        }

        public override IControlModel ToModel() => new PrefabSelectorModel
        {
            ID = ID,
            SelectedItemID = SelectedItem?.ID ?? Guid.Empty,
            SelectedItemModel = SelectedItem?.ToModel()
        };

        // FromModel is only called pre-binding (during loading), so OnPropertyChanged is not needed here.
        public override void FromModel(IControlModel model)
        {
            var m = (PrefabSelectorModel)model;
            ID = m.ID;
            if (m.SelectedItemID == Guid.Empty) return;

            var control = ControlRepository.GetControl(m.SelectedItemID);
            if (control != null)
            {
                _selectedItem = control;
                return;
            }

            if (m.SelectedItemModel == null) return;
            var created = ControlFactory.Create(m.SelectedItemModel);
            ControlRepository.AddControl(created, ID);
            _selectedItem = created;
        }
    }
}
