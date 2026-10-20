using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Undo
{
    // Undo takes the added item back out of the collection, so from that point on this command
    // owns the instance and disposes it when UndoManager drops the command.
    public class AddItemUndoCommand : IUndoCommand, IDisposable
    {
        private readonly CollectionSync _sync;
        private readonly IControl _item;
        private readonly int _index;
        private IControl _previousItem;
        private int _previousIndex;

        public AddItemUndoCommand(CollectionSync sync, IControl item, int index)
        {
            _sync = sync;
            _item = item;
            _index = index;
        }

        public void Execute()
        {
            var collection = _sync.Collection;
            _previousItem = collection.ManagerData.SelectedItem;
            _previousIndex = collection.ManagerData.Items.IndexOf(_previousItem);

            collection.InsertItem(_item, Math.Min(_index, collection.ManagerData.Items.Count));
            _sync.SendAdd(_item, collection.ManagerData.Items.IndexOf(_item));
        }

        public void Undo()
        {
            var collection = _sync.Collection;
            collection.RemoveItem(_item);
            _sync.SendRemove(_item, collection.ManagerData.SelectedIndex);

            if (_previousItem != null && collection.ManagerData.Items.Contains(_previousItem))
            {
                collection.ManagerData.SelectAt(_previousIndex);
                _sync.SendSelect(_previousItem, _previousIndex);
            }
        }

        public void Dispose()
        {
            OwnedControl.DisposeIfOrphaned(_sync.Collection.ControlRepository, _item);
        }
    }
}
