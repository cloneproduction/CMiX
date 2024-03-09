// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows;
using CMiX.Core.Prefabs.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using GongSolutions.Wpf.DragDrop;

namespace CMiX.Core.Prefabs.Managers
{
    public partial class ManagerReorderService : ObservableObject,
                                                 IControl,
                                                 IDropTarget,
                                                 IDragSource
    {
        public ManagerReorderService(PrefabManager prefabManager)
        {
            PrefabManager = prefabManager;
            ID = prefabManager.ManagerData.ID;
            //ManagerData = prefabManager.ManagerData;
            //ManagerMessenger = prefabManager.ManagerMessenger;
        }

        //public ManagerData ManagerData { get; set; }
        //public ManagerMessenger ManagerMessenger { get; set; }
        public PrefabManager PrefabManager { get; set; }


        public Guid ID { get; set; }

        [ObservableProperty]
        private bool dragHandlerIsPressed = false;


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

        public void Dropped(IDropInfo dropInfo)
        {
            var sourceIndex = dropInfo.DragInfo.SourceIndex;
            var targetIndex = dropInfo.InsertIndex;

            if (targetIndex == PrefabManager.ManagerData.Items.Count)
            {
                MoveOnDrop(sourceIndex, targetIndex);
                return;
            }

            if (targetIndex == PrefabManager.ManagerData.Items.Count - 1)
            {
                MoveOnDrop(sourceIndex, targetIndex);
                return;
            }

            if (targetIndex >= sourceIndex)
            {
                MoveOnDrop(sourceIndex, targetIndex);
                return;
            }

            PrefabManager.ManagerData.Items.Move(sourceIndex, targetIndex);
            PrefabManager.ManagerMessenger.SendMessageMoveItem(PrefabManager.ManagerData.ID, sourceIndex, targetIndex);
        }

        public void MoveOnDrop(int sourceIndex, int targetIndex)
        {
            targetIndex -= 1;
            PrefabManager.ManagerData.Items.Move(sourceIndex, targetIndex);
            PrefabManager.ManagerMessenger.SendMessageMoveItem(PrefabManager.ManagerData.ID, sourceIndex, targetIndex);
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
