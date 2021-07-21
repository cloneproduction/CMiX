// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using CMiX.Core.Models;
using CMiX.Core.Network.Messages;
using CMiX.Core.Presentation.ViewModels.Components.Factories;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.Presentation.ViewModels.Components
{
    public abstract class Component : ObservableRecipient, IComponent, IGetSetModel, IDisposable
    {
        public Component()
        {
            IsExpanded = false;
            Name = this.GetType().Name;
            IsActive = true;

            Components = new ObservableCollection<Component>();

        }


        protected override void OnActivated()
        {
            WeakReferenceMessenger.Default.Register<Component, IMessage, string>(this, "IN", (r, m) => r.Receive(m));
            // Using a method group...
            //WeakReferenceMessenger.Default.Register<Component, MessageAddComponent>(this, (r, m) => r.Receive(m));

            // ...or a lambda expression
            //Messenger.Register<MyViewModel, LoggedInUserRequestMessage>(this, (r, m) =>
            //{
            //    // Handle the message here
            //});
        }

        private void Receive(IMessage message)
        {
            if (message is IComponentMessage)
                message.Process(this);
        }


        public Visibility Visibility { get; set; }
        public ICommand VisibilityCommand { get; set; }
        public IComponentFactory ComponentFactory { get; set; }


        private Guid _id;
        public Guid ID
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

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

        private bool _isExpanded;
        public bool IsExpanded
        {
            get => _isExpanded;
            set => SetProperty(ref _isExpanded, value);
        }

        private ObservableCollection<Component> _components;
        public ObservableCollection<Component> Components
        {
            get => _components;
            set => SetProperty(ref _components, value);
        }

        public void AddComponent(Component component)
        {
            Console.WriteLine(this.GetType().Name + " SendMessageAddComponent");
            WeakReferenceMessenger.Default.Send<IMessage, string>(new MessageAddComponent(this.ID, component), "OUT");

            Components.Add(component);
            IsExpanded = true;
        }

        public void RemoveComponent(Component component)
        {
            int index = Components.IndexOf(component);
            component.Dispose();
            Components.Remove(component);
        }

        public void RemoveComponentAtIndex(int index)
        {
            Component component = Components.ElementAt(index);
            RemoveComponent(component);
        }

        public void InsertComponent(int index, Component component)
        {
            Components.Insert(index, component);
        }

        public void MoveComponent(int oldIndex, int newIndex)
        {
            Components.Move(oldIndex, newIndex);
        }

        public abstract void SetViewModel(IModel model);
        public abstract IModel GetModel();

        public void Dispose()
        {
            foreach (var component in Components)
            {
                component.Dispose();
            }
        }
    }
}
