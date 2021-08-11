// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Windows.Input;
using CMiX.Core.Models;
using CMiX.Core.Network.Messages;
using CMiX.Core.Presentation.ViewModels.Network;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.Presentation.ViewModels.Components
{
    public class ComponentManager : ObservableRecipient, IRecipient<IMessage>
    {
        public ComponentManager(IComponent rootComponent)
        {
            ComponentFactory = new ComponentFactory();

            ComponentFactory.RegisterComponentType(ComponentType.Entity, () => new Entity(new EntityModel(Guid.NewGuid())));
            ComponentFactory.RegisterComponentType(ComponentType.Composition, () => new Composition(new CompositionModel(Guid.NewGuid())));
            ComponentFactory.RegisterComponentType(ComponentType.Layer, () => new Layer(new LayerModel(Guid.NewGuid())));
            ComponentFactory.RegisterComponentType(ComponentType.Scene, () => new Scene(new SceneModel(Guid.NewGuid())));

            IsActive = true;

            Messenger.RegisterAll(this, MessageType.Internal);
            Messenger.RegisterAll(this, MessageType.In);

            Components = new Dictionary<Guid, IComponent>();
            Components.Add(rootComponent.ID, rootComponent);
            RootComponent = rootComponent;

            CreateComponentCommand = new RelayCommand<ComponentType>(CreateComponent);
            DuplicateComponentCommand = new RelayCommand<Component>(DuplicateComponent);
            DeleteComponentCommand = new RelayCommand<Component>(RemoveComponent);
            RenameComponentCommand = new RelayCommand<Component>(RenameComponent);
        }


        public ICommand CreateComponentCommand { get; }
        public ICommand DuplicateComponentCommand { get; }
        public ICommand DeleteComponentCommand { get; }
        public ICommand RenameComponentCommand { get; }

        private IComponent RootComponent { get; set; }
        private Dictionary<Guid, IComponent> Components { get; set; }
        public ComponentFactory ComponentFactory { get; set; }


        private IComponent _selectedComponent;
        public IComponent SelectedComponent
        {
            get => _selectedComponent;
            set => SetProperty(ref _selectedComponent, value);
        }


        public void RenameComponent(IComponent component) => SelectedComponent.IsRenaming = true;

        public void CreateComponent(Guid parentID, IComponentModel componentModel)
        {
            //IComponent parentComponent;
            //Components.TryGetValue(parentID, out parentComponent);


            //var newComponent = ComponentFactory.CreateComponent(componentModel);
            //parentComponent.AddComponent(newComponent);
            //Components.Add(newComponent.ID, newComponent);

            //Messenger.Send<IMessage, int>(new MessageAddComponent(parentID, newComponent), MessageType.Out);
            //Console.WriteLine(parentComponent.GetType().Name + "'s Components Count is " + parentComponent.Components.Count);
        }


        public void CreateComposition()
        {
            var composition = ComponentFactory.CreateComponent(ComponentType.Composition);
            RootComponent.AddComponent(composition);
            Messenger.Send<IMessage, int>(new MessageAddComponent(RootComponent.ID, composition), MessageType.Out);
        }

        public void CreateComponent(ComponentType componentType)
        {
            IComponent parentComponent = SelectedComponent is null ? RootComponent : SelectedComponent;

            var newComponent = ComponentFactory.CreateComponent(componentType);

            parentComponent.AddComponent(newComponent);
            Messenger.Send<IMessage, int>(new MessageAddComponent(parentComponent.ID, newComponent), MessageType.Out);

            Components.Add(newComponent.ID, newComponent);

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
            Messenger.Send<IMessage, int>(new MessageRemoveComponent(component), MessageType.Out);

            IComponent parent = ((Component)component).GetParent(RootComponent, x => x.ID == component.ID);

            parent.Components.Remove(component);
            Components.Remove(component.ID);
            component.Dispose();
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

        public void Receive(IMessage message)
        {
            switch (message)
            {
                case MessageAddComposition _:
                    this.CreateComposition();
                    break;

                case MessageAddComponent add:
                    this.CreateComponent(add.ParentID, add.ComponentModel);
                    break;

                case MessageRemoveComponent remove:
                    this.DeleteComponent(remove.ComponentID);
                    break;
            }
        }
    }
}
