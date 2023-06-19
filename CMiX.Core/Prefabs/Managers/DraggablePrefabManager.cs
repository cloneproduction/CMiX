// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Prefab.Messages;
using CMiX.Core.Services;
using CommunityToolkit.Mvvm.Messaging;
using GongSolutions.Wpf.DragDrop;

namespace CMiX.Core.Prefab.Managers
{
    public class DraggablePrefabManager<T> :
        PrefabManager<T>,
        IPrefabManagerDraggable,
        IDropTarget,
        IDragSource where T : class, IPrefab
    {
        public DraggablePrefabManager(Guid id, CompositionService compositionService, PrefabRepository<T> prefabRepository) : base(id, compositionService)
        {
            PrefabRepository = prefabRepository;
            Prefabs.CollectionChanged += Prefabs_CollectionChanged;
            PrefabOrder = new ObservableCollection<Guid>();

            IsActive = true;
        }


        private void Prefabs_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var col = sender as ObservableCollection<IPrefab>;

            if (col == null)
                return;

            UpdateComponentOrder((from x in col select x.ID).Distinct().ToList());
        }


        private ObservableCollection<Guid> _prefabOrder;
        public ObservableCollection<Guid> PrefabOrder
        {
            get => _prefabOrder;
            set => SetProperty(ref _prefabOrder, value);
        }

        public override void AddItem()
        {
            base.AddItem();
            UpdateComponentOrder((from x in Prefabs select x.ID).Distinct().ToList());
        }


        public override void DeleteItem(IPrefab prefab)
        {
            base.DeleteItem(prefab);
            UpdateComponentOrder((from x in Prefabs select x.ID).Distinct().ToList());
        }

        public void UpdateComponentOrder(IList<Guid> ids)
        {
            PrefabOrder.Clear();
            foreach (var id in ids)
            {
                PrefabOrder.Add(id);
            }
            WeakReferenceMessenger.Default.Send<IMessage, int>(new MessagePrefabOrderChange(ID, PrefabOrder), MessageType.Out);
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

            Prefabs.Move(sourceIndex, targetIndex);
        }

        private void Move(int sourceIndex, int targetIndex)
        {
            targetIndex -= 1;
            Prefabs.Move(sourceIndex, targetIndex);
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
