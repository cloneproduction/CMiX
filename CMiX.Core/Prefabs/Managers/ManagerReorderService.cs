// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows;
using System.Windows.Input;
using CMiX.Core.Prefabs.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GongSolutions.Wpf.DragDrop;

namespace CMiX.Core.Prefabs.Managers
{
    public partial class ManagerReorderService : ObservableObject,
                                                 IDropTarget,
                                                 IDragSource
    {
        public ManagerReorderService(ManagerData managerData, ManagerMessenger managerMessenger)
        {
            ID = managerData.ID;
            ManagerMessenger = managerMessenger;
            ManagerData = managerData;
        }

        ManagerMessenger ManagerMessenger { get; set; }
        ManagerData ManagerData { get; set; }

        public Guid ID { get; set; }

        [ObservableProperty]
        private bool dragHandlerIsPressed = false;



        //public void ItemUp()
        //{
        //    var selectedIndex = ManagerData.SelectedIndex;
        //    if (selectedIndex <= 0)
        //        return;

        //    ManagerData.Items.Move(selectedIndex, selectedIndex - 1);
        //    ManagerMessenger.SendMessageMoveItem(ManagerData.ID, selectedIndex, selectedIndex + 1);
        //}

        //public void ItemDown()
        //{
        //    var selectedIndex = ManagerData.SelectedIndex;
        //    if (selectedIndex == ManagerData.Items.Count - 1)
        //        return;

        //    ManagerData.Items.Move(selectedIndex, selectedIndex + 1);
        //    ManagerMessenger.SendMessageMoveItem(ManagerData.ID, selectedIndex, selectedIndex + 1);
        //}



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

            if (targetIndex == ManagerData.Items.Count)
            {
                MoveOnDrop(sourceIndex, targetIndex);
                return;
            }

            if (targetIndex == ManagerData.Items.Count - 1)
            {
                MoveOnDrop(sourceIndex, targetIndex);
                return;
            }

            if (targetIndex >= sourceIndex)
            {
                MoveOnDrop(sourceIndex, targetIndex);
                return;
            }

            ManagerData.Items.Move(sourceIndex, targetIndex);
            ManagerMessenger.SendMessageMoveItem(ManagerData.ID, sourceIndex, targetIndex);
        }

        public void MoveOnDrop(int sourceIndex, int targetIndex)
        {
            targetIndex -= 1;
            ManagerData.Items.Move(sourceIndex, targetIndex);
            ManagerMessenger.SendMessageMoveItem(ManagerData.ID, sourceIndex, targetIndex);
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
