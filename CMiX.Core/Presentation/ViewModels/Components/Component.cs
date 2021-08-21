// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using CMiX.Core.Models;
using CMiX.Core.Presentation.ViewModels.Beat;
using CMiX.Core.Presentation.ViewModels.Network;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.Presentation.ViewModels.Components
{
    public abstract class Component : ObservableRecipient, IComponent, IDisposable
    {
        public Component()
        {
            IsExpanded = false;
            Name = this.GetType().Name;
            Components = new ObservableCollection<IComponent>();
            WeakReferenceMessenger.Default.RegisterAll(this, MessageType.Internal);
        }


        public Visibility Visibility { get; set; }
        public ICommand VisibilityCommand { get; set; }


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

        private ObservableCollection<IComponent> _components;
        public ObservableCollection<IComponent> Components
        {
            get => _components;
            set => SetProperty(ref _components, value);
        }

        public void AddComponent(IComponent component)
        {
            Components.Add(component);
            IsExpanded = true;
        }

        public void RemoveComponent(IComponent component)
        {
            component.Dispose();
            Components.Remove(component);
        }

        public void ChangeChildMasterBeat(MasterBeat masterBeat)
        {
            foreach (Component component in Components)
            {
                component.ChangeChildMasterBeat(masterBeat);
            }
        }


        public IEnumerable<IComponent> GetAllDescendants(IEnumerable<IComponent> rootNodes)
        {
            var descendants = rootNodes.SelectMany(_ => GetAllDescendants(_.Components));
            return rootNodes.Concat(descendants);
        }

        public IComponent GetParent(IComponent rootNode, Func<IComponent, bool> childSelector)
        {
            var allNodes = GetAllDescendants(new[] { rootNode });
            var parentsOfSelectedChildren = allNodes.Where(node => node.Components.Any(childSelector));

            return parentsOfSelectedChildren.Single();
        }

        public void InsertComponent(int index, IComponent component)
        {
            Components.Insert(index, component);
        }

        public void MoveComponent(int oldIndex, int newIndex)
        {
            Components.Move(oldIndex, newIndex);
        }

        public abstract void SetViewModel(IComponentModel model);
        public abstract IComponentModel GetModel();

        public void Dispose()
        {
            foreach (var component in Components)
            {
                component.Dispose();
            }
        }
    }
}
