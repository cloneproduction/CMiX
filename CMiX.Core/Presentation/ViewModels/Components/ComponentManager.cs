// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using CMiX.Core.Models;
using CMiX.Core.Network.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.Presentation.ViewModels.Components
{
    public class ComponentManager : ObservableRecipient, IRecipient<IMessage>
    {
        public ComponentManager(IComponent component)
        {
            Components = new Dictionary<Guid, IComponent>();
            Components.Add(component.ID, component);
            Component = component;

            Messenger.RegisterAll(this, "IN");
            Messenger.RegisterAll(this, "INTERNAL");
            IsActive = true;

            CreateComponentCommand = new RelayCommand<Component>(CreateComponent);
            DuplicateComponentCommand = new RelayCommand<Component>(DuplicateComponent);
            DeleteComponentCommand = new RelayCommand<Component>(DeleteComponent);
            RenameComponentCommand = new RelayCommand<Component>(RenameComponent);
        }


        public ICommand CreateComponentCommand { get; }
        public ICommand DuplicateComponentCommand { get; }
        public ICommand DeleteComponentCommand { get; }
        public ICommand RenameComponentCommand { get; }


        private IComponent Component { get; set; }
        private Dictionary<Guid, IComponent> Components { get; set; }


        private Component _selectedComponent;
        public Component SelectedComponent
        {
            get => _selectedComponent;
            set => SetProperty(ref _selectedComponent, value);
        }


        public void RenameComponent(Component component) => SelectedComponent.IsRenaming = true;


        public void CreateComponent(Guid parentID, IComponentModel componentModel)
        {
            IComponent parentComponent;
            Components.TryGetValue(parentID, out parentComponent);

            var newComponent = parentComponent.ComponentFactory.CreateComponent(componentModel);
            parentComponent.AddComponent(newComponent);
            Components.Add(newComponent.ID, newComponent);

            Messenger.Send<IMessage, string>(new MessageAddComponent(parentID, newComponent), "OUT");
            Console.WriteLine(Component.GetType().Name + "'s Components Count is " + parentComponent.Components.Count);
        }

        public void CreateComponent(IComponent parentComponent)
        {
            var newComponent = parentComponent.ComponentFactory.CreateComponent();
            parentComponent.AddComponent(newComponent);
            Components.Add(newComponent.ID, newComponent);

            Messenger.Send<IMessage, string>(new MessageAddComponent(parentComponent.ID, newComponent), "OUT");
            Console.WriteLine(parentComponent.GetType().Name + "'s Components Count is " + parentComponent.Components.Count);
        }


        public void DeleteComponent(Guid componentID)
        {
            IComponent component;
            Components.TryGetValue(componentID, out component);

            RemoveComponent(component);
        }

        public void DeleteComponent(IComponent component)
        {
            RemoveComponent(component);
        }


        private void RemoveComponent(IComponent component)
        {
            var parentComponent = GetParent(Component, component.ID);
            parentComponent.RemoveComponent(component);
            Components.Remove(component.ID);
            component.Dispose();

            Messenger.Send<IMessage, string>(new MessageRemoveComponent(component), "OUT");
            Console.WriteLine(parentComponent.GetType().Name + "'s Components Count is " + parentComponent.Components.Count);
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


        //private void DeleteSelectedComponent(ObservableCollection<IComponent> components)
        //{
        //    foreach (Component component in components)
        //    {
        //        if (component.IsSelected)
        //        {
        //            components.Remove(component);
        //            break;
        //        }
        //        DeleteSelectedComponent(component.Components);
        //    }
        //}


        private IComponent GetParent(IComponent component, Guid childID)
        {
            IComponent result = null;
            foreach (IComponent child in component.Components)
            {
                if (child.ID == childID)
                {
                    result = component;
                    break;
                }
                result = GetParent(child, childID);
            }
            return result;
        }


        public void Receive(IMessage message)
        {
            if (message is IComponentMessage)
            {
                Console.WriteLine("ComponentManager ReceiveMessage");
                message.Process(this);
                Console.WriteLine("ComponentManager ProcessedMessage");
            }
        }
    }
}
