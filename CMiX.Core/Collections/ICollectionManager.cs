// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

namespace CMiX.Core.Collections
{
    public interface ICollectionManager : IControl
    {
        void AddItem(IControlModel controlModel);
        void MoveItem(int oldIndex, int newIndex);
        void DeleteItem(Guid id);
        void ReplaceItem(Guid oldItemID, Guid newItemID);
        void SelectedItemChanged(Guid id);
    }
}
