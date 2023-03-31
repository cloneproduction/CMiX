// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Networking.Messages;
using CMiX.Core.Presentations.Service;

namespace CMiX.Core.Presentations.Prefabs.Message
{
    public class MessageChangePrefabHandler : IMessageHandler
    {
        public MessageChangePrefabHandler(CompositionService compositionService)
        {
            CompositionService = compositionService;
        }

        public CompositionService CompositionService { get; set; }

        public bool Handle(IControl control, IMessage message)
        {
            if (message is MessageChangePrefab messageChangePrefab)
            {
                var prefab = CompositionService.GetPrefab(messageChangePrefab.PrefabID);
                control.GetType().GetProperty(messageChangePrefab.PropertyName).SetValue(control, prefab);
                return true;
            }

            return false;
        }
    }
}
