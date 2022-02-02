// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using CMiX.Core.Models;
using CMiX.Core.Network.Messages;
using CMiX.Core.Presentation.ViewModels.Beat;
using CMiX.Core.Presentation.ViewModels.Network;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.Presentation.ViewModels.Components
{
    public abstract class Component : 
        ObservableRecipient,
        IRecipient<MessageMasterBeatCollectionChanged>,
        IRecipient<MessageRequestMasterBeat>,
        //IRecipient<IMessage>,
        IComponent, 
        IDisposable
    {
        public Component()
        {
            IsExpanded = false;
            Name = this.GetType().Name;
            RenameCommand = new RelayCommand(Rename);
            Components = new ObservableCollection<IComponent>();
            MasterBeats = new ObservableCollection<MasterBeat>();

            WeakReferenceMessenger.Default.RegisterAll(this, MessageType.Internal);
            WeakReferenceMessenger.Default.RegisterAll(this, MessageType.In);

            SelectedBeatChangedCommand = new RelayCommand<MasterBeat>(SelectMasterBeat);
        }


        public Visibility Visibility { get; set; }
        public ICommand VisibilityCommand { get; set; }
        public ICommand RenameCommand { get; set; }
        public ICommand SelectedBeatChangedCommand { get; set; }


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

        private MasterBeat _selectedMasterBeat;
        public MasterBeat SelectedMasterBeat
        {
            get => _selectedMasterBeat;
            set => SetProperty(ref _selectedMasterBeat, value);
        }

        private ObservableCollection<MasterBeat> _masterBeats;
        public ObservableCollection<MasterBeat> MasterBeats
        {
            get => _masterBeats;
            set => SetProperty(ref _masterBeats, value);
        }


        public void Receive(MessageRequestMasterBeat message)
        {
            message.Reply(SelectedMasterBeat);
        }

        public void Receive(MessageMasterBeatCollectionChanged message)
        {
            MasterBeats = message.MasterBeats;

            if (SelectedMasterBeat == null)
                return;

            if (!this.MasterBeats.Any(x => x.ID == this.SelectedMasterBeat.ID))
            {
                if (MasterBeats.Count > 0)
                {
                    SelectMasterBeat(MasterBeats[0]);
                    return;
                }
            }
        }

        //public void Receive(IMessage message)
        //{
        //    if (message is MessageSelectedMasterBeatChange messageSelectedMasterBeatChange)
        //    {
        //        if (messageSelectedMasterBeatChange.ID == this.ID)
        //        {
        //            SelectMasterBeat(MasterBeats.FirstOrDefault(x => x.ID == messageSelectedMasterBeatChange.MasterBeatID));
        //            Console.WriteLine(this.GetType() + this.Name + " SelectedMasterBeatChange with period " + SelectedMasterBeat.Period);
        //        }
        //    }
        //}


        public void SelectMasterBeat(MasterBeat masterBeat)
        {
            SelectedMasterBeat = masterBeat;
            foreach (IComponent cp in this.Components)
            {
                cp.SelectMasterBeat(masterBeat);
            }

            if (masterBeat != null)
            {
                WeakReferenceMessenger.Default.Send(new MessageSelectedMasterBeatChange(this.ID, masterBeat), this.ID);
                WeakReferenceMessenger.Default.Send<IMessage, int>(new MessageSelectedMasterBeatChange(this.ID, masterBeat), MessageType.Out);
            }
        }


        public void AddComponent(IComponent component)
        {
            Components.Add(component);
            component.MasterBeats = this.MasterBeats;
            component.SelectMasterBeat(this.SelectedMasterBeat);
            IsExpanded = true;
        }

        public void RemoveComponent(IComponent component)
        {
            if(component != null)
            {
                component.Dispose();
                Components.Remove(component);
            }
        }

        public void RemoveComponent(Guid id)
        {
            var component = Components.FirstOrDefault(x => x.ID == id);
            Components.Remove(component);
        }

        private void Rename()
        {
            this.IsRenaming = true;
        }


        public IComponent GetComponent(Guid childID)
        {
            IComponent component = null;
            foreach (var child in Components)
            {
                if (child.ID == childID)
                    component = child;
                else
                    component = child.GetComponent(childID);
            }
            return component;
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
