// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections.ObjectModel;
using System.Windows;
using CMiX.Core.Prefabs.Messages;
using GongSolutions.Wpf.DragDrop;

namespace CMiX.Core.Prefabs.Managers
{
    public class DraggablePrefabManager : PrefabManager,
        IPrefabManagerDraggable,
        IDropTarget,
        IDragSource
    {
        public DraggablePrefabManager(PrefabRepository prefabRepository, PrefabFactory prefabFactory) : base(prefabRepository, prefabFactory)
        {
            PrefabOrder = new ObservableCollection<Guid>();
            IsActive = true;
        }


        private ObservableCollection<Guid> _prefabOrder;
        public ObservableCollection<Guid> PrefabOrder
        {
            get => _prefabOrder;
            set => SetProperty(ref _prefabOrder, value);
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

            if (targetIndex == Prefabs.Count)
            {
                Move(sourceIndex, targetIndex);
                return;
            }

            if (targetIndex == Prefabs.Count - 1)
            {
                Move(sourceIndex, targetIndex);
                return;
            }

            if (targetIndex >= sourceIndex)
            {
                Move(sourceIndex, targetIndex);
                return;
            }

            Move(sourceIndex, targetIndex + 1);
        }

        private void Move(int sourceIndex, int targetIndex)
        {
            targetIndex -= 1;
            Prefabs.Move(sourceIndex, targetIndex);
            Send(new MessageMoveItem(ID, sourceIndex, targetIndex));
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

        public void DragDropOperationFinished(DragDropEffects operationResult, IDragInfo dragInfo)
        {

        }

        public void DragCancelled()
        {

        }

        public bool TryCatchOccurredException(Exception exception)
        {
            throw new NotImplementedException();
        }

        public void DragEnter(IDropInfo dropInfo)
        {

        }

        public void DragLeave(IDropInfo dropInfo)
        {

        }

        public void Drop(IDropInfo dropInfo)
        {

        }
    }
}
