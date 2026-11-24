namespace CMiX.Core.Undo
{
    // Undo takes the added item back out of the collection, so from that point on this command
    // owns the instance and disposes it when UndoManager drops the command.
    public class AddItemUndoCommand : IUndoCommand, IDisposable
    {
        private readonly IItemList _list;
        private readonly IControl _item;
        private readonly int _index;
        private IControl _previousItem;
        private int _previousIndex;

        public AddItemUndoCommand(IItemList list, IControl item, int index)
        {
            _list = list;
            _item = item;
            _index = index;
        }

        public void Execute()
        {
            _previousItem = _list.SelectedItem;
            _previousIndex = _list.IndexOf(_previousItem);
            _list.Insert(_item, Math.Min(_index, _list.Count));
        }

        public void Undo()
        {
            _list.Remove(_item);

            if (_previousItem != null && _list.IndexOf(_previousItem) >= 0)
                _list.SelectAt(_previousIndex);
        }

        public void Dispose()
        {
            _list.Release(_item);
        }
    }
}
