// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using CMiX.Core.Models;
using CMiX.Core.Network.Messages;
using CMiX.Core.Presentation.ViewModels.Modifiers;
using CMiX.Core.Presentation.ViewModels.Network;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GongSolutions.Wpf.DragDrop;

namespace CMiX.Core.Presentation.ViewModels
{
    public class ModifierManager : ObservableRecipient, IRecipient<MessageRequestControl>, IControl, IDropTarget, IDragSource
    {
        public ModifierManager(ModifierManagerModel modifierManagerModel, IModifierFactory modifierFactory)
        {
            this.ID = modifierManagerModel.ID;

            Modifiers = new ObservableCollection<IModifier>();
            Factory = modifierFactory;
            Visibility = new ToggleButton(modifierManagerModel.Visibility);

            CreateCommand = new RelayCommand<Type>(Create);
            RemoveCommand = new RelayCommand<IModifier>(Remove);

            IsActive = true;
        }


        public Guid ID { get; set; }
        public ICommand CreateCommand { get; set; }
        public ICommand RemoveCommand { get; set; }
        public ICommand DragHandlerDownCommand { get; set; }
        public ICommand DragHandlerUpCommand { get; set; }
        public ToggleButton Visibility { get; set; }


        private bool _dragHandlerIsPressed;
        public bool DragHandlerIsPressed
        {
            get => _dragHandlerIsPressed;
            set => SetProperty(ref _dragHandlerIsPressed, value);
        }

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

        public void Add(IModifier modifier)
        {
            Modifiers.Add(modifier);
            WeakReferenceMessenger.Default.Send<IMessage, int>(new MessageAddModifier(this.ID, modifier.GetModel()), MessageType.Out);
        }


        public void Remove(IModifier modifier)
        {
            Modifiers.Remove(modifier);
            WeakReferenceMessenger.Default.Send<IMessage, int>(new MessageRemoveModifier(this.ID, modifier), MessageType.Out);
            modifier.Dispose();
        }

        public void Remove(Guid id)
        {
            var modifier = Modifiers.FirstOrDefault(x => x.ID == id);
            this.Remove(modifier);
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

        public void Receive(MessageRequestControl message)
        {
            if (message.ID == this.ID)
                message.Reply(this);
        }

        public void StartDrag(IDragInfo dragInfo)
        {

            if (DragHandlerIsPressed)
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
                MoveOnDrop(sourceIndex, targetIndex);
                return;
            }

            if (targetIndex == Modifiers.Count - 1)
            {
                MoveOnDrop(sourceIndex, targetIndex);
                return;
            }

            if (targetIndex >= sourceIndex)
            {
                MoveOnDrop(sourceIndex, targetIndex);
                return;
            }

            Modifiers.Move(sourceIndex, targetIndex);
            WeakReferenceMessenger.Default.Send<IMessage, int>(new MessageMoveModifier(this.ID, sourceIndex, targetIndex), MessageType.Out);
        }

        public void MoveOnDrop(int sourceIndex, int targetIndex)
        {
            targetIndex -= 1;
            Modifiers.Move(sourceIndex, targetIndex);
            WeakReferenceMessenger.Default.Send<IMessage, int>(new MessageMoveModifier(this.ID, sourceIndex, targetIndex), MessageType.Out);
        }

        public void Move(int oldIndex, int newIndex)
        {
            Modifiers.Move(oldIndex, newIndex);
        }

        public void DragDropOperationFinished(DragDropEffects operationResult, IDragInfo dragInfo)
        {
            DragHandlerIsPressed = false;
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
            var visualTarget = dropInfo.DragInfo?.VisualSourceItem;

            if (visualTarget == null)
                return;

            var targetIndex = dropInfo.InsertIndex;
            var sourceIndex = dropInfo.DragInfo?.SourceIndex;

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
