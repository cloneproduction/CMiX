// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using CMiX.Core.Models;
using CMiX.Core.Network.Messages;
using CMiX.Core.Presentation.ViewModels.Network;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.Presentation.ViewModels.Components
{
    public class ComponentManager : ObservableRecipient, 
        IRecipient<MessageAddComposition>,
        IRecipient<MessageAddComponent>,
        IRecipient<MessageRemoveComponent>
    {
        public ComponentManager(IMessageService messageService, IComponent rootComponent)
        {
            MessageService = messageService;
            IsActive = false;
            Components = new Dictionary<Guid, IComponent>();
            Components.Add(rootComponent.ID, rootComponent);
            RootComponent = rootComponent;

            WeakReferenceMessenger.Default.RegisterAll(this, "INTERNAL");
            WeakReferenceMessenger.Default.RegisterAll(this, "IN");

            CreateComponentCommand = new RelayCommand<Component>(CreateComponent);
            DuplicateComponentCommand = new RelayCommand<Component>(DuplicateComponent);
            DeleteComponentCommand = new RelayCommand<Component>(RemoveComponent);
            RenameComponentCommand = new RelayCommand<Component>(RenameComponent);
        }


        public ICommand CreateComponentCommand { get; }
        public ICommand DuplicateComponentCommand { get; }
        public ICommand DeleteComponentCommand { get; }
        public ICommand RenameComponentCommand { get; }

        private IMessageService MessageService { get; set; }
        private IComponent RootComponent { get; set; }
        private Dictionary<Guid, IComponent> Components { get; set; }


        private IComponent _selectedComponent;
        public IComponent SelectedComponent
        {
            get => _selectedComponent;
            set => SetProperty(ref _selectedComponent, value);
        }

        private IComponent _selectedParent;
        public IComponent SelectedParent
        {
            get => _selectedParent;
            set => SetProperty(ref _selectedParent, value);
        }

        public void RenameComponent(IComponent component) => SelectedComponent.IsRenaming = true;


        public void CreateComponent(Guid parentID, IComponentModel componentModel)
        {
            IComponent parentComponent;
            Components.TryGetValue(parentID, out parentComponent);

            var newComponent = parentComponent.ComponentFactory.CreateComponent(componentModel);
            parentComponent.AddComponent(newComponent);
            Components.Add(newComponent.ID, newComponent);

            MessageService.SendMessage(new MessageAddComponent(parentID, newComponent));
            //Messenger.Send<MessageAddComponent, string>(new MessageAddComponent(parentID, newComponent), "OUT");
            Console.WriteLine(parentComponent.GetType().Name + "'s Components Count is " + parentComponent.Components.Count);
        }

        public void CreateComponent(IComponent parentComponent)
        {
            var newComponent = parentComponent.ComponentFactory.CreateComponent();
            parentComponent.AddComponent(newComponent);
            Components.Add(newComponent.ID, newComponent);

            MessageService.SendMessage(new MessageAddComponent(parentComponent.ID, newComponent));
            //Messenger.Send<MessageAddComponent, string>(new MessageAddComponent(parentComponent.ID, newComponent), "OUT");
            Console.WriteLine(parentComponent.GetType().Name + "'s Components Count is " + parentComponent.Components.Count);
        }


        public void DeleteComponent(Guid componentID)
        {
            IComponent component;
            Components.TryGetValue(componentID, out component);

            RemoveComponent(component);
        }


        private void RemoveComponent(IComponent component)
        {
            IComponent parent = ((Component)component).GetParent(RootComponent, x => x.ID == component.ID);

            parent.Components.Remove(component);
            Components.Remove(component.ID);
            component.Dispose();

            MessageService.SendMessage(new MessageRemoveComponent(component));
            //Messenger.Send<MessageRemoveComponent, string>(new MessageRemoveComponent(component), "OUT");
            Console.WriteLine(parent.GetType().Name + "'s Components Count is " + parent.Components.Count);
        }


        public void InsertComponent(int index, Component parentComponent, Component componentToInsert)
        {
            parentComponent.InsertComponent(index, componentToInsert);
        }


        public void DuplicateComponent(Component component)
        {
            //Component result = null;
            // = GetSelectedParent(Components);
            //return result;
        }

        public void Receive(MessageAddComposition message)
        {
            this.CreateComponent(RootComponent);
        }

        public void Receive(MessageAddComponent message)
        {
            this.CreateComponent(message.ParentID, message.ComponentModel);
        }

        public void Receive(MessageRemoveComponent message)
        {
            this.DeleteComponent(message.ComponentID);
        }
    }
}
