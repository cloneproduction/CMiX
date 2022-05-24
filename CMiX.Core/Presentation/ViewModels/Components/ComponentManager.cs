// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
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
    public class ComponentManager : ObservableRecipient, IRecipient<IMessage>
    {
        public ComponentManager(Project project)
        {
            Project = project;

            WeakReferenceMessenger.Default.RegisterAll(this, MessageType.In);

            AddItemCommand = new RelayCommand(Create);
            DeleteItemCommand = new RelayCommand(Delete);
        }



        public ICommand AddItemCommand { get; set; }
        public ICommand DeleteItemCommand { get; set; }

        public Project Project { get; set; }


        private Composition _selectedComposition;
        public Composition SelectedComposition
        {
            get => _selectedComposition;
            set => SetProperty(ref _selectedComposition, value);
        }

        public void Rename() => SelectedComposition.IsRenaming = true;

        public void Create()
        {
            Composition composition = new Composition(new CompositionModel(Guid.NewGuid()));
            Project.AddComponent(composition);
            composition.IsSelected = true;
            SelectedComposition = composition;

            WeakReferenceMessenger.Default.Send<IMessage, int>(new MessageAddComponent(Project.ID, composition), MessageType.Out);
            Console.WriteLine(Project.GetType().Name + "'s Components Count is " + Project.Components.Count);
        }

        public void Create(CompositionModel compositionModel)
        {
            Composition composition = new Composition(compositionModel);
            Project.AddComponent(composition);
            composition.IsSelected = true;
            SelectedComposition = composition;
            Console.WriteLine(Project.GetType().Name + "'s Components Count is " + Project.Components.Count);
        }

        public void Delete()
        {
            var selected = SelectedComposition;
            var index = Project.Components.IndexOf(selected);

            if (selected == null)
                return;

            Project.RemoveComponent(selected);
            WeakReferenceMessenger.Default.Send<IMessage, int>(new MessageRemoveComponent(Project.ID, selected), MessageType.Out);

            if (Project.Components.Count == 0)
                return;

            if (index == 0)
            {
                SelectedComposition = Project.Components[0] as Composition;
                return;
            }

            if (index > 0)
            {
                SelectedComposition = Project.Components[index - 1] as Composition;
                return;
            }

            SelectedComposition = null;
        }


        public void Delete(Guid id)
        {
            var toDelete = Project.Components.First(x => x.ID == id);

            if (toDelete != null)
                Project.RemoveComponent(toDelete);
        }


        public void Receive(IMessage message)
        {
            if (message.ID != Project.ID)
                return;

            switch (message)
            {
                case MessageAddComponent add:
                    this.Create(add.ComponentModel as CompositionModel);
                    break;

                case MessageRemoveComponent remove:
                    this.Delete(remove.ID);
                    break;
            }
        }
    }
}
