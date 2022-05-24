// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using CMiX.Core.Models;
using CMiX.Core.Network.Messages;
using CMiX.Core.Presentation.ViewModels.Beat;
using CMiX.Core.Presentation.ViewModels.Modifiers;
using CMiX.Core.Presentation.ViewModels.Network;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GongSolutions.Wpf.DragDrop;

namespace CMiX.Core.Presentation.ViewModels
{
    public class ModifierManager : ObservableRecipient, IRecipient<IMessage>, IBeatable, IControl, IDropTarget, IDragSource
    {
        public ModifierManager(ModifierManagerModel modifierManagerModel, IModifierFactory modifierFactory)
        {
            DragHanderIsDown = false;

            this.ID = modifierManagerModel.ID;

            Modifiers = new ObservableCollection<IModifier>();
            Factory = modifierFactory;
            Visibility = new ToggleButton(modifierManagerModel.Visibility);

            WeakReferenceMessenger.Default.Register(this, MessageType.In);

            CreateCommand = new RelayCommand<Type>(Create);
            RemoveCommand = new RelayCommand<IModifier>(Remove);
            DragHandlerDownCommand = new RelayCommand(OnDragHandlerDown);
            DragHandlerUpCommand = new RelayCommand(OnDragHandlerUp);
        }


        public Guid ID { get; set; }
        public ICommand CreateCommand { get; set; }
        public ICommand RemoveCommand { get; set; }
        public ICommand DragHandlerDownCommand { get; set; }
        public ICommand DragHandlerUpCommand { get; set; }
        public ToggleButton Visibility { get; set; }


        private ObservableCollection<IModifier> _modifiers;
        public ObservableCollection<IModifier> Modifiers
        {
            get => _modifiers;
            set => SetProperty(ref _modifiers, value);
        }

        private IModifierFactory Factory { get; set; }

        public void Create(Type modifierType)
        {
            IModifier filter = Factory.Create(modifierType);
            Add(filter);
        }

        public void Create(IModifierModel modifierModel)
        {
            IModifier filter = Factory.Create(modifierModel);
            Add(filter);
        }

        public void SetMasterBeat(MasterBeat masterBeat)
        {
            Factory.SetMasterBeat(masterBeat);
        }

        public void Add(IModifier modifier)
        {
            Modifiers.Add(modifier);
            WeakReferenceMessenger.Default.Send<IMessage, int>(new MessageAddModifier(this.ID, modifier), MessageType.Out);
        }


        public void Remove(IModifier modifier)
        {
            Modifiers.Remove(modifier);
            WeakReferenceMessenger.Default.Send<IMessage, int>(new MessageRemoveModifier(this.ID, modifier), MessageType.Out);
            modifier.Dispose();
        }

        public void Remove(Guid textureFilterID)
        {
            var filter = Modifiers.FirstOrDefault(x => x.ID == textureFilterID);
            if (filter != null)
                this.Remove(filter);
        }


        public void SetViewModel(IModel model)
        {
            ModifierManagerModel modifierManagerModel = model as ModifierManagerModel;
            ID = modifierManagerModel.ID;
            Visibility.SetViewModel(modifierManagerModel.Visibility);
        }

        public IModel GetModel()
        {
            ModifierManagerModel modifierManagerModel = new ModifierManagerModel();

            modifierManagerModel.ID = this.ID;
            modifierManagerModel.Visibility = (ToggleButtonModel)Visibility.GetModel();

            return modifierManagerModel;
        }


        public void Receive(IMessage message)
        {
            if (message.ID != this.ID)
                return;

            if (message is MessageAddModifier messageAddModifier)
            {
                this.Create(messageAddModifier.ModifierModel);
                return;
            }

            if (message is MessageRemoveModifier messageRemoveModifier)
            {
                this.Remove(messageRemoveModifier.ModifierModel.ID);
                return;
            }

            if(message is MessageModifierMove messageModifierMove)
            {
                var oldIndex = messageModifierMove.OldIndex;
                var newIndex = messageModifierMove.NewIndex;
                
                Modifiers.Move(oldIndex, newIndex);
            }
        }


        private bool DragHanderIsDown { get; set; }

        private void OnDragHandlerDown()
        {
            DragHanderIsDown = true;
        }

        private void OnDragHandlerUp()
        {
            DragHanderIsDown = false;
        }

        public void StartDrag(IDragInfo dragInfo)
        {

            if (DragHanderIsDown)
            {
                dragInfo.Data = dragInfo.SourceItem;
                dragInfo.Effects = DragDropEffects.Copy | DragDropEffects.Move;
            }
        }

        public bool CanStartDrag(IDragInfo dragInfo)
        {
            return true;
        }

        public void Dropped(IDropInfo dropInfo)
        {
            var sourceIndex = dropInfo.DragInfo.SourceIndex;
            var targetIndex = dropInfo.InsertIndex;

            if (targetIndex == Modifiers.Count)
            {
                Move(sourceIndex, targetIndex);
                return;
            }

            if (targetIndex == Modifiers.Count - 1)
            {
                Move(sourceIndex, targetIndex);
                return;
            }

            if (targetIndex >= sourceIndex)
            {
                Move(sourceIndex, targetIndex);
                return;
            }

            Modifiers.Move(sourceIndex, targetIndex);
            WeakReferenceMessenger.Default.Send<IMessage, int>(new MessageModifierMove(this.ID, sourceIndex, targetIndex), MessageType.Out);
        }

        private void Move(int sourceIndex, int targetIndex)
        {
            targetIndex -= 1;
            Modifiers.Move(sourceIndex, targetIndex);
            WeakReferenceMessenger.Default.Send<IMessage, int>(new MessageModifierMove(this.ID, sourceIndex, targetIndex), MessageType.Out);
        }

        public void DragDropOperationFinished(DragDropEffects operationResult, IDragInfo dragInfo)
        {
            DragHanderIsDown = false;
        }

        public void DragCancelled()
        {
            //throw new NotImplementedException();
        }

        public bool TryCatchOccurredException(Exception exception)
        {
            throw new NotImplementedException();
        }

        public void DragEnter(IDropInfo dropInfo)
        {
            //throw new NotImplementedException();
        }

        public void DragOver(IDropInfo dropInfo)
        {
            var targetItem = dropInfo.VisualTargetItem;
            var visualTarget = dropInfo.DragInfo.VisualSourceItem;

            var targetIndex = dropInfo.InsertIndex;
            var sourceIndex = dropInfo.DragInfo.SourceIndex;

            if (targetIndex - 1 == sourceIndex)
            {
                dropInfo.Effects = DragDropEffects.None;
                return;
            }

            if (sourceIndex == targetIndex)
            {
                dropInfo.Effects = DragDropEffects.None;
                return;
            }

            if (visualTarget == targetItem)
            {
                dropInfo.Effects = DragDropEffects.None;
                return;
            }

            dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
            dropInfo.Effects = DragDropEffects.Copy | DragDropEffects.Move;
        }

        public void DragLeave(IDropInfo dropInfo)
        {
            //throw new NotImplementedException();
        }

        public void Drop(IDropInfo dropInfo)
        {

            //Filters.Move()
        }
    }
}
