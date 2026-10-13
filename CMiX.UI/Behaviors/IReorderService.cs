// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

namespace CMiX.UI.Behaviors
{
    // What ListBoxReorder needs from the object that reorders the items.
    public interface IReorderService
    {
        bool DragHandlerIsPressed { get; set; }

        bool CanDrop(int sourceIndex, int insertIndex);

        void Dropped(int sourceIndex, int insertIndex);
    }
}
