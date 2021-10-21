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
        public ComponentManager(IProject project)
        {

            Project = project;
            Components = new Dictionary<Guid, IComponent>();
            Components.Add(project.ID, project);

            ComponentFactory = new ComponentFactory();

            ComponentFactory.RegisterComponentType(typeof(Entity), () => new Entity(new EntityModel(Guid.NewGuid())));
            ComponentFactory.RegisterComponentType(typeof(Composition), () => new Composition(new CompositionModel(Guid.NewGuid())));
            ComponentFactory.RegisterComponentType(typeof(Layer), () => new Layer(new LayerModel(Guid.NewGuid())));
            ComponentFactory.RegisterComponentType(typeof(Scene), () => new Scene(new SceneModel(Guid.NewGuid())));

            IsActive = true;

            Messenger.RegisterAll(this, MessageType.Internal);
            Messenger.RegisterAll(this, MessageType.In);



            SelectItemCommand = new RelayCommand<Composition>(SelectComposition);
            AddItemCommand = new RelayCommand(CreateComposition);
            DeleteItemCommand = new RelayCommand(DeleteComposition);

            CreateComponentCommand = new RelayCommand<Type>(CreateComponent);
            DuplicateComponentCommand = new RelayCommand<Component>(DuplicateComponent);
            DeleteComponentCommand = new RelayCommand<Component>(RemoveComponent);
            RenameComponentCommand = new RelayCommand<Component>(RenameComponent);
        }


        public ICommand SelectItemCommand { get; set; }
        public ICommand AddItemCommand { get; set; }
        public ICommand DeleteItemCommand { get; set; }

        public ICommand CreateComponentCommand { get; }
        public ICommand DuplicateComponentCommand { get; }
        public ICommand DeleteComponentCommand { get; }
        public ICommand RenameComponentCommand { get; }


        private IProject _project;
        public IProject Project
        {
            get => _project;
            set => SetProperty(ref _project, value);
        }
        private Dictionary<Guid, IComponent> Components { get; set; }
        public ComponentFactory ComponentFactory { get; set; }


        private Composition _selectedComposition;
        public Composition SelectedComposition
        {
            get => _selectedComposition;
            set
            {
                SelectedComponent = null;
                SetProperty(ref _selectedComposition, value);
            }
        }

        private IComponent _selectedComponent;
        public IComponent SelectedComponent
        {
            get => _selectedComponent;
            set => SetProperty(ref _selectedComponent, value);
        }


        public void RenameComponent(IComponent component) => SelectedComponent.IsRenaming = true;


        public void SelectComposition(Composition composition)
        {
            SelectedComposition = composition;
        }

        public void CreateLayer()
        {
            if (SelectedComposition != null)
            {
                Component layer = ComponentFactory.CreateComponent(typeof(Layer));
                SelectedComposition.AddComponent(layer);
                Components.Add(layer.ID, layer);
                Messenger.Send<IMessage, int>(new MessageAddComponent(SelectedComposition.ID, layer), MessageType.Out);
            }
        }

        public void CreateComposition()
        {
            var composition = ComponentFactory.CreateComponent(typeof(Composition));
            Project.AddComponent(composition);
            SelectedComposition = composition as Composition;
            Components.Add(composition.ID, composition);
            Messenger.Send<IMessage, int>(new MessageAddComponent(Project.ID, composition), MessageType.Out);
        }


        private void DeleteComposition()
        {
            var components = Project.Components;
            int index = components.IndexOf(SelectedComposition);

            if (SelectedComposition != null)
                components.Remove(SelectedComposition);

            if (index > 0)
            {
                SelectedComposition = components[index - 1] as Composition;
                return;
            }

            if (index == 0 && components.Count > 0)
            {
                SelectedComposition = components[0] as Composition;
                return;
            }

            if (Project.Components.Count == 0)
                SelectedComposition = null;
        }



        public void CreateComponent(Type componentType)
        {
            IComponent parentComponent = SelectedComponent;
            var newComponent = ComponentFactory.CreateComponent(componentType);
            //newComponent.MasterBeat = parentComponent.MasterBeat;
            newComponent.UpdateChildMasterBeat(parentComponent.MasterBeat);
            parentComponent.AddComponent(newComponent);
            Messenger.Send<IMessage, int>(new MessageAddComponent(parentComponent.ID, newComponent), MessageType.Out);

            Components.Add(newComponent.ID, newComponent);

            Console.WriteLine(parentComponent.GetType().Name + "'s Components Count is " + parentComponent.Components.Count);
        }

        public void CreateComponent(Type componentType, Guid parentID, IComponentModel componentModel)
        {
            IComponent parentComponent;
            Components.TryGetValue(parentID, out parentComponent);

            if(parentComponent != null)
            {
                var newComponent = ComponentFactory.CreateComponent(componentType);
                parentComponent.AddComponent(newComponent);
                Components.Add(newComponent.ID, newComponent);

                Messenger.Send<IMessage, int>(new MessageAddComponent(parentID, newComponent), MessageType.Out);
                Console.WriteLine(parentComponent.GetType().Name + "'s Components Count is " + parentComponent.Components.Count);
            }
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

            IComponent parent = ((Component)component).GetParent(SelectedComposition, x => x.ID == component.ID);

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
                case MessageAddLayer _:
                    this.CreateLayer();
                    break;

                case MessageAddComposition _:
                    this.CreateComposition();
                    break;

                case MessageAddComponent add:
                    this.CreateComponent(add.ComponentType, add.ParentID, add.ComponentModel);
                    break;

                case MessageRemoveComponent remove:
                    this.DeleteComponent(remove.ComponentID);
                    break;
            }
        }

        //public void Receive(MessageRequestMasterBeat message)
        //{
        //    message.Reply(SelectedComposition.MasterBeat);
        //}
    }
}
