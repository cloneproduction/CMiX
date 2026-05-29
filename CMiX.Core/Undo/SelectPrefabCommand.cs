using CMiX.Core;
using CMiX.Core.Networking;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Prefabs.Messages;
using CMiX.Core.Undo;

public class SelectPrefabCommand : IUndoCommand
{
    private readonly PrefabSelector _selector;
    private readonly ControlMessenger _messenger;
    private readonly MessageFactory _messageFactory;
    private readonly IControl _previousItem;
    private readonly IControl _newItem;

    public SelectPrefabCommand(PrefabSelector selector,
                               ControlMessenger messenger,
                               MessageFactory messageFactory,
                               IControl previousItem,
                               IControl newItem)
    {
        _selector = selector;
        _messenger = messenger;
        _messageFactory = messageFactory;
        _previousItem = previousItem;
        _newItem = newItem;
    }

    public void Execute()
    {
        _selector.SelectedItem = _newItem;
    }

    public void Undo()
    {
        _selector.SelectedItem = _previousItem;
    }
}
