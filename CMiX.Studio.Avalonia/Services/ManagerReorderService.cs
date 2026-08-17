// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core;
using CMiX.Core.Prefabs.Managers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CMiX.Studio.Avalonia.Services
{
    public partial class ManagerReorderService : ObservableObject, IManagerReorderService
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

        [ObservableProperty]
        private bool _dragHandlerIsPressed = false;

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

        // Mirrors the WPF gong DragOver guard: dropping on itself or directly below itself is a no op.
        public bool CanDrop(int sourceIndex, int insertIndex)
        {
            return insertIndex - 1 != sourceIndex && sourceIndex != insertIndex;
        }

        // Mirrors the WPF gong Dropped index fix up before moving.
        public void Dropped(int sourceIndex, int insertIndex)
        {
            var itemCount = CollectionManager.ManagerData.Items.Count;
            var targetIndex = insertIndex;

            if (targetIndex == itemCount ||
                targetIndex == itemCount - 1 ||
                targetIndex >= sourceIndex)
                targetIndex -= 1;

            Move(sourceIndex, targetIndex);
            DragHandlerIsPressed = false;
        }

        private void Move(int sourceIndex, int targetIndex)
        {
            CollectionManager.MoveItem(sourceIndex, targetIndex);
            _onMove(sourceIndex, targetIndex);
        }
    }
}
