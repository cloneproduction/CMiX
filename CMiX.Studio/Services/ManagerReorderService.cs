// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Windows;
using CMiX.Core;
using CMiX.Core.Prefabs.Managers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GongSolutions.Wpf.DragDrop;

namespace CMiX.Studio.Services
{
    public partial class ManagerReorderService : ObservableObject,
                                                 IDropTarget,
                                                 IDragSource,
                                                 IManagerReorderService
    {
        public ManagerReorderService(CollectionManager collectionManager, Action<int, int> onMove)
        {
            CollectionManager = collectionManager;
            ID = collectionManager.ManagerData.ID;
            _onMove = onMove;
        }

        public Guid ID { get; set; }
        public CollectionManager CollectionManager { get; set; }
        private readonly Action<int, int> _onMove;

        public PrefabManager PrefabManager { get; set; }
        //public ICommand ItemUpCommand { get; set; }
        //public ICommand ItemDownCommand { get; set; }

        [ObservableProperty]
        private bool dragHandlerIsPressed = false;


        [RelayCommand]
        public void ItemUp(IControl control)
        {
            var index = CollectionManager.ManagerData.Items.IndexOf(control);
            if (index > 0) Move(index, index - 1);
        }

        [RelayCommand]
        public void ItemDown(IControl control)
        {
            var items = CollectionManager.ManagerData.Items;
            var index = items.IndexOf(control);
            if (index >= 0 && index < items.Count - 1)
                Move(index, index + 1);
        }


        public void StartDrag(IDragInfo dragInfo)
        {
            if (!DragHandlerIsPressed)
                return;

            dragInfo.Data = dragInfo.SourceItem;
            dragInfo.Effects = DragDropEffects.Copy | DragDropEffects.Move;
        }


        public bool CanStartDrag(IDragInfo dragInfo)
        {
            return true;
        }


        public void DragOver(IDropInfo dropInfo)
        {
            var targetItem = dropInfo.VisualTargetItem;
            var visualTarget = dropInfo.DragInfo?.VisualSourceItem;

            if (visualTarget == null)
                return;

            var targetIndex = dropInfo.InsertIndex;
            var sourceIndex = dropInfo.DragInfo?.SourceIndex;

            if (targetIndex - 1 == sourceIndex ||
                sourceIndex == targetIndex ||
                visualTarget == targetItem)
            {
                dropInfo.Effects = DragDropEffects.None;
                return;
            }

            dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
            dropInfo.Effects = DragDropEffects.Copy | DragDropEffects.Move;
        }


        public void Dropped(IDropInfo dropInfo)
        {
            var sourceIndex = dropInfo.DragInfo.SourceIndex;
            var targetIndex = dropInfo.InsertIndex;

            if (targetIndex == CollectionManager.ManagerData.Items.Count ||
                targetIndex == CollectionManager.ManagerData.Items.Count - 1 ||
                targetIndex >= sourceIndex)
                targetIndex -= 1;

            Move(sourceIndex, targetIndex);
        }


        private void Move(int sourceIndex, int targetIndex)
        {
            CollectionManager.MoveItem(sourceIndex, targetIndex);
            _onMove(sourceIndex, targetIndex);
        }

        public void DragDropOperationFinished(DragDropEffects operationResult, IDragInfo dragInfo)
        {
            dragHandlerIsPressed = false;
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
