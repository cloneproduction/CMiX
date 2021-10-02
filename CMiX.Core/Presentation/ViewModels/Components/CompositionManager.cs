// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Windows.Input;
using CMiX.Core.Models;
using CMiX.Core.Network.Messages;
using CMiX.Core.Presentation.ViewModels.Network;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CMiX.Core.Presentation.ViewModels.Components
{
    public class CompositionManager : ObservableRecipient
    {
        public CompositionManager(IProject project)
        {
            Project = project;
            ComponentFactory = new ComponentFactory();
            ComponentFactory.RegisterComponentType(ComponentType.Composition, () => new Composition(new CompositionModel(Guid.NewGuid())));

            AddItemCommand = new RelayCommand(AddComposition);
            DeleteItemCommand = new RelayCommand(DeleteComposition);
        }

        public IProject Project { get; set; }
        private ComponentFactory ComponentFactory { get; set; }
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
        }

        private void AddComposition()
        {
            var composition = ComponentFactory.CreateComponent(ComponentType.Composition);
            Project.AddComponent(composition);
            Messenger.Send<IMessage, int>(new MessageAddComponent(Project.ID, composition), MessageType.Out);
        }

        private Composition _selectedComposition;
        public Composition SelectedComposition
        {
            get => _selectedComposition;
            set => SetProperty(ref _selectedComposition, value);
        }


        public ICommand AddItemCommand { get; set; }
        public ICommand DeleteItemCommand { get; set; }
    }
}
