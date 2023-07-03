// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Networking.Messages;
using CMiX.Core.Prefab.Managers;

namespace CMiX.Core.Prefab.Messages
{
    internal class MessageSelectorHandler<T> : IMessageHandler where T: class, IPrefab
    {
        public MessageSelectorHandler()
        {

        }

        public bool Handle(IControl control, IMessage message)
        {
            if (control is PrefabSelector<T> prefabSelector)
            {
                if (message is MessageSelectedPrefabChanged messageSelectedPrefabChanged)
                {
                    prefabSelector.SelectedItemChanged(messageSelectedPrefabChanged.SelectedPrefabID);
                    return true;
                }
            }

            return false;
        }
    }
}
