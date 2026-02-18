// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows.Input;
using CMiX.Core.Networking;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Prefabs.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.Prefabs.Managers
{
    public partial class PrefabManager : ObservableRecipient, IControl, IRecipient<IMessage>
    {
        public PrefabManager()
        {
            
        }
        public PrefabManager(ManagerData managerData,
                             ControlRepository controlRepository,
                             ControlFactory controlFactory,
                             ControlMessenger controlMessenger,
                             MessageFactory messageFactory,
                             MessageCollectionManagerHandler messageCollectionManagerHandler)
        {
            ID = managerData.ID;

            MessageCollectionManagerHandler = messageCollectionManagerHandler;

            ControlMessenger = controlMessenger;
            ControlRepository = controlRepository;
            ControlFactory = controlFactory;
            MessageFactory = messageFactory;
            ManagerData = managerData;
            ManagerReorderService = new ManagerReorderService(this);

            AddItemCommand = new RelayCommand<Type>(AddItem);
            DeleteItemCommand = new RelayCommand<IControl>(DeleteItem);
            ReplaceSelectedItemCommand = new RelayCommand<IControl>(ReplaceItem);
            RemoveSelectedItemCommand = new RelayCommand(RemoveSelectedItem);
            ResetItemCommand = new RelayCommand<IControl>(ResetItem);
            isExpanded = true;
            IsActive = true;
        }

        public Guid ID { get; set; }
        public ICommand AddItemCommand { get; set; }
        public ICommand DeleteItemCommand { get; set; }
        public ICommand RemoveSelectedItemCommand { get; set; }
        public ICommand ReplaceSelectedItemCommand { get; set; }
        public ICommand ResetItemCommand { get; set; }

        

        [ObservableProperty]
        private bool isExpanded;
        public ManagerReorderService ManagerReorderService { get; set; }
        public ControlMessenger ControlMessenger { get; set; }
        public ControlRepository ControlRepository { get; set; }
        public ControlFactory ControlFactory { get; set; }
        public MessageFactory MessageFactory { get; set; }
        public ManagerData ManagerData { get; set; }
        public MessageCollectionManagerHandler MessageCollectionManagerHandler { get; set; }


        private IControl _selectedItem;
        public IControl SelectedItem
        {
            get => _selectedItem;
            set
            {
                SetProperty(ref _selectedItem, value);
                if (IsActive)
                {
                    var message = MessageFactory.CreateMessage<MessageSelectedItemChanged>(ManagerData.ID, SelectedItem, ManagerData.SelectedIndex);
                    ControlMessenger.SendMessage(message);
                }
            }
        }

        public void ResetItem(IControl control)
        {
            var newControl = ControlFactory.Create(control.GetType());
            var index = ManagerData.Items.IndexOf(control);
            ManagerData.Items[index] = newControl;
        }

        public void SelectedItemChanged(IControlModel controlModel, int index)
        {
            var control = controlModel != null ? ControlRepository.GetControl(controlModel.ID) : null;
            SelectedItem = control;

            ManagerData.SelectedIndex = control != null ? index : -1;

            // Add to items if not already present
            if (control != null && ManagerData.Items.All(x => x.ID != control.ID))
                ManagerData.Items.Add(control);
        }


        public void ReplaceItem(IControl control)
        {
            if (control is not IPrefab prefab)
                throw new ArgumentException("Control must implement IPrefab", nameof(control));

            var items = ManagerData.Items;
            IMessage message;

            if (items.Count == 0)
            {
                items.Add(prefab);
                message = MessageFactory.CreateMessage<MessageAddItem>(ManagerData.ID, prefab);
            }
            else
            {
                var index = ManagerData.SelectedIndex;
                items[index] = prefab;
                message = MessageFactory.CreateMessage<MessageReplaceItem>(ManagerData.ID, prefab, index);
            }

            ControlMessenger.SendMessage(message);
            SelectedItem = prefab;
        }


        public void ReplaceItem(IControlModel controlModel, int index)
        {
            var prefab = ControlRepository.GetControl(controlModel.ID);
            var items = ManagerData.Items;

            if (prefab == null)
            {
                prefab = ControlFactory.Create(controlModel);
                ControlRepository.AddControl(prefab);
            }

            if (items.Count == 0)
                items.Add(prefab);
            else
                items[index] = prefab;

            SelectedItem = prefab;
            ManagerData.SelectedIndex = index;
        }


        public void AddItem(Type type)
        {
            var prefab = ControlFactory.Create(type);
            ControlRepository.AddControl(prefab);
            var items = ManagerData.Items;

            IMessage message = null;

            if (SelectedItem is EmptyPrefab emptyPrefab && prefab is IPrefab pre && prefab is not EmptyPrefab)
            {
                items[items.IndexOf(emptyPrefab)] = prefab;
                message = MessageFactory.CreateMessage<MessageReplaceItem>(ManagerData.ID, pre, ManagerData.Items.IndexOf(prefab));
            }
            else
            {
                items.Add(prefab);
                message = MessageFactory.CreateMessage<MessageAddItem>(ManagerData.ID, prefab);
            }

            ControlMessenger.SendMessage(message);
            SelectedItem = prefab;
            ManagerData.SelectedIndex = items.IndexOf(prefab);
        }

        public void AddItem(IControlModel controlModel)
        {
            var items = ManagerData.Items;
            var prefab = ControlFactory.Create(controlModel);
            ControlRepository.AddControl(prefab);

            if (SelectedItem is EmptyPrefab empty)
            {
                var index = items.IndexOf(empty);
                if (index >= 0)
                    items[index] = prefab;
                else
                    items.Add(prefab); // fallback if somehow empty prefab is not in the list
            }
            else
                items.Add(prefab);

            SelectedItem = prefab;
        }

        public void RemoveSelectedItem()
        {
            SelectedItem = null;
            var message = MessageFactory.CreateMessage<MessageRemoveSelectedItem>(ManagerData.ID);
            ControlMessenger.SendMessage(message);
        }

        public void DeleteItem(IControl control)
        {
            if (control == null)
                return;

            var items = ManagerData.Items;
            var index = items.IndexOf(control);

            if (index < 0) // control not found
                return;

            items.RemoveAt(index);

            var message = MessageFactory.CreateMessage<MessageRemoveItem>(ManagerData.ID, control);
            ControlMessenger.SendMessage(message);

            if (items.Count == 0)
            {
                SelectedItem = null;
                ManagerData.SelectedIndex = -1;
            }
            else
            {
                var newIndex = index == 0 ? 0 : index - 1;
                SelectedItem = items[newIndex];
                ManagerData.SelectedIndex = newIndex;
            }
        }

        public void DeleteItem(Guid id)
        {
            var items = ManagerData.Items;
            var prefab = items.FirstOrDefault(x => x.ID == id);

            if (prefab == null)
                return;

            var index = items.IndexOf(prefab);
            items.Remove(prefab);

            if (items.Count == 0)
            {
                SelectedItem = null;
                ManagerData.SelectedIndex = -1;
            }
            else
            {
                var newIndex = index == 0 ? 0 : index - 1;
                SelectedItem = items[newIndex];
                ManagerData.SelectedIndex = newIndex;
            }
        }

        public void MoveItem(int oldIndex, int newIndex)
        {
            var items = ManagerData.Items;

            if (items.Count == 0)
                return;

            items.Move(oldIndex, newIndex);
        }

        public void SelectedItemIsRenaming()
        {
            var prefab = SelectedItem as IPrefab;
            prefab.PrefabService.IsRenaming.Value = true;
        }

        public void Rename()
        {
            if (SelectedItem is IPrefab prefab && prefab.GetType() != typeof(EmptyPrefab))
                prefab.PrefabService.IsRenaming.Value = true;
        }

        public void Receive(IMessage message)
        {
            if (message is not IMessageManager messageManager || this.ManagerData.ID != message.ID)
                return;

            IsActive = false;
            MessageCollectionManagerHandler.Handle(this, message);
            IsActive = true;

            Console.WriteLine($"Message {message.GetType().Name} handled by ManagerMessenger");
        }
    }
}
