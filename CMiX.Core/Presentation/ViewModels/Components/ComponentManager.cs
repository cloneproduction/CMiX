// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace CMiX.Core.Presentation.ViewModels.Components
{
    public class ComponentManager : ViewModel
    {
        public ComponentManager(IComponent component)
        {
            Component = component;

            CreateComponentCommand = new RelayCommand(p => CreateComponent(p as Component));
            DuplicateComponentCommand = new RelayCommand(p => DuplicateComponent(p as Component));
            DeleteComponentCommand = new RelayCommand(p => DeleteComponent(p as Component));
            RenameComponentCommand = new RelayCommand(p => RenameComponent(p as Component));
        }


        public ICommand CreateComponentCommand { get; }
        public ICommand DuplicateComponentCommand { get; }
        public ICommand DeleteComponentCommand { get; }
        public ICommand RenameComponentCommand { get; }


        private IComponent Component { get; set; }


        private Component _selectedComponent;
        public Component SelectedComponent
        {
            get => _selectedComponent;
            set => SetAndNotify(ref _selectedComponent, value);
        }


        public void RenameComponent(Component component) => SelectedComponent.IsRenaming = true;


        public void CreateComponent(Component component)
        {
            if (component is null)
                component = this.Component as Component;

            var newComponent = component.ComponentFactory.CreateComponent();
            newComponent.SetCommunicator(component.Communicator);
            component.AddComponent(newComponent);
        }


        public void DeleteComponent(Component component)
        {
            component.Dispose();
            if (component is Composition)
            {
                Component.RemoveComponent(component);
                return;
            }
            var selectedParent = GetParent(Component.Components);
            selectedParent.RemoveComponent(component);
        }


        public void InsertComponent(int index, Component parentComponent, Component componentToInsert)
        {
            parentComponent.InsertComponent(index, componentToInsert);
        }


        public Component DuplicateComponent(Component component)
        {
            Component result = null;
            // = GetSelectedParent(Components);
            return result;
        }


        private void DeleteSelectedComponent(ObservableCollection<Component> components)
        {
            foreach (Component component in components)
            {
                if (component.IsSelected)
                {
                    components.Remove(component);
                    break;
                }
                DeleteSelectedComponent(component.Components);
            }
        }



        private Component GetParent(ObservableCollection<Component> components)
        {
            Component result = null;
            foreach (Component component in components)
            {
                if (component.Components.Any(c => c.IsSelected))
                {
                    result = component;
                    break;
                }
                result = GetParent(component.Components);
            }
            return result;
        }
    }
}
