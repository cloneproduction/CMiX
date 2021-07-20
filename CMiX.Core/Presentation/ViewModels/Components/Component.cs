// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using CMiX.Core.Models;
using CMiX.Core.Network.Communicators;
using CMiX.Core.Network.Messages;
using CMiX.Core.Presentation.ViewModels.Components.Factories;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using MediatR;

namespace CMiX.Core.Presentation.ViewModels.Components
{
    public abstract class Component : ObservableRecipient, IComponent, IControl, IDisposable
    {
        public Component()
        {
            IsExpanded = false;
            Name = this.GetType().Name;

            Components = new ObservableCollection<Component>();
            WeakReferenceMessenger.Default.Register<Component, MessageAddComponent>(this, (r, m) => r.Receive(m));
            //Communicator = new ComponentCommunicator(this);
        }


        public Communicator Communicator { get; set; }
        public abstract void SetCommunicator(Communicator communicator);
        public abstract void UnsetCommunicator(Communicator communicator);


        protected override void OnActivated()
        {
            // Using a method group...
            WeakReferenceMessenger.Default.Register<Component, MessageAddComponent>(this, (r, m) => r.Receive(m));
            // ...or a lambda expression
            //Messenger.Register<MyViewModel, LoggedInUserRequestMessage>(this, (r, m) =>
            //{
            //    // Handle the message here
            //});
        }

        private void Receive(MessageAddComponent message)
        {
            Console.WriteLine(this.GetType().Name + " ReceivedMessageAddComponent");
            //this.Messenger.Send(new MessageAddComponent(this));
            // Handle the message here
        }


        //internal void ReceiveMessage(Message message)
        //{
        //    Console.WriteLine(this.GetType().Name + "ReceiveMessage of type" + message.GetType().Name);

        //    if (message is MessageAddComponent)
        //    {
        //        var messageAddComponent = message as MessageAddComponent;
        //        var newComponent = ComponentFactory.CreateComponent(messageAddComponent.ComponentModel);
        //        this.AddComponent(newComponent);
        //        //_componentDatabase.AddComponent(newComponent);
        //    }
        //}


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

        public IMediator Mediator { get; set; }

        public async void AddComponent(Component component)
        {
            Console.WriteLine(this.GetType().Name + " SendMessageAddComponent");
            WeakReferenceMessenger.Default.Send(new MessageAddComponent(this));
            //Messenger.Send(new MessageAddComponent(this));

            component.SetCommunicator(this.Communicator);
            Components.Add(component);
            IsExpanded = true;


            //if (Mediator != null)
            //{
            //    await Mediator.Publish(new AddNewComponentNotification(this.ID, component));
            //}
            //Communicator?.SendMessage(new MessageAddComponent(component));
        }

        public void RemoveComponent(Component component)
        {
            int index = Components.IndexOf(component);
            component.Dispose();
            component.UnsetCommunicator(this.Communicator);
            Components.Remove(component);

            Communicator?.SendMessage(new MessageRemoveComponent(index));
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
