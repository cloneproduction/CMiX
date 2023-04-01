// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections.ObjectModel;
using System.Windows.Input;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Presentations.Components;
using CMiX.Core.Presentations.Prefabs;
using CMiX.Core.Presentations.Prefabs.Message;
using CMiX.Core.Presentations.ViewModels.BaseControl;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.Presentations.ViewModels.Components
{
    public abstract class Component : 
        ObservableRecipient,
        IControl,
        IRecipient<MessageRequestControl>,
        IComponent, 
        IDisposable
    {
        public Component()
        {
            IsExpanded = false;
            RenameCommand = new RelayCommand(Rename);
            Components = new ObservableCollection<IComponent>();
            IsActive = true;
        }

        public ICommand RenameCommand { get; set; }


        private Guid _id;
        public Guid ID
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        public StringValue Name { get; set; }
        public BooleanValue IsRenaming { get; set; }
        public BooleanValue IsSelected { get; set; }


        private bool _isExpanded;
        public bool IsExpanded
        {
            get => _isExpanded;
            set => SetProperty(ref _isExpanded, value);
        }

        private ObservableCollection<IComponent> _components;
        public ObservableCollection<IComponent> Components
        {
            get => _components;
            set => SetProperty(ref _components, value);
        }


        public virtual void AddComponent(IComponent component)
        {
            Components.Add(component);
            IsExpanded = true;
        }

        public void RemoveComponent(IComponent component)
        {
            if (component == null)
                return;

            component.Dispose();
            Components.Remove(component);
        }

        public void RemoveComponent(Guid id)
        {
            var component = Components.FirstOrDefault(x => x.ID == id);
            Components.Remove(component);
        }

        private void Rename()
        {
            this.IsRenaming.Value = true;
        }


        public IComponent GetComponent(Guid childID)
        {
            IComponent component = null;
            foreach (var child in Components)
            {
                if (child.ID == childID)
                    component = child;
                else
                    component = child.GetComponent(childID);
            }
            return component;
        }


        public void InsertComponent(int index, IComponent component)
        {
            Components.Insert(index, component);
        }

        public void MoveComponent(int oldIndex, int newIndex)
        {
            Components.Move(oldIndex, newIndex);
        }

        //public abstract IModel GetModel();




        public IPrefab RequestPrefab(Guid id)
        {
            return WeakReferenceMessenger.Default.Send(new MessageRequestPrefab(id), MessageType.Internal).Response;
        }

        public void Receive(MessageRequestControl message)
        {
            if (message.ID == this.ID && !message.HasReceivedResponse)
                message.Reply(this);
        }

        public virtual void Dispose()
        {
            foreach (var component in Components)
            {
                component.Dispose();
            }
        }
    }
}
