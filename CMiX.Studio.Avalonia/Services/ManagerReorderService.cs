// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using CMiX.Core;
using CMiX.Core.Prefabs.Managers;
using CMiX.UI.Behaviors;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CMiX.Studio.Avalonia.Services
{
    public partial class ManagerReorderService : ObservableObject, IManagerReorderService, IReorderService
    {
        public ManagerReorderService(ManagerData data, Action<int, int> move)
        {
            _data = data;
            ID = data.ID;
            _move = move;
        }

        public Guid ID { get; set; }
        private readonly ManagerData _data;
        private readonly Action<int, int> _move;

        [ObservableProperty]
        private bool _dragHandleIsPressed = false;

        [RelayCommand]
        public void MoveItemUp(IControl control)
        {
            var index = _data.Items.IndexOf(control);
            if (index > 0) _move(index, index - 1);
        }

        [RelayCommand]
        public void MoveItemDown(IControl control)
        {
            var items = _data.Items;
            var index = items.IndexOf(control);
            if (index >= 0 && index < items.Count - 1)
                _move(index, index + 1);
        }

        public bool CanDrop(int sourceIndex, int insertIndex)
        {
            return insertIndex - 1 != sourceIndex && sourceIndex != insertIndex;
        }

        public void Dropped(int sourceIndex, int insertIndex)
        {
            var itemCount = _data.Items.Count;
            var targetIndex = insertIndex;

            if (targetIndex == itemCount ||
                targetIndex == itemCount - 1 ||
                targetIndex >= sourceIndex)
                targetIndex -= 1;

            _move(sourceIndex, targetIndex);
            DragHandleIsPressed = false;
        }
    }
}
