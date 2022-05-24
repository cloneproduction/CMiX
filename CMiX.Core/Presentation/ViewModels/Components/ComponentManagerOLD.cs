//// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
//// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Windows.Input;
//using CMiX.Core.Network.Messages;
//using CMiX.Core.Presentation.ViewModels.Network;
//using CommunityToolkit.Mvvm.ComponentModel;
//using CommunityToolkit.Mvvm.Input;
//using CommunityToolkit.Mvvm.Messaging;

//namespace CMiX.Core.Presentation.ViewModels.Components
//{
//    public class ComponentManagerOLD : ObservableRecipient, IRecipient<IMessage>
//    {
//        public ComponentManagerOLD(IProject project)
//        {
//            Project = project;

//            ComponentFactory = new ComponentFactory();

//            IsActive = true;

//            WeakReferenceMessenger.Default.RegisterAll(this, MessageType.Internal);
//            WeakReferenceMessenger.Default.RegisterAll(this, MessageType.In);

//            SelectItemCommand = new RelayCommand<Composition>(SelectComposition);
//            AddItemCommand = new RelayCommand(CreateComposition);
//            DeleteItemCommand = new RelayCommand(DeleteComposition);

//            CreateComponentCommand = new RelayCommand<Type>(CreateComponent);
//            DuplicateComponentCommand = new RelayCommand<Component>(DuplicateComponent);
//            DeleteComponentCommand = new RelayCommand<Component>(RemoveComponent);
//            RenameComponentCommand = new RelayCommand<Component>(RenameComponent);
//        }


//        public ICommand SelectItemCommand { get; set; }
//        public ICommand AddItemCommand { get; set; }
//        public ICommand DeleteItemCommand { get; set; }

//        public ICommand CreateComponentCommand { get; }
//        public ICommand DuplicateComponentCommand { get; }
//        public ICommand DeleteComponentCommand { get; }
//        public ICommand RenameComponentCommand { get; }

//        private ComponentFactory ComponentFactory { get; }


//        private IProject _project;
//        public IProject Project
//        {
//            get => _project;
//            set => SetProperty(ref _project, value);
//        }

//        private Composition _selectedComposition;
//        public Composition SelectedComposition
//        {
//            get => _selectedComposition;
//            set => SetProperty(ref _selectedComposition, value);
//        }

//        private IComponent _selectedComponent;
//        public IComponent SelectedComponent
//        {
//            get => _selectedComponent;
//            set => SetProperty(ref _selectedComponent, value);
//        }


//        public void RenameComponent(IComponent component) => SelectedComponent.IsRenaming = true;


//        public void SelectComposition(Composition composition)
//        {
//            SelectedComposition = composition;
//        }

//        public void CreateLayer()
//        {
//            if (SelectedComposition != null)
//            {
//                Component layer = ComponentFactory.CreateComponent(typeof(Layer));
//                SelectedComposition.AddComponent(layer);
//                WeakReferenceMessenger.Default.Send<IMessage, int>(new MessageAddComponent(SelectedComposition.ID, layer), MessageType.Out);
//                Console.WriteLine("Layer Created " + layer.ID);
//            }
//        }

//        public void CreateComposition()
//        {
//            Component composition = ComponentFactory.CreateComponent(typeof(Composition));
//            Project.AddComponent(composition);

//            SelectedComposition = composition as Composition;

//            WeakReferenceMessenger.Default.Send<IMessage, int>(new MessageAddComponent(Project.ID, composition), MessageType.Out);
//            Console.WriteLine(SelectedComposition.GetType().Name + "'s Components Count is " + SelectedComposition.Components.Count);
//        }


//        private void DeleteComposition()
//        {
//            var components = Project.Components;
//            int index = components.IndexOf(SelectedComposition);

//            if (SelectedComposition != null)
//            {
//                WeakReferenceMessenger.Default.Send<IMessage, int>(new MessageRemoveComponent(Project.ID, SelectedComposition), MessageType.Out);
//                Project.RemoveComponent(SelectedComposition);
//            }

//            if (index > 0)
//            {
//                SelectedComposition = components[index - 1] as Composition;
//                return;
//            }

//            if (index == 0 && components.Count > 0)
//            {
//                SelectedComposition = components[0] as Composition;
//                return;
//            }

//            if (Project.Components.Count == 0)
//                SelectedComposition = null;
//        }


//        public void CreateComponent(Type componentType)
//        {
//            IComponent parentComponent = SelectedComponent;

//            Component newComponent = ComponentFactory.CreateComponent(componentType);

//            parentComponent.AddComponent(newComponent);

//            Console.WriteLine(parentComponent.GetType().Name + "'s Components Count is " + parentComponent.Components.Count);
//            WeakReferenceMessenger.Default.Send<IMessage, int>(new MessageAddComponent(parentComponent.ID, newComponent), MessageType.Out);
//        }

//        public void CreateComponent(MessageAddComponent messageAddComponent)
//        {
//            var descendants = GetAllDescendants(new[] { Project });
//            IComponent parentComponent = Project;

//            foreach (var descendant in descendants)
//            {
//                if(descendant.ID == messageAddComponent.ID)
//                {
//                    parentComponent = descendant;
//                    break;
//                }
//            }

//            Component newComponent = ComponentFactory.CreateComponent(messageAddComponent.ComponentType);
//            newComponent.SetViewModel(messageAddComponent.ComponentModel);

//            parentComponent.AddComponent(newComponent);

//            Console.WriteLine(parentComponent.GetType().Name + "'s Components Count is " + parentComponent.Components.Count);
//        }


//        public void DeleteComponent(MessageRemoveComponent messageRemoveComponent)
//        {
//            IComponent parent = GetParent(Project, x => x.ID == messageRemoveComponent.ID);
//            parent?.RemoveComponent(messageRemoveComponent.ID);

//            Console.WriteLine(parent.GetType().Name + "'s Components Count is " + parent.Components.Count);
//        }


//        private IComponent GetParent(IComponent rootNode, Func<IComponent, bool> childSelector)
//        {
//            var allNodes = GetAllDescendants(new[] { rootNode });

//            IEnumerable<IComponent> parentsOfSelectedChildren = allNodes.Where(node => node.Components.Any(childSelector));

//            if (parentsOfSelectedChildren.Count() == 0)
//                return rootNode;

//            return parentsOfSelectedChildren.Single();
//        }

//        private IEnumerable<IComponent> GetAllDescendants(IEnumerable<IComponent> rootNodes)
//        {
//            var descendants = rootNodes.SelectMany(_ => GetAllDescendants(_.Components));
//            return rootNodes.Concat(descendants);
//        }


//        private void RemoveComponent(IComponent component)
//        {
//            IComponent parent = GetParent(Project, x => x.ID == component.ID);
//            parent?.RemoveComponent(component);

//            Console.WriteLine(parent.GetType().Name + "'s Components Count is " + parent.Components.Count);
//            WeakReferenceMessenger.Default.Send<IMessage, int>(new MessageRemoveComponent(parent.ID, component), MessageType.Out);
//        }


//        public void InsertComponent(int index, Component parentComponent, Component componentToInsert)
//        {
//            parentComponent.InsertComponent(index, componentToInsert);
//        }


//        public void DuplicateComponent(Component component)
//        {
//            //Component result = null;
//            // = GetSelectedParent(Components);
//            //return result;
//        }

//        public void Receive(IMessage message)
//        {

//            switch (message)
//            {
//                case MessageAddLayer _:
//                    this.CreateLayer();
//                    break;

//                case MessageAddComposition _:
//                    this.CreateComposition();
//                    break;

//                case MessageAddComponent add:
//                    this.CreateComponent(add);
//                    break;

//                case MessageRemoveComponent remove:
//                    this.DeleteComponent(remove);
//                    break;
//            }
//        }
//    }
//}
