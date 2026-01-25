// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows;
using System.Windows.Input;
using CMiX.Core.Networking.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GongSolutions.Wpf.DragDrop;

namespace CMiX.Core.Prefabs.Managers
{
    public partial class ManagerReorderService : ObservableObject,
 
                                                 IDropTarget,
                                                 IDragSource
    {
        public ManagerReorderService(PrefabManager prefabManager)
        {
            PrefabManager = prefabManager;


            ID = prefabManager.ManagerData.ID;

            ItemUpCommand = new RelayCommand<IControl>(ItemUp);
            ItemDownCommand = new RelayCommand<IControl>(ItemDown);
        }

        public Guid ID { get; set; }
        public PrefabManager PrefabManager { get; set; }
        public ICommand ItemUpCommand { get; set; }
        public ICommand ItemDownCommand { get; set; }

        [ObservableProperty]
        private bool dragHandlerIsPressed = false;


        public void ItemUp(IControl control)
        {
            var items = PrefabManager.ManagerData.Items;

            if (items.Count == 0)
                return;

            var index = items.IndexOf(control);

            if(index == 0) 
                return;

            Move(index, index - 1);
        }


        public void ItemDown(IControl control)
        {
            var items = PrefabManager.ManagerData.Items;

            if (items.Count <= 1)
                return;

            var index = items.IndexOf(control);

            if (index == items.Count - 1)
                return;

            Move(index, index + 1);
        }


        public void StartDrag(IDragInfo dragInfo)
        {
            if (!dragHandlerIsPressed)
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

            if (targetIndex == PrefabManager.ManagerData.Items.Count ||
                targetIndex == PrefabManager.ManagerData.Items.Count - 1||
                targetIndex >= sourceIndex)
            {
                targetIndex -= 1;
            }

            Move(sourceIndex, targetIndex);
        }


        private void Move(int sourceIndex, int targetIndex) 
        {
            PrefabManager.ManagerData.Items.Move(sourceIndex, targetIndex);
            var message = PrefabManager.MessageFactory.CreateMessage<MessageMoveItem>(PrefabManager.ManagerData.ID, sourceIndex, targetIndex);
            PrefabManager.ControlMessenger.SendMessage(message);
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
