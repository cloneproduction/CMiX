// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using CMiX.Core.Models.Component;
using CMiX.Core.Network.Messages;
using CMiX.Core.Presentation.ViewModels.Network;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GongSolutions.Wpf.DragDrop;

namespace CMiX.Core.Presentation.ViewModels.Components
{
    public class LayerManager : ObservableRecipient, IRecipient<IMessage>, IDropTarget, IDragSource
    {
        public LayerManager(Composition composition)
        {
            Composition = composition;
            Composition.Components.CollectionChanged += Components_CollectionChanged;
            ID = composition.ID;

            IsActive = true;

            WeakReferenceMessenger.Default.RegisterAll(this, MessageType.In);

            CreateCommand = new RelayCommand<Type>(Create);
            DeleteCommand = new RelayCommand(Delete);
            RenameCommand = new RelayCommand(Rename);
            CreateMaskCommand = new RelayCommand(CreateMask);

            LayerOrder = new ObservableCollection<Guid>();
        }

        private void Components_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            var col = sender as ObservableCollection<IComponent>;

            if (col == null)
                return;

            IList<Guid> pouet= (from x in col select x.ID).Distinct().ToList();
            UpdateComponentOrder(pouet);
        }

        public Guid ID { get; set; }

        public ICommand CreateCommand { get; }
        public ICommand DuplicateCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand RenameCommand { get; }
        public ICommand CreateMaskCommand { get; }


        private Composition _composition;
        public Composition Composition
        {
            get => _composition;
            set => SetProperty(ref _composition, value);
        }

        private Layer _selectedLayer;
        public Layer SelectedLayer
        {
            get => _selectedLayer;
            set => SetProperty(ref _selectedLayer, value);
        }


        private IList<Guid> _layerOrder;
        public IList<Guid> LayerOrder
        {
            get => _layerOrder;
            set => SetProperty(ref _layerOrder, value);
        }



        public void Rename() => SelectedLayer.IsRenaming = true;

        public void Create(Type type)
        {
            Layer layer = new Layer(new LayerModel(Guid.NewGuid()), Composition.CompositionService);
            Composition.AddComponent(layer);
            layer.IsSelected = true;
            SelectedLayer = layer;

            WeakReferenceMessenger.Default.Send<IMessage, int>(new MessageAddComponent(Composition.ID, layer), MessageType.Out);
        }

        public void Create(LayerModel layerModel)
        {
            Layer layer = new Layer(layerModel, Composition.CompositionService);
            Composition.AddComponent(layer);
            layer.IsSelected = true;
            SelectedLayer = layer;
        }

        public void CreateMask()
        {
            if (SelectedLayer == null)
                return;

            LayerMask layerMask = new LayerMask(new LayerMaskModel(), Composition.CompositionService);
            SelectedLayer.LayerMask = layerMask;
            SelectedLayer.LayerMask.SetMasterBeat(SelectedLayer.MasterBeat);
            SelectedLayer.SelectedIndex = 0;
            WeakReferenceMessenger.Default.Send<IMessage, int>(new MessageCreateLayerMask(Composition.ID, SelectedLayer), MessageType.Out);
        }


        public void CreateMask(Guid layerID, LayerMaskModel layerMaskModel)
        {
            Layer layer = Composition.Components.FirstOrDefault(x => x.ID == layerID) as Layer;

            if (layer == null)
                return;

            layer.LayerMask = new LayerMask(layerMaskModel, Composition.CompositionService);
            layer.LayerMask.SetMasterBeat(layer.MasterBeat);
            layer.SelectedIndex = 0;
        }


        public void Delete()
        {
            var selected = SelectedLayer;
            var index = Composition.Components.IndexOf(selected);

            if (selected == null)
                return;

            Composition.RemoveComponent(selected);
            WeakReferenceMessenger.Default.Send<IMessage, int>(new MessageRemoveComponent(Composition.ID, selected), MessageType.Out);

            if (Composition.Components.Count == 0)
            {
                SelectedLayer = null;
                return;
            }

            if(index == 0)
            {
                SelectedLayer = Composition.Components[0] as Layer;
                return;
            }

            if(index > 0)
            {
                SelectedLayer = Composition.Components[index - 1] as Layer;
                return;
            }
        }


        public void Delete(Guid id)
        {
            var toDelete = Composition.Components.FirstOrDefault( x => x.ID == id);

            if(toDelete != null)
                Composition.RemoveComponent(toDelete);
        }


        public void Receive(IMessage message)
        {
            if (message.ID != Composition.ID)
                return;

            switch (message)
            {
                case MessageAddComponent add:
                    this.Create(add.ComponentModel as LayerModel);
                    break;

                case MessageRemoveComponent remove:
                    this.Delete(remove.ComponentID);
                    break;

                case MessageComponentOrder order:
                    this.UpdateComponentOrder(order.IDs);
                    break;

                case MessageCreateLayerMask layerMask:
                    this.CreateMask(layerMask.LayerID, layerMask.LayerMaskModel);
                    break;
            }
        }


        public void UpdateComponentOrder(IList<Guid> ids)
        {
            LayerOrder.Clear();
            foreach (var id in ids)
            {
                LayerOrder.Add(id);
            }
            WeakReferenceMessenger.Default.Send<IMessage, int>(new MessageComponentOrder(Composition.ID, LayerOrder), MessageType.Out);
        }

        public void StartDrag(IDragInfo dragInfo)
        {
            dragInfo.Data = dragInfo.SourceItem;
            dragInfo.Effects = DragDropEffects.Copy | DragDropEffects.Move;
        }

        public bool CanStartDrag(IDragInfo dragInfo)
        {
            return true;
        }

        public void Dropped(IDropInfo dropInfo)
        {
            var sourceIndex = dropInfo.DragInfo.SourceIndex;
            var targetIndex = dropInfo.InsertIndex;

            if (targetIndex == Composition.Components.Count)
            {
                Move(sourceIndex, targetIndex);
                return;
            }

            if (targetIndex == Composition.Components.Count - 1)
            {
                Move(sourceIndex, targetIndex);
                return;
            }

            if (targetIndex >= sourceIndex)
            {
                Move(sourceIndex, targetIndex);
                return;
            }

            Composition.Components.Move(sourceIndex, targetIndex);

            //UpdateComponentOrder(LayerOrder);

        }

        private void Move(int sourceIndex, int targetIndex)
        {
            targetIndex -= 1;
            Composition.Components.Move(sourceIndex, targetIndex);
            //UpdateComponentOrder(LayerOrder);
            //WeakReferenceMessenger.Default.Send<IMessage, int>(new MessageComponentOrder(Composition.ID, LayerOrder), MessageType.Out);
        }

        public void DragDropOperationFinished(DragDropEffects operationResult, IDragInfo dragInfo)
        {
            //DragHanderIsDown = false;
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
