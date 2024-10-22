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
        public PrefabManager(ManagerData managerData,
                             ControlRepository controlRepository,
                             ControlFactory controlFactory,
                             ControlMessenger controlMessenger,
                             MessageFactory messageFactory)
        {
            ID = managerData.ID;

            MessageCollectionManagerHandler = new MessageCollectionManagerHandler();

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
            if (controlModel == null)
            {
                SelectedItem = null;
                ManagerData.SelectedIndex = index;
                return;
            }

            var control = ControlRepository.GetControl(controlModel.ID);
            SelectedItem = control;

            if(control == null)
            {
                ManagerData.SelectedIndex = -1;
                return;
            }

            ManagerData.SelectedIndex = index;
            if (!ManagerData.Items.Any(x => x.ID == control.ID) && control != null)
                ManagerData.Items.Add(SelectedItem);
        }


        public void ReplaceItem(IControl control)
        {
            var prefab = (IPrefab)control;
            var items = ManagerData.Items;

            if (items.Count == 0)
            {
                items.Add(prefab);
                var message = MessageFactory.CreateMessage<MessageAddItem>(ManagerData.ID, prefab);
                ControlMessenger.SendMessage(message);
            }
            else
            {
                var index = ManagerData.SelectedIndex;
                items[index] = prefab;
                var message = MessageFactory.CreateMessage<MessageReplaceItem>(ManagerData.ID, prefab, index);
                ControlMessenger.SendMessage(message);
            }

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
            {
                items.Add(prefab);
            }
            else
            {
                items[index] = prefab;
            }

            SelectedItem = prefab;
            ManagerData.SelectedIndex = index;
        }


        public void AddItem(Type type)
        {
            var prefab = ControlFactory.Create(type);
            ControlRepository.AddControl(prefab);

            var items = ManagerData.Items;

            if (SelectedItem is EmptyPrefab emptyPrefab && prefab is IPrefab pre)
            {
                items[items.IndexOf(emptyPrefab)] = prefab;
                var message = MessageFactory.CreateMessage<MessageReplaceItem>(ManagerData.ID, pre, ManagerData.Items.IndexOf(prefab));
                ControlMessenger.SendMessage(message);
            }
            else
            {
                items.Add(prefab);
                var message = MessageFactory.CreateMessage<MessageAddItem>(ManagerData.ID, prefab);
                ControlMessenger.SendMessage(message);
            }

            SelectedItem = prefab;
            ManagerData.SelectedIndex = items.IndexOf(prefab);
        }

        public void AddItem(IControlModel controlModel)
        {
            var selectedItem = SelectedItem;
            var items = ManagerData.Items;
            var prefab = ControlFactory.Create(controlModel);
            ControlRepository.AddControl(prefab);

            if (selectedItem is EmptyPrefab)
                items[items.IndexOf(selectedItem)] = prefab;
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
            var index = ManagerData.Items.IndexOf(control);

            if (control == null)
                return;

            ManagerData.Items.RemoveAt(index);
            var message = MessageFactory.CreateMessage<MessageRemoveItem>(ManagerData.ID, control);
            ControlMessenger.SendMessage(message);

            if (ManagerData.Items.Count == 0)
            {
                SelectedItem = null;
                ManagerData.SelectedIndex = -1;
                return;
            }

            if (index == 0)
            {
                SelectedItem = ManagerData.Items[0];
                ManagerData.SelectedIndex = 0;
                return;
            }

            if (index > 0)
            {
                SelectedItem = ManagerData.Items[index - 1];
                ManagerData.SelectedIndex = index - 1;
                return;
            }
        }

        public void DeleteItem(Guid id)
        {
            var prefab = ManagerData.Items.FirstOrDefault(x => x.ID == id);
            var index = ManagerData.Items.IndexOf(prefab);

            if (prefab == null)
                return;

            ManagerData.Items.Remove(prefab);

            if (ManagerData.Items.Count == 0)
            {
                SelectedItem = null;
                ManagerData.SelectedIndex = -1;
                return;
            }

            if (index == 0)
            {
                SelectedItem = ManagerData.Items[0];
                ManagerData.SelectedIndex = 0;
                return;
            }

            if (index > 0)
            {
                SelectedItem = ManagerData.Items[index - 1];
                ManagerData.SelectedIndex = index - 1;
                return;
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
            if (SelectedItem is IPrefab prefab)
            {
                if (prefab.GetType() != typeof(EmptyPrefab))
                    prefab.PrefabService.IsRenaming.Value = true;
            }
        }

        public void Receive(IMessage message)
        {
            if (message is not IMessageManager messageManager)
                return;

            if (this.ManagerData.ID != message.ID)
                return;

            this.IsActive = false;
            MessageCollectionManagerHandler.Handle(this, message);
            this.IsActive = true;

            Console.WriteLine("Message " + message.GetType().Name + " handled by ManagerMessenger");
        }
    }
}
