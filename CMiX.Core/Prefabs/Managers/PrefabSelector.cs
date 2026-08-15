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
    public partial class PrefabSelector : PrefabManagerBase, IDisposable
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

        // Ends this selector's lifetime, the counterpart of PrefabManager.Dispose. Called from the
        // Dispose of the control that owns the selector, which the delete path only reaches once the
        // undo stack has dropped the delete, so the dropped selection can no longer be restored: no
        // undo entry is pushed here and the selected control is disposed as soon as nothing else
        // references it. Clearing the selection alone only let go of this selector's own reference
        // and left the control itself untouched, so a control that owns managers of its own, a
        // material with its two texture slots for instance, kept their contents registered in the
        // repository forever and a new project could never empty them.
        public void Dispose()
        {
            var previousItem = _selectedItem;
            ControlMessenger.SendMessage(MessageFactory.CreateMessage<MessageRemoveSelectedItem>(ID));
            SetSelectedItemInternal(null);
            OwnedControl.DisposeIfOrphaned(ControlRepository, previousItem);
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
